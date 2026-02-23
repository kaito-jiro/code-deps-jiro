# Roadmap

## 1. 基本設計（High-Level Design）
目的は `docs/project.md` の設計概要を、実装可能な構成に落とし込むことです。
- スコープ確定: 対象は C#/.NET プロジェクト（`.csproj` またはフォルダ）
- 入出力仕様の確定
- 依存抽出の対象（クラス/インターフェース/継承/メソッド等）の優先順位
- 解析パイプラインの責務分割（`ProjectLoader`〜`Exporter`）
- CLI オプションの確定（`--dot`, `--filter`, `--rules`, `--exclude`）
- 出力形式の最小要件（Plain/DOT）

成果物:
- 基本設計メモ（`docs/` に追記）
- 主要データモデルの定義（依存関係、ノード、エッジ、ルール）

## 2. 詳細設計（Low-Level Design）
目的は各コンポーネントの入出力と内部処理を明確化することです。
- `ProjectLoader`: `.csproj`/フォルダ → `.cs` 一覧
- `SyntaxAnalyzer`: 構文木から型宣言を収集
- `SemanticAnalyzer`: 型解決とシンボル取得
- `DependencyCollector`: 依存抽出ルールと対象（フィールド/引数/戻り値/継承/実装/new）
- `GraphBuilder`: ノード/エッジ生成、循環検出
- `RuleEvaluator`: ルールファイルの読み込みと違反検出
- `Exporter`: Plain/DOT 変換

成果物:
- クラス/インターフェース設計（責務・メソッド・入力/出力）
- 主要アルゴリズムの簡易フロー

## 3. 実装（Implementation）
目的は最小実行可能な CLI を完成させることです。
- `src/DepGraph` にコンポーネントの骨組みを作成
- CLI 引数パース（入力パス、オプション）
- 解析パイプラインの接続（入力→依存抽出→出力）
- Plain 出力を先に実装、次に DOT 出力
- ルールファイルは読み込みだけ先行（評価は後続）

成果物:
- `dotnet run --project src/DepGraph/DepGraph.csproj -- ./MyProject.csproj` が動作
- 依存関係が最小限出力される

## 4. テスト（Testing）
目的は挙動の保証と回帰防止です。
- テストプロジェクト追加（例: xUnit）
- 解析対象を含む最小サンプルプロジェクトを用意
- 単体テスト: `DependencyCollector`, `GraphBuilder`
- 期待出力のスナップショットテスト（Plain/DOT）
- ルール違反検出のテスト

成果物:
- `dotnet test` が通る
- 主要な依存抽出ケースがカバーされる

## 5. 反復と拡張
- 性能改善（大規模プロジェクト対応）
- SVG/PNG 変換オプション
- CI 連携
- ルール機能の拡充
