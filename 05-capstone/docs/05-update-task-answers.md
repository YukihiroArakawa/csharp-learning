# taskを更新して200 / 400 / 404を返す: 回答シート

教材: [05-update-task.md](05-update-task.md)

状態: 完了

## 問1

titleのvalidationをtask検索より先に行うため、不正なtitleと存在しないIDを同時に指定したrequestは何statusになりますか。また、その時database検索は行われますか。

回答:

status 400 bad request will be returned.
and database access will not be done.

## 問2

`SingleOrDefaultAsync`で取得したentityのpropertyを書き換えた後、`dbContext.Tasks.Update(task)`を呼ばなくても`SaveChangesAsync`がUPDATEできるのはなぜですか。

回答:
`SingleOrDefaultAsync`で取得した`TaskItem`は、既定では`TaskDbContext`のchange trackerに登録されるため、この時点でメモリの値が書き換わる。

その後に`SaveChangesAsync`を呼ぶと、EF Coreは追跡中entityの変更を検出し、必要なUPDATEをdatabaseへ送るため、明示的にdbContextのUpdateを呼び直す必要がない。

## 問3

正常系testがPUT responseを確認した後、同じIDをGETしているのは何を区別するためですか。

回答:

PUTレスポンスの値と実際にDBに更新された値を区別するため。
