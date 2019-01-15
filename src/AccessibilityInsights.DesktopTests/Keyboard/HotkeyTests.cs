// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AccessibilityInsights.Desktop.Keyboard.Tests
{
    [TestClass()]
    public class HotkeyTests
    {
        [TestMethod()]
        public void GetInstanceTest()
        {
            Hotkey.GetInstance("shift+F10");            
        }
    }
}
