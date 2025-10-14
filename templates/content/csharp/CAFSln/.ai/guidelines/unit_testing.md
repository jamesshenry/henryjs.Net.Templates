# Unit Testing Guidelines Template

This is a template for establishing unit testing standards in your project. Customize it by replacing placeholders with your chosen testing framework, libraries, and conventions.

## Overview

This document establishes the definitive standards and idioms for writing unit tests in your [YourProjectName] project. All tests must adhere strictly to these guidelines to ensure consistency, maintainability, and quality. The reference implementation in `[YourReferenceTestFile]` serves as the canonical example—**deviate at your own risk**.

## General Principles

- **Framework**: Use [YourTestFramework] for test execution.
- **Mocking**: Use [YourMockingLibrary] for dependency mocking.
- **Assertions**: Use [YourAssertionLibrary] for fluent, readable assertions.
- **Async**: [Specify your async/sync test conventions].
- **Isolation**: Tests must be independent; no shared state between tests.
- **Naming**: Follow [YourNamingConvention] (e.g., `{Method}_{Scenario}_{ExpectedResult}`).
- **Structure**: Use Arrange-Act-Assert (AAA) pattern with comments where clarity is needed.

## Test Structure and Syntax

- **Test Class**: `public class {FeatureName}Tests`.
  - Add using directives at top of test class:
    - `using TUnit.Core;`
    - `using NSubstitute;`
    - `using AwesomeAssertions;`
    - `using [YourProjectNamespace].Features.{FeatureName};`
- **Nested Classes**: Group tests by component (e.g., `public class {ComponentName}Tests`).
- **Setup**: Use `[Before(Test)] public void Setup()` for initializing mocks and test subjects.
- **Test Methods**: `[Test] public async Task {DescriptiveName}()`.
- **Imports**: Include your testing framework namespaces and feature-specific usings.
- **Reference**: See `[YourReferenceTestFile]` for exact syntax.

## Mocking with [YourMockingLibrary]

- Create mocks: [Your mock creation syntax].
- Configure returns: [Your return configuration syntax].
- Verify calls: [Your verification syntax].
- Exceptions: [Your exception setup syntax].
- **Idiom**: Mock all dependencies in setup; avoid over-mocking.

## Assertions with [YourAssertionLibrary]

- Equality: [Your equality assertion syntax].
- Collections: [Your collection assertion syntax].
- Exceptions: [Your exception assertion syntax].
- Events: [Your event assertion syntax].
- Nulls: [Your null assertion syntax].
- **Idiom**: [Your preferred assertion style and patterns].

## Quality Assurance

- **Code Reviews**: Require peer reviews for test PRs using the checklist in `TEST_QUALITY_CHECKLIST.md`.
- **CI/CD Checks**: Automate validation of test syntax in pipelines.
- **Linting**: Use analyzers or config to enforce idioms at build time.
- **Templates**: Provide code snippets for consistent test structure.
- **Quality Metrics**: Track separately (e.g., coverage, mutation testing).
- Run tests: [Your test command].
- Build: [Your build command] before running tests.
- Documentation: Update AGENTS.md with test summaries post-implementation.

## Examples

Refer to `[YourReferenceTestFile]` for complete examples:

- [List types of testing examples in your reference file].
- [Add more example categories as needed].

## Enforcement

- **Mandatory**: All new tests must match the style in `[YourReferenceTestFile]`.
- **Review**: Pull requests with tests deviating from these guidelines will be rejected.
- **Updates**: If standards evolve, update this document and the reference file accordingly.

This guide is static and applies project-wide. For feature-specific implementation steps, refer to your implementation plan documents.
