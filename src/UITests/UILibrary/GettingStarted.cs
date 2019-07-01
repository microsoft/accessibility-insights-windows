using OpenQA.Selenium.Appium.Windows;

namespace UITests.UILibrary
{
    public class GettingStarted
    {
        WindowsDriver<WindowsElement> Session;

        public GettingStarted(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }
        public bool DismissTelemetry() => false;
        public bool DismissPage() => false;
    }
}
