# taskを作成して201 Createdを返す

## 今回の課題

- 目安時間: 30〜40分
- 前提: JSON bodyのmodel binding、入力検証、EF Coreの`SaveChangesAsync`を説明できる
- 目的: `POST /tasks`で入力を検証し、保存済みresourceのURIと内容を201 responseで返す流れを説明できる

Codexが`POST /tasks`を実装しました。clientはJSON bodyでtitleを送り、applicationは空白除去と入力検証を行います。有効なtitleだけをSQLiteへ保存し、生成されたIDを含むtaskと、そのtaskを取得できるURIを201 Created responseで返します。

作成requestは、まだ存在しないresourceを新しく作る操作です。正常に保存できた場合、単なる処理成功を表す200ではなく、resourceが作成されたことを表す201を使います。`Location` headerには、作成されたresourceを取得できる`/tasks/{id}`を設定します。

## requestからresponseまで

clientが次のJSONを送ると、ASP.NET Coreが`CreateTaskRequest`へdeserializeします。

```json
{
  "title": "  新しいtask  "
}
```

endpointは最初に`Trim`し、前後の空白を除いた`"新しいtask"`を作ります。その後、空文字・空白だけでないことと、100文字以下であることを検証します。検証に失敗した場合は400 Validation Problemを返し、command serviceを呼ばないためdatabaseには保存されません。

検証に成功した場合、`TaskCommandService`が新しい`TaskItem`をchange trackerへ追加します。この段階ではまだINSERTは実行されていません。`SaveChangesAsync`によってINSERTがSQLiteへ送られ、成功するとSQLiteが生成したIDが`task.Id`へ反映されます。

IDが確定してから、endpointは次のresponseを返します。

```http
HTTP/1.1 201 Created
Location: /tasks/6
Content-Type: application/json

{
  "id": 6,
  "title": "新しいtask",
  "isCompleted": false
}
```

`SaveChangesAsync`より前ではIDがまだ確定していないため、正しい`Location` headerを作れません。また、保存が失敗したのに201を返すことも避ける必要があります。そのため、保存の成功とID反映を待ってからresponseを組み立てます。

## Java / Springとの比較

Spring Bootではrequest DTOを`@RequestBody`で受け、Bean Validationで検証し、serviceからrepositoryの`save`を呼び、`ResponseEntity.created(uri)`で201を返す構成に近いです。

今回のMinimal APIでは、body parameterの`CreateTaskRequest`が`@RequestBody`相当の役割を持ちます。validationはまだ明示的なC#コードで行っています。serviceはHTTP statusを知らず、entityの作成と保存を担当します。endpointがserviceの結果を201 responseへ変換します。

## 新しく読む構文とAPI

### null conditional operator

```csharp
var title = request.Title?.Trim();
```

`?.`は左側が`null`ならmethodを呼ばず、式全体を`null`にします。titleが文字列なら`Trim`を呼びます。JSONからtitleが欠落した場合でも`NullReferenceException`を起こさず、後続のvalidationで必須errorにできます。

### `string.IsNullOrWhiteSpace`

```csharp
if (string.IsNullOrWhiteSpace(title))
```

値が`null`、空文字、または空白文字だけの場合にtrueを返します。`Trim`後のtitleへ使うことで、`"   "`のように見た目上内容のないtitleを保存対象から除外します。

### null-forgiving operator

```csharp
await taskCommandService.CreateTaskAsync(title!, cancellationToken);
```

`!`はcompilerのnull警告を抑えるnull-forgiving operatorです。runtimeにnull checkを追加する演算子ではありません。この行へ到達する前に`IsNullOrWhiteSpace`の検証を通り、errorがあればreturnしているため、ここではtitleがnullでないと判断できます。

### `Results.Created`

```csharp
return Results.Created($"/tasks/{task.Id}", task);
```

第1argumentが`Location` header、第2argumentがJSON response bodyです。status codeは201になります。文字列補間`$"...{task.Id}"`によって、生成されたIDをURIへ埋め込みます。

## 変更対象ファイル

| ファイル | 変更内容 |
| --- | --- |
| `05-capstone/TaskManagementApi/Models/CreateTaskRequest.cs` | JSON bodyを受けるrequest modelを追加 |
| `05-capstone/TaskManagementApi/Services/TaskCommandService.cs` | entity作成・INSERT・response model変換を行うserviceを追加 |
| `05-capstone/TaskManagementApi/Program.cs` | service登録、title検証、`POST /tasks`、201 responseを追加 |
| `05-capstone/TaskManagementApi.Tests/TaskCreationTests.cs` | 正常作成と空白titleの統合テストを追加 |
| `05-capstone/docs/04-create-task.md` | この課題説明を追加 |
| `05-capstone/docs/04-create-task-answers.md` | 回答シートを追加 |

理解確認前なので、rootの`README.md`はまだ変更しません。

## 実装コード

### `05-capstone/TaskManagementApi/Program.cs`

```csharp
app.MapPost("/tasks", async Task<IResult> (
    CreateTaskRequest request,
    TaskCommandService taskCommandService,
    CancellationToken cancellationToken) =>
{
    var title = request.Title?.Trim();
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(title))
    {
        errors[nameof(request.Title)] = ["titleは必須です。"];
    }
    else if (title.Length > 100)
    {
        errors[nameof(request.Title)] = ["titleは100文字以下で指定してください。"];
    }

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var task = await taskCommandService.CreateTaskAsync(
        title!,
        cancellationToken);

    return Results.Created($"/tasks/{task.Id}", task);
});
```

frameworkがJSONをrequest modelへ変換した後、endpointが外部入力をapplicationで扱える形へ正規化・検証します。不正なら400で処理を終え、有効ならcommand serviceを呼びます。serviceが返す保存済みtaskのIDを、bodyとLocationの両方に使います。

### `05-capstone/TaskManagementApi/Services/TaskCommandService.cs`

```csharp
public async Task<TaskSummary> CreateTaskAsync(
    string title,
    CancellationToken cancellationToken)
{
    var task = new TaskItem
    {
        Title = title,
        IsCompleted = false,
    };

    dbContext.Tasks.Add(task);
    await dbContext.SaveChangesAsync(cancellationToken);

    return new TaskSummary(
        task.Id,
        task.Title,
        task.IsCompleted);
}
```

`Add`はentityの状態をAddedとして追跡させます。`SaveChangesAsync`がINSERTを実行し、database採番のIDをentityへ反映します。新規taskの初期状態は未完了です。保存後にresponse modelへ変換するため、確定済みIDが含まれます。

### `05-capstone/TaskManagementApi.Tests/TaskCreationTests.cs`

正常系testは、空白を含むtitleをPOSTし、次を検証します。

- statusが201
- response titleの前後空白が除去されている
- 新規taskが未完了
- `Location`が`/tasks/{生成ID}`
- 作成後に同じIDをGETすると同じtaskが返る

最後のGETにより、POST responseを組み立てただけではなく、databaseへ永続化されたことまで確認できます。

異常系testは、空白だけのtitleと101文字のtitleをそれぞれPOSTし、400を確認します。`new string('a', 101)`は、文字`'a'`を101個並べた文字列を作るC#の`string` constructorです。これにより、100文字上限を1文字だけ超えた入力を用意しています。validation error時にはserviceを呼ばないためINSERTされません。

## 検証方法と結果

```fish
cd /home/yukihiro/Workspace/c#-learning/05-capstone
nix develop -c dotnet format TaskManagementApi.slnx --verify-no-changes --no-restore
nix develop -c dotnet test TaskManagementApi.slnx -m:1 --no-restore
```

既存test 5件と作成test 3件を合わせ、`Passed: 8, Failed: 0, Skipped: 0`でした。

## コードリーディング課題

1. `SaveChangesAsync`の完了後に`Results.Created`を組み立てる必要があるのはなぜですか。IDと保存成否の両方に触れてください。
2. titleを`Trim`してから`IsNullOrWhiteSpace`と最大長を検証することで、保存される文字列はどのように制限されますか。
3. 正常系testがPOST responseだけでなく、作成後に同じIDをGETしているのは何を確認するためですか。

## 設問と教材の対応確認

| 設問 | 回答に必要な説明 |
| --- | --- |
| 問1 | 「requestからresponseまで」の`SaveChangesAsync`後にIDが反映され、保存失敗時に201を返さない説明 |
| 問2 | 同節の`Trim`、空白判定、100文字上限の説明と「新しく読む構文とAPI」 |
| 問3 | 「`TaskCreationTests.cs`」の、GETでdatabase永続化を確認する説明 |

## 完了条件

- JSON bodyからrequest modelへのbindingを説明できる
- titleの正規化、必須、最大長validationを説明できる
- `Add`と`SaveChangesAsync`の実行境界、およびID反映を説明できる
- 201 status、Location header、response bodyの役割を説明できる
- POST後のGETで永続化を検証する理由を説明できる
- 全8件の統合テストが成功する
