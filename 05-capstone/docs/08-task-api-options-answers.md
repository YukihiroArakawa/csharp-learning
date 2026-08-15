# ページング設定をOptionsパターンで検証する: 回答シート

教材: [08-task-api-options.md](08-task-api-options.md)

状態: 完了

## 問1

Options validationとquery parameter validationは、それぞれ誰が用意した何の値を、いつ検証し、失敗時にどうなりますか。

回答:

- Options validation: 運用者が用意したapplication設定値をapplication起動時に検証し、失敗時はapplication起動しない。
- query parameter validation: clientがrequestに乗せたリクエストパラメータなどの入力値を、request実行時に検証し、失敗時はエラーレスポンスをclientに返す

## 問2

`DefaultPageSize`と`MaxPageSize`がそれぞれ正の整数でも、`DefaultPageSize <= MaxPageSize`を別途検証する必要があるのはなぜですか。

回答:

現在の実装では、最大ページサイズをデフォルトページサイズが上回るという業務エラーは型レベルでは検証できていないため、別途検証する必要がある。

## 問3

`pageSize`を`int?`で受けて`pageSize ?? settings.DefaultPageSize`とすることで、clientが値を指定した場合と省略した場合はそれぞれどう処理されますか。

回答:

値を設定した場合はpageSizeが利用され、設定されてない場合はsettings.DefaultPageSizeが利用される。
