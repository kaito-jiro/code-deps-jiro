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

## 3. Publish（Linux x64）

```
dotnet publish src/DepGraph/DepGraph.csproj -c Release -r linux-x64 --self-contained false
```

## 4. 実行

### 4.1 `.csproj` を指定する場合
```
./src/DepGraph/bin/Release/net10.0/linux-x64/publish/DepGraph ./samples/MyProject/MyProject.csproj
```

### 4.2 フォルダを指定する場合
```
./src/DepGraph/bin/Release/net10.0/linux-x64/publish/DepGraph ./samples/MyProject
```

## 5. dotnet で実行（クロスプラットフォーム）

```
dotnet build src/DepGraph/DepGraph.csproj
```

### 5.1 `.csproj` を指定する場合
```
dotnet src/DepGraph/bin/Debug/net10.0/DepGraph.dll ./samples/MyProject/MyProject.csproj
```

### 5.2 フォルダを指定する場合
```
dotnet src/DepGraph/bin/Debug/net10.0/DepGraph.dll ./samples/MyProject
```

## 6. オプション
- `--dot`: DOT 形式で出力
- `--filter <pattern>`: 名前空間フィルタ
- `--rules <file>`: ルールファイル指定
- `--exclude <pattern>`: 除外パス指定

例:
```
./src/DepGraph/bin/Release/net10.0/linux-x64/publish/DepGraph ./samples/MyProject --dot --exclude *Tests*
```

```
dotnet src/DepGraph/bin/Debug/net10.0/DepGraph.dll ./samples/MyProject --dot --exclude *Tests*
```

## 7. 注意点
- 現時点では C# のみ対応
- 解析失敗は警告として出力される
- `bin/` と `obj/` は自動的に除外される
