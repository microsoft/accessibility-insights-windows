
## How to
- Add a project to the solution (Always follow these steps)
  1. Add a project under AccessibilityInsights Solution(…\src\accessibilityinsights.sln).
  2. Make the project use same .Net Framework version of `AccessibilityInsights.Core` project. 
     - Currently .Net Framework 4.7.1 is used as target. 
  3. Right click on the project, click properties -> Build -> set "Treat warnings as errors" to "All".
  4. Update `VersionInfo.cs` file compilation setting (If the project is a unit test project, you don't need this and the following steps.) <br>
		 `<Compile Include="$(TEMP)\A11yInsightsVersionInfo.cs" />`
  5. Remove the following lines from the `Properties\AssemblyInfo.cs` file in the new project. Make sure to remove all the commented lines above this as well. <br>
      ```
      [assembly: AssemblyVersion("1.0.*")]
      [assembly: AssemblyFileVersion("1.0.0.0")]
      ```
  6. Add the following NuGet package
    - `Microsoft.VisualStudioEng.MicroBuild.Core` (For digital signing)
    - `Microsoft.CodeAnalysis.FxCopAnalyzers`
	
- Digital Signing (non-test binaries) - Any binaries except test binaries should be digitally signed via the build pipeline.
  - To sign, Open the project file with notepad and add below lines.<br>

		<ItemGroup>
		  <DropSignedFile Include="$(OutDir)\[your project name].dll" />
		</ItemGroup>
		<Import Project="..\..\build\settings.targets" />

- Strong name signing (test binaries) - Test binaries should be built with a strong name key only if they need to access the internal types of signed binaries. 
  - To sign, Open the project file with a text editor of your choice and add below lines. <br>
		`<Import Project="..\..\build\delaysign.targets" />`

- Add a DLL to the MSI
  - Add the dll into the `Products.wxs` file to be part of the MSI (MSI project)
		`<File Source="..\AccessibilityInsights\bin\Release\name of dll" />`
  - Files from extensions should be included as part of the `ExtensionsComp` component and should copy from the extensions project <br> `<File Source="..\AccessibilityInsights.Extensions.Telemetry\bin\Release\Microsoft.ServiceBus.dll" />`
