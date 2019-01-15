// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System.Text;

using static System.FormattableString;

namespace AccessibilityInsights.Desktop.Styles
{
    /// <summary>
    /// CaretPositions
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/hh448745(v=vs.85).aspx
    /// </summary>
    public class CaretPosition : TypeBase
    {
        const string Prefix = "CaretPosition_";

#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int CaretPosition_Unknown = 0;
        public const int CaretPosition_EndOfLine = 1;
        public const int CaretPosition_BeginningOfLine = 2;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static CaretPosition sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static CaretPosition GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new CaretPosition();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private CaretPosition() : base(Prefix) { }

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
