# クラス詳細設計

## CLI

### Options
- 役割: CLI 引数の結果を保持する DTO
- 主なプロパティ:
  - `InputPath`: 解析対象のパス
  - `OutputDot`: DOT 出力フラグ
  - `FilterPattern`: 名前空間フィルタ
  - `RulesFile`: ルールファイルパス
  - `ExcludePattern`: 除外パターン

### ArgumentParser
- 役割: CLI 引数を解析して `Options` に変換
- 入力: `string[] args`
- 出力: `Options`
- 例外: 引数不足/未知のオプションで `ArgumentException`

## ProjectLoader

### ProjectLoader
- 役割: `.csproj` またはフォルダから `.cs` ファイルを解決
- 入力: `inputPath`, `excludePattern`
- 出力: ファイルパス一覧
- 仕様:
  - `.csproj` の場合は `Compile Include/Remove` と `ProjectReference` を解決
  - `bin/` と `obj/` は除外
  - `excludePattern` はワイルドカード対応

## SyntaxAnalyzer

### SyntaxAnalyzer
- 役割: Roslyn 構文解析で型宣言を抽出
- 入力: `.cs` ファイル一覧
- 出力: `SyntaxAnalysisResult`
- 仕様:
  - クラス/インターフェースを抽出
  - 名前空間は通常/ファイルスコープ両方に対応

### SyntaxAnalysisResult
- 役割: 構文解析の結果
- 主な構成:
  - `SourceFiles`: ソース内容とパス
  - `TypeDeclarations`: 型宣言一覧

## SemanticAnalyzer

### SemanticAnalyzer
- 役割: Roslyn `Compilation` を構築
- 入力: `SyntaxAnalysisResult`
- 出力: `SemanticAnalysisResult`
- 仕様:
  - `SyntaxTrees` と `Compilation` を保持

### SemanticAnalysisResult
- 役割: セマンティック解析結果
- 主な構成:
  - `SyntaxResult`
  - `SyntaxTrees`
  - `Compilation`

## DependencyCollector

### DependencyCollector
- 役割: セマンティック情報から依存関係を抽出
- 入力: `SemanticAnalysisResult`
- 出力: `DependencyEdge` の一覧
- 依存対象:
  - 継承/実装
  - フィールド
  - プロパティ
  - メソッド引数/戻り値
  - `new` 式

## GraphBuilder

### GraphBuilder
- 役割: 依存エッジから `Graph` を構築
- 入力: `DependencyEdge` 一覧
- 出力: `Graph`

## RuleEvaluator

### RuleEvaluator
- 役割: レイヤールール違反の検出
- 入力: `Graph`, `RuleSet`
- 出力: `RuleViolation` 一覧
- 仕様:
  - 名前空間とレイヤーパターンを照合

## Exporter

### PlainTextExporter
- 役割: 依存関係を `A -> B` 形式で出力
- 入力: `Graph`, `RuleViolation` 一覧
- 出力: 文字列

### DotExporter
- 役割: Graphviz DOT 形式で出力
- 入力: `Graph`, `RuleViolation` 一覧
- 出力: 文字列

## Models

### Node
- 役割: 依存グラフのノード
- 主なプロパティ: `Id`, `Name`, `Namespace`, `Kind`

### DependencyEdge
- 役割: ノード間の依存関係
- 主なプロパティ: `From`, `To`, `RelationType`

### Graph
- 役割: ノードとエッジの集合
- 主なプロパティ: `Nodes`, `Edges`

### RuleSet / LayerRule / ViolationRule / RuleViolation
- 役割: レイヤー定義と違反検出結果
