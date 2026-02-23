# Roadmap

## 現在の状況（要約）
- 基本設計/詳細設計: 完了
- 実装: コアパイプライン実装まで完了
- テスト: 未着手
- ドキュメント: 主要ドキュメントを整備済み

## 1. 基本設計（High-Level Design）【完了】
目的は `docs/project.md` の設計概要を、実装可能な構成に落とし込むことです。
- スコープ確定: 対象は C#/.NET プロジェクト（`.csproj` またはフォルダ）
- 入出力仕様の確定
- 依存抽出の対象（クラス/インターフェース/継承/メソッド等）の優先順位
- 解析パイプラインの責務分割（`ProjectLoader`〜`Exporter`）
- CLI オプションの確定（`--dot`, `--filter`, `--rules`, `--exclude`）
- 出力形式の最小要件（Plain/DOT）

成果物:
- 基本設計書: `docs/basic-design.md`
- 主要データモデルの定義

## 2. 詳細設計（Low-Level Design）【完了】
目的は各コンポーネントの入出力と内部処理を明確化することです。
- `ProjectLoader`: `.csproj`/フォルダ → `.cs` 一覧
- `SyntaxAnalyzer`: 構文木から型宣言を収集
- `SemanticAnalyzer`: 型解決とシンボル取得
- `DependencyCollector`: 依存抽出ルールと対象（フィールド/引数/戻り値/継承/実装/new）
- `GraphBuilder`: ノード/エッジ生成、循環検出
- `RuleEvaluator`: ルールファイルの読み込みと違反検出
- `Exporter`: Plain/DOT 変換

成果物:
- 詳細設計書: `docs/detailed-design.md`
- クラス図: `docs/class-diagram.md`
- クラス詳細: `docs/class-details.md`

## 3. 実装（Implementation）【進行中】
目的は最小実行可能な CLI を完成させることです。
- `src/DepGraph` にコンポーネントの骨組みを作成
- CLI 引数パース（入力パス、オプション）
- 解析パイプラインの接続（入力→依存抽出→出力）
- `.csproj` 解析（Compile Include/Remove, ProjectReference 対応）
- Plain/DOT 出力
- ルール評価の初期実装

未完了:
- ルールファイルの読み込み（`rules.json` のパース）
- 例外・警告の整備（ログ出力設計）
- 循環依存検出

成果物:
- `dotnet build src/DepGraph/DepGraph.csproj` が通る
- `dotnet bin/Debug/net10.0/DepGraph.dll <path>` で動作

## 4. テスト（Testing）【未着手】
目的は挙動の保証と回帰防止です。
- テストプロジェクト追加（例: xUnit）
- 解析対象を含む最小サンプルプロジェクトを用意
- 単体テスト: `DependencyCollector`, `GraphBuilder`
- 期待出力のスナップショットテスト（Plain/DOT）
- ルール違反検出のテスト

成果物:
- `dotnet test` が通る
- 主要な依存抽出ケースがカバーされる

## 5. 反復と拡張【未着手】
- 性能改善（大規模プロジェクト対応）
- SVG/PNG 変換オプション
- CI 連携
- ルール機能の拡充

## 次の作業（候補）
- ルールファイルの JSON パースを実装
- 循環依存検出（DFS で最小実装）
- テスト基盤の導入

## 作業状況の管理
- 方針は `docs/work-management.md` を参照
