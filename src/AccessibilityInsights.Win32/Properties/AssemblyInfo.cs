// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("AccessibilityInsights.Win32")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("AccessibilityInsights.Win32")]
[assembly: AssemblyCopyright("Copyright © 2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("608e7ef9-c670-4152-a056-46448e2f1e18")]

// Limit P/Invoke to assemblies located in the System32 folder
[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)]

#if ENABLE_SIGNING
[assembly: InternalsVisibleTo("AccessibilityInsights,PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293")]
[assembly: InternalsVisibleTo("AccessibilityInsights.SetupLibrary,PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293")]
[assembly: InternalsVisibleTo("AccessibilityInsights.SharedUx,PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293")]
[assembly: InternalsVisibleTo("AccessibilityInsights.CommonUxComponents,PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293")]
[assembly: InternalsVisibleTo("AccessibilityInsights.Extensions,PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293")]
#else
[assembly: InternalsVisibleTo("AccessibilityInsights")]
[assembly: InternalsVisibleTo("AccessibilityInsights.SetupLibrary")]
[assembly: InternalsVisibleTo("AccessibilityInsights.SharedUx")]
[assembly: InternalsVisibleTo("AccessibilityInsights.CommonUxComponents")]
[assembly: InternalsVisibleTo("AccessibilityInsights.Extensions")]
[assembly: InternalsVisibleTo("UITests")] // we don't sign our UITests, so we only need this with signing disabled
#endif

#region FxCop analysis suppressions for entire assembly
[assembly: SuppressMessage("Microsoft.Naming", "CA1707", Justification = "Underscores are allowed to keep the same name as Win32")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1714", Justification = "Keep the same name as Win32")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1717", Justification = "Keep the same name as Win32")]
[assembly: SuppressMessage("Microsoft.Design", "CA1028", Justification = "Keep the same name as Win32")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1815", Justification = "== or != operators are not needed for parity with Win32")]
[assembly: SuppressMessage("Microsoft.Design", "CA1051", Justification = "For Win32 structure parity, allow visible instance fields")]
#endregion
