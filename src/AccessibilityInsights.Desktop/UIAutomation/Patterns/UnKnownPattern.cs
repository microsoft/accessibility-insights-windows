// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Core.Bases;

namespace Axe.Windows.Desktop.UIAutomation.Patterns
{
    /// <summary>
    /// Control pattern wrapper for Unkonwn Control Pattern
    /// </summary>
    public class UnKnownPattern : A11yPattern
    {
        public UnKnownPattern(A11yElement e, int id, string name) : base(e, id, name)
        {
            if (!PatternType.GetInstance().Exists(id))
            {
                // be silent since Unknown pattern was created since pattern interface couldn't be retrieved by unknown reason. 
            }
        }
    }
}
