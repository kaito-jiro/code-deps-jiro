# 実行手順書

この手順は、本リポジトリ内で `depgraph` を実行するためのガイドです。

## 1. 事前準備
- .NET SDK がインストールされていること
- 解析対象の C# プロジェクトが利用可能であること

## 2. 解析対象の用意
このリポジトリ配下に解析対象プロジェクトを置きます。

例:
```
./samples/MyProject
```

`.csproj` もしくはフォルダパスを指定できます。

## 3. ビルド

```
dotnet build src/DepGraph/DepGraph.csproj
```

## 4. 実行

### 4.1 `.csproj` を指定する場合
```
dotnet bin/Debug/net10.0/DepGraph.dll ./samples/MyProject/MyProject.csproj
```

### 4.2 フォルダを指定する場合
```
dotnet bin/Debug/net10.0/DepGraph.dll ./samples/MyProject
```

## 5. オプション
- `--dot`: DOT 形式で出力
- `--filter <pattern>`: 名前空間フィルタ
- `--rules <file>`: ルールファイル指定
- `--exclude <pattern>`: 除外パス指定

例:
```
dotnet bin/Debug/net10.0/DepGraph.dll ./samples/MyProject --dot --exclude *Tests*
```

## 6. 注意点
- 現時点では C# のみ対応
- 解析失敗は警告として出力される
- `bin/` と `obj/` は自動的に除外される
