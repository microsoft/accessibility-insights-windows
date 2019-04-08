// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Results;
using Microsoft.CodeAnalysis.Sarif;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Axe.Windows.CoreTests.Results
{
    [TestClass]
    public class ScanStatusExtensionsUnitTests
    {
        private const string ResultLevelOpenString = "open";
        private const string ResultLevelPassString = "pass";
        private const string ResultLevelErrorString = "error";
        private const string ResultLevelNoteString = "note";

        [TestMethod]
        [Timeout(1000)]
        public void ToResultLevel_ReceivesValidParameters_FailResult()
        {
            Assert.AreEqual(ResultLevel.Error, ScanStatus.Fail.ToResultLevel());
        }

        [TestMethod]
        [Timeout(1000)]
        public void ToResultLevel_ReceivesValidParameters_PassResult()
        {
            Assert.AreEqual(ResultLevel.Pass, ScanStatus.Pass.ToResultLevel());
        }

        [TestMethod]
        [Timeout(1000)]
        public void ToResultLevel_ReceivesValidParameters_ScanNotSupportedResult()
        {
            Assert.AreEqual(ResultLevel.Note, ScanStatus.ScanNotSupported.ToResultLevel());
        }

        [TestMethod]
        [Timeout(1000)]
        public void ToResultLevel_ReceivesValidParameters_UncertainResult()
        {
            Assert.AreEqual(ResultLevel.Open, ScanStatus.Uncertain.ToResultLevel());
        }

        [TestMethod]
        [Timeout(1000)]
        public void ToResultLevel_ReceivesValidParameters_NoResultResult()
        {
            Assert.AreEqual(ResultLevel.Open, ScanStatus.NoResult.ToResultLevel());
        }

        [TestMethod]
        [Timeout(1000)]
        public void ToResultLevel_AllValues_NoException()
        {
            foreach (ScanStatus scanStatus in Enum.GetValues(typeof(ScanStatus)))
            {
                scanStatus.ToResultLevel();
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void ToResultLevelString_ReceivesValidParameters_FailResult()
        {
            Assert.AreEqual(ResultLevelErrorString, ScanStatus.Fail.ToResultLevelString());
        }

        [TestMethod]
        [Timeout(1000)]
        public void ToResultLevelString_ReceivesValidParameters_PassResult()
        {
            Assert.AreEqual(ResultLevelPassString, ScanStatus.Pass.ToResultLevelString());
        }

        [TestMethod]
        [Timeout(1000)]
        public void ToResultLevelString_ReceivesValidParameters_ScanNotSupportedResult()
        {
            Assert.AreEqual(ResultLevelNoteString, ScanStatus.ScanNotSupported.ToResultLevelString());
        }

        [TestMethod]
        [Timeout(1000)]
        public void ToResultLevelString_ReceivesValidParameters_UncertainResult()
        {
            Assert.AreEqual(ResultLevelOpenString, ScanStatus.Uncertain.ToResultLevelString());
        }

        [TestMethod]
        [Timeout(1000)]
        public void ToResultLevelString_ReceivesValidParameters_NoResultResult()
        {
            Assert.AreEqual(ResultLevelOpenString, ScanStatus.NoResult.ToResultLevelString());
        }

        [TestMethod]
        [Timeout(1000)]
        public void ToResultLevelString_AllValues_NoException()
        {
            foreach (ScanStatus scanStatus in Enum.GetValues(typeof(ScanStatus)))
            {
                scanStatus.ToResultLevelString();
            }
        }

    }
}
