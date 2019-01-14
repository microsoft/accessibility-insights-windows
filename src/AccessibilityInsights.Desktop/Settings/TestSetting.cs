// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.RuleSelection;
using System;

using static System.FormattableString;

namespace AccessibilityInsights.Desktop.Settings
{
    /// <summary>
    /// TestSettings Class
    /// </summary>
    public class TestSetting : ConfigurationBase
    {
        #region static methods for default configuration templates
        /// <summary>
        /// Get V2 Test Configuration
        /// </summary>
        /// <returns></returns>
        static TestSetting GenerateMicrosoftStandardSuiteConfiguration()
        {
            TestSetting config = new TestSetting();

            return config;
        }

        /// <summary>
        /// Get Default Test Configuration
        /// </summary>
        /// <returns></returns>
        static TestSetting GenerateDefaultSuiteConfiguration()
        {
            return GenerateMicrosoftStandardSuiteConfiguration();
        }

        /// <summary>
        /// Get a test configuration based the configurationtype parameter
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        public static TestSetting GenerateSuiteConfiguration(SuiteConfigurationType configType)
        {
            switch(configType)
            {
                case SuiteConfigurationType.Default:
                    return GenerateDefaultSuiteConfiguration();
                case SuiteConfigurationType.MicrosoftStandard:
                    return GenerateMicrosoftStandardSuiteConfiguration();
                default:
                    throw new NotSupportedException(Invariant($"{configType} is not supported."));
            }
        }
        #endregion
    }
}
