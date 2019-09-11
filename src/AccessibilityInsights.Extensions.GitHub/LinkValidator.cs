// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AccessibilityInsights.Extensions.GitHub
{
    /// <summary>
    /// GitHub Repo Link Format Validator
    /// </summary>
    public static class LinkValidator
    {
        private static readonly string GitHubLink = Properties.Resources.GitHubLink;
        private static readonly string AlphaNumericPattern = Properties.Resources.AlphaNumricPattern;

        public static bool IsValidGitHubRepoLink(string link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));

            link = link.Replace(@"\", "/").Trim(' ').TrimEnd('/');
            string userNamePattern = string.Format(CultureInfo.InvariantCulture, Properties.Resources.UserNamePattern, AlphaNumericPattern);
            string repoNamePattern = string.Format(CultureInfo.InvariantCulture, Properties.Resources.RepoNamePattern, AlphaNumericPattern);
            string linkPattern = string.Format(CultureInfo.InvariantCulture, Properties.Resources.LinkPatttern, GitHubLink, userNamePattern, repoNamePattern);
            Regex gitHubRepoLinkRegex = new Regex(linkPattern, RegexOptions.IgnoreCase);
            if (!gitHubRepoLinkRegex.IsMatch(link))
            {
                return false;
            }

            link = Regex.Replace(link, GitHubLink, "");
            string[] parts = Regex.Split(link, @"/+");
            if (parts.Length != 2)
            {
                return false;
            }

            string userName = parts[0];
            string repoName = parts[1];
            if (!CheckUserNameLength(userName) || !CheckUserNameAtLeastOneChar(userName) || !CheckRepoNameLength(repoName) || !CheckRepoNameSpecialCases(repoName))
            {
                return false;
            }

            return true;
        }

        private static bool CheckUserNameAtLeastOneChar(string userName)
        {
            foreach (char c in userName)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CheckUserNameLength(string userName)
        {
            return (userName.Length <= 39 && userName.Length >= 1);
        }

        private static bool CheckRepoNameLength(string repoName)
        {
            return (repoName.Length <= 100 && repoName.Length >= 1);
        }

        private static bool CheckRepoNameSpecialCases(string repoName)
        {
            if (repoName.Equals(Properties.Resources.RepoNameSpecialCasesDoubleDot, StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

    }
}
