// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Results;

namespace AccessibilityInsights.Core.Fingerprint
{
    public static class FingerprintFactory
    {
        public static IFingerprint GetFingerPrint(A11yElement element, RuleId ruleId, ScanStatus status)
        {
            // Very simplistic implementation of the factory. If we decide to add a few more generators will need a switch case and type enum.
            return new ScanResultFingerprint(element, ruleId, status);
        }
    }
}
