// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System.Text;

using static System.FormattableString;

namespace AccessibilityInsights.Desktop.Types
{
    /// <summary>
    /// ChangeInfoTypes class
    /// contain ChangeInfoType values
    /// </summary>
    public class ChangeInfoType : TypeBase
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores
        /// <summary>
        /// this list is from below source code
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/mt732980(v=vs.85).aspx
        /// </summary>
        public const int UIA_SummaryChangeId = 90000; //L"UIA_SummaryChangeId";
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static ChangeInfoType sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static ChangeInfoType GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new ChangeInfoType();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private ChangeInfoType() : base() { }

        /// <summary>
        /// change name into right format in dictionary and list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string GetNameInProperFormat(string name, int id)
        {
            StringBuilder sb = new StringBuilder(name);

            sb.Append(Invariant($"({id})"));

            return sb.ToString();
        }
    }
}
