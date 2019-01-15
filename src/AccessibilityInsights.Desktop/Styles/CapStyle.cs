// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System.Text;

using static System.FormattableString;

namespace AccessibilityInsights.Desktop.Styles
{
    /// <summary>
    /// Cap Style
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee684020(v=vs.85).aspx
    /// </summary>
    public class CapStyle : TypeBase
    {
        const string Prefix = "CapStyle_";

#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int CapStyle_None = 0;
        public const int CapStyle_SmallCap = 1;
        public const int CapStyle_AllCap = 2;
        public const int CapStyle_AllPetiteCaps = 3;
        public const int CapStyle_PetiteCaps = 4;
        public const int CapStyle_Unicase = 5;
        public const int CapStyle_Titling = 6;
        public const int CapStyle_Other = 7;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static CapStyle sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static CapStyle GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new CapStyle();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private CapStyle() : base(Prefix) { }

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
