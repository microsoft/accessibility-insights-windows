// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Text;

using static System.FormattableString;

namespace AccessibilityInsights.Core.Types
{
    /// <summary>
    /// Class for Control Types
    /// </summary>
    public class ControlType:TypeBase
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int UIA_ButtonControlTypeId = 50000;
        public const int UIA_CalendarControlTypeId = 50001;
        public const int UIA_CheckBoxControlTypeId = 50002;
        public const int UIA_ComboBoxControlTypeId = 50003;
        public const int UIA_EditControlTypeId = 50004;
        public const int UIA_HyperlinkControlTypeId = 50005;
        public const int UIA_ImageControlTypeId = 50006;
        public const int UIA_ListItemControlTypeId = 50007;
        public const int UIA_ListControlTypeId = 50008;
        public const int UIA_MenuControlTypeId = 50009;
        public const int UIA_MenuBarControlTypeId = 50010;
        public const int UIA_MenuItemControlTypeId = 50011;
        public const int UIA_ProgressBarControlTypeId = 50012;
        public const int UIA_RadioButtonControlTypeId = 50013;
        public const int UIA_ScrollBarControlTypeId = 50014;
        public const int UIA_SliderControlTypeId = 50015;
        public const int UIA_SpinnerControlTypeId = 50016;
        public const int UIA_StatusBarControlTypeId = 50017;
        public const int UIA_TabControlTypeId = 50018;
        public const int UIA_TabItemControlTypeId = 50019;
        public const int UIA_TextControlTypeId = 50020;
        public const int UIA_ToolBarControlTypeId = 50021;
        public const int UIA_ToolTipControlTypeId = 50022;
        public const int UIA_TreeControlTypeId = 50023;
        public const int UIA_TreeItemControlTypeId = 50024;
        public const int UIA_CustomControlTypeId = 50025;
        public const int UIA_GroupControlTypeId = 50026;
        public const int UIA_ThumbControlTypeId = 50027;
        public const int UIA_DataGridControlTypeId = 50028;
        public const int UIA_DataItemControlTypeId = 50029;
        public const int UIA_DocumentControlTypeId = 50030;
        public const int UIA_SplitButtonControlTypeId = 50031;
        public const int UIA_WindowControlTypeId = 50032;
        public const int UIA_PaneControlTypeId = 50033;
        public const int UIA_HeaderControlTypeId = 50034;
        public const int UIA_HeaderItemControlTypeId = 50035;
        public const int UIA_TableControlTypeId = 50036;
        public const int UIA_TitleBarControlTypeId = 50037;
        public const int UIA_SeparatorControlTypeId = 50038;
        public const int UIA_SemanticZoomControlTypeId = 50039;
        public const int UIA_AppBarControlTypeId = 50040;
#pragma warning restore CA1707 // Identifiers should not contain underscores


        private static ControlType sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static ControlType GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new ControlType();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private ControlType() : base() { }


        /// <summary>
        /// change name into right format in dictionary and list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string GetNameInProperFormat(string name, int id)
        {
            StringBuilder sb = new StringBuilder(name);

            sb.Replace("UIA_", "");
            sb.Replace("ControlTypeId", "");
            sb.Append(Invariant($"({id})"));

            return sb.ToString();
        }

    }
}
