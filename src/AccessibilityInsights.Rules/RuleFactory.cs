// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AccessibilityInsights.Core.Enums;

namespace AccessibilityInsights.Rules
{
    class RuleFactory : IRuleFactory
    {
        private static readonly IReadOnlyDictionary<RuleId, Type> RuleTypes = CreateRuleTypes();
        private static Lazy<IEnumerable<RuleId>> _RuleIds = new Lazy<IEnumerable<RuleId>>(() => RuleTypes.Keys);
        public static IEnumerable<RuleId> RuleIds => _RuleIds.Value;

        private static IReadOnlyDictionary<RuleId, Type> CreateRuleTypes()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var types = assembly.GetTypes();

            var ruleTypes = from t in types
                            where t.BaseType == typeof(Rule)
                            select t;

            return ruleTypes.ToDictionary(t =>
            {
                var info = (RuleInfo)t.GetCustomAttribute(typeof(RuleInfo));
                if (info == null) throw new InvalidOperationException("All rules are expected to have the RuleInfo attribute with the ID property set.");

                return info.ID;
            });
        }

        public IRule CreateRule(RuleId id)
        {
            if (!RuleTypes.TryGetValue(id, out Type type))
                return null;

            return Activator.CreateInstance(type) as IRule;
        }
    } // class
} // namespace
