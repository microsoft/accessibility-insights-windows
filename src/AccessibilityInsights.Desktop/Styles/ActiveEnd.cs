// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System.Text;

using static System.FormattableString;

namespace AccessibilityInsights.Desktop.Styles
{
    /// <summary>
    /// ActiveEnds
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/hh448742(v=vs.85).aspx#ActiveEnd_None
    public class ActiveEnd : TypeBase
    {
        const string Prefix = "ActiveEnd_";

#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int ActiveEnd_None = 0;
        public const int ActiveEnd_Start = 1;
        public const int ActiveEnd_End = 2;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static ActiveEnd sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static ActiveEnd GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new ActiveEnd();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private ActiveEnd() : base(Prefix) { }

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
