// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using System.Text;

using static System.FormattableString;

namespace Axe.Windows.Desktop.Styles
{
    /// <summary>
    /// Animation Style
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee684012(v=vs.85).aspx#AnimationStyle_None
    /// </summary>
    public class AnimationStyle : TypeBase
    {
        const string Prefix = "AnimationStyle_";

#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int AnimationStyle_None = 0;
        public const int AnimationStyle_LasVegasLights = 1;
        public const int AnimationStyle_BlinkingBackground = 2;
        public const int AnimationStyle_SparkleText = 3;
        public const int AnimationStyle_MarchingBlackAnts = 4;
        public const int AnimationStyle_MarchingRedAnts = 5;
        public const int AnimationStyle_Shimmer = 6;
        public const int AnimationStyle_Other = 7;
#pragma warning restore CA1707 // Identifiers should not contain underscores


        private static AnimationStyle sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static AnimationStyle GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new AnimationStyle();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private AnimationStyle() : base(Prefix) { }

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
