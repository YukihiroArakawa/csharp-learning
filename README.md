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
5. capstone、認証認可の深掘り、Controller形式の比較は配属後の現場コードを題材にする。

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
| `01-csharp-differences` | SDK + `csharp-ls` | 言語サンプルのC#編集支援を有効にする |
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

- [x] GC と `IDisposable` の責務の違いを説明できる。
- [x] Generic Host を用いた `BackgroundService` を一つ実装する。
- [x] DI に singleton / scoped / transient を登録し、各 lifetime を説明できる。
- [x] scoped service を singleton に注入できない理由を確認する。
- [x] `appsettings.json` と環境変数から設定を読み込む。
- [x] Options パターンで設定を型安全に受け取る。
- [x] `ILogger<T>` で構造化ログを出力する。
- [x] ホスト停止時に `CancellationToken` を使って処理を終了させる。

### 3. ASP.NET Core

- [x] Minimal API で GET / POST エンドポイントを作る。
- [x] Controller ベースの API を読み、Minimal API との使い分けを説明できる。
- [x] route / query / body のモデルバインディングを実装する。
- [x] 入力検証と適切な HTTP ステータスコードを実装する。
- [x] middleware を追加し、登録順による挙動の違いを確認する。
- [x] 例外を Problem Details 形式で統一する。
- [x] OpenAPI を有効にして API 仕様を確認する。
- [x] 認証と認可の違い、および policy / role の使い分けを説明できる。

### 4. EF Core とテスト

- [x] `DbContext`、entity、relation を定義する。
- [x] migration を作成し、DB に適用する。
- [x] LINQ クエリが SQL に翻訳される箇所を確認する。
- [x] `DbContext` を scoped として扱う理由を説明できる。
- [x] サービス層の単体テストを 1 本以上書く。
- [x] `WebApplicationFactory` を使った API の統合テストを 1 本以上書く。
- [x] `dotnet test` を実行し、すべて成功する状態にする。

### 5. 最終課題と現場適応

- [x] task 一覧のページングと query parameter 検証を実装する。
- [x] ページング API の統合テストを追加する。
- [x] EF Core による永続化と initial migration を追加する。
- [x] task を ID で取得する endpoint を実装する。
- [x] task を作成する endpoint と入力検証を実装する。
- [x] task を更新する endpoint と入力検証を実装する。
- [x] task を削除する endpoint を実装する。
- [x] 例外応答を Problem Details 形式へ統一する。
- [x] Options パターンで設定値を検証する。
- [x] task 操作を構造化ログへ記録する。
- [x] CancellationToken を DB / I/O 処理まで伝播する。
- [ ] サービス層の単体テストを追加する。
- [ ] CRUD API の統合テストを追加する。
- [ ] `dotnet test` ですべて成功する状態にする。
- [ ] README に起動方法、必要な設定値、設計上の判断を書く。
- [ ] 現場の `TargetFramework`、Web API の形式、ORM、認証、テスト、CI/CD の構成を確認する。
- [ ] 現場の 1 リクエストを入口から DB・ログ・テストまで追跡する。

## 現場に入ったら最初に確認すること

使用している .NET / C# のバージョン、ASP.NET Core のスタイル（Controller / Minimal API）、ORM、認証方式、DI lifetime の規約、例外・ログ・エラー応答の統一方法、テスト実行方法、CI/CD とコンテナ化の有無を確認します。バージョンによってテンプレートや推奨 API は変わるため、教材は現場の `TargetFramework` を基準に読み替えます。

## 学び方のルール

1. Java との対応表を暗記しない。差が出る短いコードを書き、出力・テスト・SQL を観察する。
2. 新しい構文は「いつ使うか」と「避ける条件」までメモする。特に `async`、LINQ、`record`、nullable はレビューで問われやすい。
3. 現場のコードが手に入ったら、同じ機能を教材で再実装するより、1 リクエストを入口から DB・ログ・テストまで追う。
4. 外部ライブラリを早期に増やさず、まず `Microsoft.Extensions.*` と ASP.NET Core の標準機能を理解する。
