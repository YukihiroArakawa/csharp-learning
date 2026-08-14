# 未処理例外をProblem Detailsへ変換する: 回答シート

教材: [07-problem-details.md](07-problem-details.md)

状態: 完了

## 問1

`AddProblemDetails`と`UseExceptionHandler`は、それぞれapplication起動時とrequest実行時にどのような役割を持ちますか。

回答:

application起動時に`AddProblemDetails`を呼ぶと、Problem Details responseを書き出すserviceがDI containerへ登録されます。

`builder.Build()`後に`UseExceptionHandler`を呼ぶと、例外処理middlewareがrequest pipelineへ追加されます。

未処理例外がmiddlewareまで戻ってきた場合は、例外を捕捉してstatus 500を設定し、登録済みのProblem Details serviceを使ってJSON responseを書き出します。

## 問2

`UseExceptionHandler`をendpoint mappingより前に登録しているのは、どの範囲の例外を捕捉するためですか。

回答:
middlewareは登録順に後続処理を包みます。

UseExceptionHandlerをendpoint mappingより前に置くことで、endpoint、service、EF Coreを含む後続処理から上がる例外を捕捉できます。

## 問3

500 responseへ`SqliteException`やstack traceを含めず、代わりに`traceId`を含めるのはなぜですか。

回答:

client側が問題解決のために有用な情報ではなく、セキュリティ上の問題となりうるため最低限traceIdのみを含めるようにしている。
