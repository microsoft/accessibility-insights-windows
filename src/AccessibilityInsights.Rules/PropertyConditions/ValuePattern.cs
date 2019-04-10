// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Windows.Rules.PropertyConditions
{
    /// <summary>
    /// Exposes the value property to enable condition matching against the Value pattern of an IA11yElement.
    /// </summary>
    static class ValuePattern
    {
        public const string ValuePropertyString = "Value";
        public static StringProperty ValueProperty = new StringProperty(e => 
            e.GetPattern(Core.Types.PatternType.UIA_ValuePatternId)?.GetValue<string>(ValuePropertyString));
    }
}
