# 基本設計 (High-Level Design)

## 1. 目的
C#/.NET プロジェクトの依存関係を解析し、CLI で可視化する。循環依存や設計ルール違反の検知を補助する。

## 2. 対象と前提
- 対象入力は `.csproj` またはフォルダパス
- 解析対象は `.cs` ファイル
- Roslyn（`Microsoft.CodeAnalysis.CSharp`）で構文解析と型解決を行う
- CLI で完結し、外部サービスは不要

## 3. スコープ
- クラス/インターフェース/抽象クラスの依存抽出
- 名前空間依存の集計
- 循環依存検出
- ルールファイルによるレイヤー依存の検査
- 出力は Plain/JSON/CSV

## 4. 非スコープ（初期段階では扱わない）
- UI アプリや Web UI
- ソースコードの自動修正
- バイナリ解析

## 5. 入出力仕様（概要）
入力はパスとオプション。出力は依存一覧または機械可読フォーマット。

### 入力例
```
CodeDepsJiro ./MyProject.csproj
CodeDepsJiro ./src --format json --exclude *Tests* --filter ns:*UI*
```

### 出力例（Plain）
```
Controller -> Service
Service -> IRepository
```

### 出力例（JSON 抜粋）
```json
{
  "edges": [
    { "from": "UserController", "to": "UserService", "relationType": "Field" }
  ]
}
```

## 6. 解析パイプライン（責務分割）
- `ProjectLoader`: 入力解決、`.cs` ファイル列挙
- `SyntaxAnalyzer`: 構文木の生成と型宣言の収集
- `SemanticAnalyzer`: 型/シンボル解決
- `DependencyCollector`: 依存抽出ルール適用
- `GraphBuilder`: ノード/エッジ化、循環検出
- `RuleEvaluator`: ルールファイル適用と違反判定
- `Exporter`: Plain/JSON/CSV 変換

## 7. データモデル（概要）
- `Node`: `Id`, `Name`, `Namespace`, `Kind`
- `DependencyEdge`: `From`, `To`, `RelationType`
- `Graph`: `Nodes`, `Edges`
- `RuleSet`: `Layers`, `Violations`

## 8. 非機能要件
- 中規模プロジェクトで実用的な速度
- 解析失敗は警告として継続
- 出力は安定して再現可能

## 9. 成果物
- 設計仕様書（本書）
- 詳細設計書（`docs/detailed-design.md`）
