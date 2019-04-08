// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using Axe.Windows.Core.Bases;

namespace Axe.Windows.Rules
{
    /// <summary>
    /// This condition is used to change the output of the ToString function for a given condition.
    /// This can be useful for adding meaningful or simplified names to otherwise complicated conditions.
    /// </summary>
    class StringDecoratorCondition : Condition
    {
        private Condition Sub;
        private readonly string Decoration = null;

        public StringDecoratorCondition(Condition c, String decoration)
        {
            if (c == null) throw new ArgumentException(nameof(c));
            if (decoration == null) throw new ArgumentException();

            this.Sub = c;
            this.Decoration = decoration;
        }

        public override bool Matches(IA11yElement element)
        {
            return this.Sub.Matches(element);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, this.Decoration, this.Sub);
        }
    } // class
} // namespace
