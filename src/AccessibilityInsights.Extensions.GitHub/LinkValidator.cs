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
        private static readonly string AlphaNumricPattern = Properties.Resources.AlphaNumricPattern;

        public static bool IsValidGitHubRepoLink(string Link)
        {
            string UserNamePattern = string.Format(CultureInfo.InvariantCulture, Properties.Resources.UserNamePattern, AlphaNumricPattern);
            string RepoNamePattern = string.Format(CultureInfo.InvariantCulture, Properties.Resources.RepoNamePattern, AlphaNumricPattern);
            string LinkPatttern = string.Format(CultureInfo.InvariantCulture, Properties.Resources.LinkPatttern, GitHubLink, UserNamePattern, RepoNamePattern);
            Regex gitHubRepoLinkRegex = new Regex(LinkPatttern, RegexOptions.IgnoreCase);
            if (!gitHubRepoLinkRegex.IsMatch(Link))
            {
                return false;
            }

            string[] parts = Regex.Replace(Link, GitHubLink, "").Split('/');
            if (parts.Length != 2)
            {
                return false;
            }

            string userName = parts[0];
            string repoName = parts[1];
            if (!CheckUserNameLength(userName) || !CheckRepoNameLength(repoName) || !CheckRepoNameSpecialCases(repoName))
            {
                return false;
            }

            return true;
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
