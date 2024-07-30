// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// 

#if NET
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Description;
using CoreWCF.Dispatcher;
#else
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
#endif
using System.Collections.ObjectModel;
using System.Text;

namespace WcfService
{
    public class RequestLoggingServiceBehavior : IServiceBehavior
    {
        /* This service behavior allows adding logging to a service at the earliest possible point. This
           is done by using a custom ServiceAuthorizationManager. This behavior chains any existing authorization
           manager to preserve the authorization mechanism in use. It can be used in an EndpointResource by overriding
           the ModifyHost method. It is best to add the behavior after base.ModifyHost has been called otherwise
           any other ServiceAuthorizationManager that gets installed could replace this.
           Sample usage:
               protected override void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
               {
                   base.ModifyHost(serviceHost, context);
                   var loggingBehavior = new RequestLoggingServiceBehavior(RequestLoggingServiceBehavior.SimpleConsoleLogger);
                   serviceHost.Description.Behaviors.Add(loggingBehavior);
               }

           The constructor takes a delegate of type RequestLoggingServiceBehavior.LogRequest. There is a simple static logger that can
           be used that's available as RequestLoggingServiceBehavior.SimpleConsoleLogger and is used in the example above. As the logger
           is a delegate, you can add multiple loggers and they will be called one after the other. To add more loggers, use the AddLogger
           method.
           */
        private LogRequest _logCallback;

        public delegate void LogRequest(OperationContext operationContext, ref Message message, bool authenticated);
        public RequestLoggingServiceBehavior() { }

        public RequestLoggingServiceBehavior(LogRequest logger)
        {
            _logCallback = logger;
        }

        public void AddLogger(LogRequest logger)
        {
            _logCallback += logger;
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            serviceHostBase.Authorization.ServiceAuthorizationManager =
                new RequestLoggingAuthenticationWrapper(this, serviceHostBase.Authorization.ServiceAuthorizationManager);
            foreach (ChannelDispatcherBase t in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = t as ChannelDispatcher;
                foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                {
                    endpointDispatcher.DispatchRuntime.ServiceAuthorizationManager =
                        new RequestLoggingAuthenticationWrapper(this,
                            endpointDispatcher.DispatchRuntime.ServiceAuthorizationManager);
                }
            }
        }

        private void LogIncomingRequest(OperationContext operationContext, ref Message message, bool authenticated)
        {
            try
            {
                if (_logCallback != null)
                {
                    _logCallback(operationContext, ref message, authenticated);
                }
            }
            catch { /* Swallow any exceptions */ }
        }

        public static void SimpleConsoleLogger(OperationContext operationContext, ref Message message, bool authenticated)
        {
            var log = new StringBuilder();
            log.Append("Via:");
            object requestMessagePropertyObject;
            if (operationContext.IncomingMessageProperties.TryGetValue(HttpRequestMessageProperty.Name,
                out requestMessagePropertyObject))
            {
                var requestMessageProperty = requestMessagePropertyObject as HttpRequestMessageProperty;
                if (requestMessageProperty != null)
                {
                    log.Append(requestMessageProperty.Method).Append(' ');
                }
            }

            log.AppendLine(message.Properties.Via.ToString());
            log.Append("To Header:").AppendLine(message.Headers.Action);
            log.Append("Authentication ").AppendLine(authenticated ? "succeeded" : "failed");
            Console.WriteLine(log.ToString());
        }

        private class RequestLoggingAuthenticationWrapper : ServiceAuthorizationManager
        {
            private RequestLoggingServiceBehavior _requestLogger;
            private ServiceAuthorizationManager _parent;

            public RequestLoggingAuthenticationWrapper(RequestLoggingServiceBehavior requestLogger, ServiceAuthorizationManager parent)
            {
                _requestLogger = requestLogger;
                _parent = parent;
            }

            [Obsolete]
            public override bool CheckAccess(OperationContext operationContext, ref Message message)
            {
                bool success = false;
                try
                {
                    if (_parent != null)
                    {
                        success = _parent.CheckAccess(operationContext);
                    }
                    else
                    {
                        success = true;
                    }
                    return success;
                }
                finally
                {
                    _requestLogger.LogIncomingRequest(operationContext, ref message, success);
                }
            }
        }
    }
}
