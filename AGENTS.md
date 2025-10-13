# Agent Guidelines for henryjs.Net.Templates

## Repository Purpose
This repository provides .NET C# project templates using the dotnet template system. Templates are defined in `templates/content/csharp/CAFSln/.template.config/`.

## Template Types
- **cafsln**: ConsoleAppFramework solution template (with optional data access layer)
- **cafc**: ConsoleAppFramework console project template
- **cafbuild**: Build system and CI/CD template

## Build Commands
- **Build**: `dotnet run .build/targets.cs build` or `./build.ps1 build`
- **Clean**: `dotnet run .build/targets.cs clean`
- **Restore**: `dotnet run .build/targets.cs restore`
- **Pack**: `dotnet run .build/targets.cs pack`

## Test Commands
- **Run all tests**: `dotnet test` (test target is placeholder, run directly on test projects)
- **Run single test**: `dotnet test --filter "FullyQualifiedName~TestName"`
- **Test framework**: TUnit with Microsoft.Testing.Platform runner

## Template Development
- **Pack templates**: `dotnet run .build/targets.cs pack` (generates nupkg in dist/nuget/)
- **Install templates locally**: `dotnet new install dist/nuget/*.nupkg`
- **Test templates**: `dotnet new cafsln --name TestProject` or `dotnet new cafc --name TestConsole`
- **Test with data access**: `dotnet new cafsln --name TestProject --withDataAccess true`
- **Uninstall templates**: `dotnet new uninstall henryjs.Net.Templates`

## Template Symbols & Conditionals
- **IsSolution**: Computed boolean (true for solution template, false for console template)
- **withDataAccess**: Parameter boolean for including Entity Framework Core data layer
- Use `#if (condition)` directives in code for conditional compilation

## Code Style Guidelines

### Formatting & Structure
- **Indentation**: 4 spaces, no tabs
- **Line endings**: CRLF
- **Final newlines**: Not required
- **File-scoped namespaces**: Preferred
- **Braces**: Required for all blocks
- **Using directives**: Outside namespace

### Naming Conventions
- **Classes/Types**: PascalCase
- **Interfaces**: IPascalCase
- **Methods/Properties**: PascalCase
- **Parameters/Locals**: camelCase
- **Private fields**: _camelCase
- **Constants**: PascalCase

### Language Features
- **Nullable**: Enabled
- **Implicit usings**: Enabled
- **Target framework**: .NET 10.0
- **Primary constructors**: Preferred
- **Expression-bodied members**: Preferred for accessors, properties, lambdas

### Imports & Dependencies
- **System directives**: Sorted first
- **Import groups**: Separated
- **Predefined types**: Preferred (string vs String)

### Error Handling
- **Logging**: Serilog with structured logging
- **Console apps**: ExceptionFilter for global error handling
- **Async**: Use async/await patterns consistently

### Analysis & Linting
- **Roslynator analyzers**: Enabled for code quality
- **EditorConfig**: Comprehensive formatting rules applied
- **Code coverage**: Microsoft.Testing.Extensions.CodeCoverage