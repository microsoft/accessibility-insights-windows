// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using System;
using System.Globalization;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// Class TabStopItemViewModel
    /// this is for tab stop listview
    /// </summary>
    public class TabStopItemViewModel : ViewModelBase
    {
        /// <summary>
        /// Element
        /// </summary>
        public A11yElement Element { get; private set; }

        /// <summary>
        /// Element order number
        /// </summary>
        public String Number { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="e">A11yElement</param>
        /// <param name="sr">Scan result</param>
        public TabStopItemViewModel(A11yElement e, int num)
        {
            this.Element = e;

            Number = num.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// String for narrator
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}: {1}", Number, Element.Glimpse);
        }
    }
}
