# .NET Template Project Structure Designs

## Overview

This document outlines the proposed project structures for the .NET project templates, including console apps, solutions, and optional data access layers.

## Template Organization

### Solution Template (cafsln)

- **Structure**: Multi-project solution with console app, data layer, and tests.
- **Projects**:
  - Main console application
  - Data access layer (optional)
  - Test project
- **Build System**: Shared build props and CI/CD workflows.

### Console Template (cafc)

- **Structure**: Single-project console application.
- **Features**: Basic setup with logging, configuration, and commands.
- **Extensions**: Ready for dependency injection and services.

### Build Template (cafbuild)

- **Structure**: Build and deployment scripts.
- **Components**: Targets for build, clean, restore, pack.
- **CI/CD**: GitHub workflows for build and release.

## Core Template Elements

### Configuration Files

- **.template.config**: Defines template metadata, symbols, and sources.
- **Directory.Build.props**: Shared properties for all projects.
- **global.json**: Specifies .NET SDK version.

### Project Files

- **.csproj**: Project definitions with conditional includes.
- **Program.cs**: Entry point with dependency injection setup.
- **appsettings.json**: Configuration for logging and settings.

## Project Structure Designs

### 1. Solution Template Structure (cafsln)

**Purpose**: Full solution with console app, optional data layer, and tests.

**Layout**: Multi-project structure with shared build system.

- **Root**: Solution file and build scripts.
- **src/**: Source projects.
- **.template.config/**: Template configuration.
- **.build/**: Build assets and targets.
- **.github/**: CI/CD workflows.

```
CAFConsole.slnx
├── src/
│   ├── CAFConsole/
│   │   ├── Commands/
│   │   │   └── MyCommands.cs
│   │   ├── Configuration/
│   │   │   └── CAFConsoleSettings.cs
│   │   ├── Filters/
│   │   │   └── ExceptionFilter.cs
│   │   ├── Logging/
│   │   │   └── SourceClassEnricher.cs
│   │   ├── Properties/
│   │   │   └── launchSettings.json
│   │   ├── Services/
│   │   │   ├── IService.cs
│   │   │   ├── ServiceExtensions.cs
│   │   │   └── ServiceImplementation.cs
│   │   ├── CAFConsole.csproj
│   │   ├── config.json
│   │   ├── Constants.cs
│   │   └── Program.cs
│   ├── CAFConsole.Data/ (conditional)
│   │   ├── Services/
│   │   │   └── ServiceExtensions.cs
│   │   ├── Sqlite/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── DatabaseOptions.cs
│   │   └── CAFConsole.Data.csproj
│   └── CAFConsole.Tests/
│       ├── CAFConsole.Tests.csproj
│       └── Tests.cs
├── .template.config/
│   ├── build/
│   │   └── template.json
│   ├── console/
│   │   └── template.json
│   └── sln/
│       └── template.json
├── .build/
│   ├── assets/
│   │   ├── icon.ico
│   │   └── icon.png
│   ├── Directory.Build.props
│   ├── Directory.Build.targets
│   └── targets.cs
├── .github/
│   └── workflows/
│       ├── ci.yml
│       ├── release-nuget.yml
│       └── release.yml
├── .editorconfig
├── build.ps1
├── build.sh
├── CAFConsole.slnx
├── dotnet.config
├── ef.rsp
└── README.md
```

### 2. Console Template Structure (cafc)

**Purpose**: Simple console application template.

**Layout**: Single-project structure with essential components.

- **Root**: Project file and source code.
- **Commands/**: Command handlers.
- **Configuration/**: Settings classes.
- **Services/**: Dependency injection services.
- **Properties/**: Launch settings.

```
CAFConsole/
├── Commands/
│   └── MyCommands.cs
├── Configuration/
│   └── CAFConsoleSettings.cs
├── Filters/
│   └── ExceptionFilter.cs
├── Logging/
│   └── SourceClassEnricher.cs
├── Properties/
│   └── launchSettings.json
├── Services/
│   ├── IService.cs
│   ├── ServiceExtensions.cs
│   └── ServiceImplementation.cs
├── CAFConsole.csproj
├── config.json
├── Constants.cs
└── Program.cs
```

### 3. Build Template Structure (cafbuild)

**Purpose**: Build system and CI/CD template.

**Layout**: Build scripts and workflow configurations.

- **.build/**: Build targets and assets.
- **.github/workflows/**: CI/CD pipelines.
- **build.ps1/build.sh**: Cross-platform build scripts.

```
.build/
├── assets/
│   ├── icon.ico
│   └── icon.png
├── Directory.Build.props
├── Directory.Build.targets
└── targets.cs
.github/
└── workflows/
    ├── ci.yml
    ├── release-nuget.yml
    └── release.yml
build.ps1
build.sh
dotnet.config
global.json
```

### 4. Data Access Layer Structure

**Purpose**: Optional data layer for Entity Framework Core.

**Layout**: Services and database context.

- **Services/**: Extension methods for DI.
- **Sqlite/**: EF Core context and options.
- **Conditional**: Only included when `withDataAccess` is true.

```
CAFConsole.Data/
├── Services/
│   └── ServiceExtensions.cs
├── Sqlite/
│   ├── AppDbContext.cs
│   ├── DatabaseOptions.cs
└── CAFConsole.Data.csproj
```

### 5. Test Project Structure

**Purpose**: Unit testing setup for generated projects.

**Layout**: Test framework with basic test structure.

- **Tests.cs**: Sample test class.
- **.csproj**: Test project configuration.
- **Frameworks**: TUnit with Microsoft.Testing.Platform.

```
CAFConsole.Tests/
├── CAFConsole.Tests.csproj
└── Tests.cs
```

## Template Development Notes

- **Template Engine**: Uses dotnet template system with `.template.config` for metadata and symbols.
- **Symbols**: Define parameters like `IsSolution`, `withDataAccess` for conditional content.
- **Sources**: Organize files in subdirectories for different template types.
- **Post-Actions**: Use post-actions for running commands after template instantiation (e.g., dotnet restore).
- **Conditionals**: Use `#if` directives in code for optional features.
- **Packaging**: Pack templates into NuGet packages for distribution.

## Implementation Status

### Current Templates

- **cafsln**: Fully implemented with conditional data access layer
- **cafc**: Implemented as single-project console template
- **cafbuild**: Implemented with build targets and CI/CD workflows

### Template Features

- **Symbols**: `IsSolution` and `withDataAccess` implemented
- **Conditionals**: `#if` directives used for optional components
- **Post-Actions**: Restore command executed after instantiation
- **Packaging**: NuGet package generation working

## Development Roadmap

### Phase 1: Core Templates (Completed)

- Basic console and solution templates
- Data access layer integration
- Build system and CI/CD

### Phase 2: Enhancements (In Progress)

- **Advanced Symbols**: Add more parameters (e.g., database type selection)
- **Template Validation**: Automated testing of generated projects
- **Documentation**: Comprehensive README generation

### Phase 3: Expansion (Planned)

- **New Template Types**: Web API, Blazor, class library templates
- **Localization**: Multi-language template support
- **Advanced Features**: Custom post-actions, complex conditionals

## Future Enhancements

- **Template Marketplace**: Publish to official .NET template feeds
- **Interactive Mode**: Guided template instantiation with prompts
- **Template Updates**: Automated update mechanism for installed templates
- **Performance**: Optimize template instantiation speed
