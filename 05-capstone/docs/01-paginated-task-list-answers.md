# ページングされたtask一覧：回答シート

教材: [01-paginated-task-list.html](01-paginated-task-list.html)

状態: 完了

## 問1

`page=3&pageSize=2`を指定した場合、`Skip`する件数と返るtask IDを答えてください。

回答:

`.Skip((page - 1) * pageSize)`より4件スキップされる
全体の件数は5件で1から始まる連番なので、task id=5が返ってくる

## 問2

`TotalCount`へ`items.Count`ではなく`tasks.Count`を設定する理由を説明してください。

回答: itemsはclientにreturnする絞り込まれたtaskの数であり、taskの全体数を返したい場合はtasks.Countを設定する必要があるため

## 問3

`page=4&pageSize=2`を404ではなく、200と空の`items`で返す設計上の理由を説明してください。

回答:リクエスト形式自体は正しいため200と空のitemsを返すという設計になっていると思われる。
