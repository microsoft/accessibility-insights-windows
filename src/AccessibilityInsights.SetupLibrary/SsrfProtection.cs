// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Provides protection against Server-Side Request Forgery (SSRF) attacks
    /// by validating URLs and DNS resolutions against blocked IP ranges
    /// </summary>
    public static class SsrfProtection
    {
        private static readonly string[] BlockedIPs = new[]
        {
            "168.63.129.16",      // Azure WireServer
            "169.254.169.254",    // Azure IMDS
            "0.0.0.0",
            "127.0.0.1",          // localhost
            "::1",                // IPv6 localhost
            "localhost",
            "0177.0.23.19",       // localhost in octal form
            "2130706433",         // localhost as a 32 bit integer
        };

        private static readonly (string network, string mask)[] BlockedCIDRs = new[]
        {
            ("127.0.0.0", "255.0.0.0"),        // localhost range (127.0.0.0/8)
            ("10.0.0.0", "255.0.0.0"),         // Private (10.0.0.0/8)
            ("172.16.0.0", "255.240.0.0"),     // Private (172.16.0.0/12)
            ("192.168.0.0", "255.255.0.0"),    // Private (192.168.0.0/16)
            ("100.64.0.0", "255.192.0.0"),     // Shared address space (100.64.0.0/10)
            ("169.254.0.0", "255.255.0.0"),    // Link-local (169.254.0.0/16)
            ("192.0.0.0", "255.255.255.0"),    // IETF Protocol Assignments (192.0.0.0/24)
            ("192.0.2.0", "255.255.255.0"),    // TEST-NET-1 (192.0.2.0/24)
            ("192.88.99.0", "255.255.255.0"),  // IPv6 to IPv4 relay (192.88.99.0/24)
            ("198.51.100.0", "255.255.255.0"), // TEST-NET-2 (198.51.100.0/24)
            ("203.0.113.0", "255.255.255.0"),  // TEST-NET-3 (203.0.113.0/24)
            ("224.0.0.0", "240.0.0.0"),        // Multicast (224.0.0.0/4)
            ("240.0.0.0", "240.0.0.0"),        // Reserved (240.0.0.0/4)
            ("25.0.0.0", "255.0.0.0"),         // Reserved (25.0.0.0/8)
        };

        /// <summary>
        /// Validates that a URL is safe to use by checking DNS resolution against blocked IP ranges
        /// </summary>
        /// <param name="uri">The URI to validate</param>
        /// <returns>True if the URL is safe, false if it resolves to a blocked IP or range</returns>
        public static bool IsUrlSafe(Uri uri)
        {
            if (uri == null)
                return false;

            // Check for blocked hostnames
            string host = uri.Host.ToLowerInvariant();
            if (BlockedIPs.Any(blocked => blocked.Equals(host, StringComparison.OrdinalIgnoreCase)))
                return false;

            // Perform DNS resolution
            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(uri.Host);

                foreach (var address in addresses)
                {
                    // Check exact match against blocked IPs
                    if (BlockedIPs.Contains(address.ToString()))
                        return false;

                    // Check against CIDR ranges
                    if (IsInBlockedRange(address))
                        return false;
                }

                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
            {
                // DNS resolution failed - treat as unsafe
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Checks if an IP address falls within any of the blocked CIDR ranges
        /// </summary>
        /// <param name="address">The IP address to check</param>
        /// <returns>True if the address is in a blocked range</returns>
        private static bool IsInBlockedRange(IPAddress address)
        {
            // Only check IPv4 addresses
            if (address.AddressFamily != AddressFamily.InterNetwork)
            {
                // For IPv6, check if it's localhost or private
                if (address.IsIPv6LinkLocal || address.IsIPv6SiteLocal)
                    return true;

                // Check for IPv6 localhost (::1)
                if (IPAddress.IsLoopback(address))
                    return true;

                // Check for IPv6 private ranges (fc00::/7)
                byte[] bytes = address.GetAddressBytes();
                if (bytes.Length == 16 && (bytes[0] & 0xfe) == 0xfc)
                    return true;

                return false;
            }

            byte[] addressBytes = address.GetAddressBytes();

            foreach (var (network, mask) in BlockedCIDRs)
            {
                var networkBytes = IPAddress.Parse(network).GetAddressBytes();
                var maskBytes = IPAddress.Parse(mask).GetAddressBytes();

                bool inRange = true;
                for (int i = 0; i < 4; i++)
                {
                    if ((addressBytes[i] & maskBytes[i]) != (networkBytes[i] & maskBytes[i]))
                    {
                        inRange = false;
                        break;
                    }
                }

                if (inRange)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Validates a URL string before use
        /// </summary>
        /// <param name="url">The URL string to validate</param>
        /// <param name="validatedUri">The validated URI if successful</param>
        /// <returns>True if the URL is safe, false otherwise</returns>
        public static bool TryValidateUrl(string url, out Uri validatedUri)
        {
            validatedUri = null;

            if (string.IsNullOrWhiteSpace(url))
                return false;

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                return false;

            if (!IsUrlSafe(uri))
                return false;

            validatedUri = uri;
            return true;
        }
    }
}
