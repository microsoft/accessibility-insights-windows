// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Rules;
using AccessibilityInsights.RuleSelection;

namespace AccessibilityInsights.ReferenceLinksTests
{
    [TestClass]
    public class DefaultReferenceLinksTests
    {
        private static readonly DefaultReferenceLinks Defaults = new DefaultReferenceLinks();

        [TestMethod]
        public void EnsureAllReferencesHaveValidLinks()
        {
            foreach (var id in Enum.GetValues(typeof(A11yCriteriaId)))
            {
                var link = Defaults.GetReferenceLink(id.ToString());
                Assert.IsNotNull(link);
                Assert.IsFalse(string.IsNullOrWhiteSpace(link.ShortDescription));
                Assert.IsNotNull(link.Uri);
                Assert.IsTrue(link.Uri.IsWellFormedOriginalString());
            }
        }

        [TestMethod]
        public void EnsureNoExceptionsAreThrown()
        {
            var link = Defaults.GetReferenceLink("not valid");
            Assert.IsNotNull(link);
            Assert.IsFalse(string.IsNullOrWhiteSpace(link.ShortDescription));
        }
    } // class
} // namespace
