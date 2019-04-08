// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axe.Windows.Rules;
using Axe.Windows.RuleSelection;

namespace RuleSelectionTests
{
    [TestClass]
    public class ReferenceLinksTests
    {
        [TestMethod]
        public void EnsureAllReferencesHaveValidLinks()
        {
            foreach (A11yCriteriaId id in Enum.GetValues(typeof(A11yCriteriaId)))
            {
                var link = ReferenceLinks.GetGuidelineInfo(id);
                Assert.IsNotNull(link);
                Assert.IsFalse(string.IsNullOrWhiteSpace(link.ShortDescription));
                Assert.IsFalse(string.IsNullOrWhiteSpace(link.Url));
            }
        }

        [TestMethod]
        public void EnsureNoExceptionsAreThrown()
        {
            var link = ReferenceLinks.GetGuidelineInfo((A11yCriteriaId)0xFFFF);
            Assert.IsNotNull(link);
            Assert.IsFalse(string.IsNullOrWhiteSpace(link.ShortDescription));
        }
    } // class
} // namespace
