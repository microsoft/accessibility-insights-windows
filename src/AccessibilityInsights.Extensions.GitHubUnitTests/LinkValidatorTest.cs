// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AccessibilityInsights.Extensions.GitHub
{
    [TestClass]
    public class LinkValidatorTest
    {
        [TestMethod]
        [Timeout(1000)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsValidGitHubRepoLink_LinkIsNull_ThrowsArgumentNullException()
        {
            LinkValidator.IsValidGitHubRepoLink(null);
        }

        [TestMethod]
        public void IsValidGitHubRepoLink()
        {
            Assert.IsTrue(LinkValidator.IsValidGitHubRepoLink("https://github.com/bla/bla-blas"));
            Assert.IsTrue(LinkValidator.IsValidGitHubRepoLink("https://github.com/bla/bla-blas/"));
            Assert.IsTrue(LinkValidator.IsValidGitHubRepoLink("https:github.com/bla/bla-blas"));
            Assert.IsTrue(LinkValidator.IsValidGitHubRepoLink(@"https:\\github.com/\bla//\bla-blas\"));
            Assert.IsTrue(LinkValidator.IsValidGitHubRepoLink("www.github.com/bla/bla-blas"));
            Assert.IsTrue(LinkValidator.IsValidGitHubRepoLink("https://www.github.com/bla/bla-blas"));

            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink("https://github.com/1-1/bla-blas"));
            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink("https://github.com/a--a/bla-blas"));
            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink("https://github.com/-a/bla-blas"));
            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink("https://github.com/a-/bla-blas"));
            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink("https://github.com//bla-blas"));
            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink("https://github.com/bla/"));
            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink("https://github.com/bla/.."));
            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink(@"https://github.com/bla/bla-blas#"));
            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink(@"https://github.com/bla/bla-bla*s"));
            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink("https://githubs.com/bla/bla-blas"));
            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink("httpss://github.com/bla/bla-blas"));
            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink("wws.github.com/bla/bla-blas"));
            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink("https://wws.github.com/bla/bla-blas"));
            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink(@"https://github.com/bla1234567890123456789012345678901234567890/bla-blas"));
            Assert.IsFalse(LinkValidator.IsValidGitHubRepoLink("https://github.com/bla/bla-blas123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890"));
        }
    }
}
