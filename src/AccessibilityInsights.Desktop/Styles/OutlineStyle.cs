// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System.Text;

using static System.FormattableString;

namespace AccessibilityInsights.Desktop.Styles
{
    /// <summary>
    /// Animation Style
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671592(v=vs.85).aspx
    /// </summary>
    public class OutlineStyle : TypeBase
    {
        const string Prefix = "OutlineStyles_";

#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int OutlineStyles_None = 0;
        public const int OutlineStyles_Outline = 1;
        public const int OutlineStyles_Shadow = 2;
        public const int OutlineStyles_Engraved = 3;
        public const int OutlineStyles_Embossed = 4;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static OutlineStyle sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static OutlineStyle GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new OutlineStyle();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private OutlineStyle() : base(Prefix) { }

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
