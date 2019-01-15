// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Bases.Fakes;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Fingerprint;
using AccessibilityInsights.Core.Fingerprint.Fakes;
using AccessibilityInsights.Core.Results;
using AccessibilityInsights.Core.Types;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AccessibilityInsights.CoreTests.Fingerprint
{
    [TestClass]
    public class ScanResultFingerprintUnitTests
    {
        private readonly string _specialNameValue1;
        private readonly string _specialNameValue2;

        private const RuleId DefaultRule = RuleId.ItemTypeCorrect;
        private const ScanStatus DefaultScanStatus = ScanStatus.Fail;
        private const string DefaultRuleIdValue = "ItemTypeCorrect";
        private const string DefaultLevelValue = "error";

        public ScanResultFingerprintUnitTests()
        {
#if FIND_SPECIAL_VALUES
            Dictionary<int, string> memory = new Dictionary<int, string>();

            while (true)
            {
                string value = Guid.NewGuid().ToString();

                FingerprintContribution fc = new FingerprintContribution("Name", value);
                int hash = fc.GetHashCode();

                if (memory.TryGetValue(hash, out string oldValue))
                {
                    if (oldValue == value)
                        continue;

                    _specialNameValue1 = oldValue;
                    _specialNameValue2 = value;
                    memory.Clear();
                    return;
                }
                memory.Add(hash, value);
            }
#else
            _specialNameValue1 = "96303c8f-5ff4-423b-a74d-bab12f37efb2";
            _specialNameValue2 = "f6c89020-5898-460d-974c-6227d48ed5e2";
#endif
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout(2000)]
        public void Ctor_ElementIsNull_ThrowsArgumentNullException()
        {
            try
            {
                new ScanResultFingerprint(null, DefaultRule, DefaultScanStatus);
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("element", e.ParamName);
                throw;
            }
        }

        static void ValidateAndRemoveContribution(Dictionary<string, FingerprintContribution> contributions,
            string keyName, string expectedValue)
        {
            if (string.IsNullOrEmpty(expectedValue))
                return;

            if (!contributions.TryGetValue(keyName, out FingerprintContribution contribution))
                Assert.Fail("Unable to find value for " + keyName);

            Assert.AreEqual(expectedValue, contribution.Value, "Value mismatch for key: " + keyName);

            contributions.Remove(keyName);
        }

        static void ValidateAndRemoveLevelContribution(Dictionary<string, FingerprintContribution> contributions,
            int level, string keyName, List<string> expectedValues)
        {
            if (level >= expectedValues.Count)
                return;

            string fullName = ((level > 0) ?
                string.Format(CultureInfo.InvariantCulture, "Ancestor{0}.", level) :
                string.Empty)
                + keyName;

            ValidateAndRemoveContribution(contributions, fullName, expectedValues[level]);
        }

        static List<string> GetAncestryTreeValues(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new List<string>();

            return input.Split(new char[] { '|' }).ToList();
        }

        static void ValidateFingerprint(IFingerprint fingerprint, string ruleIdValue = DefaultRuleIdValue,
            string nameValue = null, string classNameValue = null, string controlTypeValue = "Unknown(0)",
            string localizedControlTypeValue = null, string frameworkIdValue = null, string acceleratorKeyValue = null,
            string accessKeyValue = null, string cultureValue = null, string automationIdValue = null,
            string isControlElementValue = null, string isContentElementValue = null,
            string isKeyboardFocusableValue = null, string levelValue = DefaultLevelValue)
        {
            Dictionary<string, FingerprintContribution> contributions = new Dictionary<string, FingerprintContribution>();
            foreach (FingerprintContribution contribution in fingerprint.Contributions)
            {
                contributions.Add(contribution.Key, contribution);
            }

            ValidateAndRemoveContribution(contributions, "RuleId", ruleIdValue);
            ValidateAndRemoveContribution(contributions, "Level", levelValue);
            ValidateAndRemoveContribution(contributions, "IsControlElement", isControlElementValue);
            ValidateAndRemoveContribution(contributions, "IsContentElement", isContentElementValue);
            ValidateAndRemoveContribution(contributions, "IsKeyboardFocusable", isKeyboardFocusableValue);

            List<string> acceleratorKeyValues = GetAncestryTreeValues(acceleratorKeyValue);
            List<string> accessKeyValues = GetAncestryTreeValues(accessKeyValue);
            List<string> automationIdValues = GetAncestryTreeValues(automationIdValue);
            List<string> classNameValues = GetAncestryTreeValues(classNameValue);
            List<string> controlTypeValues = GetAncestryTreeValues(controlTypeValue);
            List<string> cultureValues = GetAncestryTreeValues(cultureValue);
            List<string> frameworkIdValues = GetAncestryTreeValues(frameworkIdValue);
            List<string> localizedControlTypeValues = GetAncestryTreeValues(localizedControlTypeValue);
            List<string> nameValues = GetAncestryTreeValues(nameValue);

            int expectedLevels = Math.Max(acceleratorKeyValues.Count,
                Math.Max(accessKeyValues.Count,
                Math.Max(automationIdValues.Count,
                Math.Max(classNameValues.Count,
                Math.Max(controlTypeValues.Count,
                Math.Max(frameworkIdValues.Count,
                Math.Max(localizedControlTypeValues.Count, nameValues.Count)))))));

            int oldCount = contributions.Count;
            int level;
            bool exitLoop = false;

            const int levelLimit = 10;

            Assert.IsTrue(expectedLevels < levelLimit, "Levels exceeds test expectations");
            for (level = 0; level < levelLimit; level++)
            {
                // exit at top of loop so that level is correctly set
                if (exitLoop)
                    break;

                ValidateAndRemoveLevelContribution(contributions, level, "AcceleratorKey", acceleratorKeyValues);
                ValidateAndRemoveLevelContribution(contributions, level, "AccessKey", accessKeyValues);
                ValidateAndRemoveLevelContribution(contributions, level, "AutomationId", automationIdValues);
                ValidateAndRemoveLevelContribution(contributions, level, "ClassName", classNameValues);
                ValidateAndRemoveLevelContribution(contributions, level, "ControlType", controlTypeValues);
                ValidateAndRemoveLevelContribution(contributions, level, "Culture", cultureValues);
                ValidateAndRemoveLevelContribution(contributions, level, "FrameworkId", frameworkIdValues);
                ValidateAndRemoveLevelContribution(contributions, level, "LocalizedControlType", localizedControlTypeValues);
                ValidateAndRemoveLevelContribution(contributions, level, "Name", nameValues);

                // Set exit flag if we have no more contributions or if we removed no properties this iteration
                if (contributions.Count == 0 || (oldCount == contributions.Count))
                {
                    exitLoop = true;
                    continue;
                }

                oldCount = contributions.Count;
            }
            Assert.AreEqual(0, contributions.Count, "Extra contibution keys: " + string.Join(",", contributions.Keys));
            Assert.AreEqual(level, expectedLevels, "Exited before reaching end of expected data");
        }

        [TestMethod]
        [Timeout (2000)]
        public void ValidateTestMethod_GetAncestryTreeValues()
        {
            List<string> values = GetAncestryTreeValues(null);
            Assert.AreEqual(0, values.Count);

            values = GetAncestryTreeValues("abc|defgh|||ijkl");
            Assert.AreEqual(5, values.Count);
            Assert.AreEqual("abc", values[0]);
            Assert.AreEqual("defgh", values[1]);
            Assert.AreEqual(string.Empty, values[2]);
            Assert.AreEqual(string.Empty, values[3]);
            Assert.AreEqual("ijkl", values[4]);
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ElementIsTrivial_FingerprintIncludesRuleId()
        {
            using (A11yElement element = new A11yElement())
            {
                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, DefaultScanStatus);
                ValidateFingerprint(fingerprint);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ScanStatusIsFail_FingerprintIncludesCorrectLevel()
        {
            using (A11yElement element = new A11yElement())
            {
                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, ScanStatus.Fail);
                ValidateFingerprint(fingerprint, levelValue: "error");
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ScanStatusIsNoResult_FingerprintIncludesCorrectLevel()
        {
            using (A11yElement element = new A11yElement())
            {
                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, ScanStatus.NoResult);
                ValidateFingerprint(fingerprint, levelValue: "open");
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ScanStatusIsPass_FingerprintIncludesCorrectLevel()
        {
            using (A11yElement element = new A11yElement())
            {
                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, ScanStatus.Pass);
                ValidateFingerprint(fingerprint, levelValue: "pass");
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ScanStatusIsScanNotSupported_FingerprintIncludesCorrectLevel()
        {
            using (A11yElement element = new A11yElement())
            {
                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, ScanStatus.ScanNotSupported);
                ValidateFingerprint(fingerprint, levelValue: "note");
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ScanStatusIsUncertain_FingerprintIncludesCorrectLevel()
        {
            using (A11yElement element = new A11yElement())
            {
                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, ScanStatus.Uncertain);
                ValidateFingerprint(fingerprint, levelValue: "open");
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ElementHasName_FingerprintIncludesName()
        {
            using (ShimsContext.Create())
            {
                const string elementName = "MyElement";

                ShimA11yElement element = new ShimA11yElement
                {
                    NameGet = () => elementName,
                };

                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, DefaultScanStatus);
                ValidateFingerprint(fingerprint, nameValue: elementName);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ElementHasClassName_FingerprintIncludesClassName()
        {
            using (ShimsContext.Create())
            {
                const string className = "MyClass";

                ShimA11yElement element = new ShimA11yElement
                {
                    PropertiesGet = () => new Dictionary<int, A11yProperty>
                    {
                        { PropertyType.UIA_ClassNamePropertyId,
                            new ShimA11yProperty
                            {
                                TextValueGet = () => className,
                            }
                        },
                    }
                };

                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, DefaultScanStatus);
                ValidateFingerprint(fingerprint, classNameValue: className);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ElementHasLocalizedControlType_FingerprintIncludesLocalizedControlType()
        {
            using (ShimsContext.Create())
            {
                const string localizedControlType = "MyLocalizedControlType";

                ShimA11yElement element = new ShimA11yElement
                {
                    LocalizedControlTypeGet = () => localizedControlType,
                };

                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, DefaultScanStatus);
                ValidateFingerprint(fingerprint, localizedControlTypeValue: localizedControlType);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ElementHasUIFramework_FingerprintIncludesFrameworkId()
        {
            using (ShimsContext.Create())
            {
                const string frameworkId = "MyFrameworkId";

                ShimA11yElement element = new ShimA11yElement
                {
                    FrameworkGet = () => frameworkId,
                };

                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, DefaultScanStatus);
                ValidateFingerprint(fingerprint, frameworkIdValue: frameworkId);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ElementHasAcceleratorKey_FingerprintIncludesAcceleratorKey()
        {
            using (ShimsContext.Create())
            {
                const string acceleratorKey = "MyAcceleratorKey";

                ShimA11yElement element = new ShimA11yElement
                {
                    AcceleratorKeyGet = () => acceleratorKey,
                };

                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, DefaultScanStatus);
                ValidateFingerprint(fingerprint, acceleratorKeyValue: acceleratorKey);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ElementHasAccessKey_FingerprintIncludesAccessKey()
        {
            using (ShimsContext.Create())
            {
                const string accessKey = "MyAccessKey";

                ShimA11yElement element = new ShimA11yElement
                {
                    AccessKeyGet = () => accessKey,
                };

                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, DefaultScanStatus);
                ValidateFingerprint(fingerprint, accessKeyValue: accessKey);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ElementHasAutomationId_FingerprintIncludesAutomationId()
        {
            using (ShimsContext.Create())
            {
                const string automationId = "MyAutomationId";

                ShimA11yElement element = new ShimA11yElement
                {
                    AutomationIdGet = () => automationId,
                };

                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, DefaultScanStatus);
                ValidateFingerprint(fingerprint, automationIdValue: automationId);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ElementHasOneLevelOfAncestry_FingerprintRemovesTopParentName()
        {
            using (ShimsContext.Create())
            {
                const string elementName = "NameOfElement";
                const string parentName = "NameOfParent";
                const string desktopName = "NameOfDesktop";

                ShimA11yElement element = new ShimA11yElement
                {
                    ControlTypeIdGet = () => 50033,
                    NameGet = () => elementName,
                    ParentGet = () => new ShimA11yElement
                    {
                        ControlTypeIdGet = () => 50031,
                        NameGet = () => parentName,
                        ParentGet = () => new ShimA11yElement
                        {
                            NameGet = () => desktopName,
                        }
                    }
                };

                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, DefaultScanStatus);
                ValidateFingerprint(fingerprint, nameValue: elementName, controlTypeValue: "Pane(50033)|SplitButton(50031)");
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_ElementBasedOnWordNavPane_FingerprintIncludesExpectedFields()
        {
            using (ShimsContext.Create())
            {
                ShimA11yElement element = new ShimA11yElement
                {
                    BoundingRectangleGet = () => new Rectangle(132, 406, 402 - 132, 1928 - 406),
                    ControlTypeIdGet = () => 50033,
                    IsKeyboardFocusableGet = () => true,
                    LocalizedControlTypeGet = () => "pane",
                    NameGet = () => "Navigation",
                    ParentGet = () => new ShimA11yElement
                    {
                        BoundingRectangleGet = () => new Rectangle(124, 335, 410 - 124, 1935 - 335),
                        ControlTypeIdGet = () => 50021,
                        IsKeyboardFocusableGet = () => false,
                        LocalizedControlTypeGet = () => "tool bar",
                        ParentGet = () => new ShimA11yElement
                        {
                            BoundingRectangleGet = () => new Rectangle(124, 335, 410 - 124, 1935 - 335),
                            ControlTypeIdGet = () => 50033,
                            IsKeyboardFocusableGet = () => true,
                            LocalizedControlTypeGet = () => "pane",
                            NameGet = () => "MsoDockLeft",
                            ParentGet = () => new ShimA11yElement
                            {
                                BoundingRectangleGet = () => new Rectangle(111, 13, 3010 - 111, 2013 - 13),
                                ControlTypeIdGet = () => 50032,
                                IsKeyboardFocusableGet = () => true,
                                LocalizedControlTypeGet = () => "window",
                                NameGet = () => "Document 1 - Word",
                                ParentGet = () => new ShimA11yElement
                                {
                                    BoundingRectangleGet = () => new Rectangle(0, 0, 3000, 2000),
                                    ControlTypeIdGet = () => 50033,
                                    IsKeyboardFocusableGet = () => true,
                                    LocalizedControlTypeGet = () => "pane",
                                    NameGet = () => "Desktop 1",
                                }
                            }
                        }
                    }
                };

                IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, DefaultScanStatus);
                ValidateFingerprint(fingerprint,
                    nameValue: "Navigation||MsoDockLeft",
                    controlTypeValue: "Pane(50033)|ToolBar(50021)|Pane(50033)|Window(50032)",
                    localizedControlTypeValue: "pane|tool bar|pane|window"
                    );
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_RuleIsControlElementPropertyCorrect_FingerprintIncludesIsControlElement()
        {
            using (ShimsContext.Create())
            {
                ShimA11yElement element = new ShimA11yElement
                {
                    IsControlElementGet = () => true,
                };

                IFingerprint fingerprint = new ScanResultFingerprint(element, RuleId.IsControlElementPropertyCorrect, DefaultScanStatus);
                ValidateFingerprint(fingerprint, ruleIdValue: "IsControlElementPropertyCorrect", isControlElementValue: "True");
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_RuleIsContentElementPropertyCorrect_FingerprintIncludesIsContentElement()
        {
            using (ShimsContext.Create())
            {
                ShimA11yElement element = new ShimA11yElement
                {
                    IsContentElementGet = () => false,
                };

                IFingerprint fingerprint = new ScanResultFingerprint(element, RuleId.IsContentElementPropertyCorrect, DefaultScanStatus);
                ValidateFingerprint(fingerprint, ruleIdValue: "IsContentElementPropertyCorrect", isContentElementValue: "False");
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_RuleIsKeyboardFocusable_FingerprintIncludesIsKeyboardFocusable()
        {
            using (ShimsContext.Create())
            {
                ShimA11yElement element = new ShimA11yElement
                {
                    IsKeyboardFocusableGet = () => true,
                };

                IFingerprint fingerprint = new ScanResultFingerprint(element, RuleId.IsKeyboardFocusable, DefaultScanStatus);
                ValidateFingerprint(fingerprint, ruleIdValue: "IsKeyboardFocusable", isKeyboardFocusableValue: "True");
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_RuleIsKeyboardFocusableBasedOnPatterns_FingerprintIncludesIsKeyboardFocusable()
        {
            using (ShimsContext.Create())
            {
                ShimA11yElement element = new ShimA11yElement
                {
                    IsKeyboardFocusableGet = () => false,
                };

                IFingerprint fingerprint = new ScanResultFingerprint(element, RuleId.IsKeyboardFocusableBasedOnPatterns, DefaultScanStatus);
                ValidateFingerprint(fingerprint, ruleIdValue: "IsKeyboardFocusableBasedOnPatterns", isKeyboardFocusableValue: "False");
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void GetHashCode_MinimalContributions_ReturnsSameHashCodeMultipleTimes()
        {
            IFingerprint fingerprint = new ScanResultFingerprint(new A11yElement(), DefaultRule, DefaultScanStatus);

            int hash1 = fingerprint.GetHashCode();
            Assert.AreEqual(hash1, fingerprint.GetHashCode());
            Assert.AreEqual(hash1, fingerprint.GetHashCode());
            Assert.AreEqual(hash1, fingerprint.GetHashCode());
        }

        [TestMethod]
        [Timeout (2000)]
        public void GetHashCode_EquivalentMinimalContributions_ReturnsSameHashCode()
        {
            IFingerprint fingerprint1 = new ScanResultFingerprint(new A11yElement(), DefaultRule, DefaultScanStatus);
            IFingerprint fingerprint2 = new ScanResultFingerprint(new A11yElement(), DefaultRule, DefaultScanStatus);

            Assert.AreEqual(fingerprint1.GetHashCode(), fingerprint2.GetHashCode());
        }

        [TestMethod]
        [Timeout(2000)]
        public void GetHashCode_EquivalentComplexContributions_ReturnsSameHashCode()
        {
            using (ShimsContext.Create())
            {
                A11yElement element1 = new ShimA11yElement
                {
                    NameGet = () => "ElementName",
                    CultureGet = () => "ElementCulture",
                    AutomationIdGet = () => "ElementAutomationId",
                };
                A11yElement element2 = new ShimA11yElement
                {
                    NameGet = () => "ElementName",
                    CultureGet = () => "ElementCulture",
                    AutomationIdGet = () => "ElementAutomationId",
                };

                IFingerprint fingerprint1 = new ScanResultFingerprint(element1, DefaultRule, DefaultScanStatus);
                IFingerprint fingerprint2 = new ScanResultFingerprint(element2, DefaultRule, DefaultScanStatus);

                Assert.AreEqual(fingerprint1.GetHashCode(), fingerprint2.GetHashCode());
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void GetHashCode_DifferentInRuleOnly_ReturnsDifferentHashCode()
        {
            IFingerprint fingerprint1 = new ScanResultFingerprint(new A11yElement(), RuleId.BoundingRectangleExists, DefaultScanStatus);
            IFingerprint fingerprint2 = new ScanResultFingerprint(new A11yElement(), RuleId.BoundingRectangleNotNull, DefaultScanStatus);
            Assert.AreNotEqual(fingerprint1.GetHashCode(), fingerprint2.GetHashCode());
        }

        [TestMethod]
        [Timeout(2000)]
        public void GetHashCode_DifferentInLevelOnly_ReturnsDifferentHashCode()
        {
            IFingerprint fingerprint1 = new ScanResultFingerprint(new A11yElement(), DefaultRule, ScanStatus.Fail);
            IFingerprint fingerprint2 = new ScanResultFingerprint(new A11yElement(), DefaultRule, ScanStatus.Uncertain);
            Assert.AreNotEqual(fingerprint1.GetHashCode(), fingerprint2.GetHashCode());
        }

        [TestMethod]
        [Timeout(2000)]
        public void GetHashCode_DifferentComplexContributions_ReturnsSameHashCode()
        {
            using (ShimsContext.Create())
            {
                A11yElement element1 = new ShimA11yElement
                {
                    NameGet = () => "ElementName",
                    CultureGet = () => "ElementCulture",
                    AutomationIdGet = () => "ElementAutomationId",
                };
                A11yElement element2 = new ShimA11yElement
                {
                    NameGet = () => " ElementName",
                    CultureGet = () => "ElementCulture",
                    AutomationIdGet = () => "ElementAutomationId",
                };

                IFingerprint fingerprint1 = new ScanResultFingerprint(element1, DefaultRule, DefaultScanStatus);
                IFingerprint fingerprint2 = new ScanResultFingerprint(element2, DefaultRule, DefaultScanStatus);

                Assert.AreNotEqual(fingerprint1.GetHashCode(), fingerprint2.GetHashCode());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout (2000)]
        public void CompareTo_OtherIsNull_ThrowsArgumentNullException()
        {
            IFingerprint fingerprint = new ScanResultFingerprint(new A11yElement(), DefaultRule, DefaultScanStatus);

            try
            {
                fingerprint.CompareTo(null);
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("other", e.ParamName);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Timeout(2000)]
        public void CompareTo_OtherIsDifferentImplemention_ThrowsInvalidOperationException()
        {
            using (ShimsContext.Create())
            {
                IFingerprint fingerprint = new ScanResultFingerprint(new A11yElement(), DefaultRule, DefaultScanStatus);

                StubIFingerprint other = new StubIFingerprint();

                fingerprint.CompareTo(other);
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void CompareTo_OneElementHasMoreContributions_SortByContributionCount()
        {
            using (ShimsContext.Create())
            {
                ShimA11yElement moreContributionsElement = new ShimA11yElement
                {
                    NameGet = () => "ElementWithTwoContributions",
                    AutomationIdGet = () => "AutomationId",
                };
                ShimA11yElement fewerContributionsElement = new ShimA11yElement
                {
                    NameGet = () => "ElemetWithOneContribution",
                };

                IFingerprint higherFingerprint = new ScanResultFingerprint(moreContributionsElement, DefaultRule, DefaultScanStatus);
                IFingerprint lowerFingerprint = new ScanResultFingerprint(fewerContributionsElement, DefaultRule, DefaultScanStatus);

                Assert.AreEqual(-1, lowerFingerprint.CompareTo(higherFingerprint));
                Assert.AreEqual(1, higherFingerprint.CompareTo(lowerFingerprint));
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void CompareTo_ElementsWithSameNumberOfContributions_SortByHashValue()
        {
            using (ShimsContext.Create())
            {
                var list = new SortedList<IFingerprint, string>();
                const int max = 10;

                string elementName = string.Empty;
                for (int loop = 0; loop < max; loop++)
                {
                    elementName += "a";
                    A11yElement element = new ShimA11yElement { NameGet = () => elementName };
                    IFingerprint fingerprint = new ScanResultFingerprint(element, DefaultRule, DefaultScanStatus);
                    list.Add(fingerprint, elementName);
                }

                // If the comparison worked correctly, then the hash codes of the list will be 
                // sorted in ascending order
                int oldHashCode = int.MinValue;

                for (int loop = 0; loop < max; loop++)
                {
                    int hashCode = list.Keys[loop].GetHashCode();
                    string description = string.Format("Index {0}: Hash {1} should be less than Hash {2}",
                        loop, oldHashCode, hashCode);
                    Assert.IsTrue(oldHashCode < hashCode);
                    oldHashCode = hashCode;
                }
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void Equals_OtherIsNull_ReturnsFalse()
        {
            IFingerprint fingerprint = new ScanResultFingerprint(new A11yElement(), DefaultRule, DefaultScanStatus);
            Assert.IsFalse(fingerprint.Equals(null));
        }

        [TestMethod]
        [Timeout (2000)]
        public void Equals_OtherIsDifferentImplementation_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                IFingerprint fingerprint = new ScanResultFingerprint(new A11yElement(), DefaultRule, DefaultScanStatus);
                StubIFingerprint other = new StubIFingerprint();
                Assert.IsFalse(fingerprint.Equals(other));
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void Equals_OtherHasDifferentNumberOfContributions_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                ShimA11yElement moreContributionsElement = new ShimA11yElement
                {
                    NameGet = () => "MyTest",
                    AutomationIdGet = () => "AutomationId",
                };
                ShimA11yElement fewerContributionsElement = new ShimA11yElement
                {
                    NameGet = () => "MyTest",
                };

                IFingerprint fingerprint1 = new ScanResultFingerprint(moreContributionsElement, DefaultRule, DefaultScanStatus);
                IFingerprint fingerprint2 = new ScanResultFingerprint(fewerContributionsElement, DefaultRule, DefaultScanStatus);

                Assert.AreNotEqual(fingerprint1.Contributions.Count(), fingerprint2.Contributions.Count());
                Assert.IsFalse(fingerprint1.Equals(fingerprint2));
                Assert.IsFalse(fingerprint2.Equals(fingerprint1));
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Equals_OtherHasSameNumberOfContributionsButDifferentHash_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                A11yElement element1 = new ShimA11yElement { NameGet = () => "a" };
                A11yElement element2 = new ShimA11yElement { NameGet = () => "A" };

                IFingerprint fingerprint1 = new ScanResultFingerprint(element1, DefaultRule, DefaultScanStatus);
                IFingerprint fingerprint2 = new ScanResultFingerprint(element2, DefaultRule, DefaultScanStatus);

                Assert.AreEqual(fingerprint1.Contributions.Count(), fingerprint2.Contributions.Count());
                Assert.IsFalse(fingerprint1.Equals(fingerprint2));
                Assert.IsFalse(fingerprint2.Equals(fingerprint1));
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void Equals_OtherIsEquivalent_ReturnsTrue()
        {
            using (ShimsContext.Create())
            {
                ShimA11yElement element1 = new ShimA11yElement
                {
                    AutomationIdGet = () => "AutomationId",
                    ControlTypeIdGet = () => 50021,
                };
                ShimA11yElement element2 = new ShimA11yElement
                {
                    AutomationIdGet = () => "AutomationId",
                    ControlTypeIdGet = () => 50021,
                };

                IFingerprint fingerprint1 = new ScanResultFingerprint(element1, DefaultRule, DefaultScanStatus);
                IFingerprint fingerprint2 = new ScanResultFingerprint(element2, DefaultRule, DefaultScanStatus);

                Assert.IsTrue(fingerprint1.Equals(fingerprint2));
                Assert.IsTrue(fingerprint2.Equals(fingerprint1));
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void Equals_OtherHasSameContributionCountAndHashButDifferentContent_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                FingerprintContribution fc1 = new FingerprintContribution("Name", _specialNameValue1);
                FingerprintContribution fc2 = new FingerprintContribution("Name", _specialNameValue2);

                Assert.AreEqual(fc1.GetHashCode(), fc2.GetHashCode());

                A11yElement element1 = new ShimA11yElement { NameGet = () => _specialNameValue1 };
                A11yElement element2 = new ShimA11yElement { NameGet = () => _specialNameValue2 };
                IFingerprint fingerprint1 = new ScanResultFingerprint(element1, DefaultRule, DefaultScanStatus);
                IFingerprint fingerprint2 = new ScanResultFingerprint(element2, DefaultRule, DefaultScanStatus);

                Assert.AreEqual(fingerprint1.GetHashCode(), fingerprint2.GetHashCode());

                // Ensure that these differ only in specific content
                Assert.AreEqual(0, fingerprint1.CompareTo(fingerprint2));
                Assert.AreEqual(0, fingerprint2.CompareTo(fingerprint1));

                Assert.IsFalse(fingerprint1.Equals(fingerprint2));
                Assert.IsFalse(fingerprint2.Equals(fingerprint1));
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void DictionaryTest_FingerprintsAreDifferent_TreatsItemsCorrectly()
        {
            using (ShimsContext.Create())
            {
                Dictionary<IFingerprint, int> store = new Dictionary<IFingerprint, int>();

                A11yElement element1 = new ShimA11yElement { NameGet = () => _specialNameValue1 };
                A11yElement element2 = new ShimA11yElement { NameGet = () => _specialNameValue2 };
                IFingerprint fingerprint1 = new ScanResultFingerprint(element1, DefaultRule, DefaultScanStatus);
                IFingerprint fingerprint2 = new ScanResultFingerprint(element2, DefaultRule, DefaultScanStatus);

                store.Add(fingerprint1, 1);
                Assert.AreEqual(1, store.Count);
                Assert.IsTrue(store.TryGetValue(fingerprint1, out int value));
                Assert.AreEqual(1, value);
                Assert.IsFalse(store.TryGetValue(fingerprint2, out value));
                store.Add(fingerprint2, 2);
                Assert.AreEqual(2, store.Count);
                Assert.IsTrue(store.TryGetValue(fingerprint2, out value));
                Assert.AreEqual(2, value);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void DictionaryTest_FingerprintsAreSame_TreatsItemsCorrectly()
        {
            Dictionary<IFingerprint, int> store = new Dictionary<IFingerprint, int>();

            IFingerprint fingerprint1 = new ScanResultFingerprint(new A11yElement(), DefaultRule, DefaultScanStatus);
            IFingerprint fingerprint2 = new ScanResultFingerprint(new A11yElement(), DefaultRule, DefaultScanStatus);

            store.Add(fingerprint1, 1);
            Assert.AreEqual(1, store.Count);
            Assert.IsTrue(store.TryGetValue(fingerprint1, out int value));
            Assert.AreEqual(1, value);
            value = 0;
            Assert.IsTrue(store.TryGetValue(fingerprint2, out value));
            Assert.AreEqual(1, value);
            store[fingerprint2] = 3;
            Assert.IsTrue(store.TryGetValue(fingerprint1, out value));
            Assert.AreEqual(3, value);
        }
    }
}
