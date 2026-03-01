# code-deps-jiro

C#/.NET プロジェクトの依存関係を解析し、Plain、JSON、CSV で出力する CLI ツールです。

## 主な機能
- クラス/名前空間の依存抽出
- テキスト/JSON/CSV で依存エッジを出力
- ルールファイルによるレイヤー違反検出
- `.csproj` の `Compile Include/Remove` と `ProjectReference` を解決

## 使い方
### Publish（Linux x64）
```
dotnet publish src/CodeDepsJiro/CodeDepsJiro.csproj -c Release -r linux-x64 --self-contained false
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./path/to/MyProject/MyProject.csproj
```

### dotnet で実行（クロスプラットフォーム）
```
dotnet build src/CodeDepsJiro/CodeDepsJiro.csproj
dotnet src/CodeDepsJiro/bin/Debug/net10.0/CodeDepsJiro.dll ./path/to/MyProject/MyProject.csproj
```

### 出力オプション
```
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./path/to/MyProject/MyProject.csproj --format json --output out/code-deps-jiro.json
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./path/to/MyProject/MyProject.csproj --format csv --output out/code-deps-jiro.csv
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./path/to/MyProject/MyProject.csproj --rules rules.json --format json --output out/code-deps-jiro.json
```

`--output` を指定しない場合は標準出力に結果が出力されます。
ルール違反は JSON の `violations` にのみ出力されます。

### ルールファイル（JSON）
```
{
    "layers": [
        { "name": "Domain", "patterns": ["MyApp.Domain.*"] },
        { "name": "Application", "patterns": ["MyApp.Application.*"] }
    ],
    "violations": [
        { "from": "Application", "to": "Infrastructure" }
    ]
}
```

## 出力例
Plain:
```
MyApp.Services.UserService -> MyApp.Data.UserRepository
```

`--output` 省略時（標準出力）:
```
MyApp.Services.UserService -> MyApp.Data.UserRepository
MyApp.Controllers.UserController -> MyApp.Services.UserService
```

JSON（抜粋）:
```json
{
    "namespaces": [
        {
            "name": "MyApp.Services",
            "nodes": [
                { "name": "UserService", "kind": "Class" }
            ]
        }
    ],
    "edges": [
        { "from": "UserService", "to": "UserRepository", "relationType": "Field" }
    ]
}
```

ルール違反あり（JSON 抜粋）:
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

## ライセンス
MIT License。詳細は `LICENSE` を参照してください。

## 第三者ライセンス
依存ライブラリのライセンスは `THIRD_PARTY_NOTICES.md` を参照してください。
