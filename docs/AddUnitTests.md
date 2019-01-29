# Unit Testing

We use the [MSTest framework](https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting?view=mstest-net-1.2.0) to write unit tests.

## Unit Test Setup
Setting up unit tests for an assembly is trivial. 
The steps are:
1. Use VisualStudio to **add a new Unit Test Project** (available as a new project template listed under Visual C#, then Test)
   1. In Visual Studio, right-click on the solution and add a new project.
   2. Select unit tests from the template list.
   3. Name your project consistently with the existing test projects. If your project is Widgets.Stuff, then your test project should be named *Widgets.Stuff**Tests***.

2. **Add a unit test class** to the project. Name the class after the class it will be testing--if the class is named *ExplodingWidget*, then the test class should be named *ExplodingWidget**Tests***
    1. In Visual Studio, right-click on the test project and add a new test class.
    2. Rename the test class from UnitTests to the name that matches your class name. So if your class is SuperList, your unit test class should be SuperListTests.

>Hint: if you right-click on a method that you need to test, you'll get a template that does this for you automatically. The only thing it doesn't do is to name the actual test according to what's described in Standards

For each test, add a method like this:

```C#
[TestMethod]
[Timeout (1000)]
public void DefaultDelay_IsFiveSeconds()
{
    ExplodingWidget widget = new ExplodingWidget();
    Assert.AreEqual(TimeSpan.FromSeconds(5), widget.Delay);
}
```

The required portions in the above example are:
1. `TestMethod` attribute.
2. The test method be `public void`.


When you build, this test will be built and made available for discovery. As long as `widget.Delay` returns a `TimeSpan` object that corresponds to 5 seconds, the test will fail. If the return value is a different type, or if it has a different value, then `Assert.AreEqual` will return `false`, and the test will fail.<br>

If `widget.Delay` throws an exception, then the exception will be caught by the test framework and the test will also fail. In either case, all other tests will continue to execute, and the aggregate result will be rolled up into the reports that are included as part of the build loop.

The Assert methods also have versions that include messages, for cases where the basic assertion is unclear. For example,<br>

`Assert.IsTrue(dictionary.TryGetValue(key, out string value));` <br>

will just tell you that `TryGetValue` failed, without telling you the key it was looking for. You can easily improve the readability of the output by adding a description:<br>

`Assert.IsTrue(dictionary.TryGetValue(key, out string value, "Couldn't find key: " + key));`

The output will now tell you that `IsTrue` failed, but also why. This is especially handy in cases where your Assert statement appears within a loop

## Working with Fakes
Due to the known issue (overwriting fakes assemblies from different unit tests if they have same name), all `Fakes` assemblies are generated in a single project (`AccessibilityInsights.Fakes.Prebuild`). 

If you need to add any new `Fake` assembly, please follow the below steps:
1. Add a new fakes file into `AccessibilityInsights.Fakes.Prebuild`. 
2. Build `AccessibilityInsights.Fakes.Prebuild`.
3. Add a project dependency (not project reference) of `AccessibilityInsights.Fakes.Prebuild` into the unit test which needs a new fakes assembly.
4. Add an assembly reference of *.fakes.dll (or exe) in `AccessibilityInsights.Fakes.Prebuild/FakesAssemblies` folder to the unit test.
5. Add `Microsoft.QualityTools.Testing.Fakes.dll` into the assembly reference of the unit test.

For unit test bar and standards please visit [Unit test Bar and Standards](UnitTestBarAndStandards.md)
