// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using System.Text;

using static System.FormattableString;

namespace Axe.Windows.Desktop.Styles
{
    /// <summary>
    /// SayAsInterpretAs
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/mt798244(v=vs.85).aspx
    /// </summary>
    public class SayAsInterpretAs : TypeBase
    {
        const string Prefix = "SayAsInterpretAs_";

#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int SayAsInterpretAs_None = 0;
        public const int SayAsInterpretAs_Spell = 1;
        public const int SayAsInterpretAs_Cardinal = 2;
        public const int SayAsInterpretAs_Ordinal = 3;
        public const int SayAsInterpretAs_Number = 4;
        public const int SayAsInterpretAs_Date = 5;
        public const int SayAsInterpretAs_Time = 6;
        public const int SayAsInterpretAs_Telephone = 7;
        public const int SayAsInterpretAs_Currency = 8;
        public const int SayAsInterpretAs_Net = 9;
        public const int SayAsInterpretAs_Url = 10;
        public const int SayAsInterpretAs_Address = 11;
        public const int SayAsInterpretAs_Alphanumeric = 12;
        public const int SayAsInterpretAs_Name = 13;
        public const int SayAsInterpretAs_Media = 14;
        public const int SayAsInterpretAs_Date_MonthDayYear = 15;
        public const int SayAsInterpretAs_Date_DayMonthYear = 16;
        public const int SayAsInterpretAs_Date_YearMonthDay = 17;
        public const int SayAsInterpretAs_Date_YearMonth = 18;
        public const int SayAsInterpretAs_Date_MonthYear = 19;
        public const int SayAsInterpretAs_Date_DayMonth = 20;
        public const int SayAsInterpretAs_Date_MonthDay = 21;
        public const int SayAsInterpretAs_Date_Year = 22;
        public const int SayAsInterpretAs_Time_HoursMinutesSeconds12 = 23;
        public const int SayAsInterpretAs_Time_HoursMinutes12 = 24;
        public const int SayAsInterpretAs_Time_HoursMinutesSeconds24 = 25;
        public const int SayAsInterpretAs_Time_HoursMinutes24 = 26;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static SayAsInterpretAs sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static SayAsInterpretAs GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new SayAsInterpretAs();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private SayAsInterpretAs() : base(Prefix) { }

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
