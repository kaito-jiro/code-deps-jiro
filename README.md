# depgraph

A CLI tool that analyzes dependencies in C#/.NET projects and visualizes them as plain text or Graphviz DOT.

## Features
- Extract class and namespace dependencies
- Detect cyclic dependencies
- Validate layer rules via a rules file

## Usage
```
dotnet run --project src/DepGraph/DepGraph.csproj -- ./MyProject.csproj
```

## License
MIT License. See `LICENSE` for details.

## Third-Party Notices
See `THIRD_PARTY_NOTICES.md` for third-party licenses.
