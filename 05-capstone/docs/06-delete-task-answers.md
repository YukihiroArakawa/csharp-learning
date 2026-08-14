# taskを削除して204 / 404を返す: 回答シート

教材: [06-delete-task.md](06-delete-task.md)

状態: 完了

## 問1

`Remove`と`SaveChangesAsync`は、それぞれどの時点で何を変更しますか。メモリ上の追跡状態とdatabaseを区別して説明してください。

回答:
Remove時点ではメモリ上のEntityの状態をDeletedに更新をし、SaveChangesAsync時点でDB更新をしに行く


## 問2

`DeleteTaskAsync`がHTTP responseを直接返さず`bool`を返し、endpointが204 / 404へ変換しているのは、serviceとendpointの責務をどう分けるためですか。

回答:

serviceではDB更新および結果の返却を責務とし、endpointはHTTP Requestに対するレスポンスを返すことが責務であるため


## 問3

削除成功testが204と空bodyを確認した後、同じIDをGETして404も確認しているのはなぜですか。

回答:

DELTEリクエストのレスポンス上だけでなく, 実際にDBの値が削除されてアクセスできなくなっているかを確認するため。
