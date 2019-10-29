// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime;
using System.ServiceModel.Description;

namespace System.ServiceModel.Channels
{
    // MethodCall associates a MethodBase with the arguments to pass to it.
    internal class MethodCall
    {
        private object[] _inArgs;

        public MethodCall(object[] args)
        {
            Contract.Assert(args != null);
            Args = args;
        }

        public MethodCall(MethodBase methodBase, object[] args) : this(args)
        {
            Contract.Assert(methodBase != null);
            MethodBase = methodBase;
            CreateInArgs();
        }

        public MethodBase MethodBase { get; private set; }

        public object[] Args { get; private set; }

        public object[] InArgs => _inArgs ?? Args;

        private void CreateInArgs()
        {
            var parameters = MethodBase.GetParameters();
            int inCount = 0;
            foreach(var param in parameters)
            {
                if (ServiceReflector.FlowsIn(param))
                {
                    inCount++;
                }
            }

            if (inCount == Args.Length) // All parameters are InArgs so do nothing and fallback to returning Args
            {
                return;
            }

            _inArgs = new object[inCount];
            int inPos = 0;
            for(int argPos = 0; argPos < parameters.Length; argPos++)
            {
                if (ServiceReflector.FlowsIn(parameters[argPos]))
                {
                    _inArgs[inPos] = Args[argPos];
                    inPos++;
                }
            }

            Fx.Assert((inPos - 1) != (inCount), $"Incorrect number of arguments put into _inArgs array, expected {inCount} and copied {inPos - 1}");
        }
    }
}
