# Spring Boot と対比して ASP.NET Core を学ぶ

まず [ASP.NET Core の基本](https://learn.microsoft.com/ja-jp/aspnet/core/fundamentals/?view=aspnetcore-10.0) を読み、次の対応を頭に置きます。ただし名称が似ていても完全な一対一対応ではありません。

| Spring Boot | ASP.NET Core | 学ぶポイント |
| --- | --- | --- |
| `main` + ApplicationContext | `WebApplicationBuilder` / Generic Host | 起動時にサービスとパイプラインを組み立てる |
| Filter / Interceptor | Middleware / Filter | middleware の登録順がリクエスト処理順を決める |
| `@RestController` | Controller または Minimal API | 現場の採用スタイルに合わせる。両方を読めるようにする |
| Spring DI | 組み込み DI (`AddSingleton` / `AddScoped` / `AddTransient`) | lifetime、特に scoped を理解する |
| `application.yml` / `@ConfigurationProperties` | `appsettings.json` + environment variables + Options | プロバイダーの優先順位と環境別設定 |
| `@ControllerAdvice` | Exception-handling middleware / Problem Details | エラー形式を一箇所で統一する |
| Spring Security | Authentication / Authorization middleware | 認証と認可、policy / role の分離 |

## 順番

1. Minimal API で GET/POST の小さな API を作り、route、binding、validation、HTTP ステータスを確認する。
2. 同じ API を Controller 形式で読み、現場で用いられている形式を選ぶ。
3. middleware を一つ追加し、順序を変えたときの差を確認する。
4. `ProblemDetails` による例外処理、認証・認可、OpenAPI を追加する。

## 推奨教材

- [Web API を作成するチュートリアル](https://learn.microsoft.com/ja-jp/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0)
- [middleware](https://learn.microsoft.com/ja-jp/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0)
- [構成](https://learn.microsoft.com/ja-jp/aspnet/core/fundamentals/configuration/?view=aspnetcore-10.0)
- [エラー処理](https://learn.microsoft.com/ja-jp/aspnet/core/fundamentals/error-handling?view=aspnetcore-10.0)
- [認可](https://learn.microsoft.com/ja-jp/aspnet/core/security/authorization/introduction?view=aspnetcore-10.0)
