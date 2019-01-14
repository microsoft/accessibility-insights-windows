// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System.Text;

using static System.FormattableString;

namespace AccessibilityInsights.Desktop.Styles
{
    /// <summary>
    /// HorizontalTextAlignments
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671233(v=vs.85).aspx
    /// </summary>
    public class HorizontalTextAlignment : TypeBase
    {
        const string Prefix = "HorizontalTextAlignment_";

#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int HorizontalTextAlignment_Left = 0;
        public const int HorizontalTextAlignment_Centered = 1;
        public const int HorizontalTextAlignment_Right = 2;
        public const int HorizontalTextAlignment_Justified = 3;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static HorizontalTextAlignment sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static HorizontalTextAlignment GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new HorizontalTextAlignment();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private HorizontalTextAlignment() : base(Prefix) { }

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
