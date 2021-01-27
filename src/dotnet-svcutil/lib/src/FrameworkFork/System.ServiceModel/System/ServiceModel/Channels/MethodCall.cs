// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
