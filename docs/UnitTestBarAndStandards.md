## Bar
* All new non-UI code must have unit tests.
* Unit tests need to accompany the code in the PR. This provides implicit documentation of the expected behavior, and ensures that the behavior is being satisfied. In an exceptional circumstance, a PR might be approved without its unit tests, but that must not become the normal expectation.
* All unit tests should be passing locally before the PR is created.
* Unit tests need to conform to the standards listed below.

## Standards
* The test project is appropriately named (tests for the `Foo.Bar.dll` are in `Foo.BarTests.dll`)
* The test namespace is appropriately named (the top level namespace matches the assembly name, then the hierarchy in the assembly is preserved. So if `Foo.Bar.dll` contains a namespace named `Widget`, the tests will be in the `Foo.BarTests.Widget` namespace)
* The test class is appropriately named (tests for the `ExplodingWidget` class will be in the `ExplodingWidgetTests` class)
* Each test follows the naming conventions documented at [Unit Test Basics](https://msdn.microsoft.com/en-us/library/hh694602.aspx) Summarizing:
  * Name is in the general form `MethodName_TestCase_ExpectedBehavior`
  * If the test is expected to throw an `Exception`, the test should declare this via the `ExpectedException` attribute. If the test needs to check the contents of the `Exception`, the best pattern is to catch the expected `Exception` type, validate it, then re-throw the `Exception`
* Each test uses appropriate `Assert` methods to validate its conditions--use the version with a hint where it seems helpful
* Each test specifies a reasonable `Timeout` value (default to 1 second each). The test output is much more readable if the test times out and fails than if the entire build loop hangs and reaches the 60 minute time constraint
  * One downside to the `Timeout` attribute--since the `Timeout` applies while debugging the unit test, it basically needs to be commented out to make debugging feasible.
* Appropriate mocks decouple the test class from the underlying dependencies (Dependency Injection is a common pattern for this). This is required for new classes, best effort for changes to existing classes
* If Moq is used, mocks should ideally use `MockBehavior.Strict`--this isn't always possible, but using it makes for a much stronger test
* If Moq is used, each Mock's `VerifyAll` should be called at the end of the test, to ensure that all configured methods were exercised as expected. Where both `MockBehavior.Strict` and `VerifyAll`, the resulting test will exactly pin down the interaction with dependency objects