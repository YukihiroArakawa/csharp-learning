# taskを削除して204 / 404を返す

## 今回の課題

- 目安時間: 25〜35分
- 前提: route parameter、EF Coreのchange tracking、`SaveChangesAsync`を説明できる
- 目的: `DELETE /tasks/{id}`が既存taskを削除し、結果を204または404へ変換する流れを説明できる

Codexがtask削除APIを実装しました。clientはURLで削除対象のIDを指定します。applicationはそのtaskをSQLiteから検索し、存在すれば削除をdatabaseへ保存します。成功時は204 No Content、指定されたtaskが存在しなければ404 Not Foundを返します。

削除には新しいtask dataが必要ないため、request bodyは使いません。`DELETE /tasks/3`の`3`だけがapplicationへの入力です。

## requestからresponseまで

clientは次のrequestを送ります。

```http
DELETE /tasks/3
```

ASP.NET Coreは`{id:int}`に一致した`3`をC#の`int id`へ変換します。`TaskCommandService`はDIから取得され、`CancellationToken`にはHTTP requestの中断通知が結び付けられます。

endpointはserviceの`DeleteTaskAsync`を呼びます。serviceは最初にIDでtaskを検索します。対象がなければ`false`を返し、この場合databaseの状態は変わりません。endpointは`false`を404 Not Foundへ変換します。

対象が存在する場合、serviceはそのentityを`Remove`へ渡します。ただし、`Remove`を呼んだ時点ではSQLのDELETEはまだ実行されません。EF Coreのchange tracker上でentityの状態が`Deleted`になり、次の`SaveChangesAsync`で削除予定であることが記録されます。

`SaveChangesAsync`がDELETEをSQLiteへ送り、database処理が成功してからserviceは`true`を返します。endpointは`true`を204 No Contentへ変換します。保存完了を待ってから成功を返すため、databaseでDELETEが失敗したのに204を返すことを避けられます。

処理結果は次の2種類です。

| 条件 | serviceの結果 | HTTP status | response body |
| --- | --- | --- | --- |
| IDに一致するtaskがある | `true` | 204 No Content | なし |
| IDに一致するtaskがない | `false` | 404 Not Found | なし |

## なぜ204 No Contentなのか

削除成功後には、返すべき最新task表現がありません。今回のAPIでは削除結果の追加情報も必要ないため、成功を示すstatusだけを返す204を選びます。204 responseにbodyは付けません。

作成APIでは、作成されたresourceとそのURIをclientへ伝えるため201とbodyが必要でした。更新APIでは、更新後のresourceを返す契約にしたため200とbodyを使いました。削除APIでは、resourceがなくなったこと自体が結果なので204だけで十分です。

## `Remove`と`SaveChangesAsync`の境界

次の2行は同じ処理ではありません。

```csharp
dbContext.Tasks.Remove(task);
await dbContext.SaveChangesAsync(cancellationToken);
```

`Remove`は、追跡中のtaskを「次回保存時に削除するentity」としてchange trackerへ登録するメモリ上の操作です。ここでprocessが終了したり、`SaveChangesAsync`を呼ばずにrequestを終えたりすれば、databaseの行は残ったままです。

`SaveChangesAsync`は、change trackerが保持する変更内容をdatabaseへ反映する境界です。ここでDELETEが実行されます。`CancellationToken`を渡しているため、client切断などの停止要求をdatabase処理まで伝播できます。

## Java / Springとの比較

Spring Data JPAでrepositoryの`delete(entity)`をtransaction内で実行し、transactionのflushやcommitでSQLがdatabaseへ反映される流れに近いです。EF Coreでは`DbContext`がentityの状態を追跡し、`SaveChangesAsync`が変更をdatabaseへ送る明示的な保存境界になります。

Spring MVCのcontrollerがserviceの結果を`ResponseEntity.noContent()`または404へ変換するのと同様に、Minimal APIのendpointがHTTP statusを決めます。`TaskCommandService`はHTTPを知らず、削除できたかどうかだけを`bool`で返します。

## 新しく読む構文とAPI

### 非同期の真偽値 `Task<bool>`

```csharp
public async Task<bool> DeleteTaskAsync(...)
```

`Task<bool>`は、非同期処理の完了後に`true`または`false`が得られることを表します。今回は`true`が削除成功、`false`が対象不存在です。Javaであれば`CompletableFuture<Boolean>`に近い型ですが、C#の`await`後には通常の`bool`として扱えます。

### `DbSet.Remove`

```csharp
dbContext.Tasks.Remove(task);
```

引数のentityをchange tracker上で削除予定にします。戻り値もありますが、今回は使っていません。SQLを即時実行するmethodではない点が重要です。

### `Results.NoContent`

```csharp
Results.NoContent()
```

status code 204でbodyのないresponseを作ります。endpointでは条件演算子を使い、serviceの結果を204または404へ変換しています。

```csharp
return deleted
    ? Results.NoContent()
    : Results.NotFound();
```

## 変更対象ファイル

| ファイル | 変更内容 |
| --- | --- |
| `05-capstone/TaskManagementApi/Services/TaskCommandService.cs` | task検索、削除予定への変更、DELETE確定処理を追加 |
| `05-capstone/TaskManagementApi/Program.cs` | `DELETE /tasks/{id}`と204 / 404分岐を追加 |
| `05-capstone/TaskManagementApi.Tests/TaskDeletionTests.cs` | 削除成功・bodyなし・削除後GET・対象不存在を検証する統合テストを追加 |
| `05-capstone/docs/06-delete-task.md` | この課題説明を追加 |
| `05-capstone/docs/06-delete-task-answers.md` | 回答シートを追加 |

理解確認前なので、rootの`README.md`はまだ変更しません。

## 実装コード

### `05-capstone/TaskManagementApi/Services/TaskCommandService.cs`

```csharp
public async Task<bool> DeleteTaskAsync(
    int id,
    CancellationToken cancellationToken)
{
    var task = await dbContext.Tasks.SingleOrDefaultAsync(
        task => task.Id == id,
        cancellationToken);

    if (task is null)
    {
        return false;
    }

    dbContext.Tasks.Remove(task);
    await dbContext.SaveChangesAsync(cancellationToken);

    return true;
}
```

serviceは対象を追跡queryで取得します。存在しなければ`false`で処理を終了します。存在する場合は`Remove`で削除予定にし、`SaveChangesAsync`でdatabaseから削除してから`true`を返します。

### `05-capstone/TaskManagementApi/Program.cs`

```csharp
app.MapDelete("/tasks/{id:int}", async Task<IResult> (
    int id,
    TaskCommandService taskCommandService,
    CancellationToken cancellationToken) =>
{
    var deleted = await taskCommandService.DeleteTaskAsync(
        id,
        cancellationToken);

    return deleted
        ? Results.NoContent()
        : Results.NotFound();
});
```

frameworkがroute ID、service、tokenをparameterへ渡した後、endpointがserviceを呼びます。serviceの`bool`をHTTPへ変換するだけで、database操作の詳細はendpointへ漏らしていません。

### `05-capstone/TaskManagementApi.Tests/TaskDeletionTests.cs`

```csharp
var response = await client.DeleteAsync("/tasks/3");

Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
Assert.Empty(await response.Content.ReadAsByteArrayAsync());

var getResponse = await client.GetAsync("/tasks/3");
Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
```

最初の2つのassertionは204と空bodyというHTTP契約を確認します。しかし、これだけではendpointが204を返しただけで、本当にdatabaseから削除したかは分かりません。そのため同じIDをGETし、404になることも確認します。これによりDELETE responseの見た目だけでなく、後続requestからもtaskが見つからない永続化済み状態を検証します。

もう1件のtestはID 999を削除しようとして404になることを確認します。

## 検証方法と結果

```fish
cd /home/yukihiro/Workspace/c#-learning/05-capstone
nix develop -c dotnet format TaskManagementApi.slnx --verify-no-changes --no-restore
nix develop -c dotnet test TaskManagementApi.slnx -m:1 --no-restore
```

既存test 12件と削除test 2件を合わせ、`Passed: 14, Failed: 0, Skipped: 0`でした。

## コードリーディング課題

1. `Remove`と`SaveChangesAsync`は、それぞれどの時点で何を変更しますか。メモリ上の追跡状態とdatabaseを区別して説明してください。
2. `DeleteTaskAsync`がHTTP responseを直接返さず`bool`を返し、endpointが204 / 404へ変換しているのは、serviceとendpointの責務をどう分けるためですか。
3. 削除成功testが204と空bodyを確認した後、同じIDをGETして404も確認しているのはなぜですか。

## 設問と教材の対応確認

| 設問 | 回答に必要な説明 |
| --- | --- |
| 問1 | 「`Remove`と`SaveChangesAsync`の境界」の追跡状態とdatabase反映の説明 |
| 問2 | 「Java / Springとの比較」と`Program.cs`のserviceはHTTPを知らずendpointがstatusを決める説明 |
| 問3 | 「`TaskDeletionTests.cs`」の204だけでは削除永続化を確認できない説明 |

## 完了条件

- DELETE requestでroute IDだけを入力に使うことを説明できる
- task存在時は204、不存在時は404になることを説明できる
- `Remove`と`SaveChangesAsync`の役割と実行境界を説明できる
- serviceの`bool`をendpointがHTTP statusへ変換する責務分担を説明できる
- DELETE後のGETでdatabase上の削除を確認する理由を説明できる
- 全14件の統合テストが成功する
