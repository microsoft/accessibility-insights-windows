## Debugging Extensions

### The problem
Accessibility Insights for Windows loads extensions using [MEF](https://docs.microsoft.com/en-us/dotnet/framework/mef/), which decouples the extension implementations from the main app. As such, when you build the app in your debug environment, none of the extensions are available. This makes it more difficult to debug the extension. 

### The solution
To simplify debugging, we just need to persuade Visual Studio to put the extension into the same folder as the application. The easiest way to do this is to temporarily cause ApplicationInsights.csproj to include a reference to the extension implementation. Obviously, these changes ***must*** be removed before submitting a PR.

#### Editing via IDE
Add the following projects via the UI by opening the AccessibilityInsights project, then adding references as follows:

Extension | Project
--- | ---
AutoUpdate | Extensions.GitHubAutoUpdate
Azure Boards Issue Filing | Extensions.AzureDevOps
GitHub Issue Filing | Extensions.GitHub
Telemetry | Extensions.Telemetry

#### Editing manually
For those who prefer to edit files directly, here are the blurbs to add to AccessibilityInsights.csproj:

##### AutoUpdate (GitHub)
```
    <ProjectReference Include="..\AccessibilityInsights.Extensions.GitHubAutoUpdate\Extensions.GitHubAutoUpdate.csproj">
      <Project>{80165ccc-6d76-4590-a16b-1790a35b08e4}</Project>
      <Name>Extensions.GitHubAutoUpdate</Name>
    </ProjectReference>
```

##### Issue Filing (AzureDevOps)
```
    <ProjectReference Include="..\AccessibilityInsights.Extensions.AzureDevOps\Extensions.AzureDevOps.csproj">
      <Project>{58dbf9fe-0029-408f-a8a4-23ae500e938d}</Project>
      <Name>Extensions.AzureDevOps</Name>
    </ProjectReference>
```

##### Issue Filing (GitHub)
```
    <ProjectReference Include="..\AccessibilityInsights.Extensions.GitHub\Extensions.GitHub.csproj">
      <Project>{f421aafd-d85f-49b8-b95e-ca17718c38dd}</Project>
      <Name>Extensions.GitHub</Name>
    </ProjectReference>
```

##### Telemetry
```
    <ProjectReference Include="..\AccessibilityInsights.Extensions.Telemetry\Extensions.Telemetry.csproj">
      <Project>{d17e1749-28fe-44b9-a9f6-af8985124fe4}</Project>
      <Name>Extensions.Telemetry</Name>
    </ProjectReference>
```