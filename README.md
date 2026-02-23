# depgraph

A CLI tool that analyzes dependencies in C#/.NET projects and visualizes them as plain text or Graphviz DOT.

## Features
- Extract class and namespace dependencies
- Detect cyclic dependencies
- Validate layer rules via a rules file

## Usage
### Publish (Linux x64)
```
dotnet publish src/DepGraph/DepGraph.csproj -c Release -r linux-x64 --self-contained false
./src/DepGraph/bin/Release/net10.0/linux-x64/publish/DepGraph ./MyProject.csproj
```

### Run with dotnet (cross-platform)
```
dotnet build src/DepGraph/DepGraph.csproj
dotnet src/DepGraph/bin/Debug/net10.0/DepGraph.dll ./MyProject.csproj
```

## License
MIT License. See `LICENSE` for details.

## Third-Party Notices
See `THIRD_PARTY_NOTICES.md` for third-party licenses.
