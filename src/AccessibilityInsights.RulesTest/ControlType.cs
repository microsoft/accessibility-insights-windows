// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Axe.Windows.RulesTest
{
    static class ControlType
    {
        public const int AppBar = 50040;
        public const int Button = 50000;
        public const int Calendar = 50001;
        public const int CheckBox = 50002;
        public const int ComboBox = 50003;
        public const int Custom = 50025;
        public const int DataGrid = 50028;
        public const int DataItem = 50029;
        public const int Document = 50030;
        public const int Edit = 50004;
        public const int Group = 50026;
        public const int Header = 50034;
        public const int HeaderItem = 50035;
        public const int Hyperlink = 50005;
        public const int Image = 50006;
        public const int List = 50008;
        public const int ListItem = 50007;
        public const int Menu = 50009;
        public const int MenuBar = 50010;
        public const int MenuItem = 50011;
        public const int Pane = 50033;
        public const int ProgressBar = 50012;
        public const int RadioButton = 50013;
        public const int ScrollBar = 50014;
        public const int SemanticZoom = 50039;
        public const int Separator = 50038;
        public const int Slider = 50015;
        public const int Spinner = 50016;
        public const int SplitButton = 50031;
        public const int StatusBar = 50017;
        public const int Tab = 50018;
        public const int TabItem = 50019;
        public const int Table = 50036;
        public const int Text = 50020;
        public const int Thumb = 50027;
        public const int TitleBar = 50037;
        public const int ToolBar = 50021;
        public const int ToolTip = 50022;
        public const int Tree = 50023;
        public const int TreeItem = 50024;
        public const int Window = 50032;

        public static IEnumerable<int> All = CreateListOfAll();

        private static IEnumerable<int> CreateListOfAll()
        {
            var type = typeof(ControlType);
            var fields = type.GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static);

            return from field in fields
                   where field.FieldType == typeof(int)
                   select (int)field.GetRawConstantValue();
        }
    } // class
} // namespace
