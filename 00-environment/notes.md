## csprojの比較

diffで差分を出力

```bash
yukihiro@nixos ~/W/c/00-environment (main)> diff -u hello-dotnet/HelloDotnet.csproj hello-web-api/*.csproj
--- hello-dotnet/HelloDotnet.csproj	2026-08-05 10:43:39.847388072 +0900
+++ hello-web-api/HelloWebApi.csproj	2026-08-05 10:53:27.290562270 +0900
@@ -1,10 +1,13 @@
-﻿<Project Sdk="Microsoft.NET.Sdk">
+<Project Sdk="Microsoft.NET.Sdk.Web">

   <PropertyGroup>
-    <OutputType>Exe</OutputType>
     <TargetFramework>net10.0</TargetFramework>
-    <ImplicitUsings>enable</ImplicitUsings>
     <Nullable>enable</Nullable>
+    <ImplicitUsings>enable</ImplicitUsings>
   </PropertyGroup>

+  <ItemGroup>
+    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.10" />
+  </ItemGroup>
+
 </Project>
```

- TargetFramework: ここは変わり無し。どちらもnet10.0ということで、おそらくバージョン10.0の.NETをつかっていることを表している
- PackageReference: hello-web-apiのプロジェクトでは`Microsoft.AspNetCore.OpenApi`というライブラリを含んでいるように見える。APIを構築するために必要な依存だと思われる。
- Nullable: どちらも`enable`になっているので、null許容の設定だと思われる。project設定でnull許容かどうかを選択できると知らなかった。Javaだとなかった気がする。
- ImplicitUsings: hello-web-apiのプロジェクトではenableとなっていた。どのような機能なのか知らない。教えてほしい。
- OutputType: hello-dotnetでは`Exe`となっていた. microsoft learnによるとコンパイラーオプションみたい。https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/output . .exeファイルを作るという意味らしい. 

