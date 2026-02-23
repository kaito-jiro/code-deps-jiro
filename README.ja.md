# depgraph

C#/.NET プロジェクトの依存関係を解析し、Plain または Graphviz DOT で可視化する CLI ツールです。

## 主な機能
- クラス/名前空間の依存抽出
- 循環依存の検出
- ルールファイルによるレイヤー違反検出

## 使い方
### Publish（Linux x64）
```
dotnet publish src/DepGraph/DepGraph.csproj -c Release -r linux-x64 --self-contained false
./src/DepGraph/bin/Release/net10.0/linux-x64/publish/DepGraph ./MyProject.csproj
```

### dotnet で実行（クロスプラットフォーム）
```
dotnet build src/DepGraph/DepGraph.csproj
dotnet src/DepGraph/bin/Debug/net10.0/DepGraph.dll ./MyProject.csproj
```

## ライセンス
MIT License。詳細は `LICENSE` を参照してください。

## 第三者ライセンス
依存ライブラリのライセンスは `THIRD_PARTY_NOTICES.md` を参照してください。
