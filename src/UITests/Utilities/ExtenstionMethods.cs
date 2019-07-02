using OpenQA.Selenium.Appium.Windows;

namespace UITests.Utilities
{
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Attempts to click the WindowsElement identified by automationId
        /// </summary>
        /// <param name="driver">WinAppDriver driver session</param>
        /// <param name="automationID">AutomationID of element to click</param>
        /// <returns>True if element was found and clicked without exception; false otherwise</returns>
        internal static bool FindAndClickByAutomationID(this WindowsDriver<WindowsElement> driver, string automationID)
        {
            try
            {
                var btn = driver.FindElementByAccessibilityId(automationID);

                btn.Click();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
