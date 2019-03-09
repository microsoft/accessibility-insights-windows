using AccessibilityInsights.SharedUx.FileBug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    public class example
    {
        public string name { get; set; }
        public string value { get; set; }
    }


    public class ConnectionControlViewModel : ViewModelBase
    {
        internal Action UpdateSaveButton;
        private List<example> _issueReportingOptions;

        public List<example> IssueReportingOptions { get => _issueReportingOptions;}


        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectionControlViewModel()
        {
            example a = new example();
            a.name = "ashwin";
            a.value = "ash value";

            example b = new example();
            b.name = "avanti";
            b.value = "avi value";
            this._issueReportingOptions = new List<example>() { a, b };
        }
    }
}
