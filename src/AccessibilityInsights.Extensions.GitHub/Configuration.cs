using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    public class Configuration
    {
        public string RepoLink {get;}

        public Configuration()
        {
            this.RepoLink = String.Empty;
        }

        public Configuration(string repoLink)
        {
            this.RepoLink = repoLink;
        }
    }
}
