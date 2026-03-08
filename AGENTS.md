# Repository Guidelines

## Project Structure & Module Organization
このリポジトリは設計概要 `docs/project.md` を中心に構成されています。ここに CLI 仕様、出力形式、解析パイプラインが記載されています。  
実装は `src/` 配下に置き、以下のモジュール分割に沿って配置してください。  
例: `ProjectLoader`, `SyntaxAnalyzer`, `SemanticAnalyzer`, `DependencyCollector`, `GraphBuilder`, `RuleEvaluator`, `Exporter`。  
現在は `src/CodeDepsJiro/` 配下に各コンポーネントの `*.cs` を配置しています。CLI 関連は `src/CodeDepsJiro/Cli/`、モデル定義は `src/CodeDepsJiro/Models/`、エントリポイントは `src/CodeDepsJiro/Program.cs` です。  
テストは `src/tests/CodeDepsJiro.Tests/` 配下に配置しています。
テストのスナップショットは `src/tests/CodeDepsJiro.Tests/Snapshots/` に置きます。
設計資料は `docs/` 配下（例: `docs/project.md`, `docs/design/detailed-design.md`, `docs/design/class-diagram.md`, `docs/workflow/testing.md`）です。
README は概要と最小限の使い方に絞り、詳細手順は `docs/run-guide.md` に集約します。
依存の流れは `ProjectLoader` → `SyntaxAnalyzer` → `SemanticAnalyzer` → `DependencyCollector` → `GraphBuilder` → `Exporter` を基本とし、追加の処理はこの順序に沿って差し込みます。

## Build, Test, and Development Commands
現在の .NET CLI プロジェクトは `src/CodeDepsJiro/CodeDepsJiro.csproj` です。  
主要コマンドは以下です。
- ビルド: `dotnet build src/CodeDepsJiro/CodeDepsJiro.csproj`
- 実行: `dotnet run --project src/CodeDepsJiro/CodeDepsJiro.csproj -- ./MyProject.csproj`
- 依存復元: `dotnet restore src/CodeDepsJiro/CodeDepsJiro.csproj`
- Publish: `dotnet publish src/CodeDepsJiro/CodeDepsJiro.csproj -c Release -r linux-x64 --self-contained false`
テストは xUnit を使用しています。  
`dotnet test src/tests/CodeDepsJiro.Tests/CodeDepsJiro.Tests.csproj` で実行できます。
新しい CLI オプションや出力形式を追加した場合は、`README.md` と `docs/run-guide.md` の両方に追記し、`docs/project.md` の設計概要にも反映してください。

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
出力仕様を変えた場合はスナップショット（`Snapshots/`）を更新し、差分が意図通りか説明を残してください。
解析対象のサンプルはテスト内で完結させ、外部リポジトリへの依存は避けます。
新規テストを追加したら `docs/workflow/testing.md` の表も更新します。

## Commit & Pull Request Guidelines
このリポジトリにはまだ確立したコミット規約がありません。暫定として、命令形で簡潔に（例: `Add JSON exporter`）。PR には目的の説明、関連 Issue、出力例（Plain/JSON/CSV のスニペット）を含めてください。
大きな仕様変更は `docs/project.md` と `docs/design/detailed-design.md` を同時に更新します。
Issue を作成する際は「目的 / 仕様 / 完了条件 / 受け入れ」を含めます。
例:
```
目的: 依存解析の精度を上げる
仕様: ProjectLoaderでCompile Include/Removeを解決する
完了条件: Include/Removeが反映される
受け入れ: 小規模サンプルで期待通りのファイルが列挙される
```
PR 作成時は以下を含めます（関連Issueは必須、クローズする場合は `Closes #123` を併記）。
例:
```
目的: JSON出力の可読性を上げる
関連Issue: #123
Closes #123
変更内容: nodesをnamespaces配下に移動
出力例(JSON):
{
    "namespaces": [
        {
            "name": "MyApp.Services",
            "nodes": [
                { "name": "UserService", "kind": "Class" }
            ]
        }
    ]
}
```

## Configuration & Security Notes
入力は `.csproj` またはフォルダパスで、ルールファイル（例: `rules.json`）を受け取る設計です。未検証リポジトリを解析する場合、ファイルアクセス範囲とルール入力の取り扱いに注意してください。
出力をファイルに書き出す場合は、既存ファイルの上書き挙動を明確にし、変更時はテストとドキュメントの更新を行います。

## Agent-Specific Instructions
このガイドは 200〜400 語に収め、`docs/project.md` の内容に基づいて更新してください。存在しないコマンドやディレクトリは記載しないでください。  
作業対象はこのリポジトリ配下に限定します。配下以外の操作が必要な場合はアクセスを求めず、必要な理由とユーザーに実行してほしい具体的な作業内容を報告してください。
