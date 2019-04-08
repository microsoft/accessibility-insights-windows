// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Threading;
using Axe.Windows.Core.Bases;
using Axe.Windows.Rules.Resources;

namespace Axe.Windows.Rules
{
    abstract class Condition
    {
        public abstract bool Matches(IA11yElement element);

        /// <summary>
        /// Stores context data on a per-thread basis which may be accessed by conditions
        /// The context is typically set when using relationships like
        /// children, siblings, parents, ancestors, and descendants.
        /// This data allows a condition to compare the properties of the current element under test to saved values from other parts of the hierarchy.
        /// </summary>
        public static ThreadLocal<ConditionContext> Context = new ThreadLocal<ConditionContext>();

        /// <summary>
        /// This function must be called once per-thread that uses it.
        /// </summary>
        public static void InitContext()
        {
            if (Context.IsValueCreated) return;

            Context.Value = new ConditionContext();
        }

        public static OrCondition operator |(Condition c1, Condition c2)
        {
            return new OrCondition(c1, c2);
        }

        public static AndCondition operator &(Condition c1, Condition c2)
        {
            return new AndCondition(c1, c2);
        }

        public static NotCondition operator ~(Condition c)
        {
            return new NotCondition(c);
        }

        public static AndCondition operator -(Condition c1, Condition c2)
        {
            var notC2 = new NotCondition(c2);
            return new AndCondition(c1, notC2);
        }

        public static TreeDescentCondition operator /(Condition c1, Condition c2)
        {
            return new TreeDescentCondition(c1, c2);
        }

        public StringDecoratorCondition this[string s]
        {
            get
            {
                return new StringDecoratorCondition(this as Condition, s);
            }
        }

        public static Condition Create(DelegateCondition.MatchesDelegate d)
        {
            return new DelegateCondition(d);
        }

        public static Condition Create(DelegateCondition.MatchesDelegate d, string description)
        {
            return Create(d)[description];
        }

        public static Condition True = Create(e => true, ConditionDescriptions.True);
        public static Condition False = Create(e => false, ConditionDescriptions.False);
        public static Condition DebugBreak = Create(e => { System.Diagnostics.Debugger.Break(); return true; });
    } // class
} // namespace
