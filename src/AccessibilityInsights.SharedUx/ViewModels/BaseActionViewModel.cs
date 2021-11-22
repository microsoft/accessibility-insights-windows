// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.SharedUx.Telemetry;
using Axe.Windows.Actions;
using Axe.Windows.Core.Bases;
using Axe.Windows.Desktop.UIAutomation;
using Axe.Windows.Desktop.UIAutomation.Patterns;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// Base class for ActionViewModels
    /// ActionViewModel is to drive Action(method)s in a control
    /// </summary>
    abstract public class BaseActionViewModel: ViewModelBase
    {
        internal A11yPattern pattern;
        internal MethodInfo methodinfo;

        /// <summary>
        /// Pattern Name
        /// </summary>
        public string PatternName { get; private set; }

        /// <summary>
        /// Action name
        /// </summary>
        public string Name { get; private set; }

        public Type ReturnType { get; private set; }

        public dynamic ReturnValue { get; protected set; }

        /// <summary>
        /// Execution status: succeeded or not
        /// </summary>
        public bool IsSucceeded { get; private set;  }

        /// <summary>
        /// Parameters
        /// </summary>
        public IList<Parameter> Parameters { get; private set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="ptn"></param>
        /// <param name="m"></param>
        protected BaseActionViewModel(A11yPattern ptn, MethodInfo m)
        {
            this.pattern = ptn ?? throw new ArgumentNullException(nameof(ptn));
            this.methodinfo = m ?? throw new ArgumentNullException(nameof(m));

            this.PatternName = ptn.Name;
            this.Name = m.Name;
            this.ReturnType = m.ReturnType;

            this.Parameters = (from p in m.GetParameters()
                               select new Parameter()
                               {
                                   ParamType = p.ParameterType,
                                   Name = string.Format(CultureInfo.InvariantCulture, "{0}({1})", p.Name, p.ParameterType.Name),
                                   ParamEnums = p.ParameterType.IsEnum ? Enum.GetValues(p.ParameterType) : null
                               }).ToList();
        }

        /// <summary>
        /// convert the parameter list into actual parameter to invoke
        /// </summary>
        /// <returns></returns>
        protected object[] GetParametersArray()
        {
            return (from p in this.Parameters
                    select Convert.ChangeType(p.ParamValue, p.ParamType,CultureInfo.InvariantCulture)).ToArray();
        }

        #region Command invoke
        private ICommand _clickCommand;

        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() => DoAction(), this.methodinfo != null));
            }
        }

        public void DoAction()
        {
            var val = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", this.pattern.Name, this.Name);
            try
            {
                this.IsSucceeded = true;
                this.ReturnType = this.methodinfo.ReturnType;

                InvokeMethod();
                /// add a log
                Logger.PublishTelemetryEvent(TelemetryAction.Pattern_Invoke_Action, TelemetryProperty.PatternMethod, val);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
                MessageDialog.Show(string.Format(CultureInfo.InvariantCulture, Properties.Resources.BaseActionViewModel_ExceptionMessage,
                    val, e.InnerException == null ? e.Message : e.InnerException.Message));
                this.IsSucceeded = false;
                this.ReturnValue = e.InnerException?.HResult;
                this.ReturnType = typeof(void);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// overriden method to invoke method. it should be implemented by inherited method.
        /// </summary>
        protected virtual void InvokeMethod()
        {
            var ret = ControlPatternAction.RunAction(this.pattern.Element.UniqueId,this.pattern.Id, this.methodinfo.Name ,GetParametersArray());

            if (this.ReturnType != typeof(void))
            {
                this.ReturnValue = ret;
            }
        }

        #endregion

        /// <summary>
        /// Get appropriate Action View Model
        /// </summary>
        /// <param name="p"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        internal static BaseActionViewModel GetActionViewModel(A11yPattern p, MethodInfo m)
        {
            if (m.ReturnType == typeof(DesktopElement) || m.ReturnType == typeof(IList<DesktopElement>))
            {
                return new ReturnA11yElementsViewModel(p, m);
            }
            else if(m.ReturnType.Name != "IAccessible")
            {
                if (m.ReturnType != typeof(TextRange) && m.ReturnType != typeof(IList<TextRange>))
                {
                    return new GeneralActionViewModel(p, m);
                }
                else if(m.ReturnType == typeof(TextRange))
                {
                    return new TextRangeActionViewModel(p, m);
                }
            }

            return new NotSupportedActionViewModel(p,m);
        }
    }

#pragma warning disable CS0067
    public class CommandHandler : ICommand
    {
        private readonly Action _action;
        private readonly bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
#pragma warning restore CS0067

    /// <summary>
    /// class for keeping the parameterinfo data for view
    /// </summary>
    public class Parameter
    {
        public Type ParamType { get; set; }
        public string Name { get; set; }
        public object ParamValue { get; set; }
        public dynamic ParamEnums { get; set; }
        public override string ToString() => Name;
    }
}
