# code-deps-jiro

A CLI tool that analyzes dependencies in C#/.NET projects and exports results as JSON.

## Features
- Extract class and namespace dependencies
- Export dependency edges as JSON
- Validate layer rules via a rules file
- Resolve `.csproj` `Compile Include/Remove` and `ProjectReference`

## Usage
### Publish (Linux x64)
```
dotnet publish src/CodeDepsJiro/CodeDepsJiro.csproj -c Release -r linux-x64 --self-contained false
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./path/to/MyProject/MyProject.csproj
```

### Run with dotnet (cross-platform)
```
dotnet build src/CodeDepsJiro/CodeDepsJiro.csproj
dotnet src/CodeDepsJiro/bin/Debug/net10.0/CodeDepsJiro.dll ./path/to/MyProject/MyProject.csproj
```

### Output options
```
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./path/to/MyProject/MyProject.csproj --format json --output out/code-deps-jiro.json
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./path/to/MyProject/MyProject.csproj --rules rules.json --format json --output out/code-deps-jiro.json
```

If `--output` is not specified, results are written to standard output.
Rule violations are included only in JSON output under `violations`.

### Rules file (JSON)
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

## Example Output
When `--output` is omitted (standard output):
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
    ],
    "violations": []
}
```

JSON (excerpt):
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

JSON with rule violations (excerpt):
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

## License
MIT License. See `LICENSE` for details.

## Third-Party Notices
See `THIRD_PARTY_NOTICES.md` for third-party licenses.
