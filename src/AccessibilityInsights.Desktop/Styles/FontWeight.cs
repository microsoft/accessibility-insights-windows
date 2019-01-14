// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System.Text;

using static System.FormattableString;

namespace AccessibilityInsights.Desktop.Styles
{
    /// <summary>
    /// Font Weight
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee684020(v=vs.85).aspx
    /// </summary>
    public class FontWeight : TypeBase
    {
        const string Prefix = "FontWeight_";

#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int FontWeight_DontCare = 0;
        public const int FontWeight_Thin = 100;
        public const int FontWeight_ExtraLightOrUltraLight = 200;
        public const int FontWeight_Light = 300;
        public const int FontWeight_NormalOrRegular = 400;
        public const int FontWeight_Medium = 500;
        public const int FontWeight_SemiBold = 600;
        public const int FontWeight_Bold = 700;
        public const int FontWeight_ExtraBoldOrUltraBold = 800;
        public const int FontWeight_HeavyOrBlack = 900;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static FontWeight sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static FontWeight GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new FontWeight();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private FontWeight() : base(Prefix) { }

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
