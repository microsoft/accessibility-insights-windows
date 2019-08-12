The UIA interop files will periodically need to be updated to take advantage of new UIA features. When this happens, do the following:

1. Obtain the updated version of UIAutomationCore.dll. This file lives in the windows\system32 folder of your windows installation.
2. From an administrative command prompt, execute the following command:

   tlbimp UIAutomationCore.dll /namespace:UIAutomationClient /out:Interop.UIAutomationClient.dll

3. Create a new folder for the new Windows version
4. Copy Interop.UIAutomationClient.dll to the new folder
5. Do a global search & replace to update `UIAAssemblies\Win10.XXXXX\` to `UIAAssemblies\Win10.YYYYY\` in the csproj files. As of this writing, this will touch the following projects:

   * SharedUx.csproj
  
6. Delete the folder containing the old interop file (potentially in a separate PR, if you wish)

