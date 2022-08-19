// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary>
    /// Interaction logic for MoveDialog.xaml
    /// </summary>
    public partial class MoveTextRangeDialog : Window
    {
        private readonly TextRangeViewModel ViewModel;
        private readonly OpMode Mode;
        private readonly MethodInfo MethodInfo;
        private readonly List<Parameter> Parameters;
        private readonly Type ReturnType;
        /// <summary>
        /// Notify TextPattern Explorer to update Highlighter
        /// </summary>
        private readonly Action UpdateHighlighter;

        public MoveTextRangeDialog(TextRangeViewModel vm, OpMode mode, IList<TextRangeViewModel> customList, Action updateHighlighter)
        {
            this.UpdateHighlighter = updateHighlighter;

            this.ViewModel = vm;
            this.Mode = mode;
            this.MethodInfo = GetMethodInfo();
            this.ReturnType = MethodInfo.ReturnType;

            this.Parameters = (from p in MethodInfo.GetParameters()
                               select new Parameter()
                               {
                                   ParamType = p.ParameterType,
                                   Name = string.Format(CultureInfo.InvariantCulture, "{0}({1})", p.Name, p.ParameterType.Name),
                                   ParamEnums = p.ParameterType.IsEnum ? Enum.GetValues(p.ParameterType) : null
                               }).ToList();

            InitializeComponent();

            // Set title with proper name based on mode.
            this.Title = mode.ToString();

            this.dgParams.ItemsSource = this.Parameters.Where(p => p.ParamType != typeof(Axe.Windows.Desktop.UIAutomation.Patterns.TextRange));
            // set the list and enforce to select the first item by default.
            this.lbxTargetRanges.ItemsSource = customList;
            this.lbxTargetRanges.SelectedIndex = 0;

            switch (mode)
            {
                case OpMode.MoveEndpointByRange:
                    this.lbTargetTR.Visibility = Visibility.Visible;
                    this.lbxTargetRanges.Visibility = Visibility.Visible;
                    break;
                case OpMode.Move:
                case OpMode.MoveEndpointByUnit:
                    this.lbxTargetRanges.Visibility = Visibility.Collapsed;
                    this.lbTargetTR.Visibility = Visibility.Collapsed;
                    break;
                case OpMode.Compare:
                    this.dgParams.Visibility = Visibility.Collapsed;
                    break;
                case OpMode.CompareEndpoints:
                default:
                    break;
            }
        }

        /// <summary>
        /// Get Corresponding Method Info.
        /// </summary>
        /// <returns></returns>
        private MethodInfo GetMethodInfo()
        {
            return this.ViewModel.TextRange.GetType().GetMethod(this.Mode.ToString());
        }

        /// <summary>
        /// indicate the operation mode of this dialog
        /// </summary>
        public enum OpMode
        {
            Move,
            MoveEndpointByRange,
            MoveEndpointByUnit,
            Compare,
            CompareEndpoints,
        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ps = GetParametersArray();
                var ret = this.MethodInfo.Invoke(this.ViewModel.TextRange, ps);

                if (this.ReturnType != typeof(void))
                {
                    this.tbResult.Text = string.Format(CultureInfo.InvariantCulture, Properties.Resources.MoveTextRangeDialog_btnAction_Click_Return_0, ret);
                }

                /// Refresh Highlighter via TextPatternExplorer dialog
                UpdateHighlighter?.Invoke();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                this.tbResult.Text = GetExceptionString(ex);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private static string GetExceptionString(Exception e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(e.Message);
            sb.AppendFormat(CultureInfo.InvariantCulture, Properties.Resources.MoveTextRangeDialog_GetExceptionString_HResult_0x_0_X8, e.HResult);
            return sb.ToString();
        }

        private object[] GetParametersArray()
        {
            return (from p in this.Parameters
                    let v = GetConvertedValue(p)
                    select v).ToArray();
        }

        private object GetConvertedValue(Parameter p)
        {
            if (p.ParamType == typeof(Axe.Windows.Desktop.UIAutomation.Patterns.TextRange))
            {
                return ((TextRangeViewModel)this.lbxTargetRanges.SelectedItem).TextRange;
            }

            return Convert.ChangeType(p.ParamValue, p.ParamType, CultureInfo.InvariantCulture);
        }
    }
}
