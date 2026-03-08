# Roadmap

## 現在の状況（要約）
- 基本設計/詳細設計: 完了
- 実装: `.csproj` 解決と JSON 出力拡張まで完了
- テスト: 主要機能の単体テストまで完了
- ドキュメント: 主要ドキュメントを整備済み

## 1. 基本設計（High-Level Design）【完了】
目的は `docs/project.md` の設計概要を、実装可能な構成に落とし込むことです。
- スコープ確定: 対象は C#/.NET プロジェクト（`.csproj` またはフォルダ）
- 入出力仕様の確定
- 依存抽出対象と責務分割の方針確定
- CLI オプション方針の確定
- 出力形式の最小要件（JSON）

成果物:
- 基本設計書: `docs/design/basic-design.md`
- 主要データモデルの定義

## 2. 詳細設計（Low-Level Design）【完了】
目的は各コンポーネントの入出力と内部処理を明確化することです。
- パイプライン処理フロー定義
- コンポーネント責務定義
- データモデル定義
- ルールファイル仕様定義
- エラーハンドリング方針定義

成果物:
- 詳細設計書: `docs/design/detailed-design.md`
- クラス図: `docs/design/class-diagram.md`
- クラス詳細: `docs/design/class-details.md`

## 3. 実装（Implementation）【進行中】
目的は最小実行可能な CLI を完成させることです。
- `src/CodeDepsJiro` にコンポーネントの骨組みを作成
- CLI 引数パース（入力パス、オプション）
- 解析パイプラインの接続（入力→依存抽出→出力）
- `.csproj` 解析（Compile Include/Remove, ProjectReference 対応）
- JSON 出力
- ルール評価の初期実装

未完了:
- 例外・警告の整備（ログ出力設計）
- 循環依存検出

成果物:
- `dotnet build src/CodeDepsJiro/CodeDepsJiro.csproj` が通る
- `dotnet src/CodeDepsJiro/bin/Debug/net10.0/CodeDepsJiro.dll <path>` で動作

## 4. テスト（Testing）【進行中】
目的は挙動の保証と回帰防止です。
- テストプロジェクト追加（xUnit）
- 単体テスト: `DependencyCollector`, `GraphBuilder`
- 出力スナップショットテスト（JSON）
- ルール読み込み/違反検出のテスト
- `.csproj` 解決テスト（Compile Include/Remove, ProjectReference）

未完了:
- CLI オプションの結合テスト

成果物:
- `dotnet test` が通る
- 主要な依存抽出ケースがカバーされる

## 5. 反復と拡張【未着手】
- 性能改善（大規模プロジェクト対応）
- SVG/PNG 変換オプション
- CI 連携
- ルール機能の拡充

## 次の作業（候補）
- 循環依存検出（DFS で最小実装）
- CLI 統合テストの追加
- 例外・警告出力の整理

## 作業状況の管理
- 方針は `docs/workflow/work-management.md` を参照
