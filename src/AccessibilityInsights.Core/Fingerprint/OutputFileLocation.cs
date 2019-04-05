// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Misc;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using static System.FormattableString;

namespace Axe.Windows.Core.Fingerprint
{
    /// <summary>
    /// Implementation of ILocation to identify an element in an output file
    /// </summary>
    internal class OutputFileLocation : ILocation
    {
        private readonly string _file;
        private readonly int _elementId;
        private readonly Func<ProcessStartInfo, Process> _processStart;

        /// <summary>
        /// The description to show in the UI
        /// </summary>
        public string UserDisplayInfo => Invariant($"Element {_elementId} in file {_file}");

        /// <summary>
        /// The source of the location -- this is the full path to the output file
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// The Id within the source -- this is the string representation of the element ID in the output file
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Compare this ILocation to another
        /// </summary>
        /// <param name="other">The other ILocation being compared</param>
        /// <returns>true iff the two ILocations refer to the same element in the same file</returns>
        public bool Equals(ILocation other)
        {
            OutputFileLocation typedOther = other as OutputFileLocation;

            if (typedOther == null)
                return false;

            // There's a potential false positive here if the same file gets used over and over,
            // but we'll ignroe that case for now
            return _elementId == typedOther._elementId && _file == typedOther._file;
        }

        /// <summary>
        /// Open the location
        /// </summary>
        /// <returns>true iff the location was successfully opened</returns>
        public bool Open()
        {
            try
            {
                if (File.Exists(_file))
                {
                    // Opening to the specific issue is pending. For now, just open the file
                    return (_processStart(new ProcessStartInfo
                    {
                        FileName = _file,
                        Verb = "open",
                    }) != null);
                }
            }
            catch (Exception e)
            {
                // These are the Exceptions documented at https://msdn.microsoft.com/en-us/library/0w4h05yb.aspx
                if (!(e is InvalidOperationException) &&
                    !(e is ArgumentException) &&
                    !(e is ObjectDisposedException) &&
                    !(e is FileNotFoundException) &&
                    !(e is Win32Exception))
                {
                    throw;
                }
            }

            return false;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="file">The file that contains the element</param>
        /// <param name="element">The element in question</param>
        public OutputFileLocation(string file, A11yElement element)
            : this(file, element, (startInfo) => Process.Start(startInfo))
        {
        }

        /// <summary>
        /// internal ctor - allows hooking out the ProcessLauncher for testing
        /// </summary>
        internal OutputFileLocation(string file, A11yElement element, Func<ProcessStartInfo, Process> processLauncher)
        {
            file.ArgumentIsNotTrivialString(nameof(file));
            element.ArgumentIsNotNull(nameof(element));

            try
            {
                _file = Path.GetFullPath(file);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Unable to get full path to file: " + file, nameof(file), e);
            }

            if (!File.Exists(_file))
                throw new ArgumentException("File does not exist: " + _file, nameof(file));

            _elementId = element.UniqueId;
            _processStart = processLauncher;
            Source = _file;
            Id = _elementId.ToString(CultureInfo.InvariantCulture);
        }
    }
}
