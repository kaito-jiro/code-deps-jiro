# code-deps-jiro

C#/.NET プロジェクトの依存関係を解析し、Plain、JSON、CSV で出力する CLI ツールです。

## 主な機能
- クラス/名前空間の依存抽出
- テキスト/JSON/CSV で依存エッジを出力
- ルールファイルによるレイヤー違反検出

## 使い方
### Publish（Linux x64）
```
dotnet publish src/CodeDepsJiro/CodeDepsJiro.csproj -c Release -r linux-x64 --self-contained false
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./MyProject.csproj
```

### dotnet で実行（クロスプラットフォーム）
```
dotnet build src/CodeDepsJiro/CodeDepsJiro.csproj
dotnet src/CodeDepsJiro/bin/Debug/net10.0/CodeDepsJiro.dll ./MyProject.csproj
```

### 出力オプション
```
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./MyProject.csproj --format json --output out/code-deps-jiro.json
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./MyProject.csproj --format csv --output out/code-deps-jiro.csv
```

`--output` を指定しない場合は標準出力に結果が出力されます。

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
  "edges": [
    { "from": "MyApp.Services.UserService", "to": "MyApp.Data.UserRepository", "relationType": "Field" }
  ]
}
```

## ライセンス
MIT License。詳細は `LICENSE` を参照してください。

## 第三者ライセンス
依存ライブラリのライセンスは `THIRD_PARTY_NOTICES.md` を参照してください。
