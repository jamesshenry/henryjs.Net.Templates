# Test Quality Checklist Template

This is a template checklist for ensuring high-quality unit tests in your project. Customize it by replacing placeholders with your project's specific testing framework, guidelines, and conventions.

Use this checklist during code reviews for unit test PRs to ensure adherence to your project's testing standards.

## Getting Started

1. Replace `[YourTestingGuidelines]` with your project's testing guidelines document
2. Replace `[YourReferenceTestFile]` with a canonical test file in your project
3. Customize the framework-specific items (e.g., test attributes, mocking library)
4. Add or remove items based on your project's testing approach

## Syntax and Structure

- [ ] All test methods follow your project's async/sync conventions (e.g., `public async Task` for async tests).
- [ ] Naming follows your project's convention (e.g., `{Method}_{Scenario}_{ExpectedResult}`).
- [ ] Tests use Arrange-Act-Assert structure with comments if needed.
- [ ] Test organization (classes, nesting) follows your project's patterns.
- [ ] `[YourSetupAttribute]` setup initializes mocks and test subjects correctly.

## Mocking and Dependencies

- [ ] NSubstitute used for all mocks (e.g., `Substitute.For<T>()` for NSubstitute).
- [ ] All dependencies mocked; no real implementations in unit tests.
- [ ] Mock configurations are realistic and cover scenarios.
- [ ] Verifications use appropriate methods (e.g., `Received()` or `DidNotReceive()`).

## Assertions and Coverage

- [ ] AwesomeAssertions used for assertions (e.g., `.Should().Be()` for FluentAssertions).
- [ ] Tests cover happy paths, error conditions, and edge cases.
- [ ] All public APIs have at least one test with meaningful scenarios.
- [ ] No superficial tests added just to increase metrics.

## Quality and Best Practices

- [ ] Tests are independent and isolated (no shared state).
- [ ] Async operations tested properly (e.g., cancellation tokens).
- [ ] Error handling tested (exceptions, invalid inputs).
- [ ] Code follows the style in `[YourReferenceTestFile]` exactly.

## Review Process

- [ ] PR includes links to [YourTestingGuidelines] and reference examples.
- [ ] CI/CD passes without syntax errors.
- [ ] Reviewer confirms no deviations from standards.

If any item is unchecked, request revisions before merging.
