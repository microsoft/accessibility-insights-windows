// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;

namespace Axe.Windows.Rules
{
    /// <summary>
    /// Handles the push and pop operations of setting context information.
    /// The context is a set of data that is stored during a rule run.
    /// It can be accessed by other conditions.
    /// </summary>
    class ContextCondition : Condition
    {
        public delegate void ContextDelegate(IA11yElement element);
        private readonly ContextDelegate Initialize = null;
        private readonly ContextDelegate Finalize = null;
        private readonly Condition Sub = null;

        public ContextCondition(Condition sub, ContextDelegate initialize, ContextDelegate finalize)
        {
            if (sub == null) throw new ArgumentException(nameof(sub));
            if (initialize == null) throw new ArgumentException(nameof(initialize));
            if (finalize == null) throw new ArgumentException(nameof(finalize));

            this.Sub = sub;
            this.Initialize = initialize;
            this.Finalize = finalize;
        }

        public override bool Matches(IA11yElement e)
        {
            // Ensure the context is initialized
            Condition.InitContext();
            this.Initialize(e);

            bool retVal = false;

            try
            {
                retVal = Sub.Matches(e);
            }
            catch (Exception)
            {
                this.Finalize(e);
                throw;
            }

            this.Finalize(e);

            return retVal;
        }

        public override string ToString()
        {
            return this.Sub?.ToString();
        }
    } // class
} // namespace
