// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.KeyboardHelpers;
using AccessibilityInsights.Win32;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;

namespace AccessibilityInsights.SharedUxTests.KeyboardHelpers
{
    [TestClass]
    public class HotKeyUnitTests
    {
        [TestMethod]
        public void GetInstance_ShiftF10_HasCorrectCharacteristics()
        {
            HotKey hotkey = HotKey.GetInstance("shift+F10");
            Assert.AreEqual(Keys.F10, hotkey.Key);
            Assert.AreEqual(HotkeyModifier.MOD_SHIFT, hotkey.Modifier);
        }

        [TestMethod]
        public void GetInstance_ControlShiftF9_HasCorrectCharacteristics()
        {
            HotKey hotkey = HotKey.GetInstance("control,shift+F9");
            Assert.AreEqual(Keys.F9, hotkey.Key);
            Assert.AreEqual(HotkeyModifier.MOD_SHIFT | HotkeyModifier.MOD_CONTROL, hotkey.Modifier);
        }
    }
}
