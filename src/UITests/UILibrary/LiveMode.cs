using OpenQA.Selenium.Appium.Windows;

namespace UITests.UILibrary
{
    public class LiveMode
    {
        WindowsDriver<WindowsElement> Session;

        public LiveMode(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }
        public bool OpenFile(string folder, string fileName) => false;
    }
}
