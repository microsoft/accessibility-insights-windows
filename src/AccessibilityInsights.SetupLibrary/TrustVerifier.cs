// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using static AccessibilityInsights.Win32.NativeMethods;

namespace AccessibilityInsights.SetupLibrary
{
    public class TrustVerifier : IDisposable
    {
        // This list intentionally excludes the CN, since we are validating only the organization
        private static readonly IReadOnlyCollection<string> TrustedCertIssuerEndings = new List<string>
        {
            ", O=Microsoft Corporation, L=Redmond, S=Washington, C=US",
        };

        // used to keep the file handle open for the lifetime of the object
        // Thus preventing modification after the file has been verified.
        private FileStream _file;

        public bool IsVerified { get; }

        public TrustVerifier(string filePath)
        {
            try
            {
                IsVerified = IsFileTrusted(filePath) && IsFileIssuingOrganizationTrusted(filePath);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private bool IsFileTrusted(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            _file = File.OpenRead(filePath);

            using (var fileInfo = new WinTrustFileInfo(filePath))
            using (var winTrustData = new WinTrustData(fileInfo))
            {
                var result = WinVerifyTrust(IntPtr.Zero, WINTRUST_ACTION_GENERIC_VERIFY_V2, winTrustData);
                return result == WinVerifyTrustResult.Success;
            }
        }

        public static bool IsFileIssuingOrganizationTrusted(string filePath)
        {
            using (X509Certificate cert = X509Certificate.CreateFromSignedFile(filePath))
            {
                string issuer = cert.Issuer;
                foreach (string trustedCertIssuerEnding in TrustedCertIssuerEndings)
                {
                    if (issuer.EndsWith(trustedCertIssuerEnding, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        ~TrustVerifier()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_file != null)
            {
                _file.Dispose();
                _file = null;
            }
        }
    } // class
} // namespace
