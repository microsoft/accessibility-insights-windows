// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.SharedUx.Telemetry
{
    /// <summary>
    /// Telemetry Actions, following the pattern of Scope_Verb_Result
    /// </summary>
    public enum TelemetryAction
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores
        /// <summary>
        /// Test request
        /// </summary>
        Test_Requested,

        Hilighter_Expand_AllDescendants,

        Hierarchy_Save,
        Hierarchy_Load_NewFormat,

        Event_Start_Record,
        Event_Save,
        Event_Load,

        Pattern_Invoke_Action,

        Scan_File_Bug,
        Issue_Save,
        Issue_File_Attempt,

        Mainwindow_Timer_Started,
        Mainwindow_Startup,

        /// <summary>
        /// Upgrade related traces
        /// </summary>
        Upgrade_Update_ReleaseNote,
        Upgrade_Update_Dismiss,
        Upgrade_InstallationError,
        Upgrade_GetUpgradeOption,
        Upgrade_DoInstallation,
        Upgrade_VersionSwitcherResults,

        TabStop_Record_On, // indicate whether TabStop Recording was on
        TabStop_Select_Records, // When any of recorded element is selected.

        TestSelection_Set_Scope, // set the scope

        ColorContrast_AutoDetect,
        ColorContrast_Click_Autodetect_Toggle,
        ColorContrast_Click_Eyedropper,
        ColorContrast_Click_Dropdown,
        ColorContrast_Click_HexChange,

        ReleaseChannel_ChangeConsidered,

        Custom_UIA,
#pragma warning restore CA1707 // Identifiers should not contain underscores
    }
}
