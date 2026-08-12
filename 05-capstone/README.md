# 最終課題: 小さな業務 API

「タスク管理」程度の小さな API を、次の条件で完成させます。機能の多さより、.NET の流儀を一巡させることを優先します。

- `TaskItem` の CRUD、ページング、入力検証
- EF Core + RDB、migration
- DI lifetime を明示したサービス登録
- `appsettings.json` と環境変数を用いた設定、Options パターン
- 構造化ログ、統一された Problem Details のエラー応答
- `CancellationToken` を DB / I/O 処理まで渡す
- 単体テストと統合テスト、`dotnet test` が成功する状態
- README に起動方法、設定値、設計上の判断を書く
