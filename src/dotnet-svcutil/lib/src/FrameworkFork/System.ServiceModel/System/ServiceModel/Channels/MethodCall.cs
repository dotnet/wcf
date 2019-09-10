// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Contracts;
using System.Reflection;

namespace System.ServiceModel.Channels
{
    // MethodCall associates a MethodBase with the arguments to pass to it.
    internal class MethodCall
    {
        public MethodCall(object[] args)
        {
            Contract.Assert(args != null);
            Args = args;
        }

        public MethodCall(MethodBase methodBase, object[] args) : this(args)
        {
            Contract.Assert(methodBase != null);
            MethodBase = methodBase;
        }

        public MethodBase MethodBase { get; private set; }

        public object[] Args { get; private set; }
    }
}
