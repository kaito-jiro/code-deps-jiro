# Repository Guidelines

## Project Structure & Module Organization
このリポジトリは設計概要 `docs/project.md` を中心に構成されています。ここに CLI 仕様、出力形式、解析パイプラインが記載されています。  
実装は `src/` 配下に置き、以下のモジュール分割に沿って配置してください。  
例: `ProjectLoader`, `SyntaxAnalyzer`, `SemanticAnalyzer`, `DependencyCollector`, `GraphBuilder`, `RuleEvaluator`, `Exporter`。  
現在は `src/CodeDepsJiro/` 配下に各コンポーネントの `*.cs` を配置しています。
テストは `src/tests/CodeDepsJiro.Tests/` 配下に配置しています。

## Build, Test, and Development Commands
現在の .NET CLI プロジェクトは `src/CodeDepsJiro/CodeDepsJiro.csproj` です。  
主要コマンドは以下です。
- ビルド: `dotnet build src/CodeDepsJiro/CodeDepsJiro.csproj`
- 実行: `dotnet run --project src/CodeDepsJiro/CodeDepsJiro.csproj -- ./MyProject.csproj`
- 依存復元: `dotnet restore src/CodeDepsJiro/CodeDepsJiro.csproj`
テストは xUnit を使用しています。  
`dotnet test src/tests/CodeDepsJiro.Tests/CodeDepsJiro.Tests.csproj` で実行できます。

## Coding Style & Naming Conventions
具体的なルールはまだありません。C# を追加する場合は一般的な .NET 規約に従ってください。
- インデント: 4 スペース、タブ可
- 命名: 型/公開メンバーは `PascalCase`、ローカル/引数は `camelCase`、プライベートフィールドは `_camelCase`
- ファイル名は主要型名と一致（例: `GraphBuilder.cs`）
- 公開/非公開を問わずメソッドには XML サマリコメント（`summary`/`param`/`returns`）を付与する
フォーマッタやリンタ（`dotnet format`, `.editorconfig`）を導入したら追記します。

## Testing Guidelines
テスト基盤は xUnit を使用しています。  
命名規約は `*Tests.cs`、ディレクトリは `src/tests/CodeDepsJiro.Tests/` です。

## Commit & Pull Request Guidelines
このリポジトリにはまだ確立したコミット規約がありません。暫定として、命令形で簡潔に（例: `Add JSON exporter`）。PR には目的の説明、関連 Issue、出力例（Plain/JSON/CSV のスニペット）を含めてください。

## Configuration & Security Notes
入力は `.csproj` またはフォルダパスで、ルールファイル（例: `rules.json`）を受け取る設計です。未検証リポジトリを解析する場合、ファイルアクセス範囲とルール入力の取り扱いに注意してください。

## Agent-Specific Instructions
このガイドは 200〜400 語に収め、`docs/project.md` の内容に基づいて更新してください。存在しないコマンドやディレクトリは記載しないでください。  
作業対象はこのリポジトリ配下に限定します。配下以外の操作が必要な場合はアクセスを求めず、必要な理由とユーザーに実行してほしい具体的な作業内容を報告してください。
