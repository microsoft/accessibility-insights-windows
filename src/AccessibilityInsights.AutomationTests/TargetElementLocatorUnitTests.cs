// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Axe.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axe.Windows.AutomationTests
{
    [TestClass]
    public class TargetElementLocatorUnitTests
    {
        [TestMethod]
        [Timeout (1000)]
        [ExpectedException(typeof(A11yAutomationException))]
        public void LocateElement_NoTargetSpecifiedInParameters_ThrowsAutomationException_ErrorAutomation007()
        {
            try
            {
                CommandParameters parameters = new CommandParameters(new Dictionary<string, string>(), string.Empty);
                TargetElementLocator.LocateElement(parameters);
            }
            catch (A11yAutomationException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Automation007:"));
                throw;
            }
        }


        [TestMethod]
        [Timeout(1000)]
        [ExpectedException(typeof(A11yAutomationException))]
        public void LocateElement_SpecifiedPIDNotExist_ThrowsAutomationException_ErrorAutomation017()
        {
            try
            {
                var ps = new Dictionary<string, string>();
                ps.Add(CommandConstStrings.TargetProcessId, "-1"); // invalid process id. 
                CommandParameters parameters = new CommandParameters(ps, string.Empty);
                TargetElementLocator.LocateElement(parameters);
            }
            catch (A11yAutomationException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Automation017:"));
                throw;
            }
        }

    }
}
