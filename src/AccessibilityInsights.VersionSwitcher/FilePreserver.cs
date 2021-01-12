// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AccessibilityInsights.VersionSwitcher
{
    /// <summary>
    /// A class to preserve files in a folder. It preserves files only in the specified
    /// path, ignoring any subfolders. The folder will be recreated on restore, if needed.
    /// </summary>
    internal class FilePreserver : IDisposable
    {
        private bool disposedValue;

        private readonly string _pathToPreserve;
        private bool _pathExisted;
        private Dictionary<string, string> _filesToPreserve = new Dictionary<string, string>();

        internal FilePreserver(string pathToPreserve)
        {
            _pathToPreserve = pathToPreserve;
        }

        internal void PreserveFiles()
        {
            _pathExisted = Directory.Exists(_pathToPreserve);
            if (_pathExisted)
            {
                StringBuilder sb = new StringBuilder();

                foreach (string fileName in Directory.EnumerateFiles(_pathToPreserve))
                {
                    _filesToPreserve.Add(fileName, File.ReadAllText(fileName, Encoding.UTF8));
                }

                LogFiles("Preserved");
            }
        }

        private void RestorePreservedFiles()
        {
            if (!_pathExisted)
                return;

            FileHelpers.CreateFolder(_pathToPreserve);
            foreach (KeyValuePair<string, string> pair in _filesToPreserve)
            {
                File.WriteAllText(pair.Key, pair.Value, Encoding.UTF8);  // Will overwrite if necessary
            }

            LogFiles("Restored");

            _filesToPreserve.Clear();
            _pathExisted = false;
        }

        private void LogFiles(string action)
        {
            StringBuilder sb = new StringBuilder(action);
            sb.AppendFormat(" {0} files:\n", _filesToPreserve.Count);
            foreach(KeyValuePair<string, string> pair in _filesToPreserve)
            {
                sb.AppendFormat("  {0} ({1} bytes)\n", pair.Key, pair.Value.Length);
            }
            EventLogger.WriteInformationalMessage(sb.ToString());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    RestorePreservedFiles();
                }

                disposedValue = true;
            }
        }

        ~FilePreserver()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
