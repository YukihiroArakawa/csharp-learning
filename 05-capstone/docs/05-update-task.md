# taskを更新して200 / 400 / 404を返す

## 今回の課題

- 目安時間: 30〜40分
- 前提: JSON bodyのmodel binding、入力検証、EF Coreのchange trackingを概ね理解している
- 目的: `PUT /tasks/{id}`が入力を検証し、既存taskを更新して、結果に応じたHTTP statusを返す流れを説明できる

Codexがtask更新APIを実装しました。clientはURLで更新対象のIDを、JSON bodyで新しいtitleと完了状態を送ります。applicationはtitleを整えて検証し、入力が正しければ対象taskをSQLiteから探します。対象があれば値を書き換えて保存し、更新後のtaskを200 OKで返します。

今回の`PUT`は、taskのtitleと完了状態をまとめて新しい値へ置き換える操作として扱います。部分的な項目だけを変更する`PATCH`ではありません。そのためrequest bodyには`title`と`isCompleted`の両方を含めます。

## requestからresponseまで

clientは次のrequestを送ります。

```http
PUT /tasks/2
Content-Type: application/json

{
  "title": "  ページングを修正する  ",
  "isCompleted": true
}
```

ASP.NET Coreはrouteの`2`を`id`へ、JSON bodyを`UpdateTaskRequest`へ変換します。`TaskCommandService`はDIから、`CancellationToken`はHTTP requestの中断通知と結び付けて渡されます。

endpointは最初にtitleの前後の空白を`Trim`で除去します。次に、null・空文字・空白だけの値ではないことと、100文字以下であることを検証します。検証に失敗した場合は、その時点で400 Validation Problemを返し、database検索も更新も行いません。

入力が正しい場合だけ、command serviceがIDに一致するtaskを検索します。対象が存在しなければserviceは`null`を返し、endpointは404 Not Foundへ変換します。対象が存在すればtitleと完了状態を書き換え、`SaveChangesAsync`でUPDATEを実行します。その後、更新済みtaskを200 OKで返します。

したがって処理結果は次の3種類です。

| 条件 | status | databaseへの変更 |
| --- | --- | --- |
| titleが不正 | 400 Bad Request | 変更しない |
| 入力は正しいがIDが存在しない | 404 Not Found | 変更しない |
| 入力が正しくIDも存在する | 200 OK | UPDATEする |

検証を検索より先に行うため、例えば存在しないIDへ空白titleを送った場合も400になります。まずrequest自体がapplicationの入力条件を満たすかを判定し、その後で対象resourceの存在を調べる順序です。

## EF Coreが変更をUPDATEとして認識する仕組み

`SingleOrDefaultAsync`で取得した`TaskItem`は、既定では`TaskDbContext`のchange trackerに登録されます。change trackerは、取得時のproperty値と現在のproperty値を比較できる状態を保ちます。

取得した同じinstanceに対して次の代入を行うと、メモリ上の値が変わります。

```csharp
task.Title = title;
task.IsCompleted = isCompleted;
```

その後に`SaveChangesAsync`を呼ぶと、EF Coreは追跡中entityの変更を検出し、必要なUPDATEをdatabaseへ送ります。今回は別instanceを作ってattachする処理ではないため、`dbContext.Tasks.Update(task)`を明示的に呼ぶ必要はありません。

前の一覧取得課題では読み取り専用queryに`AsNoTracking`を使いました。しかし更新処理で`AsNoTracking`を使うと、取得したentityはchange trackerへ登録されません。そのままpropertyを書き換えて`SaveChangesAsync`を呼んでも、EF Coreは更新対象として認識できません。更新するentityを通常の追跡queryで取得している点が重要です。

## Java / Springとの比較

Spring Data JPAでtransaction内にentityを取得し、そのsetterで値を変えると、dirty checkingによってUPDATEされる動きに近いです。EF Coreではこの役割を`DbContext`のchange trackerが担います。

Minimal APIのendpointはSpring MVCのcontroller methodに相当し、route値とrequest bodyを受け、validation error・resource不存在・更新成功をHTTP responseへ変換します。`TaskCommandService`はHTTP statusを返さず、更新済みdataまたは`null`を返します。HTTPの判断をendpointに残すことで、serviceをHTTP以外の呼び出し元からも扱いやすくしています。

## 新しく読む構文とAPI

### nullableな戻り値 `TaskSummary?`

```csharp
public async Task<TaskSummary?> UpdateTaskAsync(...)
```

外側の`Task<...>`は非同期処理を、内側の`TaskSummary?`は結果がtaskまたは`null`になり得ることを表します。ここでの`null`は、指定IDのtaskが存在しないというservice上の結果です。Javaなら`CompletableFuture<Optional<TaskSummary>>`に近い意図ですが、C#ではnullable reference typeを使っています。

### `SingleOrDefaultAsync`

```csharp
var task = await dbContext.Tasks.SingleOrDefaultAsync(
    task => task.Id == id,
    cancellationToken);
```

条件に一致する行が1件ならentityを、0件なら`null`を返します。複数件一致すると例外になりますが、今回はprimary keyであるIDを条件にするため、database上の一致は最大1件です。`CancellationToken`を渡すことで、client切断などの中断要求をdatabase処理まで伝播します。

### nullによる200 / 404分岐

```csharp
return task is null
    ? Results.NotFound()
    : Results.Ok(task);
```

条件演算子は`条件 ? trueの場合 : falseの場合`の形です。serviceが`null`を返した場合は404を、それ以外では更新済みtaskを含む200を返します。

## 変更対象ファイル

| ファイル | 変更内容 |
| --- | --- |
| `05-capstone/TaskManagementApi/Models/UpdateTaskRequest.cs` | titleと完了状態を受けるrequest modelを追加 |
| `05-capstone/TaskManagementApi/Services/TaskCommandService.cs` | 追跡query、property更新、UPDATE処理を追加 |
| `05-capstone/TaskManagementApi/Program.cs` | title検証と`PUT /tasks/{id}`を追加 |
| `05-capstone/TaskManagementApi.Tests/TaskUpdateTests.cs` | 200、404、空白titleの400、101文字titleの400を検証する統合テストを追加 |
| `05-capstone/docs/05-update-task.md` | この課題説明を追加 |
| `05-capstone/docs/05-update-task-answers.md` | 回答シートを追加 |

理解確認前なので、rootの`README.md`はまだ変更しません。

## 実装コード

### `05-capstone/TaskManagementApi/Models/UpdateTaskRequest.cs`

```csharp
public sealed record UpdateTaskRequest(
    string? Title,
    bool IsCompleted);
```

JSONの`title`と`isCompleted`を受け取るrequest modelです。titleは外部入力なので、欠落やJSONの`null`もvalidationへ渡せるよう`string?`にしています。`bool`はnullableではないため、JSONから省略された場合は既定値の`false`になります。今回はPUTで全体を置き換える契約なので、`false`も新しい完了状態として扱います。

### `05-capstone/TaskManagementApi/Program.cs`

```csharp
app.MapPut("/tasks/{id:int}", async Task<IResult> (
    int id,
    UpdateTaskRequest request,
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

    var task = await taskCommandService.UpdateTaskAsync(
        id,
        title!,
        request.IsCompleted,
        cancellationToken);

    return task is null
        ? Results.NotFound()
        : Results.Ok(task);
});
```

routeとbodyがbindingされた後、endpointはtitleを正規化・検証します。errorがあれば400でreturnするためserviceには進みません。有効な入力ではserviceの結果を待ち、`null`なら404、更新済みtaskなら200へ変換します。

### `05-capstone/TaskManagementApi/Services/TaskCommandService.cs`

```csharp
public async Task<TaskSummary?> UpdateTaskAsync(
    int id,
    string title,
    bool isCompleted,
    CancellationToken cancellationToken)
{
    var task = await dbContext.Tasks.SingleOrDefaultAsync(
        task => task.Id == id,
        cancellationToken);

    if (task is null)
    {
        return null;
    }

    task.Title = title;
    task.IsCompleted = isCompleted;
    await dbContext.SaveChangesAsync(cancellationToken);

    return new TaskSummary(
        task.Id,
        task.Title,
        task.IsCompleted);
}
```

serviceは追跡queryで対象entityを取得します。存在しなければ状態を変えずに`null`を返します。存在すれば同じinstanceのpropertyを変更し、`SaveChangesAsync`でUPDATEを確定してからresponse modelへ変換します。

### `05-capstone/TaskManagementApi.Tests/TaskUpdateTests.cs`

正常系testはID 2へtitleと`true`をPUTし、200 responseのID・空白除去後title・完了状態を確認します。その後、同じIDをGETしてPUT responseと等しいことを確認します。これにより、responseだけを書き換えたのではなく、databaseへ更新が永続化されたことを検証します。

異常系testは次の契約を確認します。

- 正しい入力でもID 999が存在しなければ404
- 空白だけのtitleなら400
- 101文字のtitleなら400

## 検証方法と結果

```fish
cd /home/yukihiro/Workspace/c#-learning/05-capstone
nix develop -c dotnet format TaskManagementApi.slnx --verify-no-changes --no-restore
nix develop -c dotnet test TaskManagementApi.slnx -m:1 --no-restore
```

既存test 8件と更新test 4件を合わせ、`Passed: 12, Failed: 0, Skipped: 0`でした。

## コードリーディング課題

1. titleのvalidationをtask検索より先に行うため、不正なtitleと存在しないIDを同時に指定したrequestは何statusになりますか。また、その時database検索は行われますか。
2. `SingleOrDefaultAsync`で取得したentityのpropertyを書き換えた後、`dbContext.Tasks.Update(task)`を呼ばなくても`SaveChangesAsync`がUPDATEできるのはなぜですか。
3. 正常系testがPUT responseを確認した後、同じIDをGETしているのは何を区別するためですか。

## 設問と教材の対応確認

| 設問 | 回答に必要な説明 |
| --- | --- |
| 問1 | 「requestからresponseまで」のvalidation、early return、検索順序の説明 |
| 問2 | 「EF Coreが変更をUPDATEとして認識する仕組み」の追跡queryとchange trackerの説明 |
| 問3 | 「`TaskUpdateTests.cs`」のresponse変更とdatabase永続化を区別する説明 |

## 完了条件

- route IDとJSON bodyが別々の入力としてbindingされることを説明できる
- 400、404、200へ分岐する条件と処理順序を説明できる
- 追跡中entityのproperty変更が`SaveChangesAsync`でUPDATEされる理由を説明できる
- 更新処理で`AsNoTracking`を使わない理由を説明できる
- PUT後のGETでdatabase永続化を検証する理由を説明できる
- 全12件の統合テストが成功する
