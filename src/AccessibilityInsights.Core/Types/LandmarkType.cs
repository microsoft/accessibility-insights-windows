// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Text;

using static System.FormattableString;

namespace AccessibilityInsights.Core.Types
{
    /// <summary>
    /// LandMarkTypes class
    /// contain LandMarkLevel values
    /// </summary>
    public class LandmarkType : TypeBase
    {
        /// <summary>
        /// this list is from below source code
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/mt759299(v=vs.85).aspx
        /// </summary>
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int UIA_CustomLandmarkTypeId = 80000;
        public const int UIA_FormLandmarkTypeId = 80001;
        public const int UIA_MainLandmarkTypeId = 80002;
        public const int UIA_NavigationLandmarkTypeId = 80003;
        public const int UIA_SearchLandmarkTypeId = 80004;
#pragma warning restore CA1707 // Identifiers should not contain underscores


        private static LandmarkType sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static LandmarkType GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new LandmarkType();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private LandmarkType() : base() { }

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

            sb.Append(Invariant($"({id})"));

            return sb.ToString();
        }
    }
}
