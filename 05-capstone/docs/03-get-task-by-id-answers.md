# IDを指定してtaskを1件取得する: 回答シート

教材: [03-get-task-by-id.md](03-get-task-by-id.md)

状態: 完了

## 問1

`/tasks/999`に対応するrowが存在しない場合、serviceとendpointはそれぞれ何を返しますか。

回答:

serviceはnullであるTaskSummary?を返す
endpointは404を返す

## 問2

`SingleOrDefaultAsync`が2件以上の結果で例外になるにもかかわらず、今回は安全に使える理由を説明してください。

回答: pkでクエリしているため.


## 問3

`GetTaskAsync`で`async`と`await`を使わず、EF Coreの`Task<TaskSummary?>`をそのまま返せる理由を説明してください。

回答:

Task<>が非同期の操作で値が返されることを示すラッパーであるため
