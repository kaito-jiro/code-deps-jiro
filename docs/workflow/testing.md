# テスト方針

## 目的
- 依存抽出とグラフ構築の回帰防止
- 出力フォーマット変更の影響を素早く検知
- CLI 変更時の動作確認コストを下げる

## 現在のテスト
- フレームワーク: xUnit
- プロジェクト: `src/tests/CodeDepsJiro.Tests/CodeDepsJiro.Tests.csproj`
- スナップショット: `src/tests/CodeDepsJiro.Tests/Snapshots/` 配下の JSON

## 実行方法
```
dotnet test src/tests/CodeDepsJiro.Tests/CodeDepsJiro.Tests.csproj
```

## 命名・配置ルール
- テストクラスは `*Tests.cs`
- テスト対象ごとにファイルを分割
- 新規テストは `src/tests/CodeDepsJiro.Tests/` 配下に追加
- スナップショット更新時は差分理由を明記し、出力仕様と整合を確認する

## テストデータ作成の共通方針
1. データ配置
- 固定のサンプルプロジェクトは原則持たず、各テストで一時ディレクトリ配下に都度生成する。

2. 生成単位
- 最小構成の `.csproj` と `.cs` をテストごとに作成する。
- 必要時のみ `rules.json` などの補助ファイルを同ディレクトリに作成する。

3. 共通化
- テストヘルパーで以下を共通化する。
- 一時ディレクトリ作成/削除
- テストファイル書き出し
- 実行結果（標準出力/標準エラー）の取得

4. 検証ポリシー
- まずは終了可否、出力ファイル生成有無、主要メッセージを優先して検証する。
- 出力本文の厳密比較は必要最小限にし、構造確認を優先する。

5. 可搬性
- 改行コードは比較前に正規化する。
- パス比較は絶対パス化または正規化してOS差分を吸収する。

## テスト一覧
| テスト名 | 種別 | 対象 | 検証内容 |
| --- | --- | --- | --- |
| `DependencyCollectorTests` | 単体/結合の中間 | `SemanticAnalyzer`, `DependencyCollector` | 依存種別（継承/実装/フィールド/プロパティ/引数/戻り値/new）の抽出 |
| `GraphBuilderTests` | 単体 | `GraphBuilder` | ノード重複排除 |
| `OutputSnapshotTests` | 結合/システム寄り | `SyntaxAnalyzer`〜`Exporter` | JSON 出力のスナップショット一致 |
| `RuleSetLoaderTests` | 単体 | `RuleSetLoader` | ルールファイルの読み込み/入力バリデーション |
| `RuleEvaluatorTests` | 単体 | `RuleEvaluator` | レイヤールール違反の検出 |
| `ProjectLoaderCsprojTests` | 単体/結合の中間 | `ProjectLoader` | `Compile Include/Remove` と `ProjectReference` の解決 |
| `CliOptionsIntegrationTests` | 結合 | `Program`, `ArgumentParser`〜`Exporter` | CLIオプションの正常系/異常系（出力ファイル、未知オプション、必須引数不足） |

## テスト内容の詳細
- `DependencyCollectorTests`（単体/結合の中間）: 単一ファイルに定義した `Target` が `Base`/`IService`/`Dependency`/`Other` に依存することを確認  
  - Roslyn の `SemanticAnalyzer` と `DependencyCollector` を組み合わせているため、純粋な単体ではなく「軽い結合テスト」に近い
- `GraphBuilderTests`（単体）: 同一 `Id` のノードが複数エッジに出ても `Nodes` が重複しないことを確認
- `OutputSnapshotTests`（結合/システム寄り）: `dependencies.json` に対して出力が一致することを確認  
  - `SyntaxAnalyzer` → `SemanticAnalyzer` → `DependencyCollector` → `GraphBuilder` → `Exporter` の一連を通すため、結合〜システム寄りの検証
- `RuleSetLoaderTests`（単体）: ルールファイルの正常系読み込みと必須項目欠落/不正 JSON の例外を確認
- `RuleEvaluatorTests`（単体）: レイヤー一致/不一致と違反ルール適用の判定を確認
- `ProjectLoaderCsprojTests`（単体/結合の中間）: `Compile Include/Remove` と `ProjectReference` の解決を確認
- `CliOptionsIntegrationTests`（結合）: `Program` 起点でCLIを実行し、入力のみ実行、`--output` 出力、`--rules/--filter/--exclude` 併用、未知オプション、必須引数不足を確認

## 作成検討中のテスト
| テスト名 | 種別 | 対象 | 目的 | 備考 |
| --- | --- | --- | --- | --- |
| `SemanticAnalyzerReferenceTests` | 単体/結合の中間 | `SemanticAnalyzer` | 参照解決が必要な型の解析可否 | 参照解決の対象範囲が未定 |
