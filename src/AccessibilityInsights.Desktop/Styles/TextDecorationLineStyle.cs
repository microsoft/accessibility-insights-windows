// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System.Text;

using static System.FormattableString;

namespace AccessibilityInsights.Desktop.Styles
{
    /// <summary>
    /// TextDecorationLineStyles
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671663(v=vs.85).aspx
    public class TextDecorationLineStyle : TypeBase
    {
        const string Prefix = "TextDecorationLineStyle_";

#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int TextDecorationLineStyle_None = 0;
        public const int TextDecorationLineStyle_Single = 1;
        public const int TextDecorationLineStyle_WordsOnly = 2;
        public const int TextDecorationLineStyle_Double = 3;
        public const int TextDecorationLineStyle_Dot = 4;
        public const int TextDecorationLineStyle_Dash = 5;
        public const int TextDecorationLineStyle_DashDot = 6;
        public const int TextDecorationLineStyle_DashDotDot = 7;
        public const int TextDecorationLineStyle_Wavy = 8;
        public const int TextDecorationLineStyle_ThickSingle = 9;
        public const int TextDecorationLineStyle_DoubleWavy = 10;
        public const int TextDecorationLineStyle_ThickWavy = 11;
        public const int TextDecorationLineStyle_LongDash = 12;
        public const int TextDecorationLineStyle_ThickDash = 13;
        public const int TextDecorationLineStyle_ThickDashDot = 14;
        public const int TextDecorationLineStyle_ThickDashDotDot = 15;
        public const int TextDecorationLineStyle_ThickDot = 16;
        public const int TextDecorationLineStyle_ThickLongDash = 17;
        public const int TextDecorationLineStyle_Other = 18;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static TextDecorationLineStyle sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static TextDecorationLineStyle GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new TextDecorationLineStyle();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private TextDecorationLineStyle() : base(Prefix) { }

        /// <summary>
        /// change name into right format in dictionary and list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string GetNameInProperFormat(string name, int id)
        {
            StringBuilder sb = new StringBuilder(name);

            sb.Replace(Prefix, "");
            sb.Append(Invariant($" ({id})"));

            return sb.ToString();
        }
    }
}
