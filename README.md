# code-deps-jiro

A CLI tool that analyzes dependencies in C#/.NET projects and exports results as plain text, JSON, or CSV.

## Features
- Extract class and namespace dependencies
- Export dependency edges to text, JSON, or CSV
- Validate layer rules via a rules file

## Usage
### Publish (Linux x64)
```
dotnet publish src/CodeDepsJiro/CodeDepsJiro.csproj -c Release -r linux-x64 --self-contained false
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./MyProject.csproj
```

### Run with dotnet (cross-platform)
```
dotnet build src/CodeDepsJiro/CodeDepsJiro.csproj
dotnet src/CodeDepsJiro/bin/Debug/net10.0/CodeDepsJiro.dll ./MyProject.csproj
```

### Output options
```
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./MyProject.csproj --format json --output out/code-deps-jiro.json
./src/CodeDepsJiro/bin/Release/net10.0/linux-x64/publish/CodeDepsJiro ./MyProject.csproj --format csv --output out/code-deps-jiro.csv
```

If `--output` is not specified, results are written to standard output.

## Example Output
Plain text:
```
MyApp.Services.UserService -> MyApp.Data.UserRepository
```

When `--output` is omitted (standard output):
```
MyApp.Services.UserService -> MyApp.Data.UserRepository
MyApp.Controllers.UserController -> MyApp.Services.UserService
```

JSON (excerpt):
```json
{
    "edges": [
        { "from": "MyApp.Services.UserService", "to": "MyApp.Data.UserRepository", "relationType": "Field" }
    ]
}
```

## License
MIT License. See `LICENSE` for details.

## Third-Party Notices
See `THIRD_PARTY_NOTICES.md` for third-party licenses.
