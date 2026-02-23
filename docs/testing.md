# テスト方針

## 目的
- 依存抽出とグラフ構築の回帰防止
- 出力フォーマット変更の影響を素早く検知
- CLI 変更時の動作確認コストを下げる

## 現在のテスト
- フレームワーク: xUnit
- プロジェクト: `src/tests/CodeDepsJiro.Tests/CodeDepsJiro.Tests.csproj`

## 実行方法
```
dotnet test src/tests/CodeDepsJiro.Tests/CodeDepsJiro.Tests.csproj
```

## 命名・配置ルール
- テストクラスは `*Tests.cs`
- テスト対象ごとにファイルを分割
- 新規テストは `src/tests/CodeDepsJiro.Tests/` 配下に追加

## テスト一覧
| テスト名 | 種別 | 対象 | 検証内容 |
| --- | --- | --- | --- |
| `DependencyCollectorTests` | 単体/結合の中間 | `SemanticAnalyzer`, `DependencyCollector` | 依存種別（継承/実装/フィールド/プロパティ/引数/戻り値/new）の抽出 |
| `GraphBuilderTests` | 単体 | `GraphBuilder` | ノード重複排除 |
| `OutputSnapshotTests` | 結合/システム寄り | `SyntaxAnalyzer`〜`Exporter` | JSON/CSV 出力のスナップショット一致 |

## テスト内容の詳細
- `DependencyCollectorTests`（単体/結合の中間）: 単一ファイルに定義した `Target` が `Base`/`IService`/`Dependency`/`Other` に依存することを確認  
  - Roslyn の `SemanticAnalyzer` と `DependencyCollector` を組み合わせているため、純粋な単体ではなく「軽い結合テスト」に近い
- `GraphBuilderTests`（単体）: 同一 `Id` のノードが複数エッジに出ても `Nodes` が重複しないことを確認
- `OutputSnapshotTests`（結合/システム寄り）: `dependencies.json` と `dependencies.csv` に対して出力が一致することを確認  
  - `SyntaxAnalyzer` → `SemanticAnalyzer` → `DependencyCollector` → `GraphBuilder` → `Exporter` の一連を通すため、結合〜システム寄りの検証



## 作成検討中のテスト
| テスト名 | 種別 | 対象 | 目的 | 備考 |
| --- | --- | --- | --- | --- |
| `CliOptionsIntegrationTests` | 結合 | `ArgumentParser`〜`Exporter` | `--format/--output/--exclude` の組み合わせ動作 | CLI 仕様の安定化待ち |
| `ProjectLoaderCsprojTests` | 単体/結合の中間 | `ProjectLoader` | `Compile Include/Remove` と `ProjectReference` の解決 | ケース設計が未着手 |
| `RuleEvaluatorTests` | 単体 | `RuleEvaluator` | レイヤールール一致/不一致の検証 | ルールファイルのパースが未実装 |
| `SemanticAnalyzerReferenceTests` | 単体/結合の中間 | `SemanticAnalyzer` | 参照解決が必要な型の解析可否 | 参照解決の対象範囲が未定 |
