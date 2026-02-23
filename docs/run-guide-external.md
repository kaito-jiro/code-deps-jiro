# 外部リポジトリでの実行手順書

この手順は、本リポジトリ外にある C# プロジェクトを対象に `depgraph` を実行するためのガイドです。

## 1. 前提
- .NET SDK がインストールされていること
- 解析対象の C# プロジェクトが別ディレクトリに存在すること

## 2. 解析対象の確認
対象プロジェクトの `.csproj` もしくはルートフォルダのパスを確認します。

例:
```
../OtherProject/OtherProject.csproj
```

## 3. Publish（Linux x64）

```
dotnet publish src/DepGraph/DepGraph.csproj -c Release -r linux-x64 --self-contained false
```

## 4. 実行

### 4.1 `.csproj` を指定する場合
```
./src/DepGraph/bin/Release/net10.0/linux-x64/publish/DepGraph ../OtherProject/OtherProject.csproj
```

### 4.2 フォルダを指定する場合
```
./src/DepGraph/bin/Release/net10.0/linux-x64/publish/DepGraph ../OtherProject
```

## 5. dotnet で実行（クロスプラットフォーム）

```
dotnet build src/DepGraph/DepGraph.csproj
```

### 5.1 `.csproj` を指定する場合
```
dotnet src/DepGraph/bin/Debug/net10.0/DepGraph.dll ../OtherProject/OtherProject.csproj
```

### 5.2 フォルダを指定する場合
```
dotnet src/DepGraph/bin/Debug/net10.0/DepGraph.dll ../OtherProject
```

## 6. オプション
- `--dot`: DOT 形式で出力
- `--filter <pattern>`: 名前空間フィルタ
- `--rules <file>`: ルールファイル指定
- `--exclude <pattern>`: 除外パス指定

例:
```
./src/DepGraph/bin/Release/net10.0/linux-x64/publish/DepGraph ../OtherProject --dot --exclude *Tests*
```

```
dotnet src/DepGraph/bin/Debug/net10.0/DepGraph.dll ../OtherProject --dot --exclude *Tests*
```

## 7. 注意点
- 現時点では C# のみ対応
- 解析失敗は警告として出力される
- `bin/` と `obj/` は自動的に除外される
