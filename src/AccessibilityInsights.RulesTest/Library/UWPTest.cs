// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules.PropertyConditions;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class UWPTest
    {
        [TestMethod]
        public void TestUWPTitleBarTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.AutomationId = "TitleBarLeftButtons";
                e.ClassName = "ApplicationFrameTitleBarWindow";
                e.Framework = Framework.Win32;

                Assert.IsTrue(UWP.TitleBar.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestUWPTitleBarAutomationIDFalse()
        {
            using (var e = new MockA11yElement())
            {
                // e.AutomationID = "TitleBarLeftButtons";
                e.ClassName = "ApplicationFrameTitleBarWindow";
                e.Framework = Framework.Win32;

                Assert.IsFalse(UWP.TitleBar.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestUWPTitleBarClassNameFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.AutomationId = "TitleBarLeftButtons";
                // e.ClassName = "ApplicationFrameTitleBarWindow";
                e.Framework = Framework.Win32;

                Assert.IsFalse(UWP.TitleBar.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestUWPTitleBarFrameworkFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.AutomationId = "TitleBarLeftButtons";
                e.ClassName = "ApplicationFrameTitleBarWindow";
                // e.UIFramework = Frameworks.Win32;

                Assert.IsFalse(UWP.TitleBar.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestUWPMenuBarTrue()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.Parent = parent;

                e.AutomationId = "SystemMenuBar";
                parent.Framework = Framework.Win32;

                Assert.IsTrue(UWP.MenuBar.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestUWPMenuBarAutomationIDFalse()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.Parent = parent;

                // e.AutomationID = "SystemMenuBar";
                parent.Framework = Framework.Win32;

                Assert.IsFalse(UWP.MenuBar.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestUWPMenuBarFrameworkFalse()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.Parent = parent;

                e.AutomationId = "SystemMenuBar";
                // parent.UIFramework = Frameworks.Win32;

                Assert.IsFalse(UWP.MenuBar.Matches(e));
            } // using
        }
    } // class
} // namespace
