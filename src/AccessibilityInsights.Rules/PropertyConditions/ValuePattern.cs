using Axe.Windows.Core.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Windows.Rules.PropertyConditions
{
    /// <summary>
    /// Contains commonly used conditions for testing against the Value pattern of an IA11yElement.
    /// </summary>
    static class ValuePattern
    {
        public const string ValuePropertyString = "Value";
        public static StringProperty ValueProperty = new StringProperty(e => 
            e.GetPattern(Core.Types.PatternType.UIA_ValuePatternId)?.GetValue<string>(ValuePropertyString));
    }
}
