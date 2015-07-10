// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge.Tests.Commands
{
    public class TestCommand
    {
        public const string Value = "Test";
        public string Execute()
        {
            return Value;
        }
    }
}
