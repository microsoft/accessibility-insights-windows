// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using static AccessibilityInsights.RulesTest.ControlType;
using Misc = AccessibilityInsights.Rules.PropertyConditions.ElementGroups;

namespace AccessibilityInsights.RulesTest.PropertyConditions
{
    [TestClass]
    public class NameTest
    {
        private readonly IEnumerable<int> RequiredTypes = null;
        private readonly IEnumerable<int> OptionalTypes = null;
        private readonly IEnumerable<int> ExcludedTypes = null;

        public NameTest()
        {
            RequiredTypes = new List<int>
            {
                Calendar, CheckBox, ComboBox,
               DataGrid, DataItem, Document,
               Edit, HeaderItem, Hyperlink,
               List, ListItem, Menu,
               MenuBar, MenuItem, ProgressBar,
               RadioButton, SemanticZoom, Slider, Spinner,
               SplitButton, TabItem,
               Table, ToolBar,
                ToolTip, Tree, TreeItem, Window
        };

            OptionalTypes = new List<int> { Group, Pane };

            int[] specialTests =
                {
                Button, Custom, Header, Image, StatusBar, Text
            };

            var temp = new List<int>(RequiredTypes);
            temp.AddRange(OptionalTypes);
            temp.AddRange(specialTests);

            ExcludedTypes = ControlType.All.Difference(temp);
        }

        [TestMethod]
        public void TestRequiredTypes()
        {
            using (var e = new MockA11yElement())
            {
                foreach (var controlType in RequiredTypes)
                {
                    e.ControlTypeId = controlType;
                    Assert.IsTrue(Misc.NameRequired.Matches(e));
                }
            } // using
        }

        [TestMethod]
        public void TestOptionalTypes()
        {
            using (var e = new MockA11yElement())
            {
                foreach (var controlType in OptionalTypes)
                {
                    e.ControlTypeId = controlType;
                    Assert.IsTrue(Misc.NameOptional.Matches(e));
                }
            } // using
        }

        [TestMethod]
public void TestExcludedTypes()
{
    using (var e = new MockA11yElement())
    {
        foreach (var controlType in ExcludedTypes)
        {
            e.ControlTypeId = controlType;
            Assert.IsFalse(Misc.NameRequired.Matches(e));
        }
    } // using
}

[TestMethod]
public void TestButtonWithAllowedParents()
{
    using (var parent = new MockA11yElement())
    using (var e = new MockA11yElement())
    {
        var parentTypes = ControlType.All.Difference(ComboBox, ScrollBar, Slider, Spinner, SplitButton, TitleBar);

        e.ControlTypeId = Button;
        e.Parent = parent;

        foreach (var controlType in parentTypes)
        {
            parent.ControlTypeId = controlType;
            Assert.IsTrue(Misc.NameRequired.Matches(e));
        }
    } // using
}

[TestMethod]
public void TestButtonWithDisallowedParents()
{
    using (var parent = new MockA11yElement())
    using (var e = new MockA11yElement())
    {
        int[] parentTypes = { ComboBox, ScrollBar, TitleBar };

        e.ControlTypeId = Button;
        e.Parent = parent;

        foreach (var controlType in parentTypes)
        {
            parent.ControlTypeId = controlType;
            Assert.IsFalse(Misc.NameRequired.Matches(e));
        }
    } // using
}

[TestMethod]
public void TestMinMaxCloseButtons()
{
    using (var e = new MockA11yElement())
    {
        string[] automationIDs = { "Minimize", "Maximize", "Close" };

        e.ControlTypeId = Button;

        foreach (var automationID in automationIDs)
        {
            e.AutomationId = automationID;
            Assert.IsFalse(Misc.NameRequired.Matches(e));
        }
    } // using
}

[TestMethod]
public void TestCustomWithAllowedParents()
{
    using (var parent = new MockA11yElement())
    using (var e = new MockA11yElement())
    {
        e.ControlTypeId = Custom;
        e.Parent = parent;

        Assert.IsTrue(Misc.NameRequired.Matches(e));
    } // using
}

[TestMethod]
public void TestCustomWithDisallowedParents()
{
    using (var parent = new MockA11yElement())
    using (var e = new MockA11yElement())
    {
        e.ControlTypeId = Custom;
        e.Framework = Framework.WPF;
        e.Parent = parent;
        parent.ControlTypeId = DataItem;

        Assert.IsFalse(Misc.NameRequired.Matches(e));
    } // using
}

[TestMethod]
public void TestElementsWithSibblingsOfSameControlType()
{
    // this only applies to headers and status bars at the moment
    int[] controlTypes = { Header, StatusBar };

    using (var parent = new MockA11yElement())
    using (var sibbling = new MockA11yElement())
    using (var e = new MockA11yElement())
    {
        e.Parent = parent;
        parent.Children.Add(e);
        parent.Children.Add(sibbling);

        foreach (var controlType in controlTypes)
        {
            e.ControlTypeId = controlType;
            sibbling.ControlTypeId = controlType;
            Assert.IsTrue(Misc.NameRequired.Matches(e));
        }
    } // using
}

[TestMethod]
public void TestElementsWithNoSibblingsOfSameControlType()
{
    // this only applies to headers and status bars at the moment
    int[] controlTypes = { Header, StatusBar };

    using (var parent = new MockA11yElement())
    using (var e = new MockA11yElement())
    {
        e.Parent = parent;
        parent.Children.Add(e);

        foreach (var controlType in controlTypes)
        {
            e.ControlTypeId = controlType;
            Assert.IsFalse(Misc.NameRequired.Matches(e));
        }
    } // using
}

        [TestMethod]
        public void TestImageWithAllowedParents()
        {
            using (var parent = new MockA11yElement())
            using (var e = new MockA11yElement())
            {
                var parentTypes = ControlType.All.Difference(Button, ListItem, MenuItem, TreeItem);

                e.ControlTypeId = Image;
                e.Parent = parent;

                foreach (var controlType in parentTypes)
                {
                    parent.ControlTypeId = controlType;
                    Assert.IsTrue(Misc.NameRequired.Matches(e));
                }
            } // using
        }

        [TestMethod]
        public void TestImageWithDisallowedParents()
        {
            using (var parent = new MockA11yElement())
            using (var e = new MockA11yElement())
            {
                int[] parentTypes = { Button, ListItem, MenuItem, TreeItem };

                e.ControlTypeId = Image;
                e.Parent = parent;

                foreach (var controlType in parentTypes)
                {
                    parent.ControlTypeId = controlType;
                    Assert.IsFalse(Misc.NameRequired.Matches(e));
                }
            } // using
        }

        [TestMethod]
        public void TestTextWithAllowedPlatformProperties()
        {
            using (var e = new MockA11yElement())
            {
                e.ControlTypeId = Text;

                Assert.IsTrue(Misc.NameRequired.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestTextWithDisallowedPlatformProperties()
        {
            const uint WS_EX_STATICEDGE = 0x00020000;

            using (var e = new MockA11yElement())
            {
                e.ControlTypeId = Text;
                e.Framework = Framework.WinForm;
                Assert.IsTrue(Misc.NameRequired.Matches(e));

                var prop = new A11yProperty(PlatformPropertyType.Platform_WindowsExtendedStylePropertyId, WS_EX_STATICEDGE);
                e.PlatformProperties.Add(PlatformPropertyType.Platform_WindowsExtendedStylePropertyId, prop);
                Assert.IsFalse(Misc.NameRequired.Matches(e));

                e.Framework = null;
                Assert.IsTrue(Misc.NameRequired.Matches(e));
            } // using
        }
    } // class
} // namespace
