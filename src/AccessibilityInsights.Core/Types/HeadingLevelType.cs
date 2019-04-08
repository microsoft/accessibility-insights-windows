// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Text;

using static System.FormattableString;

namespace Axe.Windows.Core.Types
{
    /// <summary>
    /// HeadingLevelTypes class
    /// contain HeadingLevel values
    /// </summary>
    public class HeadingLevelType : TypeBase
    {
        /// <summary>
        /// this list is from below source code
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/mt829376(v=vs.85).aspx
        /// </summary>
        public const int HeadingLevelNone = 80050;
        public const int HeadingLevel1 = 80051;
        public const int HeadingLevel2 = 80052;
        public const int HeadingLevel3 = 80053;
        public const int HeadingLevel4 = 80054;
        public const int HeadingLevel5 = 80055;
        public const int HeadingLevel6 = 80056;
        public const int HeadingLevel7 = 80057;
        public const int HeadingLevel8 = 80058;
        public const int HeadingLevel9 = 80059;

        private static HeadingLevelType sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static HeadingLevelType GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new HeadingLevelType();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private HeadingLevelType() : base() { }

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
