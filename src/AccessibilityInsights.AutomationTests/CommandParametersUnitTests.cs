// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.IO;
using System.IO.Fakes;
using System.Linq;
using AccessibilityInsights.Automation;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AccessibilityInsights.AutomationTests
{
    [TestClass]
    public class CommandParametersUnitTests
    {
        private static Dictionary<string, string> EmptyInput = new Dictionary<string, string>();
        private const string Key1 = "aBc";
        private const string Key2 = "pDq";
        private const string Key3 = "xYz";
        private const string StringValue = "I'm not a number";
        private const string LongAsString = "-98765432109876";
        private const long LongAsNumber = -98765432109876;
        private const string BoolAsString = "True";
        private const bool BoolAsBool = true;
        private const string FakeConfigFile = "FakeConfigFile.json";
        private const string JsonSettingKey1ToStringValue = "{\"abc\":\"I'm not a number\"}";

        [TestMethod]
        [Timeout (1000)]
        public void Ctor_TrivialInputs_HasEmptyProperties_UsedConfigFileIsFalse()
        {
            CommandParameters parameters = new CommandParameters(EmptyInput, string.Empty);
            Assert.IsFalse(parameters.ConfigCopy.Any());
            Assert.IsFalse(parameters.UsedConfigFile);
        }

        [TestMethod]
        [Timeout (1000)]
        public void Ctor_ValuesInDictionary_HasCaseInsensitiveMatchingProperties_UsedConfigFileIsFalse()
        {
            Dictionary<string, string> input = new Dictionary<string, string>
            {
                {Key1, StringValue},
                {Key2, LongAsString},
                {Key3, BoolAsString}
            };

            CommandParameters parameters = new CommandParameters(input, string.Empty);
            Assert.IsFalse(parameters.UsedConfigFile);
            Assert.AreEqual(input.Count, parameters.ConfigCopy.Count);
            Assert.IsTrue(parameters.TryGetString(Key1.ToUpper(), out string value));
            Assert.AreEqual(StringValue, value);
            Assert.IsTrue(parameters.TryGetString(Key2.ToLower(), out value));
            Assert.AreEqual(LongAsString, value);
            Assert.IsTrue(parameters.TryGetString(Key3, out value));
            Assert.AreEqual(BoolAsString, value);
        }

        [TestMethod]
        [Timeout (1000)]
        [ExpectedException(typeof(A11yAutomationException))]
        public void Ctor_ConfigFileDoesNotExist_ThrowsAutomationException_ErrorAutomation001()
        {
            try
            {
                string configFile = Path.Combine("Junk12345667890", "0987654321knuJ",
                    "!@#$%^&()_+", FakeConfigFile);
                CommandParameters parameters = new CommandParameters(EmptyInput, configFile);
            }
            catch (A11yAutomationException e)
            {
                Assert.IsTrue(e.Message.Contains(" Automation001:"));
                throw;
            }
        }

        [TestMethod]
        [Timeout (1000)]
        [ExpectedException(typeof(A11yAutomationException))]
        public void Ctor_ConfigFileIsEmpty_ThrowsAutomationException_ErrorAutomation013()
        {
            using (ShimsContext.Create())
            {
                try
                {
                    ShimFile.ExistsString = (_) => true;
                    ShimFile.ReadAllTextString = (_) => string.Empty;

                    CommandParameters parameters = new CommandParameters(EmptyInput, FakeConfigFile);
                }
                catch (A11yAutomationException e)
                {
                    Assert.IsTrue(e.Message.Contains(" Automation013:"));
                    throw;
                }
            }
        }

        [TestMethod]
        [Timeout (1000)]
        [ExpectedException(typeof(A11yAutomationException))]
        public void Ctor_ConfigFileIsInvalidJson_ThrowsAutomationException_ErrorAutomation014()
        {
            using (ShimsContext.Create())
            {
                try
                {
                    ShimFile.ExistsString = (_) => true;
                    ShimFile.ReadAllTextString = (_) => "This isn't valid JSON!";

                    CommandParameters parameters = new CommandParameters(EmptyInput, FakeConfigFile);
                }
                catch (A11yAutomationException e)
                {
                    Assert.IsTrue(e.Message.Contains(" Automation014:"));
                    throw;
                }
            }
        }

        [TestMethod]
        [Timeout (1000)]
        public void Ctor_ValuesInValidConfigFile_HasMatchingProperties_UsedConfigFileIsTrue()
        {
            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (_) => true;
                ShimFile.ReadAllTextString = (_) => JsonSettingKey1ToStringValue;

                CommandParameters parameters = new CommandParameters(EmptyInput, FakeConfigFile);
                Assert.IsTrue(parameters.UsedConfigFile);
                Assert.AreEqual(1, parameters.ConfigCopy.Count);
                Assert.IsTrue(parameters.TryGetString(Key1, out string value));
                Assert.AreEqual(StringValue, value);
            }
        }

        [TestMethod]
        [Timeout (1000)]
        public void Ctor_ValuesInValidConfigFile_OverrideInDictionary_PropertyMatchesDictionary()
        {
            using (ShimsContext.Create())
            {
                ShimFile.ExistsString = (_) => true;
                ShimFile.ReadAllTextString = (_) => JsonSettingKey1ToStringValue;

                Dictionary<string, string> input = new Dictionary<string, string>
                {
                    {Key1, BoolAsString}
                };
                CommandParameters parameters = new CommandParameters(input, FakeConfigFile);
                Assert.AreEqual(1, parameters.ConfigCopy.Count);
                Assert.IsTrue(parameters.TryGetString(Key1, out string value));
                Assert.AreEqual(BoolAsString, value);
            }
        }

        [TestMethod]
        [Timeout (1000)]
        public void Ctor_UseCommandParameter_PrimaryConfigTakesPriorityCaseInsensitive_UsedConfigFileIsFalse()
        {
            Dictionary<string, string> input = new Dictionary<string, string>
            {
                {Key1.ToUpper(), StringValue},
                {Key2, LongAsString},
            };

            CommandParameters sessionParameters = new CommandParameters(input, string.Empty);

            Dictionary<string, string> primaryConfig = new Dictionary<string, string>
            {
                {Key1.ToLower(), BoolAsString},   // This should take priority
                {Key3, StringValue}
            };

            CommandParameters parameters = new CommandParameters(primaryConfig, sessionParameters);
            Assert.IsFalse(parameters.UsedConfigFile);
            Assert.AreEqual(3, parameters.ConfigCopy.Count);
            Assert.IsTrue(parameters.TryGetString(Key1, out string value));
            Assert.AreEqual(BoolAsString, value);
            Assert.IsTrue(parameters.TryGetString(Key2, out value));
            Assert.AreEqual(LongAsString, value);
            Assert.IsTrue(parameters.TryGetString(Key3, out value));
            Assert.AreEqual(StringValue, value);
        }

        [TestMethod]
        [Timeout (1000)]
        public void TryGetString_NoMatchingKey_ReturnsFalse()
        {
            CommandParameters parameters = new CommandParameters(EmptyInput, string.Empty);
            Assert.IsFalse(parameters.TryGetString(Key1, out string value));
        }

        [TestMethod]
        [Timeout (1000)]
        public void TryGetLong_NoMatchingKey_ReturnsFalse()
        {
            CommandParameters parameters = new CommandParameters(EmptyInput, string.Empty);
            Assert.IsFalse(parameters.TryGetLong(Key1, out long value));
        }

        [TestMethod]
        [Timeout (1000)]
        public void TryGetLong_StringValue_ReturnsFalse()
        {
            Dictionary<string, string> input = new Dictionary<string, string>
            {
                {Key1, StringValue}
            };

            CommandParameters parameters = new CommandParameters(input, string.Empty);
            Assert.IsFalse(parameters.TryGetLong(Key1, out long longValue));
        }

        [TestMethod]
        [Timeout (1000)]
        public void TryGetLong_NumericValue_ReturnsTrue_SetsCorrectValue()
        {
            Dictionary<string, string> input = new Dictionary<string, string>
            {
                {Key1, LongAsString}
            };

            CommandParameters parameters = new CommandParameters(input, string.Empty);
            Assert.IsTrue(parameters.TryGetLong(Key1, out long longValue));
            Assert.AreEqual(LongAsNumber, longValue);
        }

        [TestMethod]
        [Timeout (1000)]
        public void TryGetBool_NoMatchingKey_ReturnsFalse()
        {
            CommandParameters parameters = new CommandParameters(EmptyInput, string.Empty);
            Assert.IsFalse(parameters.TryGetBool(Key1, out bool value));
        }

        [TestMethod]
        [Timeout (1000)]
        public void TryGetLong_BoolValue_ReturnsFalse()
        {
            Dictionary<string, string> input = new Dictionary<string, string>
            {
                {Key1, BoolAsString}
            };

            CommandParameters parameters = new CommandParameters(input, string.Empty);
            Assert.IsFalse(parameters.TryGetLong(Key1, out long longValue));
        }

        [TestMethod]
        [Timeout (1000)]
        public void TryGetBool_StringValue_ReturnsFalse()
        {
            Dictionary<string, string> input = new Dictionary<string, string>
            {
                {Key1, StringValue}
            };

            CommandParameters parameters = new CommandParameters(input, string.Empty);
            Assert.IsFalse(parameters.TryGetBool(Key1, out bool boolValue));
        }

        [TestMethod]
        [Timeout (1000)]
        public void TryGetBool_NumericValue_ReturnsFalse()
        {
            Dictionary<string, string> input = new Dictionary<string, string>
            {
                {Key1, LongAsString}
            };

            CommandParameters parameters = new CommandParameters(input, string.Empty);
            Assert.IsFalse(parameters.TryGetBool(Key1, out bool boolValue));
        }

        [TestMethod]
        [Timeout (1000)]
        public void TryGetBool_BoolValue_ReturnsTrue_SetsCorrectValue()
        {
            Dictionary<string, string> input = new Dictionary<string, string>
            {
                {Key1, BoolAsString}
            };

            CommandParameters parameters = new CommandParameters(input, string.Empty);
            Assert.IsTrue(parameters.TryGetBool(Key1, out bool boolValue));
            Assert.AreEqual(BoolAsBool, boolValue);
        }
    }
}
