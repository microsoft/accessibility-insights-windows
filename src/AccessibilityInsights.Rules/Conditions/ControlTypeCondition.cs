// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;

namespace Axe.Windows.Rules
{
    class ControlTypeCondition : Condition
    {
        public int ControlType { get; } = 0;


        public ControlTypeCondition(int controlType)
        {
            if (controlType == 0) throw new ArgumentException();

            this.ControlType = controlType;
        }

        public override bool Matches(IA11yElement element)
        {
            if (element == null) throw new ArgumentException();

            return element.ControlTypeId == this.ControlType;
        }

        public override string ToString()
        {
            // stripping away the integer name because it makes conditions harder to read
            var s = Axe.Windows.Core.Types.ControlType.GetInstance()?.GetNameById(this.ControlType);
            return s.Substring(0, s.IndexOf('('));
        }
    } // class
} // namespace
