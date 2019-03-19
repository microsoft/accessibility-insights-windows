// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
#if FAKES_SUPPORTED
using AccessibilityInsights.Actions.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
#endif

namespace AccessibilityInsights.AutomationTests
{
    [TestClass]
    public class AutomationSessionUnitTests
    {
        // Timeouts in this file are a bit longer than most, due to the amount of shim work
        // that needs to happen

        const bool suppressTelemetryOutput = true;

        private readonly CommandParameters TestParameters = new CommandParameters(new Dictionary<string, string>(), string.Empty);

#if FAKES_SUPPORTED
        /// <summary>
        /// Set up all of the shims needed for the unit tests--we make no attempt to
        /// identify a subset of shims to configure
        /// </summary>
        private static void InitializeShims()
        {
            ShimDataManager shimDataManager = new ShimDataManager();
            ShimDataManager.GetDefaultInstance = () => shimDataManager;

            ShimSelectAction shimSelectAction = new ShimSelectAction();
            ShimSelectAction.GetDefaultInstance = () => shimSelectAction;
        }
#endif

#if FAKES_SUPPORTED
        [TestMethod]
        [Timeout(2000)]
        [ExpectedException(typeof(StackOverflowException))]
        public void NewInstance_NoInstanceExists_ExceptionInCtor_RethrowsSameException()
        {
            const string expectedMessage = "Heads up!";

            using (ShimsContext.Create())
            {
                InitializeShims();
                ShimDataManager.GetDefaultInstance = () =>
                {
                    throw new StackOverflowException(expectedMessage);
                };
                try
                {
                    AutomationSession.NewInstance(TestParameters, null);
                }
                catch (StackOverflowException e)
                {
                    Assert.AreEqual(expectedMessage, e.Message);
                    throw;
                }
            }
        }
#endif

        [TestMethod]
        [Timeout (2000)]
        public void NewInstance_ClearInstance_NotPowerShell_AssemblyResolverIsNull_DoesNotThrow()
        {
            try
            {
                AutomationSession.NewInstance(TestParameters, null);
            }
            finally
            {
                AutomationSession.ClearInstance();
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void NewInstance_NoInstanceExists_ReturnsInstance()
        {
            AutomationSession session = null;

            try
            {
                session = AutomationSession.NewInstance(TestParameters, null);
            }
            finally
            {
                AutomationSession.ClearInstance();
            }

            Assert.IsNotNull(session);
        }

        [TestMethod]
        [Timeout (2000)]
        [ExpectedException(typeof(A11yAutomationException))]
        public void NewInstance_InstanceAlreadyExists_ThrowsAutomationException_ErrorAutomation009()
        {
            AutomationSession session = AutomationSession.NewInstance(TestParameters, null);
            Assert.IsNotNull(session);

            try
            {
                AutomationSession.NewInstance(TestParameters, null);
            }
            catch (A11yAutomationException e)
            {
                Assert.IsTrue(e.Message.Contains(" Automation009:"));
                throw;
            }
            finally
            {
                AutomationSession.ClearInstance();
            }
        }

        [TestMethod]
        [Timeout (2000)]
        [ExpectedException(typeof(A11yAutomationException))]
        public void Instance_NoInstanceExists_ThrowsAutomationException_Automation010()
        {
            try
            {
                AutomationSession.Instance();
            }
            catch (A11yAutomationException e)
            {
                Assert.IsTrue(e.Message.Contains(" Automation010:"));
                throw;
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void Instance_InstanceExists_ReturnsSameInstance()
        {
            AutomationSession session = AutomationSession.NewInstance(TestParameters, null);
            Assert.IsNotNull(session);
            try
            {
                Assert.AreSame(session, AutomationSession.Instance());
            }
            finally
            {
                AutomationSession.ClearInstance();
            }
        }

        [TestMethod]
        [Timeout (2000)]
        [ExpectedException(typeof(A11yAutomationException))]
        public void ClearInstance_NoInstanceExists_ThrowsAutomationException_Automation011()
        {
            try
            {
                AutomationSession.ClearInstance();
            }
            catch (A11yAutomationException e)
            {
                Assert.IsTrue(e.Message.Contains(" Automation011:"));
                throw;
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void SessionParameters_MatchesInputParameters()
        {
            try
            {
                AutomationSession session = AutomationSession.NewInstance(TestParameters, null);
                Assert.AreSame(TestParameters, session.SessionParameters);
            }
            finally
            {
                AutomationSession.ClearInstance();
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void NewInstance_ClearInstance_AssemblyResolverIsDisposed()
        {
                DummyDisposable assemblyResolver = new DummyDisposable();

                try
                {
                    AutomationSession session = AutomationSession.NewInstance(TestParameters, assemblyResolver);
                    Assert.AreEqual(0, assemblyResolver.TimesDisposed);
                }
                finally
                {
                    AutomationSession.ClearInstance();
                    Assert.AreEqual(1, assemblyResolver.TimesDisposed);
                }
        }
    }
}
