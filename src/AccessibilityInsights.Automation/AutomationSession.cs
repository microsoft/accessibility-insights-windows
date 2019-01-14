// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using AccessibilityInsights.Actions;
using AccessibilityInsights.RuleSelection;
using AccessibilityInsights.Desktop.Settings;
using AccessibilityInsights.Desktop.Telemetry;

namespace AccessibilityInsights.Automation
{
    /// <summary>
    /// This class holds various items that persist for the entire lifetime of an
    /// automation session. Cleanup occurs only when the stop command is executed
    /// </summary>
    internal class AutomationSession
    {
        private readonly DataManager dataManager;
        private readonly SelectAction selectAction;
        private readonly IDisposable _assemblyResolver;

        /// <summary>
        /// ctor - Initializes the app in automation mode
        /// </summary>
        /// <param name="parameters">The parameters from the command line</param>
        /// <param name="assemblyResolver">The custom assembly resolver</param>
        private AutomationSession(CommandParameters parameters, IDisposable assemblyResolver)
        {
            try
            {
                _assemblyResolver = assemblyResolver;
                var ts = TestSetting.GenerateSuiteConfiguration(SuiteConfigurationType.Default);
                this.dataManager = DataManager.GetDefaultInstance();
                this.selectAction = SelectAction.GetDefaultInstance();
                this.SessionParameters = parameters;
            }
            catch (Exception)
            {
                Cleanup();
                throw;
            }
        }

        private void Cleanup()
        {
            this.selectAction?.Dispose();
            this.dataManager?.Dispose();
            this._assemblyResolver?.Dispose();
        }

        private static AutomationSession instance;
        private static readonly object lockObject = new object();

        /// <summary>
        /// Returns the CommandParameters for the session in question
        /// </summary>
        internal CommandParameters SessionParameters { get; private set; }

        /// <summary>
        /// Obtain a new instance of the AutomationSession object. Only one can exist at a time
        /// </summary>
        /// <param name="parameters">The parameters to associate with the object</param>
        /// <param name="customAssemblyResolver">The custom assembly resolver</param>
        /// <exception cref="A11yAutomationException">Thrown if session already exists</exception>
        /// <returns>The current AutomationSession object</returns>
        internal static AutomationSession NewInstance(CommandParameters parameters, IDisposable customAssemblyResolver)
        {
            lock (lockObject)
            {
                if (instance != null)
                    throw new A11yAutomationException(DisplayStrings.ErrorAlreadyStarted);

                instance = new AutomationSession(parameters, customAssemblyResolver);
                instance.SessionParameters = parameters;

                Dictionary<string, string> config = parameters.ConfigCopy;
                config[TelemetryProperty.AutomationUsedConfigFile.ToString()] = parameters.UsedConfigFile.ToString();

                return instance;
            }
        }

        /// <summary>
        /// Obtain the current instance of the AutomationSession object. Only one can exist at a time.
        /// Will also track the action if an appropriate Action is provided
        /// </summary>
        /// <exception cref="A11yAutomationException">Thrown if session object does not exist</exception>
        /// <returns>The current AutomationSession object</returns>
        internal static AutomationSession Instance()
        {
            lock (lockObject)
            {
                if (instance == null)
                    throw new A11yAutomationException(DisplayStrings.ErrorNotStarted_Instance);
                return instance;
            }
        }

        /// <summary>
        /// Clear the current instance of the AutomationSession object
        /// </summary>
        /// <exception cref="A11yAutomationException">Thrown if session object does not exist</exception>
        internal static void ClearInstance()
        {
            lock (lockObject)
            {
                if (instance == null)
                    throw new A11yAutomationException(DisplayStrings.ErrorNotStarted_Clear);
                instance.Cleanup();
                instance = null;
            }
        }
    }
}
