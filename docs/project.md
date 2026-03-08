# code-deps-jiro — CLIベースの依存関係可視化ツール（設計概要）

## 1. 背景・目的

`code-deps-jiro` は、.NET/C# プロジェクトを対象にコードの依存関係を解析し、クラス間・名前空間間の依存グラフを CLI で可視化する開発支援ツールです。

目的は以下：

- クラスや名前空間の依存構造を可視化する
- 循環依存や設計ルール違反の早期発見
- 設計理解・コード品質向上
- SonarQube 等の静的解析と合わせた補完的診断

なお、C# のコード構造解析は Roslyn（.NET Compiler Platform）を利用して行います。


## 2. 主な機能
### 2.1 解析機能

- クラス間依存関係の抽出
- 名前空間依存関係の集計
- 依存サイクル（循環依存）の検出
- 特定パターン（例：レイヤー境界越え）のルール違反検出

### 2.2 出力
#### JSON
- 依存エッジを機械可読な形式で出力
- 大規模プロジェクトでの後処理・集計に利用
- JSON は `namespaces` 配下にクラスノードをまとめる
- ルール違反は JSON の `violations` にのみ出力


## 3. CLI 使用例
CodeDepsJiro ./path/to/MyProject/MyProject.csproj

### オプション例

- 主なオプション: `--output`, `--filter`, `--rules`, `--exclude`
- 詳細な使い方と実行例は `docs/run-guide.md` を参照

## 4. 入力仕様

- .csproj もしくはフォルダパスを受け取る
- 内部でプロジェクトを読み込み、すべての .cs ファイルを解析対象とする
- Roslyn の SemanticModel を用いた型解決により正確な依存抽出を行う



## 5. 解析アーキテクチャ

```
code-deps-jiro/
├─ Program.cs
├─ ProjectLoader
├─ SyntaxAnalyzer
├─ SemanticAnalyzer
├─ DependencyCollector
├─ GraphBuilder
├─ RuleEvaluator
└─ Exporter
```

### 各コンポーネント説明

#### ProjectLoader
- .csproj を読み込み
- ソースファイル一覧を収集
- `Compile Include/Remove` を評価して対象ファイルを確定する
- `ProjectReference` を辿り参照先の `.csproj` を含める

#### SyntaxAnalyzer
- SyntaxTree を構築
- クラス・インターフェース宣言を収集

#### SemanticAnalyzer
- SemanticModel を取得
- 型シンボルを解決

#### DependencyCollector
- フィールド型
- プロパティ型
- メソッド引数/戻り値
- new 式
- 継承/実装
などから依存を抽出

#### GraphBuilder
- 依存関係をグラフ構造に変換
- 循環依存検出

#### RuleEvaluator
- レイヤールール違反検出

#### Exporter
- JSON 形式で出力


## 6. 技術詳細
### 6.1 Roslyn API

- Microsoft.CodeAnalysis.CSharp を利用
- SyntaxTree で構文解析
- SemanticModel で型解決

### 6.2 依存対象

- クラス
- インターフェース
- 抽象クラス
- 継承関係
- 実装関係
- メソッド呼び出し
- フィールド参照


## 7. 依存抽出例

```
public class MyService {
    private readonly IUserRepository _repo;
    public MyService(IUserRepository repo) {
        _repo = repo;
    }
}
```

生成される依存：

MyService -> IUserRepository



## 8. ルールファイル（概要）

```
{
  "layers": [
    { "name": "Domain", "patterns": ["MyApp.Domain.*"] }
  ],
  "violations": [
    { "from": "Application", "to": "Infrastructure" }
  ]
}
```

ルールファイル指定時は JSON を読み込み、レイヤー違反を評価する。  
必須項目、バリデーション、エラー条件は `docs/design/detailed-design.md` を正本とする。

## 9. 成功の指標

- クラス依存グラフが出力できる
- JSON で機械処理できる
- 循環依存が検出できる
- ルール違反検出ができる


## 10. 将来拡張

- Web UI 可視化
- CI 連携
- SonarQube 統合
- Autodesk API 強調表示
- 設計劣化の時系列比較
