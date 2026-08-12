# EF Core とテスト

JPA/Hibernate の知識は活かせますが、EF Core の `DbContext` は通常リクエスト単位の scoped service として扱い、LINQ が SQL に翻訳される境界を意識します。`IQueryable<T>` のまま層をまたがせず、DB 問い合わせは必要な場所で完結させる方針が安全です。

## 推奨教材

- [EF Core の概要](https://learn.microsoft.com/ja-jp/ef/core/)
- [migration の概要](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/)
- [EF Core のテスト](https://learn.microsoft.com/ja-jp/ef/core/testing/)
- [.NET でのテスト](https://learn.microsoft.com/ja-jp/dotnet/core/testing/)
- [ASP.NET Core の統合テスト](https://learn.microsoft.com/ja-jp/aspnet/core/test/integration-tests?view=aspnetcore-10.0)

## 演習

第 3 フェーズの API に永続化を加え、migration を作成します。サービス層の単体テストと、`WebApplicationFactory` を使う API の統合テストをそれぞれ 1 本以上書きます。テストフレームワークは現場に合わせますが、新規学習なら xUnit を第一候補にします。
