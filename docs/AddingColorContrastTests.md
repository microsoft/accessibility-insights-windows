# Color Contrast Tests
When working with color contrast bugs it can be difficult for various environments
to replicate functionality identically. There are too many factors at play in how
various screens render different elements. The only way to reliably replicate behaviors
is to capture the actual input used in the algorithm, convert it to a BMP, and add it
to the test images.

### Steps

- Convert the image fed to the algorithm at RunTime to a bitmap.
- Add the bitmap to `Axe.Windows.DesktopTests/TestImages`
- Give it a meaningful name.
- Create a new [TestCase](https://github.com/Microsoft/accessibility-insights-windows/blob/d1fe24c1a763dbbca423fc1d9c4708eb7396a44c/src/AccessibilityInsights.DesktopTests/ColorContrastAnalyzer/ImageTests.cs#L28-L39) 

### Creating a Good Test Case

Be careful when creating a Color Contrast test case. They can be fragile. Some good
rules to follow

- If the confidence is not `high` utilize things like Visibly Similar to, rather than precise colors.
- Always include an assertion about confidence level.
- If a test case is in a response to a specific issue, link it in comments rather than verbosely describing what you're testing.