// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.CodeDom.Compiler
{
    using System;

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class GeneratedCodeAttribute : Attribute
    {
        private readonly string _tool;
        private readonly string _version;

        public GeneratedCodeAttribute(string tool, string version)
        {
            _tool = tool;
            _version = version;
        }

        public string Tool
        {
            get { return _tool; }
        }

        public string Version
        {
            get { return _version; }
        }
    }
}
