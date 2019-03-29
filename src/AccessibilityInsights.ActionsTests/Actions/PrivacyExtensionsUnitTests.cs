// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Results;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Desktop.UIAutomation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
#if FAKES_SUPPORTED
using AccessibilityInsights.Core.Results.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
#endif

namespace AccessibilityInsights.ActionsTests.Actions
{
    [TestClass]
    public class PrivacyExtensionsUnitTests
    {
        [TestMethod]
        public void CreateScrubbedCopyWithoutRelationships_InputIsGeneric_ResultIsGenericCopy()
        {
            A11yElement input = new A11yElement();
            A11yElement copy = input.CreateScrubbedCopyWithoutRelationships();
            Assert.AreNotSame(input, copy);
            Assert.IsNull(copy.IssueDisplayText);
            Assert.IsNull(copy.ScanResults);
            Assert.AreEqual(input.TreeWalkerMode, copy.TreeWalkerMode);
            Assert.AreEqual(input.UniqueId, copy.UniqueId);
            Assert.IsNull(copy.Glimpse);
            Assert.IsNull(copy.Children);
            Assert.IsNull(copy.Parent);
        }

        [TestMethod]
        public void CreateScrubbedCopyWithoutRelationships_InputHasBugId_ResultHasSameBugId()
        {
            const string issueDisplayText = "12345";
            A11yElement input = new A11yElement
            {
                IssueDisplayText = issueDisplayText,
            };
            A11yElement copy = input.CreateScrubbedCopyWithoutRelationships();
            Assert.AreEqual(issueDisplayText, copy.IssueDisplayText);
        }

#if FAKES_SUPPORTED
        [TestMethod]
        public void CreateScrubbedCopyWithoutRelationships_InputHasScanResults_ResultHasSameScanResults()
        {
            using (ShimsContext.Create())
            {
                ScanResults scanResults = new ShimScanResults();
                A11yElement input = new A11yElement
                {
                    ScanResults = scanResults,
                };
                A11yElement copy = input.CreateScrubbedCopyWithoutRelationships();
                Assert.AreSame(input.ScanResults, copy.ScanResults);
            }
        }
#endif

        [TestMethod]
        public void CreateScrubbedCopyWithoutRelationships_InputSetsTreeWalkerMode_ResultHasSameTreeWalkerMode()
        {
            A11yElement input = new A11yElement
            {
                TreeWalkerMode = TreeViewMode.Control,
            };
            A11yElement copy = input.CreateScrubbedCopyWithoutRelationships();
            Assert.AreEqual(TreeViewMode.Control, copy.TreeWalkerMode);
        }

        [TestMethod]
        public void CreateScrubbedCopyWithoutRelationships_InputSetsUniqueId_ResultHasSameUniqueId()
        {
            const int uniqueId = 98765;

            A11yElement input = new A11yElement
            {
                UniqueId = uniqueId
            };
            A11yElement copy = input.CreateScrubbedCopyWithoutRelationships();
            Assert.AreEqual(uniqueId, copy.UniqueId);
        }

        [TestMethod]
        public void CreateScrubbedCopyWithoutRelationships_InputHasChildren_ResultHasEmptyListOfChildren()
        {
            A11yElement child = new A11yElement();
            A11yElement input = new A11yElement
            {
                Children = new List<A11yElement> { child },
            };
            A11yElement copy = input.CreateScrubbedCopyWithoutRelationships();
            Assert.IsNotNull(copy.Children);
            Assert.IsFalse(copy.Children.Any());
        }

        [TestMethod]
        public void CreateScrubbedCopyWithoutRelationships_InputHasNameProperty_ResultHasShortenedNameProperty()
        {
            A11yElement input = new A11yElement
            {
                Properties = new Dictionary<int, A11yProperty>
                {
                    { PropertyType.UIA_NamePropertyId, new A11yProperty(PropertyType.UIA_NamePropertyId, "abcde") },
                },
            };
            Assert.AreEqual("abcde", input.Name);
            A11yElement copy = input.CreateScrubbedCopyWithoutRelationships();
            Assert.AreEqual("abc", copy.Name);
        }

        [TestMethod]
        public void CreateScrubbedCopyWithoutRelationships_InputHasGlimpse_ResultHasTruncatedGlimpse()
        {
            A11yElement input = new A11yElement
            {
                Properties = new Dictionary<int, A11yProperty>
                {
                    { PropertyType.UIA_NamePropertyId, new A11yProperty(PropertyType.UIA_NamePropertyId, "abcde") },
                    { PropertyType.UIA_LocalizedControlTypePropertyId, new A11yProperty(PropertyType.UIA_LocalizedControlTypePropertyId, "MyLocalizedControl") },
                },
            };
            input.UpdateGlimpse();

            Assert.AreEqual(@"MyLocalizedControl 'abcde'", input.Glimpse);
            A11yElement copy = input.CreateScrubbedCopyWithoutRelationships();
            Assert.AreEqual(@"MyLocalizedControl 'abc'", copy.Glimpse);
        }

        [TestMethod]
        public void CreateScrubbedCopyWithoutRelationships_InputHasChildren_ResultHasEmptyChildrenList()
        {
            A11yElement input = new A11yElement
            {
                Children = new List<A11yElement>
                {
                    new A11yElement(),
                    new A11yElement(),
                },
            };

            A11yElement copy = input.CreateScrubbedCopyWithoutRelationships();
            Assert.IsNotNull(copy.Children);
            Assert.IsFalse(copy.Children.Any());
        }

        [TestMethod]
        public void CreateScrubbedCopyWithoutRelationships_InputHasParent_ResultHasNoParent()
        {
            A11yElement input = new A11yElement
            {
                Parent = new A11yElement(),
            };

            A11yElement copy = input.CreateScrubbedCopyWithoutRelationships();
            Assert.IsNull(copy.Parent);
        }

        [TestMethod]
        public void CreateScrubbedCopyWithoutRelationships_InputHasAllProperties_ResultHasFilteredProperties()
        {
            const int propertiesTypesBase = 30000;
            const int inputProperties = 200;

            HashSet<int> allowedProperties = new HashSet<int>
            {
                PropertyType.UIA_AcceleratorKeyPropertyId,
                PropertyType.UIA_AccessKeyPropertyId,
                PropertyType.UIA_AutomationIdPropertyId,
                PropertyType.UIA_BoundingRectanglePropertyId,
                PropertyType.UIA_ClassNamePropertyId,
                PropertyType.UIA_ControlTypePropertyId,
                PropertyType.UIA_FrameworkIdPropertyId,
                PropertyType.UIA_IsContentElementPropertyId,
                PropertyType.UIA_IsControlElementPropertyId,
                PropertyType.UIA_IsKeyboardFocusablePropertyId,
                PropertyType.UIA_LocalizedControlTypePropertyId,
                PropertyType.UIA_NamePropertyId,
            };

            Dictionary<int, A11yProperty> properties = new Dictionary<int, A11yProperty>();

            // This gives us a superset of all const values in the PropertyTypes class
            for (int loop = 0; loop < inputProperties; loop++)
            {
                int type = propertiesTypesBase + loop;
                properties.Add(type, new A11yProperty(type, type.ToString()));
            }
            A11yElement input = new A11yElement
            {
                Properties = properties,
            };
            A11yElement copy = input.CreateScrubbedCopyWithoutRelationships();
            Assert.AreEqual(inputProperties, input.Properties.Count);

            // Confirm the allowed properties
            foreach (KeyValuePair<int, A11yProperty> pair in copy.Properties)
            {
                A11yProperty inputProperty = input.Properties[pair.Key];
                A11yProperty copyProperty = pair.Value;
                if (pair.Key == PropertyType.UIA_NamePropertyId)  // Name is special cased and makes a copy
                {
                    Assert.AreNotSame(inputProperty, copyProperty);
                }
                else
                {
                    Assert.AreSame(inputProperty, copyProperty);
                }
                Assert.AreEqual(inputProperty.Id, copyProperty.Id);
                Assert.IsTrue(allowedProperties.Contains(pair.Key), pair.Key.ToString() + " is not allowed");
                allowedProperties.Remove(pair.Key);
            }

            Assert.IsFalse(allowedProperties.Any(), allowedProperties.Count + " unexpected elements");
        }

        [TestMethod]
        public void GetScrubbedElementTree_InputIsNull_ReturnsNull()
        {
            A11yElement input = null;
            Assert.IsNull(input.GetScrubbedElementTree());
        }

        [TestMethod]
        public void GetScrubbedElementTree_InputHasNoParent_ReturnsCloneWithNoParentOrChild()
        {
            A11yElement input = new A11yElement();
            A11yElement copy = input.GetScrubbedElementTree();
            Assert.IsNotNull(copy);
            Assert.IsNull(copy.Children);
            Assert.IsNull(copy.Parent);
        }

        [TestMethod]
        public void GetScrubbedElementTree_InputHasParent_ReturnsRootOfTree_TreeHasTwoLevels()
        {
            A11yElement input = new A11yElement();
            A11yElement parent = new A11yElement
            {
                Children = new List<A11yElement> { input },
            };
            input.Parent = parent;

            A11yElement copy = input.GetScrubbedElementTree();
            Assert.IsNotNull(copy);
            Assert.IsNull(copy.Parent);
            Assert.IsNotNull(copy.Children);
            Assert.AreEqual(1, copy.Children.Count);
            Assert.IsNull(copy.Children[0].Children);
            Assert.AreSame(copy, copy.Children[0].Parent);
        }
    }
}
