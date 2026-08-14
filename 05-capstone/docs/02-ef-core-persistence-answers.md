# EF Coreでページング結果を永続DBから取得する: 回答シート

教材: [02-ef-core-persistence.md](02-ef-core-persistence.md)

状態: 完了

## 問1

一回の正常な一覧requestで、serviceがSQLiteへ二つのqueryを送るのはなぜですか。それぞれ何を取得しますか。

回答:

タスクの総数、ページ分割されたタスクの要素をそれぞれ取得している
クライアントサイドにタスクの総数を返したいため、ページ以外に総数もクエリしている

## 問2

production用`TaskDbContext`をtestでそのまま使わず、`TaskManagementApiFactory`が登録を差し替える理由を説明してください。

回答:

テスト順序やlocal環境のテスト状況にテスト結果が左右されないようにするため

## 問3

`CountAsync`と`ToListAsync`の両方へ同じ`CancellationToken`を渡す理由を説明してください。

回答:

クライアント切断を感知したCancellationTokenを渡してクエリを取り消す必要があるため
