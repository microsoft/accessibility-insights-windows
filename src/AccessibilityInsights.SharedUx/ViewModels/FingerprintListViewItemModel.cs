// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Fingerprint;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// FingerprintListViewItemModel class
    /// ViewModel for FingerprintListViewItemModel
    /// </summary>
    public class FingerprintListViewItemModel: ViewModelBase
    {
        /// <summary>
        /// Issue wrapped by VM
        /// </summary>
        readonly Issue issue;

        /// <summary>
        /// Action to open a given results file
        /// </summary>
        readonly Action<string, int?> LoadFileElement;

        /// <summary>
        /// Text of issue
        /// </summary>
        public string IssueText
        {
            get
            {
                return this.issue.IssueType;
            }
        }

        /// <summary>
        /// Number of issues
        /// </summary>
        public int IssueCount
        {
            get
            {
                return this.issue.Locations.Count();
            }
        }

        /// <summary>
        /// Textual representation of fingerprint
        /// </summary>
        public string FingerprintText
        {
            get
            {
                return JsonConvert.SerializeObject(this.issue.Fingerprint.Contributions, Formatting.Indented);
            }
        }

        /// <summary>
        /// Collection of file sources
        /// </summary>
        public ObservableCollection<UIElement> Locations { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="i"></param>
        /// <param name="loadFile"></param>
        public FingerprintListViewItemModel(Issue i, Action<string, int?> loadFile)
        {
            this.LoadFileElement = loadFile;
            this.issue = i;
            Locations = new ObservableCollection<UIElement>();

            foreach(var loc in i.Locations)
            {
                var lbl = new Label()
                {
                    Content = $"{loc.Source.Substring(loc.Source.LastIndexOf('\\') + 1)}- {loc.Id}",
                    ToolTip = loc.UserDisplayInfo,
                };

                var btn = new Button()
                {
                    Content = "Open",
                    DataContext = loc,
                };
                btn.Click += Btn_Click;

                var sp = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                };
                sp.Children.Add(btn);
                sp.Children.Add(lbl);
                Locations.Add(sp);
            }
            
        }

        /// <summary>
        /// Open results file from click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            var loc = (sender as Button).DataContext as ILocation;
            int.TryParse(loc.Id, NumberStyles.Integer, CultureInfo.InvariantCulture, out int elId);
            LoadFileElement?.Invoke(loc.Source, elId);
        }

        /// <summary>
        /// Generate list of VMs based on issues
        /// </summary>
        /// <param name="issues"></param>
        /// <param name="loadFile"></param>
        /// <returns></returns>
        public static List<FingerprintListViewItemModel> GetFingerprintVMs(IEnumerable<Issue> issues, Action<string, int?> loadFile)
        {
            var ret = new List<FingerprintListViewItemModel>();
            foreach(var issue in issues)
            {
                ret.Add(new FingerprintListViewItemModel(issue, loadFile));
            }
            return ret;
        }
    }
}
