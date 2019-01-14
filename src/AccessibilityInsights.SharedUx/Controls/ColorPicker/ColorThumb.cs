// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.Controls.ColorPicker
{
    /// <summary>
    /// Credit to: https://blogs.msdn.microsoft.com/wpfsdk/2006/10/26/uncommon-dialogs-font-chooser-color-picker-dialogs/
    /// </summary>
    public class ColorThumb : System.Windows.Controls.Primitives.Thumb
    {
        static ColorThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorThumb),
                new FrameworkPropertyMetadata(typeof(ColorThumb)));
        }

        public static readonly DependencyProperty ThumbColorProperty =
        DependencyProperty.Register
        ("ThumbColor", typeof(Color), typeof(ColorThumb),
            new FrameworkPropertyMetadata(Colors.Transparent));

        public static readonly DependencyProperty PointerOutlineThicknessProperty =
        DependencyProperty.Register
        ("PointerOutlineThickness", typeof(double), typeof(ColorThumb),
            new FrameworkPropertyMetadata(1.0));

        public static readonly DependencyProperty PointerOutlineBrushProperty =
        DependencyProperty.Register
        ("PointerOutlineBrush", typeof(Brush), typeof(ColorThumb),
            new FrameworkPropertyMetadata(null));

        public Color ThumbColor
        {
            get
            {
                return (Color)GetValue(ThumbColorProperty);
            }
            set
            {

                SetValue(ThumbColorProperty, value);
            }
        }

        public double PointerOutlineThickness
        {
            get
            {
                return (double)GetValue(PointerOutlineThicknessProperty);
            }
            set
            {
                SetValue(PointerOutlineThicknessProperty, value);
            }
        }

        public Brush PointerOutlineBrush
        {
            get
            {
                return (Brush)GetValue(PointerOutlineBrushProperty);
            }
            set
            {
                SetValue(PointerOutlineBrushProperty, value);
            }
        }
    }
}
