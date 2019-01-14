#The steps to add a project to the solution (always do these steps)
	a. Add a project under AccessibilityInsights Solution(…\src\accessibilityinsights.sln)
	b. Make the project use same .NetFramework version of Core project. 
		i. Currently .NetFramework 4.7.1 is used as target. 
	c. Right click on the project, click properties -> Build -> set "Treat warnings as errors" to "All"
	d. Update VersionInfo.cs file compilation setting(If the project is a unit test project, you don't need this and below steps.)
		 <Compile Include="$(TEMP)\A11yInsightsVersionInfo.cs" />
	e. Remove the following lines from the Properties\AssemblyInfo.cs file in the new project. Make sure to remove all the commented lines above this as well.
			[assembly: AssemblyVersion("1.0.*")]
			[assembly: AssemblyFileVersion("1.0.0.0")]
	f. Add the following NuGet package
		i. Microsoft.VisualStudioEng.MicroBuild.Core (For digital signing)
		ii. Microsoft.CodeAnalysis.FxCopAnalyzers
	
#Digital Signing (non-test binaries) - Any binaries except test binaries should be digitally signed via the build pipeline.
	○ To sign, Open the project file with notepad and add below lines.
		<ItemGroup>
		    <DropSignedFile Include="$(OutDir)\[your project name].dll" />
		</ItemGroup>
		<Import Project="..\..\build\settings.targets" />

#Strong name signing (test binaries) - Test binaries should be built with a strong name key only if they need to access the internal types of signed binaries. 
	○ To sign, Open the project file with notepad and add below lines.
		<Import Project="..\..\build\delaysign.targets" />

#If a particular DLL needs to be shipped
	○ Add the dll into the Products.wxs file to be part of MSI(MSI project)
		<File Source="..\AccessibilityInsights\bin\Release\name of dll" />
	○ Files from extensions should be included as part of the "ExtensionsComp" component and should copy from the extensions project
		<File Source="..\AccessibilityInsights.Extensions.Telemetry\bin\Release\Microsoft.ServiceBus.dll" />
