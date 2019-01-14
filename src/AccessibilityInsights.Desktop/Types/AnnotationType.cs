// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Types;
using System.Text;

using static System.FormattableString;

namespace AccessibilityInsights.Desktop.Types
{
    /// <summary>
    /// Annotation Types
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/hh448743(v=vs.85).aspx
    /// </summary>
    public class AnnotationType : TypeBase
    {
        const string Prefix = "AnnotationType_";

#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int AnnotationType_Unknown = 60000;
        public const int AnnotationType_SpellingError = 60001;
        public const int AnnotationType_GrammarError = 60002;
        public const int AnnotationType_Comment = 60003;
        public const int AnnotationType_FormulaError = 60004;
        public const int AnnotationType_TrackChanges = 60005;
        public const int AnnotationType_Header = 60006;
        public const int AnnotationType_Footer = 60007;
        public const int AnnotationType_Highlighted = 60008;
        public const int AnnotationType_Endnote = 60009;
        public const int AnnotationType_Footnote = 60010;
        public const int AnnotationType_InsertionChange = 60011;
        public const int AnnotationType_DeletionChange = 60012;
        public const int AnnotationType_MoveChange = 60013;
        public const int AnnotationType_FormatChange = 60014;
        public const int AnnotationType_UnsyncedChange = 60015;
        public const int AnnotationType_EditingLockedChange = 60016;
        public const int AnnotationType_ExternalChange = 60017;
        public const int AnnotationType_ConflictingChange = 60018;
        public const int AnnotationType_Author = 60019;
        public const int AnnotationType_AdvancedProofingIssue = 60020;
        public const int AnnotationType_DataValidationError = 60021;
        public const int AnnotationType_CircularReferenceError = 60022;
        public const int AnnotationType_Mathematics = 60023;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static AnnotationType sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static AnnotationType GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new AnnotationType();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private AnnotationType() : base(Prefix) { }

        /// <summary>
        /// change name into right format in dictionary and list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string GetNameInProperFormat(string name, int id)
        {
            StringBuilder sb = new StringBuilder(name);

            sb.Replace(Prefix, "");
            sb.Append(Invariant($" ({id})"));

            return sb.ToString();
        }
    }
}
