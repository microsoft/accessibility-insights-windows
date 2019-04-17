// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Results;
using Axe.Windows.Core.Types;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Axe.Windows.Desktop.Misc
{
    /// <summary>
    /// Class to access stored link information
    /// </summary>
    public class StandardLinksHelper
    {
        /// <summary>
        /// Dictionary of stored links
        /// </summary>
        private readonly IReadOnlyDictionary<string, string> StoredLinks;

        /// <summary>
        /// Constructor
        /// </summary>
        StandardLinksHelper()
        {
            // get the path of dictionary file. 
            var json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "links.json"));
            StoredLinks = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        /// <summary>
        /// Is there a stored link for the given scan 
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public bool HasStoredLink(ScanMetaInfo mi)
        {
            return StoredLinks.ContainsKey($"{mi.UIFramework}-{mi.ControlType}-{PropertyType.GetInstance().GetNameById(mi.PropertyId)}");
        }

#pragma warning disable CA1055 // Uri return values should not be strings
        /// <summary>
        /// Get CELA Snippet Query Url
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public string GetSnippetQueryUrl(ScanMetaInfo mi)
#pragma warning restore CA1055 // Uri return values should not be strings
        {
            return StoredLinks[$"{mi.UIFramework}-{mi.ControlType}-{PropertyType.GetInstance().GetNameById(mi.PropertyId)}"];
        }

        #region static members
        /// <summary>
        /// Default link helper
        /// </summary>
        static StandardLinksHelper sDefaultInstance = null;

        /// <summary>
        /// Get the default instance of StandardLinksHelper
        /// </summary>
        /// <returns></returns>
        public static StandardLinksHelper GetDefaultInstance()
        {
            if (sDefaultInstance == null)
            {
                sDefaultInstance = new StandardLinksHelper();
            }

            return sDefaultInstance;
        }
        #endregion
    }
}
