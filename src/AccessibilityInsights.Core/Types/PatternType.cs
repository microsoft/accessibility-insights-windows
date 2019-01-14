// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Text;

namespace AccessibilityInsights.Core.Types
{
    public class PatternType:TypeBase
    {
        // allow having underscore to keep the same name of original names in Win32
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int UIA_InvokePatternId = 10000;
        public const int UIA_SelectionPatternId = 10001;
        public const int UIA_ValuePatternId = 10002;
        public const int UIA_RangeValuePatternId = 10003;
        public const int UIA_ScrollPatternId = 10004;
        public const int UIA_ExpandCollapsePatternId = 10005;
        public const int UIA_GridPatternId = 10006;
        public const int UIA_GridItemPatternId = 10007;
        public const int UIA_MultipleViewPatternId = 10008;
        public const int UIA_WindowPatternId = 10009;
        public const int UIA_SelectionItemPatternId = 10010;
        public const int UIA_DockPatternId = 10011;
        public const int UIA_TablePatternId = 10012;
        public const int UIA_TableItemPatternId = 10013;
        public const int UIA_TextPatternId = 10014;
        public const int UIA_TogglePatternId = 10015;
        public const int UIA_TransformPatternId = 10016;
        public const int UIA_ScrollItemPatternId = 10017;
        public const int UIA_LegacyIAccessiblePatternId = 10018;
        public const int UIA_ItemContainerPatternId = 10019;
        public const int UIA_VirtualizedItemPatternId = 10020;
        public const int UIA_SynchronizedInputPatternId = 10021;
        public const int UIA_ObjectModelPatternId = 10022;
        public const int UIA_AnnotationPatternId = 10023;
        public const int UIA_TextPattern2Id = 10024;
        public const int UIA_StylesPatternId = 10025;
        public const int UIA_SpreadsheetPatternId = 10026;
        public const int UIA_SpreadsheetItemPatternId = 10027;
        public const int UIA_TransformPattern2Id = 10028;
        public const int UIA_TextChildPatternId = 10029;
        public const int UIA_DragPatternId = 10030;
        public const int UIA_DropTargetPatternId = 10031;
        public const int UIA_TextEditPatternId = 10032;
        public const int UIA_CustomNavigationPatternId = 10033;
        public const int UIA_SelectionPattern2Id = 10034;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static PatternType sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static PatternType GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new PatternType();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private PatternType() : base() { }

        /// <summary>
        /// change name into right format in dictionary and list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string GetNameInProperFormat(string name, int id)
        {
            StringBuilder sb = new StringBuilder(name);

            sb.Replace("UIA_", "");
            sb.Replace("Id", "");
            //sb.AppendFormat("({0})", id); // skip Id for now

            return sb.ToString();
        }
     }
}
