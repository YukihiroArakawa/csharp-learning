# C# の差分を集中的に学ぶ

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

## 推奨教材

- [nullable reference types](https://learn.microsoft.com/ja-jp/dotnet/csharp/nullable-references)
- [record 型](https://learn.microsoft.com/ja-jp/dotnet/csharp/language-reference/builtin-types/record)
- [LINQ の概要](https://learn.microsoft.com/ja-jp/dotnet/csharp/linq/)
- [非同期プログラミング](https://learn.microsoft.com/ja-jp/dotnet/csharp/asynchronous-programming/)
- [パターン マッチング](https://learn.microsoft.com/ja-jp/dotnet/csharp/fundamentals/functional/pattern-matching)
