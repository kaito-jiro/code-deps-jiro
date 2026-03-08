# 作業状況の管理方法

## 目的
現在の進捗と次の作業を分かりやすく共有する。

## 1. 進捗の見える化
- `docs/workflow/roadmap.md` に「完了/進行中/次の作業」を明記する
- 各マイルストーンに日付ではなく状態を付ける（例: 完了/進行中/未着手）

## 2. Issue 運用（GitHub）
- 大きなタスクは Issue に分割する
- Issue には「目的/完了条件/検証方法」を書く
- ラベル例: `design`, `implementation`, `testing`, `docs`

## 3. ブランチ運用
- `feature/*` で作業し、`main` にはレビュー後に統合
- 作業単位は小さくし、差分が追いやすいサイズで分割

## 4. ドキュメント更新ルール
- 実装を変更したら関連する `docs/` も更新する
- `README.md` は概要・使い方・ライセンスのみ簡潔に保つ
- CLI オプション追加時は `docs/project.md`、`docs/run-guide.md`、README を更新する
- 出力形式変更時は `docs/project.md`、`docs/design/detailed-design.md`、`docs/workflow/testing.md` を更新する
- コンポーネント責務変更時は `docs/design/class-diagram.md`、`docs/design/class-details.md`、`docs/design/detailed-design.md` を更新する
- フェーズ進捗変更時は `docs/workflow/roadmap.md` を更新し、必要に応じて本書も更新する

## 5. PR 前チェックリスト
- `docs/project.md` と `docs/workflow/roadmap.md` に変更内容が反映されている
- `README.md` / `README.ja.md` とソースの整合が取れている
- 新規/変更した機能に対応するテストがある（または未作成理由を記載）
- `dotnet test src/tests/CodeDepsJiro.Tests/CodeDepsJiro.Tests.csproj` を実行済み
- 追加した出力例・スナップショットが最新の仕様に一致している

## 6. 定期レビュー
- 週単位で `docs/workflow/roadmap.md` の「次の作業」を更新
- 仕様変更があれば `docs/design/detailed-design.md` に反映
