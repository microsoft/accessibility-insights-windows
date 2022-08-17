// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.SetupLibraryUnitTests
{
    internal enum TestEnum
    {
        Default,
        Option1,
        Option2,
        Option3
    }

    [TestClass]
    public class SettingsDictionaryUnitTests
    {
        const string Key1 = "Key1";
        const string Key2 = "Key2";
        const string Key3 = "Key3";
        const string Key1LowerCase = "key1";

        const string StringValue = "Value1";
        const int IntValue = 12345;
        const bool BoolValue = true;
        const long LongValue = 54321;
        static readonly int[] IntArrayValue = new[] { 1, 2, 3, 4, 5 };
        static readonly JArray JArrayValue = BuildJArray();

        static JArray BuildJArray()
        {
            JArray array = new JArray
            {
                2,
                4,
                6
            };

            return array;
        }

        [TestMethod]
        [Timeout(2000)]
        public void Ctor_DictionaryIsEmpty()
        {
            SettingsDictionary settings = new SettingsDictionary();
            Assert.AreEqual(0, settings.Count);
        }

        [TestMethod]
        [Timeout(2000)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyCtor_OriginalIsNull_ThrowsArgumentNullException()
        {
            new SettingsDictionary(null);
        }

        [TestMethod]
        [Timeout(2000)]
        public void CopyCtor_SettingsAreShallowCopied()
        {
            SettingsDictionary settings1 = new SettingsDictionary
            {
                { Key1, StringValue },
                { Key2, IntArrayValue },
                { Key3, JArrayValue }
            };

            SettingsDictionary settings2 = new SettingsDictionary(settings1);

            Assert.AreEqual(3, settings1.Count);
            Assert.AreEqual(3, settings2.Count);
            Assert.AreEqual(settings1[Key1], settings2[Key1]);
            Assert.AreSame(settings1[Key2], settings2[Key2]);
            Assert.AreSame(settings1[Key3], settings1[Key3]);
        }

        [TestMethod]
        [Timeout(2000)]
        public void SettingsAreCaseSensitive()
        {
            SettingsDictionary settings = new SettingsDictionary
            {
                { Key1, StringValue }
            };
            Assert.IsFalse(settings.TryGetValue(Key1LowerCase, out _));
            settings.Add(Key1LowerCase, LongValue);

            Assert.AreEqual(2, settings.Count);
            Assert.AreEqual(StringValue, settings[Key1]);
            Assert.AreEqual(LongValue, settings[Key1LowerCase]);
        }

        [TestMethod]
        [Timeout(2000)]
        public void CopyCtor_LongsAreConvertedToInt()
        {
            SettingsDictionary settings1 = new SettingsDictionary
            {
                { Key1, LongValue }
            };

            SettingsDictionary settings2 = new SettingsDictionary(settings1);

            Assert.AreEqual(1, settings2.Count);
            Assert.AreEqual(typeof(int), settings2[Key1].GetType());
            Assert.AreNotEqual(LongValue, settings2[Key1]);
            Assert.AreEqual((int)LongValue, settings2[Key1]);
        }

        [TestMethod]
        [Timeout(2000)]
        public void CopyCtor_JArrayIsConvertedToIntArray()
        {
            SettingsDictionary settings1 = new SettingsDictionary
            {
                { Key1, JArrayValue }
            };

            SettingsDictionary settings2 = new SettingsDictionary(settings1);

            Assert.AreEqual(1, settings2.Count);
            Assert.AreEqual(typeof(int[]), settings2[Key1].GetType());
            int[] intArray = (int[])settings2[Key1];
            Assert.AreEqual(3, intArray.Length);
            Assert.AreEqual(2, intArray[0]);
            Assert.AreEqual(4, intArray[1]);
            Assert.AreEqual(6, intArray[2]);
        }

        [TestMethod]
        [Timeout(2000)]
        public void RemapSetting_KeyDoesNotExist_ChangesNothing()
        {
            SettingsDictionary settings = new SettingsDictionary
            {
                { Key1, BoolValue }
            };

            settings.RemapSetting(Key2, Key3);

            Assert.AreEqual(1, settings.Count);
            Assert.AreEqual(BoolValue, settings[Key1]);
        }

        [TestMethod]
        [Timeout(2000)]
        public void RemapSetting_KeyExists_ChangesKeyName()
        {
            SettingsDictionary settings = new SettingsDictionary
            {
                { Key1, IntArrayValue }
            };

            settings.RemapSetting(Key1, Key2);

            Assert.AreEqual(1, settings.Count);
            Assert.AreSame(IntArrayValue, settings[Key2]);
        }

        [TestMethod]
        [Timeout(2000)]
        public void RemapIntToEnumName_KeyDoesNotExist_ChangesNothing()
        {
            SettingsDictionary settings = new SettingsDictionary
            {
                { Key1, (int)TestEnum.Default }
            };

            settings.RemapIntToEnumName<TestEnum>(Key2);

            Assert.AreEqual(1, settings.Count);
            Assert.AreEqual((int)TestEnum.Default, settings[Key1]);
        }

        [TestMethod]
        [Timeout(2000)]
        public void RemapIntToEnumName_KeyExists_ValueIsLong_ValueIsValid_ReturnsCorrectEnum()
        {
            SettingsDictionary settings = new SettingsDictionary
            {
                { Key1, (long)TestEnum.Option1 }
            };

            settings.RemapIntToEnumName<TestEnum>(Key1);

            Assert.AreEqual(1, settings.Count);
            Assert.AreEqual(TestEnum.Option1.ToString(), settings[Key1]);
        }

        [TestMethod]
        [Timeout(2000)]
        public void RemapIntToEnumName_KeyExists_ValueIsLong_ValueIsNotValid_ReturnsCorrectEnum()
        {
            SettingsDictionary settings = new SettingsDictionary
            {
                { Key1, LongValue }
            };

            settings.RemapIntToEnumName<TestEnum>(Key1);

            Assert.AreEqual(1, settings.Count);
            Assert.AreEqual(LongValue, settings[Key1]);
        }

        [TestMethod]
        [Timeout(2000)]
        public void RemapIntToEnumName_KeyExists_ValueIsInt_ValueIsNotValid_ReturnsCorrectEnum()
        {
            SettingsDictionary settings = new SettingsDictionary
            {
                { Key1, IntValue }
            };

            settings.RemapIntToEnumName<TestEnum>(Key1);

            Assert.AreEqual(1, settings.Count);
            Assert.AreEqual(IntValue, settings[Key1]);
        }

        [TestMethod]
        [Timeout(2000)]
        public void RemapIntToEnumName_KeyExists_ValueIsEnum_ChangesNothing()
        {
            SettingsDictionary settings = new SettingsDictionary
            {
                { Key1, TestEnum.Option2 }
            };

            settings.RemapIntToEnumName<TestEnum>(Key1);

            Assert.AreEqual(1, settings.Count);
            Assert.AreEqual(TestEnum.Option2, settings[Key1]);
        }

        [TestMethod]
        [Timeout(2000)]
        public void RemapIntToEnumName_KeyExists_TypeIsNotEnum_ChangesNothing()
        {
            SettingsDictionary settings = new SettingsDictionary
            {
                { Key1, (int)TestEnum.Option2 }
            };

            settings.RemapIntToEnumName<DateTime>(Key1);

            Assert.AreEqual(1, settings.Count);
            Assert.AreEqual((int)TestEnum.Option2, settings[Key1]);
        }

        [TestMethod]
        [Timeout(2000)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Diff_OtherIsNull_ThrowsArgumentNullException()
        {
            SettingsDictionary settings = new SettingsDictionary
            {
                { Key1, BoolValue },
                { Key2, IntValue },
                { Key3, LongValue },
            };

            settings.Diff(null);
        }

        [TestMethod]
        [Timeout(2000)]
        public void Diff_SameObjects_ReturnsEmptySet()
        {
            SettingsDictionary settings = new SettingsDictionary
            {
                { Key1, BoolValue },
                { Key2, IntValue },
                { Key3, LongValue },
            };

            Assert.AreEqual(0, settings.Diff(settings).Count);
        }

        [TestMethod]
        [Timeout(2000)]
        public void Diff_OneValueChanges_CompatibleType_SameValue_ReturnsEmptySet()
        {
            SettingsDictionary settings1 = new SettingsDictionary
            {
                { Key1, BoolValue },
                { Key2, IntValue },
            };

            SettingsDictionary settings2 = new SettingsDictionary
            {
                { Key1, BoolValue },
                { Key2, (long)IntValue },
            };

            IReadOnlyDictionary<string, object> diff = settings1.Diff(settings2);

            Assert.AreEqual(0, diff.Count);
        }

        [TestMethod]
        [Timeout(2000)]
        public void Diff_OneValueChanges_IncompatibleType_ReturnsChangedValue()
        {
            SettingsDictionary settings1 = new SettingsDictionary
            {
                { Key1, BoolValue },
                { Key2, IntValue },
            };

            SettingsDictionary settings2 = new SettingsDictionary
            {
                { Key1, BoolValue },
                { Key2, BoolValue },
            };

            IReadOnlyDictionary<string, object> diff = settings1.Diff(settings2);

            Assert.AreEqual(1, diff.Count);
            Assert.AreEqual(BoolValue, diff[Key2]);
        }

        [TestMethod]
        [Timeout(2000)]
        public void Diff_OneValueChanges_CompatibleType_DifferntValue_ReturnsChangedValue()
        {
            SettingsDictionary settings1 = new SettingsDictionary
            {
                { Key1, BoolValue },
                { Key2, IntValue },
            };

            SettingsDictionary settings2 = new SettingsDictionary
            {
                { Key1, BoolValue },
                { Key2, IntValue + 1 },
            };

            IReadOnlyDictionary<string, object> diff = settings1.Diff(settings2);

            Assert.AreEqual(1, diff.Count);
            Assert.AreEqual(IntValue + 1, diff[Key2]);
        }
    }
}
