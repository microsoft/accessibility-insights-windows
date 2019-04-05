// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axe.Windows.Core.Types;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class LocalizedLandmarkTypeNotCustomTests
    {
        private Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.LocalizedLandmarkTypeNotCustom();

        [TestMethod]
        public void LocalizedLandmarkTypeNotCustom_Pass()
        {
            using (var e = new MockA11yElement())
            {
                e.LandmarkType = LandmarkType.UIA_CustomLandmarkTypeId;
                e.LocalizedLandmarkType = "not custom";
                Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
            } // using
        }

        [TestMethod]
        public void LocalizedLandmarkTypeNotCustom_Faile()
        {
            using (var e = new MockA11yElement())
            {
                e.LandmarkType = LandmarkType.UIA_CustomLandmarkTypeId;
                e.LocalizedLandmarkType = "Custom";
                Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
            } // using
        }
    } // class
} // namespace
