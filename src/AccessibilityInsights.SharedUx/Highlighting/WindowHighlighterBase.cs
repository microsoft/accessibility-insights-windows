// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.Highlighting
{
    /// <summary>
    /// Base functionality and data for window-based highlighers
    /// </summary>
    public abstract class WindowHighlighterBase
    {
        /// <summary>
        /// Callback for error textblock click
        /// </summary>
        internal MouseButtonEventHandler TBCallback;

        /// <summary>
        /// Color for borders
        /// </summary>
        internal SolidColorBrush Brush { get; set; }

        /// <summary>
        /// Highlighter gap width
        /// </summary>
        internal int GapWidth { get; set; }

        /// <summary>
        /// Window on which to display highlights
        /// </summary>
        internal Window HighlightWindow;

        /// <summary>
        /// Highlighted elements and related item
        /// </summary>
        public Dictionary<A11yElement, GroupHighlighterItem> Items { get; internal set; }

        /// <summary>
        /// DPI scale for window
        /// </summary>
        internal double DPI = 1.0;

        /// <summary>
        /// Set the state of visibility
        /// </summary>
        public bool IsVisible { get; internal set; }

        /// <summary>
        /// Base selected element
        /// </summary>
        internal A11yElement BaseElement;

        /// <summary>
        /// Bounding rectangle for original window
        /// </summary>
        internal Rectangle Dimensions { get; set; }

        /// <summary>
        /// Canvas on which borders are drawn
        /// </summary>
        internal Canvas canvas { get; set; }

        /// <summary>
        /// Is highlighter currently open
        /// </summary>
        internal bool IsClosed;

        /// <summary>
        /// get the middle point to start drawing from
        /// </summary>
        /// <param name="startPoint">the X or Y of the element</param>
        /// <param name="length1">The width or height of the element not scaled</param>
        /// <param name="length2">the width or height you want to use to draw scaled</param>
        private double GetMidPoint(double startPoint, double length1, double length2)
        {
            return startPoint + (length1 / 2.0) / DPI - (length2 / 2.0);
        }

        /// <summary>
        /// Update highlighter with new bounding box from element
        /// </summary>
        /// <param name="el"></param>
        public void UpdateElement(A11yElement el)
            {
            if (el != null && Items.Keys.Contains(el))
            {
                var hdo = Items[el];

                if (!el.BoundingRectangle.IsEmpty && this.Dimensions.IntersectsWith(el.BoundingRectangle))
                {
                    var l = (el.BoundingRectangle.Left - Dimensions.Left) / DPI;
                    var t = (el.BoundingRectangle.Top - Dimensions.Top) / DPI;

                    hdo.TbError.SetValue(Canvas.LeftProperty, GetMidPoint(l, el.BoundingRectangle.Width, hdo.TbError.Width));
                    hdo.TbError.SetValue(Canvas.TopProperty, GetMidPoint(t, el.BoundingRectangle.Height, hdo.TbError.Height));

                    hdo.BrdrError.Width = 28;
                    hdo.BrdrError.Height = 28 ;
                    hdo.BrdrError.SetValue(Canvas.LeftProperty, GetMidPoint(l, el.BoundingRectangle.Width, hdo.BrdrError.Width));
                    hdo.BrdrError.SetValue(Canvas.TopProperty, GetMidPoint(t, el.BoundingRectangle.Height, hdo.BrdrError.Height));

                    hdo.Show();
                }
                else
                {
                    hdo.Hide();
                }
            }
        }

        /// <summary>
        /// Highlight a single element
        /// </summary>
        /// <param name="el"></param>
        /// <param name="txt"></param>
        public void SetSingleElement(A11yElement el, string txt = null)
        {
            foreach (var elem in this.Items.Values.AsParallel())
            {
                canvas?.Children.Remove(elem.BrdrError);
                canvas?.Children.Remove(elem.TbError);
                elem.Clear();
            }
            this.Items.Clear();

            AddElement(el, txt);
        }

        /// <summary>
        /// Add element and highlight
        /// </summary>
        /// <param name="el"></param>
        /// <param name="txt"></param>
        public void AddElement(A11yElement el, string txt = "!")
        {
            if (el != null)
            {
                if (!Items.Keys.Contains(el))
                {
                    if (this.IsVisible && !el.BoundingRectangle.IsEmpty && this.Dimensions.IntersectsWith(el.BoundingRectangle))
                    {
                        var l = (el.BoundingRectangle.Left - Dimensions.Left) / DPI - GapWidth;
                        var t = (el.BoundingRectangle.Top - Dimensions.Top) / DPI - GapWidth;

                        var bord = new Border()
                        {
                            BorderBrush = Brush,
                            BorderThickness = new Thickness(4),
                            Width = el.BoundingRectangle.Width / DPI + GapWidth * 2,
                            Height = el.BoundingRectangle.Height / DPI + GapWidth * 2,
                        };
                        TextBlock tb = null;
                        if (txt != null)
                        {
                            tb = new TextBlock()
                            {
                                Text = txt,
                                Background = Brush,
                                Foreground = new SolidColorBrush(Colors.White),
                                Tag = el,
                                Width = 28,
                                TextAlignment = TextAlignment.Center,
                                Height = 28
                            };
                            if (TBCallback != null)
                            {
                                tb.MouseDown += TBCallback;
                            }

                            canvas.Children.Add(tb);
                            tb.SetValue(Canvas.LeftProperty, l + el.BoundingRectangle.Width / DPI - tb.Width + GapWidth * 2);
                            tb.SetValue(Canvas.TopProperty, t);
                            tb.SetValue(Canvas.ZIndexProperty, 1);
                        }
                        canvas.Children.Add(bord);
                        bord.SetValue(Canvas.LeftProperty, l);
                        bord.SetValue(Canvas.TopProperty, t);
                        bord.SetValue(Canvas.ZIndexProperty, 0);
                        Items[el] = new GroupHighlighterItem(bord, tb);
                    }
                }
                else
                {
                    Items[el].AddRef();
                }
            }
        }

        /// <summary>
        /// Add element and highlight
        /// </summary>
        /// <param name="el"></param>
        /// <param name="txt"></param>
        public void AddElementRoundBorder(A11yElement el, string txt = "!")
        {
            if (el != null)
            {
                if (!Items.Keys.Contains(el))
                {
                    if (this.IsVisible && !el.BoundingRectangle.IsEmpty && this.Dimensions.IntersectsWith(el.BoundingRectangle))
                    {
                        var l = (el.BoundingRectangle.Left - Dimensions.Left) / DPI;
                        var t = (el.BoundingRectangle.Top - Dimensions.Top) / DPI;

                        var bord = new Border()
                        {
                            BorderBrush = Application.Current.Resources["SearchAndIconForegroundBrush"] as SolidColorBrush,
                            BorderThickness = new Thickness(2),
                            Background = Application.Current.Resources["SecondaryBGBrush"] as SolidColorBrush,
                            Width = 28,
                            Height = 28,
                            CornerRadius = new CornerRadius(28),
                        };

                        TextBlock tb = null;
                        if (txt != null)
                        {
                            tb = new TextBlock()
                            {
                                Text = txt,
                                Tag = el,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Foreground = Application.Current.Resources["PrimaryFGBrush"] as SolidColorBrush,
                            };
                            if (TBCallback != null)
                            {
                                tb.MouseDown += TBCallback;
                            }

                            bord.Child = tb;
                        }
                        canvas.Children.Add(bord);
                        bord.SetValue(Canvas.LeftProperty, GetMidPoint(l, el.BoundingRectangle.Width, bord.Width));
                        bord.SetValue(Canvas.TopProperty, GetMidPoint(t, el.BoundingRectangle.Height, bord.Height));
                        bord.SetValue(Canvas.ZIndexProperty, 0);
                        Items[el] = new GroupHighlighterItem(bord, tb);
                    }
                }
                else
                {
                    Items[el].AddRef();
                }
            }
        }
        /// <summary>
        /// Remove element
        /// </summary>
        /// <param name="el"></param>
        public void RemoveElement(A11yElement el)
        {
            if (el != null && Items.Keys.Contains(el))
            {
                var elem = Items[el];
                if (elem.Release())
                {
                    canvas.Children.Remove(elem.BrdrError);
                    canvas.Children.Remove(elem.TbError);
                    elem.Clear();
                    Items.Remove(el);
                }
            }
        }

        /// <summary>
        /// Clear all highlighting
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Hide window
        /// </summary>
        public abstract void Hide();

        /// <summary>
        /// Show window
        /// </summary>
        public abstract void Show();

    }
}
