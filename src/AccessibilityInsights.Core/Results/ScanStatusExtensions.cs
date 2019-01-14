// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.CodeAnalysis.Sarif;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using static System.FormattableString;

namespace AccessibilityInsights.Core.Results
{
    public static class ScanStatusExtensions
    {
        private readonly static IReadOnlyDictionary<ScanStatus, Tuple <ResultLevel, string>> ScanStatusToResultLevelMappings = new ReadOnlyDictionary<ScanStatus, Tuple<ResultLevel, string>>(
                new Dictionary<ScanStatus, Tuple<ResultLevel, string>>()
                {
                    { ScanStatus.Fail, new Tuple<ResultLevel, string>(ResultLevel.Error, "error") },
                    { ScanStatus.Pass,new Tuple<ResultLevel, string>(ResultLevel.Pass, "pass") },
                    { ScanStatus.ScanNotSupported, new Tuple<ResultLevel, string>(ResultLevel.Note, "note") },
                    { ScanStatus.Uncertain, new Tuple<ResultLevel, string>(ResultLevel.Open, "open") },
                    { ScanStatus.NoResult, new Tuple<ResultLevel, string>(ResultLevel.Open, "open") },
                }
            );

        /// <summary>
        ///  Maps ScanStatus (AccessibilityInsights) result levels to ResultLevel (Sarif) standard result levels. Throws an exception if a mapping does not exist.
        /// </summary>
        /// <param name="status"> AccessibilityInsights scan status </param>
        /// <returns> Returns corresponding ResultLevel </returns>
        public static ResultLevel ToResultLevel(this ScanStatus status)
        {
            if (!ScanStatusToResultLevelMappings.TryGetValue(status, out Tuple<ResultLevel, string> result))
            {
                throw new ArgumentException(Invariant($"The ScanStatus {status} does not have a corresponding mapping"), nameof(status));
            }

            return result.Item1;
        }

        /// <summary>
        ///  Maps ScanStatus (AccessibilityInsights) result levels to ResultLevel (Sarif) standard result levels in the string format. Throws an exception if a mapping does not exist.
        /// </summary>
        /// <param name="status"> AccessibilityInsights scan status </param>
        /// <returns> Returns corresponding ResultLevel </returns>
        public static string ToResultLevelString(this ScanStatus status)
        {
            if (!ScanStatusToResultLevelMappings.TryGetValue(status, out Tuple<ResultLevel, string> result))
            {
                throw new ArgumentException(Invariant($"The ScanStatus {status} does not have a corresponding mapping"), nameof(status));
            }

            return result.Item2;
        }
    }
}
