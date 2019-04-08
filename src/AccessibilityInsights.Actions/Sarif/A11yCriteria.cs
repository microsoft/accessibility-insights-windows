// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.CodeAnalysis.Sarif;

namespace Axe.Windows.Actions.Sarif
{
    class A11yCriteria
    {
        public Message standardName { get; }
        public string requirementId { get; }
        public Message requirementName { get; }
        public string requirementUri { get; }
        public string standardKey { get; }

        public A11yCriteria(string standardKey, Message standardName, string requirementId, Message requirementName, string requirementUri)
        {
            this.standardName = standardName;
            this.requirementId = requirementId;
            this.requirementName = requirementName;
            this.requirementUri = requirementUri;
            this.standardKey = standardKey;
        }
    }
}
