using OpenQA.Selenium.Appium.Windows;

namespace UITests.UILibrary
{
    public class EventsMode
    {
        WindowsDriver<WindowsElement> Session;
        public EventsMode(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }
    }
}
