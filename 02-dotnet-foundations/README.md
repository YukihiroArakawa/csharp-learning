# ランタイムと汎用ホスト

JVM と CLR はともにマネージドランタイムですが、現場で効くのは実装の細部より、オブジェクトの寿命とアプリケーションの起動・停止の扱いです。

- GC は世代別で動き、`IDisposable` は GC の代替ではありません。ファイル、HTTP 応答、DB 接続などの**非マネージド資源・有限資源は明示的に破棄**します。
- Java の `try-with-resources` に近いのが `using` です。`HttpClient` はリクエストごとに生成・破棄せず、DI 経由で利用する慣習を確認します。
- `CancellationToken` は HTTP 切断やホスト停止を下流へ伝える標準的な仕組みです。I/O を伴うメソッドでは引数として受け渡す癖を付けます。
- Spring の `ApplicationContext` と同様に DI は中心的ですが、.NET は組み込みコンテナー、Generic Host、構造化ログ、Options パターンが一体で使われます。

## 推奨教材

- [ガベージ コレクションの基礎](https://learn.microsoft.com/ja-jp/dotnet/standard/garbage-collection/fundamentals)
- [IDisposable パターン](https://learn.microsoft.com/ja-jp/dotnet/standard/design-guidelines/dispose-pattern)
- [CancellationToken の概要](https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.cancellationtoken)
- [.NET の依存関係の挿入](https://learn.microsoft.com/ja-jp/dotnet/core/extensions/dependency-injection/overview)
- [.NET の構成](https://learn.microsoft.com/ja-jp/dotnet/core/extensions/configuration)
- [.NET のログ](https://learn.microsoft.com/ja-jp/dotnet/core/extensions/logging)

## 演習

`BackgroundService` を一つ作り、設定を Options として読み込み、`ILogger` で構造化ログを出し、停止時に `CancellationToken` で安全に終了させます。
