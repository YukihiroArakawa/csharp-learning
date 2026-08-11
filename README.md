# Java 経験者のための C# / .NET キャッチアップ

Java（および Spring Boot）を約 3 年経験した開発者が、次の現場で C# / .NET を安全に読み書き・変更できるようになるための学習リポジトリです。

## 方針

Java の基礎（オブジェクト指向、HTTP、DI、ORM、テスト）は既知とし、初歩的なチュートリアルを網羅しません。代わりに、Java と C# / .NET の**同じ名前だが挙動や慣習が異なる部分**を、小さなコードと Web API の実装で確かめます。

目標は「C# が書ける」ことではなく、既存の ASP.NET Core アプリケーションを読んで、設計上の理由を把握し、保守的な変更をテスト付きで出せることです。

## 学習の全体像

| フェーズ | 目安 | 主題 | 到達基準 |
| --- | ---: | --- | --- |
| 0. 開発環境 | 半日 | SDK、CLI、プロジェクト構造 | `dotnet` CLI で作成・ビルド・テスト・実行できる |
| 1. C# の型と構文 | 1〜2 週 | null、安全な値型、record、LINQ、非同期 | Java との差分を説明して小さなコードを書ける |
| 2. .NET ランタイムと標準基盤 | 1 週 | GC、例外、`Task`、`CancellationToken`、設定・ログ・DI | リソースとライフサイクルを意識して実装できる |
| 3. ASP.NET Core | 2 週 | hosting、middleware、DI、binding、認可、エラー処理 | CRUD API をフレームワークの流儀で実装できる |
| 4. データアクセスとテスト | 1〜2 週 | EF Core、migration、テスト | DB を伴う変更をテスト・migration 付きで出せる |
| 5. 現場適応 | 継続 | 設定、観測性、運用、既存コード読解 | 対象現場のリポジトリで小さな変更を完結できる |

週数は平日 1 時間程度を想定しています。配属までの時間が短ければ、フェーズ 1・3・4 を優先します。

## 2026-09-01 配属前の優先計画

このREADMEの全TODOは配属後も含む学習計画であり、9月1日までに全てを完了させる計画ではありません。配属前は、既存の ASP.NET Core アプリケーションを読んで小さな変更を安全に出すための最短経路を優先します。

週末は学習時間として計画に含めません。8月10日から31日までの平日16日のうち、12回の学習セッション（各60〜90分、合計12〜18時間）で必須コアを完了させ、残る4平日は欠席・復習・詰まりのためのバッファとします。週末に学習できた場合だけ、後述の「配属後へ繰り越せる項目」を前倒しします。

### マイルストーン

| 期限 | 到達基準 | 優先度 | 遅れた場合の扱い |
| --- | --- | --- | --- |
| 8/7 | フェーズ1前半（型、property、LINQ、resource disposal）を完了 | 完了 | 基準点 |
| 8/14 | `Task` / `async` / `await`、`CancellationToken`、パターンマッチング、最小テストを一通り書く | 必須 | 翌週前半のバッファで回復。高度な構文の深掘りはしない |
| 8/21 | Generic Host、DI lifetime、設定 / Options、構造化ログを一つの Worker でつなげる | 必須 | Workerの発展課題を止め、DI・設定・ログの理解を優先 |
| 8/28 | Minimal API の GET / POST、binding、validation、Problem Details、OpenAPI を実装する | 必須 | Controller比較・認証認可を配属後へ繰り越す |
| 8/31 | 未完了の必須項目を回収し、対象プロジェクトの `build` / `test` / `format` を通す | 必須 | 新機能を増やさず、復習と現場リポジトリの確認に切り替える |

期限は**後ろ倒しにしない**。マイルストーンに遅れたら、次の優先度の低い項目を配属後へ移し、必須コアの時間を守ります。

### 学習する順序

1. フェーズ1の残り: 非同期、キャンセル、パターン、テスト。以後の .NET / Web API を読むための言語基礎にする。
2. フェーズ2の最小縦切り: Worker で Host、DI、設定、Options、ログ、停止時キャンセルを一度につなげる。
3. フェーズ3の最小縦切り: Minimal API でHTTP入力から検証・エラー応答・OpenAPIまでをつなげる。
4. 余裕がある場合だけフェーズ4: EF Core、migration、SQL変換、テストを扱う。
5. capstone、認証認可の深掘り、Controller形式の比較、Neovim DAPの作り込みは配属後の現場コードを題材にする。

### 毎回の進め方

- 学習できる日に「次へ」と伝える。Codexは当日の残り時間を前提に、現在のマイルストーンの完了に最も寄与する15〜45分の課題を一つだけ案内する。
- 各マイルストーンの期限日に、完了・未完了・繰り越しをREADMEへ記録する。未完了でも必須コアを優先し、週末の学習を前提に回復計画を立てない。
- 8月31日は新しい課題を始めない。`dotnet build`、`dotnet test`、`dotnet format --verify-no-changes`、および現場で使う構成の確認に使う。

## ディレクトリ構成

フェーズごとにディレクトリを分けます。チェック項目ごとに必ずプロジェクトを作るのではなく、関連する差分を一つの小さなプロジェクトにまとめます。`01` は検証用の小さなサンプル群、`04` 以降は実際に動く API を育てる場所です。

```text
.
├── .envrc                         # リポジトリ横断ツール用の direnv 設定
├── .githooks/pre-commit           # C# の staged changes をフォーマット検証
├── flake.nix                      # 横断ツール用（dotnet format）
├── README.md
├── 00-environment/
│   ├── .envrc                        # direnv でこのフェーズの flake を有効化
│   ├── flake.nix                     # .NET SDK
│   └── hello-dotnet/                 # CLI、csproj、基本コマンドの確認
├── 01-csharp-differences/
│   ├── .envrc
│   ├── flake.nix                     # .NET SDK、C# LSP
│   ├── TypeSystemSamples/            # null、class / struct / record、property
│   ├── LinqSamples/                  # IEnumerable、LINQ、遅延実行
│   ├── AsyncSamples/                 # Task、async / await、CancellationToken
│   └── CSharpDifferences.Tests/      # 差分の挙動を固定するテスト
├── 02-dotnet-foundations/
│   ├── .envrc
│   ├── flake.nix                     # .NET SDK、C# LSP、デバッガ
│   └── HostedWorker/                 # Generic Host、DI、Options、ログ、停止処理
├── 03-aspnet-core/
│   ├── .envrc
│   ├── flake.nix                     # .NET SDK、C# LSP、デバッガ
│   ├── MinimalApiSample/             # Minimal API の基本
│   └── ControllerApiSample/          # Controller 形式との比較
├── 04-data-and-testing/
│   ├── .envrc
│   ├── flake.nix                     # .NET SDK、C# LSP、デバッガ、SQLite CLI
│   └── TodoApiWithEfCore/            # EF Core、migration、単体・統合テスト
└── 05-capstone/
    ├── .envrc
    ├── flake.nix                     # .NET SDK、C# LSP、デバッガ、SQLite CLI
    └── TaskManagementApi/            # 最終課題の業務 API
```

プロジェクトを分けるのは「別の起動方法・依存関係・用途が必要になったとき」です。たとえば nullable と record の比較は `TypeSystemSamples` に同居させ、Worker と Web API は別プロジェクトにします。各ディレクトリには、そのサンプルで確認する差分と実行コマンドを書いた小さな README を置きます。

### Nix / flake の方針

開発ツールは原則としてフェーズごとの `flake.nix` で管理します。これにより「この演習で何が必要か」を依存関係からも把握できます。各フェーズには `.envrc` があり、ディレクトリへ入るだけで対応する開発環境が有効になります。

```bash
cd 01-csharp-differences
direnv allow
dotnet --info
```

`.envrc` の初回利用時、または内容を変更した後だけ `direnv allow` を実行します。以後は対象ディレクトリへ `cd` するだけでよく、手動の `nix develop` は不要です。

各フェーズの依存関係は次のように増やします。

| フェーズ | 依存関係 | 追加する理由 |
| --- | --- | --- |
| `00-environment` | `dotnet-sdk_10` | CLI、テンプレート、ビルド、テストを試す |
| `01-csharp-differences` | SDK + `csharp-ls` | 言語サンプルを Neovim で読む・書く |
| `02-dotnet-foundations` | SDK + LSP + `netcoredbg` | Worker の起動・停止をデバッグする |
| `03-aspnet-core` | SDK + LSP + デバッガ | HTTP リクエストを受ける API をデバッグする |
| `04-data-and-testing` | SDK + LSP + デバッガ + `sqlite` | EF Core が作る SQLite DB と migration を観察する |
| `05-capstone` | `04` と同じ | 最終課題で API・DB・デバッグを一通り使う |

各 flake は独立した `flake.lock` を持てます。最初の `direnv allow` で作成・固定します。意図的にフェーズ単位で更新できる一方、SDK バージョンを揃えたい場合は、すべての flake で同じ `nixpkgs` revision を使うよう lock を更新します。

### フォーマット検証フック

ルートの `flake.nix` はフェーズ固有ではない横断ツール用です。`dotnet format` を使う Git の pre-commit フックを提供しますが、各フェーズが必要とする LSP、デバッガ、SQLite などの依存は引き続き各フェーズの flake に置きます。

初回だけリポジトリ直下で次を実行します。

```bash
direnv allow
git config --local core.hooksPath .githooks
```

以後、ステージ済みの C# ファイルを含むコミット時に、全 `.csproj` に対して `dotnet format --verify-no-changes --no-restore` が実行されます。整形が必要ならコミットは中止されるため、先に対象プロジェクトで次を実行します。

```bash
dotnet format TypeSystemSamples/TypeSystemSamples.csproj
```

### Neovim のセットアップ

フル IDE は不要です。対象フェーズで direnv により環境を有効にした状態で Neovim を起動し、C# ファイルを開くと `csharp-ls` が attach する構成にします。既存の Neovim 設定で `csharp-ls` は有効になっています。

- [ ] `01-csharp-differences` で `direnv allow` を実行し、`dotnet --info` と `csharp-ls --version` を実行する。
- [ ] `02-dotnet-foundations` で `netcoredbg --version` を実行する。
- [ ] `*.cs` ファイルを開き、`:checkhealth vim.lsp` で LSP の状態を確認する。
- [ ] `gd`（定義ジャンプ）、`gr`（参照検索）、`K`（ホバー）、`<leader>rn`（リネーム）、`<leader>ca`（コードアクション）を試す。
- [ ] Neovim の DAP プラグインを導入・設定し、`netcoredbg` で Console アプリにブレークポイントを張る。
- [ ] `:make` または Neovim のターミナルから `dotnet test` を実行し、失敗箇所へ移動できるようにする。

## TODO リスト

進捗管理用のチェックリストです。教材を読むだけで終えず、各項目のコードまたは成果物をこのリポジトリに残します。

### 0. 開発環境と CLI

- [x] `00-environment` の `direnv allow` で .NET SDK を利用できるようにする。
- [x] `dotnet --info` で SDK / runtime の構成を確認する。
- [x] `dotnet new console` でプロジェクトを作り、`build` と `run` を実行する。
- [x] `dotnet new webapi` でプロジェクトを作り、`run` して OpenAPI UI またはエンドポイントを確認する。
- [x] `dotnet restore`、`build`、`test`、`watch`、`format` を実行する。
- [x] `*.csproj` の `TargetFramework`、`PackageReference`、`Nullable`、`ImplicitUsings` を説明できる。

### 1. C# の型と構文

- [x] nullable reference types（`string` / `string?`）とコンパイラー警告を確認する。
- [x] `class`、`struct`、`record`、`record struct` の等価性と代入時の挙動を比較する。
- [x] auto-property、`init`、primary constructor を使った型を読む・書く。
- [x] `using` / `await using` で `IDisposable` / `IAsyncDisposable` を確実に破棄する。
- [x] LINQ の `Select`、`Where`、`GroupBy`、`FirstOrDefault` を使い、遅延実行を確認する。
- [x] `IEnumerable<T>` と `IQueryable<T>` の違いを説明できる。
- [x] `async` / `await` と `Task` を用いた I/O 処理を実装する。
- [x] `CancellationToken` を受け取り、キャンセル可能な処理を実装する。
- [x] `switch` 式とパターンマッチングを用いた分岐を書く。
- [x] 上記差分の最小コードにテストを付ける。

### 2. ランタイムと標準基盤

- [ ] GC と `IDisposable` の責務の違いを説明できる。
- [x] Generic Host を用いた `BackgroundService` を一つ実装する。
- [x] DI に singleton / scoped / transient を登録し、各 lifetime を説明できる。
- [x] scoped service を singleton に注入できない理由を確認する。
- [x] `appsettings.json` と環境変数から設定を読み込む。
- [x] Options パターンで設定を型安全に受け取る。
- [x] `ILogger<T>` で構造化ログを出力する。
- [x] ホスト停止時に `CancellationToken` を使って処理を終了させる。

### 3. ASP.NET Core

- [x] Minimal API で GET / POST エンドポイントを作る。
- [ ] Controller ベースの API を読み、Minimal API との使い分けを説明できる。
- [x] route / query / body のモデルバインディングを実装する。
- [ ] 入力検証と適切な HTTP ステータスコードを実装する。
- [ ] middleware を追加し、登録順による挙動の違いを確認する。
- [ ] 例外を Problem Details 形式で統一する。
- [ ] OpenAPI を有効にして API 仕様を確認する。
- [ ] 認証と認可の違い、および policy / role の使い分けを説明できる。

### 4. EF Core とテスト

- [ ] `DbContext`、entity、relation を定義する。
- [ ] migration を作成し、DB に適用する。
- [ ] LINQ クエリが SQL に翻訳される箇所を確認する。
- [ ] `DbContext` を scoped として扱う理由を説明できる。
- [ ] サービス層の単体テストを 1 本以上書く。
- [ ] `WebApplicationFactory` を使った API の統合テストを 1 本以上書く。
- [ ] `dotnet test` を実行し、すべて成功する状態にする。

### 5. 最終課題と現場適応

- [ ] タスク管理 API の CRUD とページングを実装する。
- [ ] 永続化、migration、入力検証、統一エラー応答を追加する。
- [ ] 設定、Options、構造化ログ、キャンセル伝播を組み込む。
- [ ] 単体テストと統合テストを追加する。
- [ ] README に起動方法、必要な設定値、設計上の判断を書く。
- [ ] 現場の `TargetFramework`、Web API の形式、ORM、認証、テスト、CI/CD の構成を確認する。
- [ ] 現場の 1 リクエストを入口から DB・ログ・テストまで追跡する。

## 0. 開発環境と CLI

まず IDE 固有の操作ではなく `dotnet` CLI を押さえます。Java の Maven/Gradle に相当するのは `*.csproj` と `dotnet restore/build/test/run` であり、依存関係は主に NuGet で管理します。

- [ .NET SDK の概要](https://learn.microsoft.com/ja-jp/dotnet/core/sdk)
- [dotnet CLI の概要](https://learn.microsoft.com/ja-jp/dotnet/core/tools/)
- [プロジェクト SDK の概要](https://learn.microsoft.com/ja-jp/dotnet/core/project-sdk/overview)

実施すること:

1. `dotnet new console` と `dotnet new webapi` で雛形を作る。
2. `dotnet build`、`test`、`run`、`watch`、`format` を一度ずつ実行する。
3. `*.csproj` を開き、`TargetFramework`、`PackageReference`、`Nullable`、`ImplicitUsings` の役割を説明できるようにする。

## 1. C# の差分を集中的に学ぶ

教材の主軸は [Java 開発者向け C#](https://learn.microsoft.com/ja-jp/dotnet/csharp/tour-of-csharp/tips-for-java-developers) と [C# のツアー](https://learn.microsoft.com/ja-jp/dotnet/csharp/tour-of-csharp/) です。読みながら、下表の項目ごとに Java と C# の最小例を作り、テストで挙動を固定します。

| 観点 | Java から特に意識する差分 | 優先度 |
| --- | --- | --- |
| 型と null | `string` と `string?`、nullable reference types はコンパイル時の解析。`int?` は nullable value type | 最優先 |
| 値と参照 | `struct`、`record struct`、boxing、`ref` / `in` / `out`。`record` は値ベース等価性を持つ | 最優先 |
| プロパティ | field + getter/setter の代わりに property を公開する。`init`、primary constructor も読む | 最優先 |
| コレクションとクエリ | LINQ の遅延実行、`IEnumerable<T>` と `IQueryable<T>`、拡張メソッド | 最優先 |
| 非同期 | `Task` / `ValueTask` と `async` / `await`。`CompletableFuture` のように明示的な合成より直列に書く場面が多い | 最優先 |
| 例外とリソース | checked exception はない。`using` / `await using` と `IDisposable` / `IAsyncDisposable` が重要 | 高 |
| パターン | `switch` 式、property/list pattern、型パターン、null pattern | 高 |
| ジェネリクス | reified generics、制約、共変・反変。Java の型消去との違いを確認する | 中 |

特に `var` は「動的型」ではなく静的型推論です。また、`IEnumerable<T>` に対する LINQ は**実行時まで評価されない**ことがあるため、DB 接続や列挙回数を含めて確認します。

推奨教材:

- [nullable reference types](https://learn.microsoft.com/ja-jp/dotnet/csharp/nullable-references)
- [record 型](https://learn.microsoft.com/ja-jp/dotnet/csharp/language-reference/builtin-types/record)
- [LINQ の概要](https://learn.microsoft.com/ja-jp/dotnet/csharp/linq/)
- [非同期プログラミング](https://learn.microsoft.com/ja-jp/dotnet/csharp/asynchronous-programming/)
- [パターン マッチング](https://learn.microsoft.com/ja-jp/dotnet/csharp/fundamentals/functional/pattern-matching)

## 2. ランタイムと汎用ホスト

JVM と CLR はともにマネージドランタイムですが、現場で効くのは実装の細部より、オブジェクトの寿命とアプリケーションの起動・停止の扱いです。

- GC は世代別で動き、`IDisposable` は GC の代替ではありません。ファイル、HTTP 応答、DB 接続などの**非マネージド資源・有限資源は明示的に破棄**します。
- Java の `try-with-resources` に近いのが `using` です。`HttpClient` はリクエストごとに生成・破棄せず、DI 経由で利用する慣習を確認します。
- `CancellationToken` は HTTP 切断やホスト停止を下流へ伝える標準的な仕組みです。I/O を伴うメソッドでは引数として受け渡す癖を付けます。
- Spring の `ApplicationContext` と同様に DI は中心的ですが、.NET は組み込みコンテナー、Generic Host、構造化ログ、Options パターンが一体で使われます。

推奨教材:

- [ガベージ コレクションの基礎](https://learn.microsoft.com/ja-jp/dotnet/standard/garbage-collection/fundamentals)
- [IDisposable パターン](https://learn.microsoft.com/ja-jp/dotnet/standard/design-guidelines/dispose-pattern)
- [CancellationToken の概要](https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.cancellationtoken)
- [.NET の依存関係の挿入](https://learn.microsoft.com/ja-jp/dotnet/core/extensions/dependency-injection/overview)
- [.NET の構成](https://learn.microsoft.com/ja-jp/dotnet/core/extensions/configuration)
- [.NET のログ](https://learn.microsoft.com/ja-jp/dotnet/core/extensions/logging)

演習: `BackgroundService` を一つ作り、設定を Options として読み込み、`ILogger` で構造化ログを出し、停止時に `CancellationToken` で安全に終了させます。

## 3. Spring Boot と対比して ASP.NET Core を学ぶ

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

順番:

1. Minimal API で GET/POST の小さな API を作り、route、binding、validation、HTTP ステータスを確認する。
2. 同じ API を Controller 形式で読み、現場で用いられている形式を選ぶ。
3. middleware を一つ追加し、順序を変えたときの差を確認する。
4. `ProblemDetails` による例外処理、認証・認可、OpenAPI を追加する。

推奨教材:

- [Web API を作成するチュートリアル](https://learn.microsoft.com/ja-jp/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0)
- [middleware](https://learn.microsoft.com/ja-jp/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0)
- [構成](https://learn.microsoft.com/ja-jp/aspnet/core/fundamentals/configuration/?view=aspnetcore-10.0)
- [エラー処理](https://learn.microsoft.com/ja-jp/aspnet/core/fundamentals/error-handling?view=aspnetcore-10.0)
- [認可](https://learn.microsoft.com/ja-jp/aspnet/core/security/authorization/introduction?view=aspnetcore-10.0)

## 4. EF Core とテスト

JPA/Hibernate の知識は活かせますが、EF Core の `DbContext` は通常リクエスト単位の scoped service として扱い、LINQ が SQL に翻訳される境界を意識します。`IQueryable<T>` のまま層をまたがせず、DB 問い合わせは必要な場所で完結させる方針が安全です。

- [EF Core の概要](https://learn.microsoft.com/ja-jp/ef/core/)
- [migration の概要](https://learn.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/)
- [EF Core のテスト](https://learn.microsoft.com/ja-jp/ef/core/testing/)
- [.NET でのテスト](https://learn.microsoft.com/ja-jp/dotnet/core/testing/)
- [ASP.NET Core の統合テスト](https://learn.microsoft.com/ja-jp/aspnet/core/test/integration-tests?view=aspnetcore-10.0)

演習: 第 3 フェーズの API に永続化を加え、migration を作成します。サービス層の単体テストと、`WebApplicationFactory` を使う API の統合テストをそれぞれ 1 本以上書きます。テストフレームワークは現場に合わせますが、新規学習なら xUnit を第一候補にします。

## 5. 最終課題: 小さな業務 API

「タスク管理」程度の小さな API を、次の条件で完成させます。機能の多さより、.NET の流儀を一巡させることを優先します。

- `TaskItem` の CRUD、ページング、入力検証
- EF Core + RDB、migration
- DI lifetime を明示したサービス登録
- `appsettings.json` と環境変数を用いた設定、Options パターン
- 構造化ログ、統一された Problem Details のエラー応答
- `CancellationToken` を DB / I/O 処理まで渡す
- 単体テストと統合テスト、`dotnet test` が成功する状態
- README に起動方法、設定値、設計上の判断を書く

## 現場に入ったら最初に確認すること

使用している .NET / C# のバージョン、ASP.NET Core のスタイル（Controller / Minimal API）、ORM、認証方式、DI lifetime の規約、例外・ログ・エラー応答の統一方法、テスト実行方法、CI/CD とコンテナ化の有無を確認します。バージョンによってテンプレートや推奨 API は変わるため、教材は現場の `TargetFramework` を基準に読み替えます。

## 学び方のルール

1. Java との対応表を暗記しない。差が出る短いコードを書き、出力・テスト・SQL を観察する。
2. 新しい構文は「いつ使うか」と「避ける条件」までメモする。特に `async`、LINQ、`record`、nullable はレビューで問われやすい。
3. 現場のコードが手に入ったら、同じ機能を教材で再実装するより、1 リクエストを入口から DB・ログ・テストまで追う。
4. 外部ライブラリを早期に増やさず、まず `Microsoft.Extensions.*` と ASP.NET Core の標準機能を理解する。
