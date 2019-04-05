// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using System.Text;

using static System.FormattableString;

namespace Axe.Windows.Desktop.Styles
{
    /// <summary>
    /// FlowDirections
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671227(v=vs.85).aspx
    public class FlowDirection : TypeBase
    {
        const string Prefix = "FlowDirections_";

#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int FlowDirections_Default = 0;
        public const int FlowDirections_RightToLeft = 1;
        public const int FlowDirections_BottomToTop = 2;
        public const int FlowDirections_Vertical = 3;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static FlowDirection sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static FlowDirection GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new FlowDirection();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private FlowDirection() : base(Prefix) { }

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
