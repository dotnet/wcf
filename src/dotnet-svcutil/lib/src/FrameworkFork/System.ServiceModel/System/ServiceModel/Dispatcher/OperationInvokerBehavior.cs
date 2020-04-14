// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace System.ServiceModel.Dispatcher
{
    public class OperationInvokerBehavior : IOperationBehavior
    {
        public OperationInvokerBehavior()
        {
        }

        void IOperationBehavior.Validate(OperationDescription description)
        {
        }

        void IOperationBehavior.AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
        {
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatch)
        {
            if (dispatch == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("dispatch");
            }
            if (description == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("description");
            }

            if (description.TaskMethod != null)
            {
                dispatch.Invoker = new TaskMethodInvoker(description.TaskMethod, description.TaskTResult);
            }
            else if (description.SyncMethod != null)
            {
                if (description.BeginMethod != null)
                {
                    // both sync and async methods are present on the contract, prefer the Async method. This is a change from desktop.
                    throw new PlatformNotSupportedException();
                }
                else
                {
                    // only sync method is present on the contract
                    dispatch.Invoker = new SyncMethodInvoker(description.SyncMethod);
                }
            }
            else
            {
                if (description.BeginMethod != null)
                {
                    // only async method is present on the contract
                    throw new PlatformNotSupportedException();
                }
            }
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription description, ClientOperation proxy)
        {
        }
    }
}
