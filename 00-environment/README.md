# 開発環境と CLI

まず IDE 固有の操作ではなく `dotnet` CLI を押さえます。Java の Maven/Gradle に相当するのは `*.csproj` と `dotnet restore/build/test/run` であり、依存関係は主に NuGet で管理します。

- [.NET SDK の概要](https://learn.microsoft.com/ja-jp/dotnet/core/sdk)
- [dotnet CLI の概要](https://learn.microsoft.com/ja-jp/dotnet/core/tools/)
- [プロジェクト SDK の概要](https://learn.microsoft.com/ja-jp/dotnet/core/project-sdk/overview)

## 実施すること

1. `dotnet new console` と `dotnet new webapi` で雛形を作る。
2. `dotnet build`、`test`、`run`、`watch`、`format` を一度ずつ実行する。
3. `*.csproj` を開き、`TargetFramework`、`PackageReference`、`Nullable`、`ImplicitUsings` の役割を説明できるようにする。
