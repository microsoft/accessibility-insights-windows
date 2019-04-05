// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Axe.Windows.Rules.PropertyConditions
{
    static class EdgeConditions
    {
        public static Condition InsideEdge = StringProperties.ProcessName.Is("MicrosoftEdgeCP");
        public static Condition NotInsideEdge = ~InsideEdge;
    } // class
} // namespace
