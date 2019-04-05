// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Desktop.Styles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Axe.Windows.Desktop.Types
{
    /// <summary>
    /// TextAttributeTypes
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ee671662(v=vs.85).aspx
    /// </summary>
    public class TextAttributeType : TypeBase
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public const int UIA_TextAttributeId = 0;

        public const int UIA_AnimationStyleAttributeId = 40000;
        public const int UIA_BackgroundColorAttributeId = 40001;
        public const int UIA_BulletStyleAttributeId = 40002;
        public const int UIA_CapStyleAttributeId = 40003;
        public const int UIA_CultureAttributeId = 40004;
        public const int UIA_FontNameAttributeId = 40005;
        public const int UIA_FontSizeAttributeId = 40006;
        public const int UIA_FontWeightAttributeId = 40007;
        public const int UIA_ForegroundColorAttributeId = 40008;
        public const int UIA_HorizontalTextAlignmentAttributeId = 40009;
        public const int UIA_IndentationFirstLineAttributeId = 40010;
        public const int UIA_IndentationLeadingAttributeId = 40011;
        public const int UIA_IndentationTrailingAttributeId = 40012;
        public const int UIA_IsHiddenAttributeId = 40013;
        public const int UIA_IsItalicAttributeId = 40014;
        public const int UIA_IsReadOnlyAttributeId = 40015;
        public const int UIA_IsSubscriptAttributeId = 40016;
        public const int UIA_IsSuperscriptAttributeId = 40017;
        public const int UIA_MarginBottomAttributeId = 40018;
        public const int UIA_MarginLeadingAttributeId = 40019;
        public const int UIA_MarginTopAttributeId = 40020;
        public const int UIA_MarginTrailingAttributeId = 40021;
        public const int UIA_OutlineStylesAttributeId = 40022;
        public const int UIA_OverlineColorAttributeId = 40023;
        public const int UIA_OverlineStyleAttributeId = 40024;
        public const int UIA_StrikethroughColorAttributeId = 40025;
        public const int UIA_StrikethroughStyleAttributeId = 40026;
        public const int UIA_TabsAttributeId = 40027;
        public const int UIA_TextFlowDirectionsAttributeId = 40028;
        public const int UIA_UnderlineColorAttributeId = 40029;
        public const int UIA_UnderlineStyleAttributeId = 40030;
        public const int UIA_AnnotationTypesAttributeId = 40031;
        public const int UIA_AnnotationObjectsAttributeId = 40032;
        public const int UIA_StyleNameAttributeId = 40033;
        public const int UIA_StyleIdAttributeId = 40034;
        public const int UIA_LinkAttributeId = 40035;
        public const int UIA_IsActiveAttributeId = 40036;
        public const int UIA_SelectionActiveEndAttributeId = 40037;
        public const int UIA_CaretPositionAttributeId = 40038;
        public const int UIA_CaretBidiModeAttributeId = 40039;
        public const int UIA_LineSpacingAttributeId = 40040;
        public const int UIA_BeforeParagraphSpacingAttributeId = 40041;
        public const int UIA_AfterParagraphSpacingAttributeId = 40042;
        public const int UIA_SayAsInterpretAsAttributeId = 40043;
#pragma warning restore CA1707 // Identifiers should not contain underscores

        private static TextAttributeType sInstance;

        /// <summary>
        /// static method to get an instance of this class
        /// singleton
        /// </summary>
        /// <returns></returns>
        public static TextAttributeType GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new TextAttributeType();
            }

            return sInstance;
        }

        /// <summary>
        /// private constructor since it would be singleton model
        /// </summary>
        private TextAttributeType() : base() { }

        /// <summary>
        /// change name into right format in dictionary and list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string GetNameInProperFormat(string name, int id)
        {
            StringBuilder sb = new StringBuilder(name);

            sb.Replace("UIA_", "");
            sb.Replace("AttributeId", "");

            return sb.ToString();
        }

        /// <summary>
        /// Get template of each attribute type
        /// it is used in TextRangeFindDialog.
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, string, dynamic, Type>> GetTemplate()
        {
            var boolList = new List<KeyValuePair<bool, string>>() { new KeyValuePair<bool, string>(false, "False"), new KeyValuePair<bool, string>(true, "True") };
            var list = new List<Tuple<int, string, dynamic, Type>>
            {
                new Tuple<int, string, dynamic, Type>(UIA_TextAttributeId, GetNameById(UIA_TextAttributeId), null, typeof(string)),
                new Tuple<int, string, dynamic, Type>(UIA_AnimationStyleAttributeId, GetNameById(UIA_AnimationStyleAttributeId), AnimationStyle.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_BackgroundColorAttributeId, GetNameById(UIA_BackgroundColorAttributeId), null, typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_BulletStyleAttributeId, GetNameById(UIA_BulletStyleAttributeId), BulletStyle.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_CapStyleAttributeId, GetNameById(UIA_CapStyleAttributeId), CapStyle.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_CultureAttributeId, GetNameById(UIA_CultureAttributeId), CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).Select(c => new KeyValuePair<int, string>(c.LCID, c.EnglishName)).ToList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_FontNameAttributeId, GetNameById(UIA_FontNameAttributeId), null, typeof(string)),
                new Tuple<int, string, dynamic, Type>(UIA_FontSizeAttributeId, GetNameById(UIA_FontSizeAttributeId), null, typeof(double)),
                new Tuple<int, string, dynamic, Type>(UIA_FontWeightAttributeId, GetNameById(UIA_FontWeightAttributeId), FontWeight.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_ForegroundColorAttributeId, GetNameById(UIA_ForegroundColorAttributeId), null, typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_HorizontalTextAlignmentAttributeId, GetNameById(UIA_HorizontalTextAlignmentAttributeId), HorizontalTextAlignment.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_IndentationFirstLineAttributeId, GetNameById(UIA_IndentationFirstLineAttributeId), null, typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_IndentationLeadingAttributeId, GetNameById(UIA_IndentationLeadingAttributeId), null, typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_IndentationTrailingAttributeId, GetNameById(UIA_IndentationTrailingAttributeId), null, typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_IsHiddenAttributeId, GetNameById(UIA_IsHiddenAttributeId), boolList, typeof(bool)),
                new Tuple<int, string, dynamic, Type>(UIA_IsItalicAttributeId, GetNameById(UIA_IsItalicAttributeId), boolList, typeof(bool)),
                new Tuple<int, string, dynamic, Type>(UIA_IsReadOnlyAttributeId, GetNameById(UIA_IsReadOnlyAttributeId), boolList, typeof(bool)),
                new Tuple<int, string, dynamic, Type>(UIA_IsSubscriptAttributeId, GetNameById(UIA_IsSubscriptAttributeId), boolList, typeof(bool)),
                new Tuple<int, string, dynamic, Type>(UIA_IsSuperscriptAttributeId, GetNameById(UIA_IsSuperscriptAttributeId), boolList, typeof(bool)),
                new Tuple<int, string, dynamic, Type>(UIA_MarginBottomAttributeId, GetNameById(UIA_MarginBottomAttributeId), null, typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_MarginLeadingAttributeId, GetNameById(UIA_MarginLeadingAttributeId), null, typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_MarginTopAttributeId, GetNameById(UIA_MarginTopAttributeId), null, typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_MarginTrailingAttributeId, GetNameById(UIA_MarginTrailingAttributeId), null, typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_OutlineStylesAttributeId, GetNameById(UIA_OutlineStylesAttributeId), OutlineStyle.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_OverlineColorAttributeId, GetNameById(UIA_OverlineColorAttributeId), null, typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_OverlineStyleAttributeId, GetNameById(UIA_OverlineStyleAttributeId), TextDecorationLineStyle.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_StrikethroughColorAttributeId, GetNameById(UIA_StrikethroughColorAttributeId), null, typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_StrikethroughStyleAttributeId, GetNameById(UIA_StrikethroughStyleAttributeId), TextDecorationLineStyle.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_TabsAttributeId, GetNameById(UIA_TabsAttributeId), null, typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_TextFlowDirectionsAttributeId, GetNameById(UIA_TextFlowDirectionsAttributeId), FlowDirection.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_UnderlineColorAttributeId, GetNameById(UIA_UnderlineColorAttributeId), null, typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_UnderlineStyleAttributeId, GetNameById(UIA_UnderlineStyleAttributeId), TextDecorationLineStyle.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_AnnotationTypesAttributeId, GetNameById(UIA_AnnotationTypesAttributeId), AnnotationType.GetInstance().GetKeyValuePairList(), typeof(int)),
                //new Tuple<int, string, dynamic, Type>(UIA_AnnotationObjectsAttributeId, GetNameById(UIA_AnnotationObjectsAttributeId), AnimationStyles.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_StyleNameAttributeId, GetNameById(UIA_StyleNameAttributeId), null, typeof(string)),
                new Tuple<int, string, dynamic, Type>(UIA_StyleIdAttributeId, GetNameById(UIA_StyleIdAttributeId), null, typeof(int)),
                //new Tuple<int, string, dynamic, Type>(UIA_LinkAttributeId, GetNameById(UIA_LinkAttributeId), AnimationStyles.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_IsActiveAttributeId, GetNameById(UIA_IsActiveAttributeId), boolList, typeof(bool)),
                new Tuple<int, string, dynamic, Type>(UIA_SelectionActiveEndAttributeId, GetNameById(UIA_SelectionActiveEndAttributeId), ActiveEnd.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_CaretPositionAttributeId, GetNameById(UIA_CaretPositionAttributeId), CaretPosition.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_CaretBidiModeAttributeId, GetNameById(UIA_CaretBidiModeAttributeId), CaretBidiMode.GetInstance().GetKeyValuePairList(), typeof(int)),
                new Tuple<int, string, dynamic, Type>(UIA_LineSpacingAttributeId, GetNameById(UIA_LineSpacingAttributeId), null, typeof(string)),
                new Tuple<int, string, dynamic, Type>(UIA_BeforeParagraphSpacingAttributeId, GetNameById(UIA_BeforeParagraphSpacingAttributeId), null, typeof(double)),
                new Tuple<int, string, dynamic, Type>(UIA_AfterParagraphSpacingAttributeId, GetNameById(UIA_AfterParagraphSpacingAttributeId), null, typeof(double)),
                new Tuple<int, string, dynamic, Type>(UIA_SayAsInterpretAsAttributeId, GetNameById(UIA_SayAsInterpretAsAttributeId), SayAsInterpretAs.GetInstance().GetKeyValuePairList(), typeof(int)),
            };
            return list;
        }
    }
}
