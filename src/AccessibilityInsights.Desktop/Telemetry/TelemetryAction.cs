// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.Desktop.Telemetry
{
    /// <summary>
    /// Telemetry Actions, following the pattern of Scope_Verb_Result
    /// </summary>
    public enum TelemetryAction
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores
        /// <summary>
        /// Mode change event
        /// </summary>
        Mode_Changed,

        /// <summary>
        /// Test request
        /// </summary>
        Test_Requested,
        SingleRule_Tested_Results,

        Selector_Change_ByMousePoint,
        Selector_Change_ByKeyboard,
        Hilighter_Turned_On,
        Hilighter_Turned_Off,
        Hilighter_Expand_AllDescendants,
        Hilighter_Screenshot_Turned_Off,
        Hilighter_Screenshot_Turned_On,

        Hierarchy_Select_View, // select hierarchy view
        Hierarchy_Save,
        Hierarchy_Load_NewFormat,
        Hierarchy_Load_OldFormat,
        Hierarchy_Load_Protocol,
        Hierarchy_Select_Item,
        Hierarchy_Change_TreeWalkMode,
        Hierarchy_Refresh_Tree,
        Hierarchy_MoveTo_Parent,
        Hierarchy_Change_ShowAncestry,
        Hierarchy_Change_ShowUncertain,
        Hierarchy_RefreshAndExpand_Ancestor,

        Event_Select_View,
        Event_Start_Record,
        Event_Stop_Record,
        Event_Save,
        Event_Load,
        Event_Select_Item,
        Event_Open_Configuration, 
        Event_Change_Configuration, 

        Pattern_Select_Item, 
        Pattern_Expand_Item, 
        Pattern_Invoke_Action,
        Pattern_Missing_Action,
        Pattern_Track_UnknownPattern,

        Properties_Show_Basic, 
        Properties_Show_All, 
        Properties_FilterBy_Name, 

        TextPattern_SearchBy_Name,
        TextPattern_Show_TextRange,
        TextPattern_Show_Annotation,
        TextPattern_NoSupported_From,
        TextPattern_Change_From,

        TextRange_Select,
        TextRange_AddToSelection,
        TextRange_RemoveFromSelection,
        TextRange_ScrollIntoTop,
        TextRange_ScrollIntoBottom,
        TextRange_GetChildren,
        TextRange_GetEnclosingElement,
        TextRange_Move,
        TextRange_ExpandToEnclosingUnit,
        TextRange_Clone,
        TextRange_Compare,

        Scan_Show_All,            // expand all test results. 
        Scan_Navigate_HelpLink,
        Scan_Navigate_Snippet,
        Scan_Select_Item,
        Scan_File_Bug,
        Scan_View_ExistingBug,
        Scan_Save_File, // save a file from Automated check view.
        Scan_Statistics,

        Issue_Save,
        Issue_File_Attempt,
        Bug_Cancel,

        Mainwindow_Open_Configuration, 
        Mainwindow_Open_ProcessList, 
        Mainwindow_Minimize_byHotkey,
        Mainwindow_Maximize_byHotkey,
        Mainwindow_Open_Help,
        Mainwindow_Update_Configuration,
        Mainwindow_Update_Hotkey,
        Mainwindow_Login_Attempted,
        Mainwindow_Login_Succeeded,
        Mainwindow_Login_Failed,
        Mainwindow_Logout_Requested,
        Mainwindow_Timer_Started,
        Mainwindow_Startup,

        Connection_Click_Disconnect,
        Connection_Click_Refresh,
        Connection_Click_Next,
        Connection_Click_Change,
        Connection_Open_Configuration,

        Splash_Uncheck_ShowThis,

        Feedback_Open,
        Feedback_Send,
        Feedback_Cancel,

        /// <summary>
        /// Upgrade related traces
        /// </summary>
        Upgrade_Update_ReleaseNote,
        Upgrade_Update_Start,
        Upgrade_Update_Dismiss,
        Upgrade_Notify_MissingMSIInfo,
        Upgrade_InstallationError,
        Upgrade_GetUpgradeOption,
        Upgrade_DoInstallation,

        SharedData_Load_MainConfiguration,
        SharedData_Load_TestConfiguration,
        SharedData_Load_RecordConfiguration,

        TabStop_Record_On, // indicate whether TabStop Recording was on
        TabStop_Record_Off,
        TabStop_Clear_Records, // when clear button is clicked.
        TabStop_Select_Records, // When any of recorded element is selected. 
        TabStop_Test_Looped, // While testing, if tabstop hits any of already recorded tab stops. 

        TestSelection_Set_Scope, // set the scope

        ColorContrast_Open_HowToTest,
        ColorContrast_Click_Eyedropper,
        ColorContrast_Click_Dropdown,
        ColorContrast_FileBug_OutsideGivenScope, // users can file a bug outside of the glimpse tested
        ColorContrast_Click_HexChange,
#pragma warning restore CA1707 // Identifiers should not contain underscores
    }
}
