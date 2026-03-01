# 実行手順書

この手順は、`code-deps-jiro` を実行するための共通ガイドです。

## 1. 事前準備
- .NET SDK がインストールされていること
- 解析対象の C# プロジェクトが利用可能であること

## 2. 解析対象の用意
`.csproj` もしくはフォルダパスを指定できます。

### 2.1 `.csproj` を指定する場合
```
./path/to/MyProject/MyProject.csproj
```

### 2.2 フォルダを指定する場合
```
./path/to/MyProject
```

## 3. Publish（Linux x64）

```
dotnet publish src/CodeDepsJiro/CodeDepsJiro.csproj -c Release -r linux-x64 --self-contained false
```

## 4. 実行

### 4.1 `.csproj` を指定する場合
```
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./path/to/MyProject/MyProject.csproj
```

### 4.2 フォルダを指定する場合
```
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./path/to/MyProject
```

## 5. dotnet で実行（クロスプラットフォーム）

```
dotnet build src/CodeDepsJiro/CodeDepsJiro.csproj
```

### 5.1 `.csproj` を指定する場合
```
dotnet src/CodeDepsJiro/bin/Debug/net10.0/CodeDepsJiro.dll ./path/to/MyProject/MyProject.csproj
```

### 5.2 フォルダを指定する場合
```
dotnet src/CodeDepsJiro/bin/Debug/net10.0/CodeDepsJiro.dll ./path/to/MyProject
```

## 6. オプション
- `--format <plain|json|csv>`: 出力形式
- `--output <file>`: 出力ファイル（未指定時は標準出力）
- `--filter <pattern>`: 名前空間フィルタ
- `--rules <file>`: ルールファイル指定
- `--exclude <pattern>`: 除外パス指定

例:
```
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./path/to/MyProject --format json --output out/code-deps-jiro.json
```

```
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ../OtherProject --exclude *Tests*
```

```
dotnet src/CodeDepsJiro/bin/Debug/net10.0/CodeDepsJiro.dll ./path/to/MyProject --format csv --output out/code-deps-jiro.csv
```

## 7. 出力例
### 7.1 標準出力（`--output` 未指定）
```
MyApp.Services.UserService -> MyApp.Data.UserRepository
MyApp.Controllers.UserController -> MyApp.Services.UserService
```

### 7.2 ファイル出力（`--output` 指定）
```
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./path/to/MyProject --format json --output out/dependencies.json
```

### 7.3 ルール違反の出力（JSON 抜粋）
```
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./path/to/MyProject --rules rules.json --format json --output out/dependencies.json
```

```json
{
    "violations": [
        {
            "fromLayer": "Application",
            "toLayer": "Infrastructure",
            "from": "MyApp.Services.UserService",
            "to": "MyApp.Data.UserRepository",
            "relationType": "Field"
        }
    ]
}
```

## 8. 注意点
- 現時点では C# のみ対応
- 解析失敗は警告として出力される
- `bin/` と `obj/` は自動的に除外される
