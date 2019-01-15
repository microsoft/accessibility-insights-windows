// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Text;

namespace AccessibilityInsights.Core.Types
{
    /// <summary>
    /// Class for Platform Property  Type IDs
    /// this is platform specific Ids
    /// </summary>
    public class PlatformPropertyType:TypeBase
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int Platform_WindowsStylePropertyId = 1;
        public const int Platform_WindowsExtendedStylePropertyId = 2;
        public const int Platform_ProcessNamePropertyId = 3;
#pragma warning restore CA1707 // Identifiers should not contain underscores


        private static PlatformPropertyType sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static PlatformPropertyType GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new PlatformPropertyType();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private PlatformPropertyType() : base("Platform_") { }

        /// <summary>
        /// change name into right format in dictionary and list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string GetNameInProperFormat(string name, int id)
        {
            StringBuilder sb = new StringBuilder(name);

            sb.Replace("Platform_", "");
            sb.Replace("PropertyId", "");
            //sb.AppendFormat("({0})", id);

            return sb.ToString();
        }
    }
}
