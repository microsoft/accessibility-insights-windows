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

        public static bool IsValidGitHubRepoLink(string Link)
        {
            Link = Link.Replace(@"\", "/").Trim(' ').TrimEnd('/');
            string UserNamePattern = string.Format(CultureInfo.InvariantCulture, Properties.Resources.UserNamePattern, AlphaNumericPattern);
            string RepoNamePattern = string.Format(CultureInfo.InvariantCulture, Properties.Resources.RepoNamePattern, AlphaNumericPattern);
            string LinkPattern = string.Format(CultureInfo.InvariantCulture, Properties.Resources.LinkPatttern, GitHubLink, UserNamePattern, RepoNamePattern);
            Regex gitHubRepoLinkRegex = new Regex(LinkPattern, RegexOptions.IgnoreCase);
            if (!gitHubRepoLinkRegex.IsMatch(Link))
            {
                return false;
            }

            Link = Regex.Replace(Link, GitHubLink, "");
            string[] parts = Regex.Split(Link, @"/+");
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
            if (repoName.Equals(Properties.Resources.RepoNameSpecialCasesDoubleDot, StringComparison.InvariantCulture))
            {
                return false;
            }

            return true;
        }

    }
}
