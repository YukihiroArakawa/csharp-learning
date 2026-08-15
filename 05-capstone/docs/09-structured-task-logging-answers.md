# task操作を構造化logへ記録する: 回答シート

教材: [09-structured-task-logging.md](09-structured-task-logging.md)

状態: 完了

## 問1

`$"Created task {task.Id}"`ではなく`"Created task {TaskId}"`と`task.Id`を別々にloggerへ渡すと、logの分析時に何ができるようになりますか。

回答:

log基盤などで、log eventに対して以下のようにプロパティベースでログを検索できるようになるため、特定のログレベル、TaskIdなどで絞り込みなどを行いやすくなる。

Level      = Information
Category   = TaskManagementApi.Services.TaskCommandService
Template   = Created task {TaskId}
TaskId     = 6

## 問2

作成・更新・削除の成功logを`SaveChangesAsync`より後に置く必要があるのはなぜですか。

回答:

DB更新が確定した情報をlogに残すことで、logとdbの事実を一致させる必要があるため。

## 問3

task titleをlogへ含めず、`TaskId`と必要な状態だけに絞っているのはなぜですか。

回答:

logの情報量が増えることで検索時の速度、ストレージ容量の圧迫などの弊害があるため。
また個人情報流出の観点でもtask titleをログ出力することは好ましくない。
