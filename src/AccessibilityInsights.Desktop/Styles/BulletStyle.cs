// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System.Text;

using static System.FormattableString;

namespace AccessibilityInsights.Desktop.Styles
{
    /// <summary>
    /// Bullet Style
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee684018(v=vs.85).aspx
    /// </summary>
    public class BulletStyle : TypeBase
    {
        const string Prefix = "BulletStyle_";

#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int BulletStyle_None = 0;
        public const int BulletStyle_HollowRoundBullet = 1;
        public const int BulletStyle_FilledRoundBullet = 2;
        public const int BulletStyle_HollowSquareBullet = 3;
        public const int BulletStyle_FilledSquareBullet = 4;
        public const int BulletStyle_DashBullet = 5;
        public const int BulletStyle_Other = 6;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static BulletStyle sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static BulletStyle GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new BulletStyle();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private BulletStyle() : base(Prefix) { }

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
