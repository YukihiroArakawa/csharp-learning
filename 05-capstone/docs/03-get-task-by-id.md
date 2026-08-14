# IDを指定してtaskを1件取得する

## 今回の課題

- 目安時間: 25〜35分
- 前提: route parameter、nullable reference type、EF Coreのprojectionを説明できる
- 目的: `GET /tasks/{id}`のrequestが、DB検索結果に応じて200または404になる流れを説明できる

Codexが`GET /tasks/{id}`を実装しました。clientがURL pathへtask IDを指定すると、ASP.NET Coreが整数へ変換し、`TaskQueryService`がSQLiteから該当taskを検索します。taskが存在すれば200とJSONを返し、存在しなければ404を返します。

一覧取得では、条件に一致する要素が0件でも「一覧は存在し、結果が空」として200と空配列を返しました。今回のendpointは特定IDの単一resourceを要求します。そのIDに対応するresourceがなければ、取得対象自体が存在しないため404にします。この違いが今回の中心です。

## requestからresponseまで

`/tasks/3`へのrequestでは、まずroutingが`/tasks/{id:int}`を選びます。`{id:int}`の`int`はroute constraintであり、この位置が整数形式の場合だけendpointへ一致します。ASP.NET Coreは文字列`"3"`をC#の`int id`へ変換します。

endpointはIDとrequestの`CancellationToken`を`TaskQueryService`へ渡します。serviceは`WHERE Id = ...`に相当する条件を付け、entity全体ではなく`TaskSummary`に必要なcolumnだけを選びます。SQL実行結果が1件なら`TaskSummary`、0件なら`null`が返ります。

endpointは結果を次のようにHTTPへ対応付けます。

| serviceの結果 | HTTP response | 意味 |
| --- | --- | --- |
| `TaskSummary` | 200 + JSON | 指定されたtaskが存在する |
| `null` | 404 | 指定されたIDのtaskが存在しない |

## Java / Springとの比較

Spring Data JPAでは、ID検索を`Optional<TaskItem>`として返し、controllerで`ResponseEntity.ok(...)`または`ResponseEntity.notFound()`へ分岐する形に近いです。C#ではnullable reference typeの`TaskSummary?`で「値がない可能性」を表し、pattern matchingで`null`を判定します。

Javaの`Optional`はcontainerへ値を包みますが、C#のreference typeでは`?`がcompilerのnull解析へ情報を与えます。runtimeに新しいwrapper objectを返すという意味ではありません。

## 新しく読む構文とAPI

### route constraint

```csharp
app.MapGet("/tasks/{id:int}", ...);
```

`{id}`がroute parameter名、`:int`が整数形式という制約です。`/tasks/3`は一致しますが、`/tasks/abc`はこのendpointへ一致しません。endpoint内部で文字列から整数へ変換する処理を書く必要はありません。

### `Task<TaskSummary?>`

```csharp
public Task<TaskSummary?> GetTaskAsync(...)
```

外側の`Task<...>`はdatabase I/Oが非同期に完了することを表します。内側の`TaskSummary?`は、完了後の検索結果がtaskまたは`null`であることを表します。

```text
Task<TaskSummary?>
│    └─ 完了後の値は TaskSummary または null
└────── 非同期処理
```

`SingleOrDefaultAsync`自身も`Task<TaskSummary?>`を返します。呼び出し側methodの戻り値と同じ型であり、query完了後に変換やログなどの追加処理を行わない場合は、その`Task`を直接返せます。

```csharp
// await後に追加処理がないため、こちらで十分
public Task<TaskSummary?> GetTaskAsync(...)
{
    return query.SingleOrDefaultAsync(token);
}

// 同じ結果だが、この場合はasync/awaitを追加する必要がない
public async Task<TaskSummary?> GetTaskAsync(...)
{
    return await query.SingleOrDefaultAsync(token);
}
```

一方、await後に結果を加工したりログを記録したりするなら、そこで処理を続けるため`async`と`await`が必要です。

```csharp
public async Task<TaskSummary?> GetTaskAsync(...)
{
    var task = await query.SingleOrDefaultAsync(token);
    logger.LogInformation("taskを検索しました");
    return task;
}
```

### `SingleOrDefaultAsync`

```csharp
.SingleOrDefaultAsync(cancellationToken);
```

query結果が0件ならdefault valueである`null`、1件ならその要素を返します。2件以上なら「単一である」という前提に反するため例外になります。今回はprimary keyの`Id`で検索するため、database制約上2件以上にはなりません。

`FirstOrDefaultAsync`でも0件と1件を扱えますが、`SingleOrDefaultAsync`は「この条件では最大1件」という意図をより強く表します。

### conditional operatorとnull pattern

```csharp
return task is null
    ? Results.NotFound()
    : Results.Ok(task);
```

`condition ? A : B`はC#のconditional operatorで、Javaの三項演算子と同じ形です。`task is null`がtrueなら404、falseなら200を返します。`is null`はoverloadされた等価演算子の影響を受けないnull patternです。

## 変更対象ファイル

| ファイル | 変更内容 |
| --- | --- |
| `05-capstone/TaskManagementApi/Services/TaskQueryService.cs` | IDでtaskを1件検索する`GetTaskAsync`を追加 |
| `05-capstone/TaskManagementApi/Program.cs` | `GET /tasks/{id:int}`と200 / 404分岐を追加 |
| `05-capstone/TaskManagementApi.Tests/TaskDetailTests.cs` | 存在時と不存在時の統合テストを追加 |
| `05-capstone/docs/03-get-task-by-id.md` | この課題説明を追加 |
| `05-capstone/docs/03-get-task-by-id-answers.md` | 回答シートを追加 |

理解確認前なので、rootの`README.md`はまだ変更しません。

## 実装コード

### `05-capstone/TaskManagementApi/Services/TaskQueryService.cs`

```csharp
public Task<TaskSummary?> GetTaskAsync(
    int id,
    CancellationToken cancellationToken)
{
    return dbContext.Tasks
        .AsNoTracking()
        .Where(task => task.Id == id)
        .Select(task => new TaskSummary(
            task.Id,
            task.Title,
            task.IsCompleted))
        .SingleOrDefaultAsync(cancellationToken);
}
```

このmethodには`await`後の追加処理がないため、EF Coreが返す`Task<TaskSummary?>`をそのまま呼び出し元へ返しています。database accessは`SingleOrDefaultAsync`によって開始され、requestのキャンセル通知も渡されます。

`AsNoTracking`は更新しない読み取り専用queryであることを表します。`Select`を`SingleOrDefaultAsync`より前に置くことで、databaseはresponseに必要な3 columnだけを返します。

### `05-capstone/TaskManagementApi/Program.cs`

```csharp
app.MapGet("/tasks/{id:int}", async Task<IResult> (
    int id,
    TaskQueryService taskQueryService,
    CancellationToken cancellationToken) =>
{
    var task = await taskQueryService.GetTaskAsync(id, cancellationToken);

    return task is null
        ? Results.NotFound()
        : Results.Ok(task);
});
```

ASP.NET CoreがrouteからID、DIからscoped service、request中断通知からtokenを渡します。serviceのnullableな結果をHTTP statusへ翻訳する責務はendpointに置いています。

### `05-capstone/TaskManagementApi.Tests/TaskDetailTests.cs`

```csharp
var response = await client.GetAsync("/tasks/3");

response.EnsureSuccessStatusCode();
var task = await response.Content.ReadFromJsonAsync<TaskSummary>();
Assert.Equal(3, task.Id);
```

存在時testはrouting、route binding、DI、SQL、JSONを通り、ID 3の内容まで確認します。不存在時testは`/tasks/999`へrequestを送り、404を確認します。この2件により200 / 404分岐を固定しています。

## 検証方法と結果

```fish
cd /home/yukihiro/Workspace/c#-learning/05-capstone
nix develop -c dotnet format TaskManagementApi.slnx --verify-no-changes --no-restore
nix develop -c dotnet test TaskManagementApi.slnx -m:1 --no-restore
```

既存のページングtest 3件と今回の詳細取得test 2件を合わせ、`Passed: 5, Failed: 0, Skipped: 0`でした。

## コードリーディング課題

1. `/tasks/999`に対応するrowが存在しない場合、serviceとendpointはそれぞれ何を返しますか。
2. `SingleOrDefaultAsync`が2件以上の結果で例外になるにもかかわらず、今回は安全に使える理由を説明してください。
3. `GetTaskAsync`で`async`と`await`を使わず、EF Coreの`Task<TaskSummary?>`をそのまま返せる理由を説明してください。

## 完了条件

- route constraintとroute parameter bindingの役割を説明できる
- DBの0件という結果を、serviceの`null`からHTTP 404へ変換する流れを説明できる
- primary key検索で`SingleOrDefaultAsync`を使える理由を説明できる
- 非同期methodが`Task`を直接返せる条件を説明できる
- 全5件の統合テストが成功する
