// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using System.Text;

using static System.FormattableString;

namespace Axe.Windows.Desktop.Styles
{
    /// <summary>
    /// CaretBidiModes
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/hh448744(v=vs.85).aspx
    /// </summary>
    public class CaretBidiMode : TypeBase
    {
        const string Prefix = "CaretBidiMode_";

#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int CaretBidiMode_LTR = 0;
        public const int CaretBidiMode_RTL = 1;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static CaretBidiMode sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static CaretBidiMode GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new CaretBidiMode();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private CaretBidiMode() : base(Prefix) { }

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
