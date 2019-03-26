using System.Windows;
using System.Windows.Media;

namespace AccessibilityInsights.Extensions.GitHub
{
    public static class UIResources
    {
        public static string PlaceHolder { get; } = Properties.Resources.PlaceHolder;
        public static SolidColorBrush BlackBrush { get; } = Application.Current.Resources["TextBrush"] as SolidColorBrush;
        public static SolidColorBrush GrayBrush { get; } = Application.Current.Resources["TextBrushGray"] as SolidColorBrush;
    }
}
