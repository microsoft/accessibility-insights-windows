## Supporting High Contrast

When adding new controls with custom colors, attempt to follow patterns from other controls in the project. Here are some tips:
- Hard code the color value into the control or style **only** if both these statements are true:<ul><li>a. The color/style in question will not affect usability and <li>b. The color/style in question will apply only to a small area of the ux (ex a single border on a button)</ul>
- If a control is made but no colors need to be set, the control will likely support high contrast by default. 
- When colors need to be set, attempt to find the appropriate brush in `AccessibilityInsights.SharedUx/Resources/Light/Brushes.xaml`. For example, most fabric icons should be colored using the IconBrush resource.
- If a brush from the Light theme is appropriate, look for that brush in `AccessibilityInsights.SharedUx/Resources/HighContrast/Brushes.xaml`. If it exists there, ensure that its value makes sense.
- To use a brush, follow the following practice, where Foreground and Background are example properties:
```
Foreground="{DynamicResource ResourceKey=<brush key>}"
```
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;or
```
<Setter Property="Background" Value="{DynamicResource ResourceKey=<brush key>}"/>
```
- Do not modify color values in the Brushes files unless you are sure you want to change every instance of that brush. Instead, create new brushes if you are unable to use an existing brush.
- Be sure to test your modification in at least the normal, High Contrast Black, and High Contrast White themes. 
- Test all states of your changes, including selected, mouse hover, and disabled states (if applicable).
- In general, buttons should use the BtnStandard style in Styles.xaml

