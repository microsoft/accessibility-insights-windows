## Adding a new project
Do the following when adding a new project:

### For all projects
1. Add a project to the AccessibilityInsights Solution (`src\AccessibilityInsights.sln`).
2. Right-click on the project and select Properties.
2. In the Application tab, configure "Target Framework" to use the same .NET Framework version used by the `AccessibilityInsights` project.
   - Currently .NET Framework 4.8 is used as target. 
3. In the build tab, set the following for both Debug and Release configurations:
   1. "Warning level" to 4.
   2. "Treat warnings as errors" to "All".

### For *non-test* projects only
1. Add the following NuGet packages in Visual Studio to enable signing and code analysis:
   ```
   Microsoft.VisualStudioEng.MicroBuild.Core
   Microsoft.CodeAnalysis.FxCopAnalyzers
   ```
2. Close the solution and use your text editor to make the following changes to your `.csproj` file to properly configure the version and signing options:
   1. Add the following line with the other `.cs` files):<br>
   `<Compile Include="$(TEMP)\A11yInsightsVersionInfo.cs" />`
   2. Add the following below the last ItemGroup:<br>
   ```
   <ItemGroup>
      <DropSignedFile Include="$(OutDir)\[your project name].dll" />
   </ItemGroup>
   <Import Project="..\..\build\settings.targets" />
   ```
3. Use your text editor to make the following changes to your project's `Properties\AssemblyInfo` file to set the correct version:
   1. Remove the following lines from the following lines, as well as the commented lines above them: <br>
      ```
      [assembly: AssemblyVersion("1.0.*")]
      [assembly: AssemblyFileVersion("1.0.0.0")]
      ```
4. Verify that Visual Studio can successfully load and build the entire solution in both Debug and Release.
5. If your new assembly is to be shipped in the MSI, use your text editor to add the following to the `Product.wxs` file in the `MSI` project:
   1. If your project does not implement an extension interface, add the following with the other main assemblies:<br>
   `<File Source="..\AccessibilityInsights\bin\Release\<AssemblyName>.dll" />`
   2. If your project implements an extension interface, repeat the following pattern for your assembly and each of its dependencies in the `ExtensionsComp` component:<br>
   `<File Source="..\AccessibilityInsights.Extensions.Telemetry\bin\Release\<AssemblyName>.dll" Id = <AssemblyName>_extension"/>`<br>
   (Add assembly dependencies if they aren't already installed by another extension--if you aren't are of your dependencies, look at the contents of your project's `bin\release` folder).
   3. After updating the `.WXS` file, manually build the `MSI` project by right-clicking on it in the Solution Explorer, then clicking "Build". Address any errors that occur.

### For *test* projects only
1. Close the solution and use your text editor to add the following line to your `.csproj` file to properly configure [Strong Naming](https://docs.microsoft.com/en-us/dotnet/framework/app-domains/strong-named-assemblies) for your assembly:<br>
   `<Import Project="..\..\build\delaysign.targets" />`
2. Verify that Visual Studio can successfully load and build the entire solution in both Debug and Release. Ensure that the tests are appropriately discovered in the test explorer, and that they all run successfully.