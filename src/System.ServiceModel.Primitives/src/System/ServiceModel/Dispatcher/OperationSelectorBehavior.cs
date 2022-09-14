// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Collections.Generic;
using System.Reflection;

namespace System.ServiceModel.Dispatcher
{
    internal class OperationSelectorBehavior : IContractBehavior
    {
        void IContractBehavior.Validate(ContractDescription description, ServiceEndpoint endpoint)
        {
        }

        void IContractBehavior.AddBindingParameters(ContractDescription description, ServiceEndpoint endpoint, BindingParameterCollection parameters)
        {
        }

        void IContractBehavior.ApplyDispatchBehavior(ContractDescription description, ServiceEndpoint endpoint, DispatchRuntime dispatch)
        {
            if (dispatch.ClientRuntime != null)
            {
                dispatch.ClientRuntime.OperationSelector = new MethodInfoOperationSelector(description, MessageDirection.Output);
            }
        }

        void IContractBehavior.ApplyClientBehavior(ContractDescription description, ServiceEndpoint endpoint, ClientRuntime proxy)
        {
            proxy.OperationSelector = new MethodInfoOperationSelector(description, MessageDirection.Input);
        }

        internal class MethodInfoOperationSelector : IClientOperationSelector
        {
            private Dictionary<IntPtr, string> _operationMap;

            internal MethodInfoOperationSelector(ContractDescription description, MessageDirection directionThatRequiresClientOpSelection)
            {
                _operationMap = new Dictionary<IntPtr, string>();

                for (int i = 0; i < description.Operations.Count; i++)
                {
                    OperationDescription operation = description.Operations[i];
                    if (operation.Messages[0].Direction == directionThatRequiresClientOpSelection)
                    {
                        if (operation.SyncMethod != null)
                        {
                            if (!_operationMap.ContainsKey(operation.SyncMethod.MethodHandle.Value))
                            {
                                _operationMap.Add(operation.SyncMethod.MethodHandle.Value, operation.Name);
                            }
                        }

                        if (operation.BeginMethod != null)
                        {
                            if (!_operationMap.ContainsKey(operation.BeginMethod.MethodHandle.Value))
                            {
                                _operationMap.Add(operation.BeginMethod.MethodHandle.Value, operation.Name);
                                _operationMap.Add(operation.EndMethod.MethodHandle.Value, operation.Name);
                            }
                        }

                        if (operation.TaskMethod != null)
                        {
                            if (!_operationMap.ContainsKey(operation.TaskMethod.MethodHandle.Value))
                            {
                                _operationMap.Add(operation.TaskMethod.MethodHandle.Value, operation.Name);
                            }
                        }
                    }
                }
            }

            public bool AreParametersRequiredForSelection
            {
                get { return false; }
            }

            public string SelectOperation(MethodBase method, object[] parameters)
            {
                if (_operationMap.ContainsKey(method.MethodHandle.Value))
                {
                    return _operationMap[method.MethodHandle.Value];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
