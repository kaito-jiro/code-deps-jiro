# Documentation Guide

このファイルは `docs/` 配下のドキュメント案内と責務定義の正本です。

## 正本ポリシー
- `docs/README.md` を、`docs/` 配下の構成と責務定義の正本とする。
- ドキュメントの分類・責務変更は本ファイルを先に更新する。

## 構成
- `docs/project.md`
  - プロジェクト全体の概要、対象範囲、主要機能、アーキテクチャ要約。
- `docs/run-guide.md`
  - 実行手順、CLI オプション、出力例。

## 設計ドキュメント（`docs/design/`）
- `docs/design/basic-design.md`
  - 高レベル設計方針、非機能、入出力の全体像。
- `docs/design/detailed-design.md`
  - 処理フロー、コンポーネント責務、データモデル詳細、例外方針。
- `docs/design/class-diagram.md`
  - レイヤ別クラス関係の図示（構造の可視化）。
- `docs/design/class-details.md`
  - クラスごとの責務・入出力・主要メソッド。

## WorkFlow ドキュメント（`docs/workflow/`）
- `docs/workflow/roadmap.md`
  - フェーズ進捗、未完了タスク、次アクション。
- `docs/workflow/testing.md`
  - テスト戦略、テスト一覧、未実装理由。
- `docs/workflow/work-management.md`
  - Issue/PR/ブランチ運用、ドキュメント更新ルール、PR 前チェックリスト。

## 参照ルール
- 仕様変更時の更新先は `docs/workflow/work-management.md` の「ドキュメント更新ルール」を正本とする。
- 実行方法の詳細は `docs/run-guide.md` を正本とする。
- 設計仕様の詳細は `docs/design/detailed-design.md` を正本とする。

## 更新トリガー
- CLI オプション追加:
  - `docs/project.md`、`docs/run-guide.md`、README を更新する。
- 出力形式変更:
  - `docs/project.md`、`docs/design/detailed-design.md`、`docs/workflow/testing.md` を更新する。
- コンポーネント責務変更:
  - `docs/design/class-diagram.md`、`docs/design/class-details.md`、`docs/design/detailed-design.md` を更新する。
- フェーズ進捗変更:
  - `docs/workflow/roadmap.md` を更新し、必要に応じて `docs/workflow/work-management.md` を更新する。

## 配置ルール
- `docs/` 直下:
  - プロジェクト全体共通の説明、利用手順、ドキュメント案内を配置する。
- `docs/design/`:
  - 設計仕様・構造定義・クラス責務など実装根拠を配置する。
- `docs/workflow/`:
  - 進捗管理、テスト運用、Issue/PR 運用ルールを配置する。

## 廃止ルール
- docs の責務定義は本ファイルを正本とし、作業完了後に一時的な整理資料は廃止する。

## 運用手順
- PR 前に `docs/workflow/work-management.md` のチェックリストで更新漏れを確認する。
