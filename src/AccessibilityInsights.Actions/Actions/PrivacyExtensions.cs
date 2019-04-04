// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Desktop.UIAutomation;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AccessibilityInsights.Actions
{
    internal static class PrivacyExtensions
    {
        private const int MaxScrubbedNameLength = 3;

        private static readonly HashSet<int> PropertiesToInclude = new HashSet<int>
        {
            PropertyType.UIA_AcceleratorKeyPropertyId,
            PropertyType.UIA_AccessKeyPropertyId,
            PropertyType.UIA_AutomationIdPropertyId,
            PropertyType.UIA_BoundingRectanglePropertyId,
            PropertyType.UIA_ClassNamePropertyId,
            PropertyType.UIA_ControlTypePropertyId,
            PropertyType.UIA_FrameworkIdPropertyId,
            PropertyType.UIA_IsContentElementPropertyId,
            PropertyType.UIA_IsControlElementPropertyId,
            PropertyType.UIA_IsKeyboardFocusablePropertyId,
            PropertyType.UIA_LocalizedControlTypePropertyId,
            PropertyType.UIA_NamePropertyId,
        };

        internal static A11yElement GetScrubbedElementTree(this A11yElement pointOfInterest)
        {
            A11yElement previous = null;
            for (A11yElement currentNode = pointOfInterest; currentNode != null; currentNode = currentNode.Parent)
            {
                A11yElement clone = currentNode.CreateScrubbedCopyWithoutRelationships();

                if (previous != null)
                {
                    clone.Children.Add(previous);
                    previous.Parent = clone;
                }

                previous = clone;
            }

            return previous;
        }

        /// <summary>
        /// Given the input element, create a scrubbed copy that has no parent or children
        /// </summary>
        /// <param name="element">The element to clone</param>
        /// <returns>A new element that has been scrubbed of key information and properties</returns>
        internal static A11yElement CreateScrubbedCopyWithoutRelationships(this A11yElement element)
        {
            A11yElement clone = new A11yElement
            {
                IssueDisplayText = element.IssueDisplayText,
                ScanResults = element.ScanResults,
                TreeWalkerMode = element.TreeWalkerMode,
                UniqueId = element.UniqueId,
                Children = element.Children == null ? null : new List<A11yElement>()
            };

            if (element.Properties != null)
            {
                clone.Properties = new Dictionary<int, A11yProperty>();
                foreach (var pair in element.Properties)
                {
                    if (PropertiesToInclude.Contains(pair.Key))
                    {
                        if (pair.Key == PropertyType.UIA_NamePropertyId)
                        {
                            A11yProperty scrubbedProperty = new A11yProperty(pair.Key, pair.Value.ToString().Substring(0, MaxScrubbedNameLength));
                            clone.Properties.Add(pair.Key, scrubbedProperty);
                        }
                        else
                        {
                            clone.Properties.Add(pair.Key, pair.Value);
                        }
                    }
                }
            }

            if (element.Glimpse != null)
            {
                clone.UpdateGlimpse();
            }

            return clone;
        }

        /// <summary>
        /// Given the root to an element tree, synthesize a "yellow box" bitmap to describe it
        /// </summary>
        /// <param name="rootElement">The root element for the tree</param>
        /// <returns>A bitmap representing the element tee (normalized to having the app window at location (0,0)</returns>
        internal static Bitmap SynthesizeBitmapFromElements(this A11yElement rootElement)
        {
            const int penWidth = 4;  // This is the same width as our highlighter

            if (rootElement == null) return null;

            Rectangle rootBoundingRectangle = rootElement.BoundingRectangle;

            // Synthesize the bitmap
            bool isHighContrast = SystemInformation.HighContrast;
            Color brushColor = isHighContrast ? SystemColors.Window : Color.DarkGray;
            Color penColor = isHighContrast ? SystemColors.WindowFrame : Color.Yellow;
            Bitmap bitmap = new Bitmap(rootBoundingRectangle.Width, rootBoundingRectangle.Height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (SolidBrush brush = new SolidBrush(brushColor))
            using (Pen pen = new Pen(penColor, penWidth))
            {
                graphics.FillRectangle(brush, 0, 0, rootBoundingRectangle.Width, rootBoundingRectangle.Height);
                graphics.TranslateTransform(-rootBoundingRectangle.Left, -rootBoundingRectangle.Top);
                AddBoundingRectsRecursively(graphics, pen, rootElement);
            }

            return bitmap;
        }

        /// <summary>
        /// Add the bounding rect for this element to the Graphics surface
        /// </summary>
        /// <param name="graphics">Graphics surface to manipulate</param>
        /// <param name="pen">The pen to use (cached for performance)</param>
        /// <param name="element">The element to add--its children will also be added</param>
        private static void AddBoundingRectsRecursively(Graphics graphics, Pen pen, IA11yElement element)
        {
            graphics.DrawRectangle(pen, element.BoundingRectangle);

            if (element.Children != null)
            {
                foreach (var child in element.Children)
                {
                    AddBoundingRectsRecursively(graphics, pen, child);
                }
            }
        }
    }
}
