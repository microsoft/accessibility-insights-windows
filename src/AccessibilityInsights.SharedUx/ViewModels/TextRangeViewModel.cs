// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using Axe.Windows.Desktop.Styles;
using Axe.Windows.Desktop.Types;
using Axe.Windows.Desktop.UIAutomation;
using Axe.Windows.Desktop.UIAutomation.Patterns;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using UIAutomationClient;
using static System.FormattableString;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// TextRange ViewModel class
    /// </summary>
    public class TextRangeViewModel:ViewModelBase
    {
        const int MaxTextSize = 5000; // based on Inspect code.

        /// <summary>
        /// TextRange Instance
        /// </summary>
        public TextRange TextRange { get; private set; }

        /// <summary>
        /// Text Range Attributes
        /// </summary>
        public IList<TextAttributeViewModel> GetAttributes(bool collapse)
        {
            var list = new List<TextAttributeViewModel>();

            var kvl = TextAttributeType.GetInstance().GetKeyValuePairList();

            foreach (var kv in kvl)
            {
                var kvp = GetTextRangeAttributeKeyValuePair(this.TextRange, kv, collapse);
                if (kvp != null)
                {
                    list.AddRange(kvp);
                }
            }

            return list;
        }

        /// <summary>
        /// List of BoundingRectangles
        /// </summary>
        public IList<Rectangle> BoundingRectangles
        {
            get
            {
                return this.TextRange.GetBoundingRectangles();
            }
        }

        /// <summary>
        /// TextPattern which this text range is from.
        /// </summary>
        public TextPattern TextPattern { get; private set; }

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                this.isSelected = value;

                this.MenuVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility menuVisibility = Visibility.Collapsed;
        public Visibility MenuVisibility
        {
            get
            {
                return this.menuVisibility;
            }

            set
            {
                this.menuVisibility = value;
                OnPropertyChanged(nameof(MenuVisibility));
            }
        }

        /// <summary>
        /// Text
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Header
        /// it is displayed in ListView
        /// </summary>
        public string Header { get; set; }
        public Visibility AddMenuVisibility { get; private set; }
        public Visibility RemoveMenuVisibility { get; private set; }

        /// <summary>
        /// indicate whether item is listed in custom list or not.
        /// </summary>
        bool listed;
        public bool Listed
        {
            get
            {
                return listed;
            }

            set
            {
                this.AddMenuVisibility = value == false ? Visibility.Visible : Visibility.Collapsed;
                this.RemoveMenuVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                this.listed = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="header"></param>
        /// <param name="isAdded"></param>
        public TextRangeViewModel(TextRange tr, string header)
        {
            this.TextRange = tr ?? throw new ArgumentNullException(nameof(tr));
            this.TextPattern = tr.TextPattern;
            this.Header = header;
            this.AddMenuVisibility = this.Listed == false ? Visibility.Visible : Visibility.Collapsed;
            this.RemoveMenuVisibility = this.Listed ? Visibility.Visible : Visibility.Collapsed;

        }

        private List<TextAttributeViewModel> GetTextRangeAttributeKeyValuePair(TextRange tr, KeyValuePair<int, string> kv, bool collapse)
        {
            List<TextAttributeViewModel> list = new List<TextAttributeViewModel>();

            dynamic value = tr.GetAttributeValue(kv.Key);

            switch (kv.Key)
            {
                case TextAttributeType.UIA_AnimationStyleAttributeId:
                    if(value is int)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, AnimationStyle.GetInstance().GetNameById(value)));
                    }
                    break;
                case TextAttributeType.UIA_BackgroundColorAttributeId:
                case TextAttributeType.UIA_ForegroundColorAttributeId:
                case TextAttributeType.UIA_OverlineColorAttributeId:
                case TextAttributeType.UIA_StrikethroughColorAttributeId:
                case TextAttributeType.UIA_UnderlineColorAttributeId:
                    if (value is int)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, string.Format("#{0:X8}", value)));
                    }
                    break;
                case TextAttributeType.UIA_BulletStyleAttributeId:
                    if (value is int)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, BulletStyle.GetInstance().GetNameById(value)));
                    }
                    break;
                case TextAttributeType.UIA_CapStyleAttributeId:
                    if (value is int)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, CapStyle.GetInstance().GetNameById(value)));
                    }
                    break;
                case TextAttributeType.UIA_CultureAttributeId:
                    if (value is int culture)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, Invariant($"{CultureInfo.GetCultureInfo(culture).EnglishName} ({culture})")));
                    }
                    break;
                case TextAttributeType.UIA_StyleIdAttributeId:
                    if (value is int)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, StyleId.GetInstance().GetNameById(value)));
                    }
                    break;
                case TextAttributeType.UIA_SayAsInterpretAsAttributeId:
                    // VT_I4
                    if (value is int)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, SayAsInterpretAs.GetInstance().GetNameById(value)));
                    }
                    break;
                case TextAttributeType.UIA_FontNameAttributeId:
                case TextAttributeType.UIA_StyleNameAttributeId:
                case TextAttributeType.UIA_LineSpacingAttributeId:
                    // VT_BSTR
                    if (value is string)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, value));
                    }
                    break;
                case TextAttributeType.UIA_FontSizeAttributeId:
                case TextAttributeType.UIA_IndentationFirstLineAttributeId:
                case TextAttributeType.UIA_IndentationLeadingAttributeId:
                case TextAttributeType.UIA_IndentationTrailingAttributeId:
                case TextAttributeType.UIA_MarginBottomAttributeId:
                case TextAttributeType.UIA_MarginLeadingAttributeId:
                case TextAttributeType.UIA_MarginTopAttributeId:
                case TextAttributeType.UIA_MarginTrailingAttributeId:
                case TextAttributeType.UIA_BeforeParagraphSpacingAttributeId:
                case TextAttributeType.UIA_AfterParagraphSpacingAttributeId:
                    // VT_R8
                    if (value is double || value is long)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, value.ToString()));
                    }
                    break;
                case TextAttributeType.UIA_FontWeightAttributeId:
                    if (value is int)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, Axe.Windows.Desktop.Styles.FontWeight.GetInstance().GetNameById(value)));
                    }
                    break;
                case TextAttributeType.UIA_HorizontalTextAlignmentAttributeId:
                    if (value is int)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, Axe.Windows.Desktop.Styles.FontWeight.GetInstance().GetNameById(value)));
                    }
                    break;
                case TextAttributeType.UIA_IsHiddenAttributeId:
                case TextAttributeType.UIA_IsItalicAttributeId:
                case TextAttributeType.UIA_IsReadOnlyAttributeId:
                case TextAttributeType.UIA_IsSubscriptAttributeId:
                case TextAttributeType.UIA_IsSuperscriptAttributeId:
                case TextAttributeType.UIA_IsActiveAttributeId:
                    if (value is bool)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, value.ToString()));
                    }
                    break;
                case TextAttributeType.UIA_OutlineStylesAttributeId:
                    if (value is int)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, OutlineStyle.GetInstance().GetNameById(value)));
                    }
                    break;
                case TextAttributeType.UIA_OverlineStyleAttributeId:
                case TextAttributeType.UIA_StrikethroughStyleAttributeId:
                case TextAttributeType.UIA_UnderlineStyleAttributeId:
                    if (value is int)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, TextDecorationLineStyle.GetInstance().GetNameById(value)));
                    }
                    break;
                case TextAttributeType.UIA_TabsAttributeId:
                    var txt = ConvertArrayToString(value);

                    if(txt != null)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, txt));
                    }
                    break;
                case TextAttributeType.UIA_TextFlowDirectionsAttributeId:
                    if (value is int)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, Axe.Windows.Desktop.Styles.FlowDirection.GetInstance().GetNameById(value)));
                    }
                    break;
                case TextAttributeType.UIA_AnnotationTypesAttributeId:
                    StringBuilder sb = new StringBuilder();
                    if (value is double)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, AnnotationType.GetInstance().GetNameById((int)value)));
                    }
                    else if (value is Array arr)
                    {
                        if (collapse && arr.Length > 0) // collapse the array into a single row
                        {
                            var count = new Dictionary<string, int>();
                            foreach (var val in arr)
                            {
                                var key = AnnotationType.GetInstance().GetNameById((int)val);
                                if (count.ContainsKey(key))
                                {
                                    count[key]++;
                                }
                                else
                                {
                                    count[key] = 1;
                                }
                            }

                            StringBuilder strBuild = new StringBuilder();
                            foreach (var item in count)
                            {
                                strBuild.Append(Invariant($"{item.Key}: {item.Value}, "));
                            }
                            strBuild.Length-=2; //remove final , and <space>
                            list.Add(new TextAttributeViewModel(kv.Key, kv.Value,strBuild.ToString()));
                        }
                        else // create a row for each array value
                        {
                            if (arr.Length > 0)
                            {
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    list.Add(new TextAttributeViewModel(kv.Key, string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", kv.Value, i), AnnotationType.GetInstance().GetNameById((int)arr.GetValue(i))));
                                }
                            }
                        }
                    }
                    break;
                case TextAttributeType.UIA_SelectionActiveEndAttributeId:
                    if (value is int)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, ActiveEnd.GetInstance().GetNameById(value)));
                    }
                    break;
                case TextAttributeType.UIA_CaretPositionAttributeId:
                    if (value is int)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, CaretPosition.GetInstance().GetNameById(value)));
                    }
                    break;
                case TextAttributeType.UIA_CaretBidiModeAttributeId:
                    if (value is int)
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, CaretBidiMode.GetInstance().GetNameById(value)));
                    }
                    break;
                case TextAttributeType.UIA_AnnotationObjectsAttributeId:
                    if (value is IUIAutomationElementArray)
                    {
                        IUIAutomationElementArray arr = value;
                        if (arr.Length > 0)
                        {
                            for (int i = 0; i < arr.Length; i++)
                            {
                                list.Add(new TextAttributeViewModel(kv.Key, string.Format(CultureInfo.InvariantCulture, Resources.TextRangeViewModel_GetTextRangeAttributeKeyValuePair_AnnotationObjects_0, i), new DesktopElement((IUIAutomationElement)arr.GetElement(i))));
                            }
                        }
                    }
                    break;
                case TextAttributeType.UIA_LinkAttributeId:
                    // do nothing for now until it is shown as necessary information.
                    //try
                    //{
                    //    IUIAutomationTextRange lnk = Marshal.GetObjectForIUnknown(value) as IUIAutomationTextRange;
                    //    list.Add(new TextAttributeViewModel(kv.Value, new TextRangeViewModel(new TextRange(lnk))));
                    //}
                    //catch (Exception e)
                    //{
                    //    e.ReportException();
                    //}
                    break;
                default:
                    // need to make a decision for these Attributes since it return Object.
                    if (value.GetType().Name != "__ComObject")
                    {
                        list.Add(new TextAttributeViewModel(kv.Key, kv.Value, value));
                    }
                    break;
            }

            return list;
        }

        static string ConvertArrayToString(dynamic value)
        {
            StringBuilder sb = new StringBuilder();
            if (value is double)
            {
                sb.AppendFormat("[{0}]", value);
            }
            else if (value is Array)
            {
                Array arr = value;
                sb.Append('[');
                if (arr.Length > 0)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", arr.GetValue(0));
                    for (int i = 1; i < arr.Length; i++)
                    {
                        sb.AppendFormat(CultureInfo.InvariantCulture, ", {0}", arr.GetValue(i));
                    }
                }
                sb.Append(']');
            }

            return sb.Length != 0 ? sb.ToString() : null;
        }

        /// <summary>
        /// Get text value of pattern
        /// </summary>
        /// <param name="showFormattingChars">Show paragraph marks and other hidden formatting symbols</param>
        /// <returns></returns>
        internal string GetText(bool withFormattingChars)
        {
            var str = this.TextRange.GetText(MaxTextSize);
            if (withFormattingChars)
            {
                str = str.Replace(' ', '·');
                str = str.Replace('\t', '→');
                str = Regex.Replace(str, @"\r\n|\n|\r", "¶\n");
            }
            return str;
        }
    }
}
