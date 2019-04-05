// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Axe.Windows.Core.Enums;

namespace Axe.Windows.Rules
{
    /// <summary>
    /// Supplies rules, ensuring that each rule is created only once, and only when requested.
    /// </summary>
    class RuleProvider
    {
        private readonly IRuleFactory RuleFactory = null;
        private readonly ConcurrentDictionary<RuleId, IRule> AllRules = new ConcurrentDictionary<RuleId, IRule>();
        private readonly Object AllRulesLock = new Object();
        private bool AreAllRulesInitialized = false;

        public RuleProvider(IRuleFactory ruleFactory)
        {
            if (ruleFactory == null) throw new ArgumentNullException(nameof(ruleFactory));

            this.RuleFactory = ruleFactory;
        }

        private void InitAllRules()
        {
            if (AreAllRulesInitialized) return;

            /* the lock is an optimization of the ConcurrentDictionary
             * So that if multiple threads simultaneously run all rules
             * the ConcurrentDictionary won't end up creating extra objects that will never be used
             * and only be fodder for the garbage collector.
             * */
            lock (AllRulesLock)
            {
                if (AreAllRulesInitialized) return;

                // Calling GetRule ensures that the rule is created.
                foreach (var k in Axe.Windows.Rules.RuleFactory.RuleIds)
                    GetRule(k);

                AreAllRulesInitialized = true;
            } // lock
        }

        public IRule GetRule(RuleId id)
        {
            return AllRules.GetOrAdd(id, key => this.RuleFactory.CreateRule(key));
        }

        public IEnumerable<IRule> All
        {
            get
            {
                InitAllRules();

                return AllRules.Values;
            }
        }
    } // class
} // namespace
