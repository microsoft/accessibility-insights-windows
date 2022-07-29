// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Dialogs;
using Axe.Windows.Desktop.Types;
using Axe.Windows.Desktop.UIAutomation;
using System;
using System.Windows;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// Class for Text
    /// </summary>
    public class TextAttributeViewModel : ViewModelBase
    {
#pragma warning disable IDE0052 // TODO: Is this needed?
        private readonly TextRangeViewModel TextRange;
#pragma warning restore IDE0052
        private readonly DesktopElement Element;
        public int Id { get; private set; }

        public string Name { get; private set; }

        public string Text { get; private set; }

        public Visibility DetailsVisibility { get; private set; } = Visibility.Collapsed;

        /// <summary>
        /// Constructor with DesktopElement
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Text"></param>
        /// <param name="e"></param>
        public TextAttributeViewModel(int id, string name, DesktopElement e)
        {
            this.Id = id;
            this.Name = name;
            this.Element = e ?? throw new ArgumentNullException(nameof(e));
            this.Text = e.Glimpse;
            this.DetailsVisibility = Visibility.Visible;
            this.Command = new CommandHandler(() => ShowElementInfo(), true);
        }

        /// <summary>
        /// Constructor with TextRangeViewModel
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <param name="tr"></param>
        public TextAttributeViewModel(int id, string name, TextRangeViewModel tr)
        {
            this.Id = id;
            this.Name = name;
            this.TextRange = tr ?? throw new ArgumentNullException(nameof(tr));
            this.Text = tr.Text;
            this.DetailsVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Normal constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        public TextAttributeViewModel(int id, string name, string text)
        {
            this.Id = id;
            this.Name = name;
            this.Text = text;
            this.DetailsVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Constructor with null value
        /// </summary>
        /// <param name="id"></param>
        public TextAttributeViewModel(int id)
        {
            this.Id = id;
            this.Name = TextAttributeType.GetInstance().GetNameById(id);
            this.Text = null;
            this.DetailsVisibility = Visibility.Collapsed;
        }

        private ICommand _clickCommand;
        private readonly CommandHandler Command;

        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = this.Command);
            }
        }

        private void ShowElementInfo()
        {
            var dlg = new ElementInfoDialog(this.Element)
            {
                Owner = Application.Current.MainWindow
            };

            dlg.ShowDialog();
        }

        /// <summary>
        /// To string for copying
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}: {Text}";
        }
    }
}
