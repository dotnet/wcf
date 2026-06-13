// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Collections.Generic;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace System.ServiceModel.Description
{
    public class WebHttpBehavior : IEndpointBehavior
    {
        internal const string GET = "GET";
        internal const string POST = "POST";
        internal const string WildcardAction = "*";
        internal const string WildcardMethod = "*";
        internal static readonly string s_defaultStreamContentType = "application/octet-stream";
        internal static readonly string s_defaultCallbackParameterName = "callback";
        private WebMessageBodyStyle _defaultBodyStyle;
        private WebMessageFormat _defaultOutgoingReplyFormat;
        private WebMessageFormat _defaultOutgoingRequestFormat;
        private string _contractNamespace;
        private readonly UnwrappedTypesXmlSerializerManager _xmlSerializerManager;

        public WebHttpBehavior()
            : this(null)
        {
        }

        public WebHttpBehavior(IServiceProvider serviceProvider)
        {
            _defaultOutgoingRequestFormat = WebMessageFormat.Xml;
            _defaultOutgoingReplyFormat = WebMessageFormat.Xml;
            _defaultBodyStyle = WebMessageBodyStyle.Bare;
            _xmlSerializerManager = new UnwrappedTypesXmlSerializerManager();

            ServiceProvider = serviceProvider;
        }

        internal IServiceProvider ServiceProvider { get; }

        internal delegate void Effect();

        public virtual WebMessageBodyStyle DefaultBodyStyle
        {
            get { return _defaultBodyStyle; }
            set
            {
                if (!WebMessageBodyStyleHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }

                _defaultBodyStyle = value;
            }
        }

        public virtual WebMessageFormat DefaultOutgoingRequestFormat
        {
            get
            {
                return _defaultOutgoingRequestFormat;
            }
            set
            {
                if (!WebMessageFormatHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }

                _defaultOutgoingRequestFormat = value;
            }
        }

        public virtual WebMessageFormat DefaultOutgoingResponseFormat
        {
            get
            {
                return _defaultOutgoingReplyFormat;
            }
            set
            {
                if (!WebMessageFormatHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }

                _defaultOutgoingReplyFormat = value;
            }
        }

        public virtual bool HelpEnabled { get; set; }

        public virtual bool AutomaticFormatSelectionEnabled { get; set; }

        public virtual bool FaultExceptionEnabled { get; set; }

        internal Uri HelpUri { get; set; }

        public virtual void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            // do nothing
        }

        public virtual void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            if (endpoint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(endpoint));
            }

            if (clientRuntime == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(clientRuntime));
            }

            WebMessageEncodingBindingElement webEncodingBindingElement = endpoint.Binding.CreateBindingElements().Find<WebMessageEncodingBindingElement>();
            if (webEncodingBindingElement != null && webEncodingBindingElement.CrossDomainScriptAccessEnabled)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.CrossDomainJavascriptNotsupported));
            }

            _contractNamespace = endpoint.Contract.Namespace;
        }

        public virtual void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            // dotnet/wcf is a client-only library. The dispatch-side wiring (UriTemplateDispatchFormatter,
            // WebHttpDispatchOperationSelector, WebErrorHandler, FormatSelectingMessageInspector,
            // HelpPage, HttpUnhandledOperationInvoker, MultiplexingDispatchMessageFormatter,
            // CompositeDispatchFormatter, WebFaultFormatter, PrefixEndpointAddressMessageFilter,
            // MatchAllMessageFilter, WebHttpServiceModelCompat) is not part of this port. Use
            // CoreWCF.WebHttp on the server side.
            if (endpoint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(endpoint));
            }

            if (endpointDispatcher == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(endpointDispatcher));
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                new NotSupportedException(SR.WebHttpServerSideOperationSelectorNotSupported));
        }

        public virtual void Validate(ServiceEndpoint endpoint)
        {
            if (endpoint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(endpoint));
            }

            ValidateNoMessageHeadersPresent(endpoint);
            ValidateBinding(endpoint);
            ValidateContract(endpoint);
        }

        private void ValidateNoMessageHeadersPresent(ServiceEndpoint endpoint)
        {
            if (endpoint == null || endpoint.Address == null)
            {
                return;
            }

            EndpointAddress address = endpoint.Address;
            if (address.Headers.Count > 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.WebHttpServiceEndpointCannotHaveMessageHeaders, address)));
            }
        }

        protected virtual void ValidateBinding(ServiceEndpoint endpoint)
        {
            ValidateIsWebHttpBinding(endpoint, GetType().ToString());
        }

        internal static string GetWebMethod(OperationDescription od)
        {
            WebGetAttribute wga = ((KeyedByTypeCollection<IOperationBehavior>)od.OperationBehaviors).Find<WebGetAttribute>();
            WebInvokeAttribute wia = ((KeyedByTypeCollection<IOperationBehavior>)od.OperationBehaviors).Find<WebInvokeAttribute>();
            EnsureOk(wga, wia, od);
            if (wga != null)
            {
                return GET;
            }
            else if (wia != null)
            {
                return wia.Method ?? POST;
            }
            else
            {
                return POST;
            }
        }

        internal static string GetWebUriTemplate(OperationDescription od)
        {
            // return exactly what is on the attribute
            WebGetAttribute wga = ((KeyedByTypeCollection<IOperationBehavior>)od.OperationBehaviors).Find<WebGetAttribute>();
            WebInvokeAttribute wia = ((KeyedByTypeCollection<IOperationBehavior>)od.OperationBehaviors).Find<WebInvokeAttribute>();
            EnsureOk(wga, wia, od);
            if (wga != null)
            {
                return wga.UriTemplate;
            }
            else if (wia != null)
            {
                return wia.UriTemplate;
            }
            else
            {
                return null;
            }
        }

        internal static string GetDescription(OperationDescription od)
        {
            object[] attributes = null;
            if (od.SyncMethod != null)
            {
                attributes = od.SyncMethod.GetCustomAttributes(typeof(DescriptionAttribute), true);
            }
            else if (od.BeginMethod != null)
            {
                attributes = od.BeginMethod.GetCustomAttributes(typeof(DescriptionAttribute), true);
            }
            else if (od.TaskMethod != null)
            {
                attributes = od.TaskMethod.GetCustomAttributes(typeof(DescriptionAttribute), true);
            }

            if (attributes != null && attributes.Length > 0)
            {
                return ((DescriptionAttribute)attributes[0]).Description;
            }
            else
            {
                return string.Empty;
            }
        }

        internal static bool IsTypedMessage(MessageDescription message) => message != null && message.MessageType != null;

        internal static bool IsUntypedMessage(MessageDescription message)
        {
            if (message == null)
            {
                return false;
            }

            return (message.Body.ReturnValue != null && message.Body.Parts.Count == 0 && message.Body.ReturnValue.Type == typeof(Message)) ||
                (message.Body.ReturnValue == null && message.Body.Parts.Count == 1 && message.Body.Parts[0].Type == typeof(Message));
        }

        internal static MessageDescription MakeDummyMessageDescription(MessageDirection direction)
        {
            MessageDescription messageDescription = new MessageDescription("urn:dummyAction", direction);
            return messageDescription;
        }

        internal static bool SupportsJsonFormat(OperationDescription od)
        {
            // if the type is XmlSerializable, then we cannot create a json serializer for it
            DataContractSerializerOperationBehavior dcsob = ((KeyedByTypeCollection<IOperationBehavior>)od.OperationBehaviors).Find<DataContractSerializerOperationBehavior>();
            return (dcsob != null);
        }

        internal static void ValidateIsWebHttpBinding(ServiceEndpoint serviceEndpoint, string behaviorName)
        {
            Binding binding = serviceEndpoint.Binding;
            if (binding.Scheme != "http" && binding.Scheme != "https")
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SR.Format(SR.WCFBindingCannotBeUsedWithUriOperationSelectorBehaviorBadScheme,
                    serviceEndpoint.Contract.Name, behaviorName)));
            }

            if (binding.MessageVersion != MessageVersion.None)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SR.Format(SR.WCFBindingCannotBeUsedWithUriOperationSelectorBehaviorBadMessageVersion,
                    serviceEndpoint.Address.Uri.AbsoluteUri, behaviorName)));
            }

            TransportBindingElement transportBindingElement = binding.CreateBindingElements().Find<TransportBindingElement>();
            if (transportBindingElement != null && !transportBindingElement.ManualAddressing)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SR.Format(SR.ManualAddressingCannotBeFalseWithTransportBindingElement,
                    serviceEndpoint.Address.Uri.AbsoluteUri, behaviorName, transportBindingElement.GetType().Name)));
            }
        }

        internal WebMessageBodyStyle GetBodyStyle(OperationDescription od)
        {
            WebGetAttribute wga = ((KeyedByTypeCollection<IOperationBehavior>)od.OperationBehaviors).Find<WebGetAttribute>();
            WebInvokeAttribute wia = ((KeyedByTypeCollection<IOperationBehavior>)od.OperationBehaviors).Find<WebInvokeAttribute>();
            EnsureOk(wga, wia, od);
            if (wga != null)
            {
                return wga.GetBodyStyleOrDefault(DefaultBodyStyle);
            }
            else if (wia != null)
            {
                return wia.GetBodyStyleOrDefault(DefaultBodyStyle);
            }
            else
            {
                return DefaultBodyStyle;
            }
        }

        protected virtual void AddServerErrorHandlers(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            // Server-side only. WebErrorHandler is not ported (use CoreWCF.WebHttp on the server).
        }

        // GetOperationSelector removed - server-side only (WebHttpDispatchOperationSelector is not ported).

        protected virtual QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription) => new QueryStringConverter();

        internal virtual bool UseBareReplyFormatter(WebMessageBodyStyle style, OperationDescription operationDescription, WebMessageFormat responseFormat, out Type parameterType)
        {
            parameterType = null;

            return IsBareResponse(style) && TryGetNonMessageParameterType(operationDescription.Messages[1], operationDescription, false, out parameterType);
        }

        internal virtual IDispatchMessageFormatter GetReplyDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            // Server-side only. The original implementation builds a MultiplexingDispatchMessageFormatter
            // composed of dispatch-side helpers that are not ported (ContentTypeSettingDispatchMessageFormatter,
            // MultiplexingDispatchMessageFormatter, MessagePassthroughFormatter).
            return null;
        }

        internal virtual IDispatchMessageFormatter GetRequestDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            // Server-side only. The original implementation builds a UriTemplateDispatchFormatter
            // composed with body formatters, then wraps the result in another UriTemplateDispatchFormatter.
            // UriTemplateDispatchFormatter is not ported (server-side dispatch).
            return null;
        }

        private static void CloneMessageDescriptionsBeforeActing(OperationDescription operationDescription, Effect effect)
        {
            MessageDescription originalRequest = operationDescription.Messages[0];
            bool thereIsAReply = operationDescription.Messages.Count > 1;
            MessageDescription originalReply = thereIsAReply ? operationDescription.Messages[1] : null;
            operationDescription.Messages[0] = originalRequest.Clone();
            if (thereIsAReply)
            {
                operationDescription.Messages[1] = originalReply.Clone();
            }

            effect();
            operationDescription.Messages[0] = originalRequest;
            if (thereIsAReply)
            {
                operationDescription.Messages[1] = originalReply;
            }
        }

        internal virtual bool UseBareRequestFormatter(WebMessageBodyStyle style, OperationDescription operationDescription, out Type parameterType)
        {
            parameterType = null;

            return IsBareRequest(style) && TryGetNonMessageParameterType(operationDescription.Messages[0], operationDescription, true, out parameterType);
        }

        private static Collection<MessagePartDescription> CloneParts(MessageDescription md)
        {
            MessagePartDescriptionCollection bodyParameters = md.Body.Parts;
            Collection<MessagePartDescription> bodyParametersClone = new Collection<MessagePartDescription>();
            for (int i = 0; i < bodyParameters.Count; ++i)
            {
                MessagePartDescription copy = bodyParameters[i].Clone();
                bodyParametersClone.Add(copy);
            }

            return bodyParametersClone;
        }

        private static void EnsureNotUntypedMessageNorMessageContract(OperationDescription operationDescription)
        {
            // Called when there are UriTemplate parameters.  UT does not compose with Message
            // or MessageContract because the SOAP and REST programming models must be uniform here.
            bool isUnadornedWebGet = false;
            if (GetWebMethod(operationDescription) == GET && GetWebUriTemplate(operationDescription) == null)
            {
                isUnadornedWebGet = true;
            }

            if (IsTypedMessage(operationDescription.Messages[0]))
            {
                if (isUnadornedWebGet)
                {
                    // WebGet will give you UriTemplate parameters by default.
                    // We need a special error message for this case to prevent confusion.
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        SR.Format(SR.GETCannotHaveMCParameter, operationDescription.Name, operationDescription.DeclaringContract.Name, operationDescription.Messages[0].MessageType.Name)));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(
                        SR.UTParamsDoNotComposeWithMessageContract, operationDescription.Name, operationDescription.DeclaringContract.Name)));
                }
            }

            if (IsUntypedMessage(operationDescription.Messages[0]))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(
                    SR.UTParamsDoNotComposeWithMessage, operationDescription.Name, operationDescription.DeclaringContract.Name)));
            }
        }

        private static void EnsureOk(WebGetAttribute wga, WebInvokeAttribute wia, OperationDescription od)
        {
            if (wga != null && wia != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SR.Format(SR.MultipleWebAttributes, od.Name, od.DeclaringContract.Name)));
            }
        }

        private static void HideReplyMessage(OperationDescription operationDescription, Effect effect)
        {
            MessageDescription temp = null;
            if (operationDescription.Messages.Count > 1)
            {
                temp = operationDescription.Messages[1];
                operationDescription.Messages[1] = MakeDummyMessageDescription(MessageDirection.Output);
            }

            effect();
            if (operationDescription.Messages.Count > 1)
            {
                operationDescription.Messages[1] = temp;
            }
        }

        // HideRequestUriTemplateParameters(OperationDescription, UriTemplateDispatchFormatter, Effect)
        // removed - server-side only (UriTemplateDispatchFormatter is not ported).

        private static void HideRequestUriTemplateParameters(OperationDescription operationDescription, Dictionary<int, string> pathMapping, Dictionary<int, KeyValuePair<string, Type>> queryMapping, Effect effect)
        {
            // mutate description to hide UriTemplate parameters
            Collection<MessagePartDescription> originalParts = CloneParts(operationDescription.Messages[0]);
            Collection<MessagePartDescription> parts = CloneParts(operationDescription.Messages[0]);
            operationDescription.Messages[0].Body.Parts.Clear();
            int newIndex = 0;
            for (int i = 0; i < parts.Count; ++i)
            {
                if (!pathMapping.ContainsKey(i) && !queryMapping.ContainsKey(i))
                {
                    operationDescription.Messages[0].Body.Parts.Add(parts[i]);
                    parts[i].Index = newIndex++;
                }
            }

            effect();
            // unmutate description
            operationDescription.Messages[0].Body.Parts.Clear();
            for (int i = 0; i < originalParts.Count; ++i)
            {
                operationDescription.Messages[0].Body.Parts.Add(originalParts[i]);
            }
        }

        private static bool IsBareRequest(WebMessageBodyStyle style) => style == WebMessageBodyStyle.Bare || style == WebMessageBodyStyle.WrappedResponse;

        private static bool IsBareResponse(WebMessageBodyStyle style) => style == WebMessageBodyStyle.Bare || style == WebMessageBodyStyle.WrappedRequest;

        internal static bool TryGetNonMessageParameterType(MessageDescription message, OperationDescription declaringOperation, bool isRequest, out Type type)
        {
            type = null;
            if (message == null)
            {
                return true;
            }

            if (IsTypedMessage(message) || IsUntypedMessage(message))
            {
                return false;
            }

            if (isRequest)
            {
                if (message.Body.Parts.Count > 1)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.AtMostOneRequestBodyParameterAllowedForUnwrappedMessages, declaringOperation.Name, declaringOperation.DeclaringContract.Name)));
                }

                if (message.Body.Parts.Count == 1 && message.Body.Parts[0].Type != typeof(void))
                {
                    type = message.Body.Parts[0].Type;
                }

                return true;
            }
            else
            {
                if (message.Body.Parts.Count > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.OnlyReturnValueBodyParameterAllowedForUnwrappedMessages, declaringOperation.Name, declaringOperation.DeclaringContract.Name)));
                }

                if (message.Body.ReturnValue != null && message.Body.ReturnValue.Type != typeof(void))
                {
                    type = message.Body.ReturnValue.Type;
                }

                return true;
            }
        }

        private static bool TryGetStreamParameterType(MessageDescription message, OperationDescription declaringOperation, bool isRequest, out Type type)
        {
            type = null;
            if (message == null || IsTypedMessage(message) || IsUntypedMessage(message))
            {
                return false;
            }

            if (isRequest)
            {
                bool hasStream = false;
                for (int i = 0; i < message.Body.Parts.Count; ++i)
                {
                    if (typeof(Stream) == message.Body.Parts[i].Type)
                    {
                        type = message.Body.Parts[i].Type;
                        hasStream = true;
                        break;
                    }

                }

                if (hasStream && message.Body.Parts.Count > 1)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.AtMostOneRequestBodyParameterAllowedForStream, declaringOperation.Name, declaringOperation.DeclaringContract.Name)));
                }

                return hasStream;
            }
            else
            {
                // validate that the stream is not an out or ref param
                for (int i = 0; i < message.Body.Parts.Count; ++i)
                {
                    if (typeof(Stream) == message.Body.Parts[i].Type)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.NoOutOrRefStreamParametersAllowed, message.Body.Parts[i].Name, declaringOperation.Name, declaringOperation.DeclaringContract.Name)));
                    }
                }

                if (message.Body.ReturnValue != null && typeof(Stream) == message.Body.ReturnValue.Type)
                {
                    // validate that there are no out or ref params
                    if (message.Body.Parts.Count > 0)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.NoOutOrRefParametersAllowedWithStreamResult, declaringOperation.Name, declaringOperation.DeclaringContract.Name)));
                    }
                    type = message.Body.ReturnValue.Type;
                    return true;
                }

                else
                {
                    return false;
                }
            }
        }

        private static void ValidateAtMostOneStreamParameter(OperationDescription operation, bool request)
        {
            Type dummy;
            if (request)
            {
                TryGetStreamParameterType(operation.Messages[0], operation, true, out dummy);
            }
            else
            {
                if (operation.Messages.Count > 1)
                {
                    TryGetStreamParameterType(operation.Messages[1], operation, false, out dummy);
                }
            }
        }

        private IDispatchMessageFormatter GetDefaultDispatchFormatter(OperationDescription od, bool useJson, bool isWrapped)
        {
            // Server-side only. Used by the stubbed GetReplyDispatchFormatter / GetRequestDispatchFormatter.
            // The original constructs a dummy EndpointDispatcher (server-only 3-arg constructor)
            // and invokes IOperationBehavior.ApplyDispatchBehavior to harvest the formatter -
            // none of which is wired in the client-only port.
            return null;
        }

        internal virtual DataContractJsonSerializerOperationFormatter CreateDataContractJsonSerializerOperationFormatter(OperationDescription od, DataContractSerializerOperationBehavior dcsob, bool isWrapped)
        {
            //return new DataContractJsonSerializerOperationFormatter(od, dcsob.MaxItemsInObjectGraph, dcsob.IgnoreExtensionDataObject, isWrapped, false, JavascriptCallbackParameterName);

            OperationDescription clonedOperation = new OperationDescription(od.Name, od.DeclaringContract);
            foreach (MessageDescription message in od.Messages)
            {
                MessageDescription clonedMessage = message.Clone();
                if (IsValidReturnValue(clonedMessage.Body.ReturnValue))
                {
                    if (clonedMessage.Body.Parts.Count == 0)
                    {
                        if (clonedMessage.Body.ReturnValue.Type == typeof(Stream))
                        {
                            clonedMessage.Body.WrapperName = JsonGlobals.RootString;
                            clonedMessage.Body.WrapperNamespace = string.Empty;
                        }
                    }
                }
                else
                {
                    if (clonedMessage.Body.Parts.Count == 1)
                    {
                        if (clonedMessage.Body.Parts[0].Type == typeof(Stream))
                        {
                            clonedMessage.Body.WrapperName = JsonGlobals.RootString;
                            clonedMessage.Body.WrapperNamespace = string.Empty;
                        }
                    }
                }

                clonedOperation.Messages.Add(clonedMessage);
            }

            return new DataContractJsonSerializerOperationFormatter(clonedOperation, dcsob.MaxItemsInObjectGraph, dcsob.IgnoreExtensionDataObject, isWrapped, false, null);
        }

        private static MessagePartDescription GetStreamPart(MessageDescription messageDescription)
        {
            if (IsValidReturnValue(messageDescription.Body.ReturnValue))
            {
                if (messageDescription.Body.Parts.Count == 0)
                {
                    if (messageDescription.Body.ReturnValue.Type == typeof(Stream))
                    {
                        return messageDescription.Body.ReturnValue;
                    }
                }
            }
            else
            {
                if (messageDescription.Body.Parts.Count == 1)
                {
                    if (messageDescription.Body.Parts[0].Type == typeof(Stream))
                    {
                        return messageDescription.Body.Parts[0];
                    }
                }
            }

            return null;
        }

        private static bool IsValidReturnValue(MessagePartDescription returnValue) => (returnValue != null) && (returnValue.Type != typeof(void));

        // TODO: Can I remove this?
        //IClientMessageFormatter GetDefaultXmlAndJsonClientFormatter(OperationDescription od, bool isWrapped)
        //{
        //    IClientMessageFormatter xmlFormatter = GetDefaultClientFormatter(od, false, isWrapped);
        //    if (!SupportsJsonFormat(od))
        //    {
        //        return xmlFormatter;
        //    }
        //    IClientMessageFormatter jsonFormatter = GetDefaultClientFormatter(od, true, isWrapped);
        //    Dictionary<WebContentFormat, IClientMessageFormatter> map = new Dictionary<WebContentFormat, IClientMessageFormatter>();
        //    map.Add(WebContentFormat.Xml, xmlFormatter);
        //    map.Add(WebContentFormat.Json, jsonFormatter);
        //    // In case there is no format property, the default formatter to use is XML
        //    return new DemultiplexingClientMessageFormatter(map, xmlFormatter);
        //}

        private IDispatchMessageFormatter GetDefaultXmlAndJsonDispatchFormatter(OperationDescription od, bool isWrapped)
        {
            // Server-side only. Returns a DemultiplexingDispatchMessageFormatter which is not ported.
            return null;
        }

        internal WebMessageFormat GetRequestFormat(OperationDescription od)
        {
            WebGetAttribute wga = ((KeyedByTypeCollection<IOperationBehavior>)od.OperationBehaviors).Find<WebGetAttribute>();
            WebInvokeAttribute wia = ((KeyedByTypeCollection<IOperationBehavior>)od.OperationBehaviors).Find<WebInvokeAttribute>();
            EnsureOk(wga, wia, od);
            if (wga != null)
            {
                return wga.IsRequestFormatSetExplicitly ? wga.RequestFormat : this.DefaultOutgoingRequestFormat;
            }
            else if (wia != null)
            {
                return wia.IsRequestFormatSetExplicitly ? wia.RequestFormat : this.DefaultOutgoingRequestFormat;
            }
            else
            {
                return DefaultOutgoingRequestFormat;
            }
        }

        internal WebMessageFormat GetResponseFormat(OperationDescription od)
        {
            WebGetAttribute wga = ((KeyedByTypeCollection<IOperationBehavior>)od.OperationBehaviors).Find<WebGetAttribute>();
            WebInvokeAttribute wia = ((KeyedByTypeCollection<IOperationBehavior>)od.OperationBehaviors).Find<WebInvokeAttribute>();
            EnsureOk(wga, wia, od);
            if (wga != null)
            {
                return wga.IsResponseFormatSetExplicitly ? wga.ResponseFormat : this.DefaultOutgoingResponseFormat;
            }
            else if (wia != null)
            {
                return wia.IsResponseFormatSetExplicitly ? wia.ResponseFormat : this.DefaultOutgoingResponseFormat;
            }
            else
            {
                return DefaultOutgoingResponseFormat;
            }
        }

        private void ValidateBodyParameters(OperationDescription operation, bool request)
        {
            string method = GetWebMethod(operation);
            if (request)
            {
                ValidateGETHasNoBody(operation, method);
            }

            // validate that if bare is chosen for request/response, then at most 1 parameter is possible
            ValidateBodyStyle(operation, request);
            // validate if the request or response body is a stream, no other body parameters
            // can be specified
            ValidateAtMostOneStreamParameter(operation, request);
        }

        private void ValidateBodyStyle(OperationDescription operation, bool request)
        {
            WebMessageBodyStyle style = GetBodyStyle(operation);
            Type dummy;
            if (request && IsBareRequest(style))
            {
                TryGetNonMessageParameterType(operation.Messages[0], operation, true, out dummy);
            }

            if (!request && operation.Messages.Count > 1 && IsBareResponse(style))
            {
                TryGetNonMessageParameterType(operation.Messages[1], operation, false, out dummy);
            }
        }

        private void ValidateGETHasNoBody(OperationDescription operation, string method)
        {
            if (method == GET)
            {
                if (!IsUntypedMessage(operation.Messages[0]) && operation.Messages[0].Body.Parts.Count != 0)
                {
                    if (!IsTypedMessage(operation.Messages[0]))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                            SR.Format(SR.GETCannotHaveBody, operation.Name, operation.DeclaringContract.Name, operation.Messages[0].Body.Parts[0].Name)));
                    }
                    else
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                            SR.Format(SR.GETCannotHaveMCParameter, operation.Name, operation.DeclaringContract.Name, operation.Messages[0].MessageType.Name)));
                    }
                }
            }
        }

        private void ValidateContract(ServiceEndpoint endpoint)
        {
            foreach (OperationDescription od in endpoint.Contract.Operations)
            {
                ValidateNoOperationHasEncodedXmlSerializer(od);
                ValidateNoMessageContractHeaders(od.Messages[0], od.Name, endpoint.Contract.Name);
                ValidateNoBareMessageContractWithMultipleParts(od.Messages[0], od.Name, endpoint.Contract.Name);
                ValidateNoMessageContractWithStream(od.Messages[0], od.Name, endpoint.Contract.Name);
                if (od.Messages.Count > 1)
                {
                    ValidateNoMessageContractHeaders(od.Messages[1], od.Name, endpoint.Contract.Name);
                    ValidateNoBareMessageContractWithMultipleParts(od.Messages[1], od.Name, endpoint.Contract.Name);
                    ValidateNoMessageContractWithStream(od.Messages[1], od.Name, endpoint.Contract.Name);
                }
            }
        }

        internal static bool IsXmlSerializerFaultFormat(OperationDescription operationDescription)
        {
            XmlSerializerOperationBehavior xsob = ((KeyedByTypeCollection<IOperationBehavior>)operationDescription.OperationBehaviors).Find<XmlSerializerOperationBehavior>();

            return (xsob != null && xsob.XmlSerializerFormatAttribute.SupportFaults);
        }

        private void ValidateNoMessageContractWithStream(MessageDescription md, string opName, string contractName)
        {
            if (IsTypedMessage(md))
            {
                foreach (MessagePartDescription description in md.Body.Parts)
                {
                    if (description.Type == typeof(Stream))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(SR.Format(SR.StreamBodyMemberNotSupported, GetType().ToString(), contractName, opName, md.MessageType.ToString(), description.Name)));
                    }
                }
            }
        }

        private void ValidateNoOperationHasEncodedXmlSerializer(OperationDescription od)
        {
            XmlSerializerOperationBehavior xsob = ((KeyedByTypeCollection<IOperationBehavior>)od.OperationBehaviors).Find<XmlSerializerOperationBehavior>();
            if (xsob != null && (xsob.XmlSerializerFormatAttribute.Style == OperationFormatStyle.Rpc || xsob.XmlSerializerFormatAttribute.Use == OperationFormatUse.Encoded))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.RpcEncodedNotSupportedForNoneMessageVersion, od.Name, od.DeclaringContract.Name, od.DeclaringContract.Namespace)));
            }
        }

        private void ValidateNoBareMessageContractWithMultipleParts(MessageDescription md, string opName, string contractName)
        {
            if (IsTypedMessage(md) && md.Body.WrapperName == null)
            {
                if (md.Body.Parts.Count > 1)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        SR.Format(SR.InvalidMessageContractWithoutWrapperName, opName, contractName, md.MessageType)));
                }

                if (md.Body.Parts.Count == 1 && md.Body.Parts[0].Multiple)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.MCAtMostOneRequestBodyParameterAllowedForUnwrappedMessages, opName, contractName, md.MessageType)));
                }
            }
        }

        private void ValidateNoMessageContractHeaders(MessageDescription md, string opName, string contractName)
        {
            if (md.Headers.Count != 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SR.Format(SR.InvalidMethodWithSOAPHeaders, opName, contractName)));
            }
        }

        internal class MessagePassthroughFormatter : IClientMessageFormatter, IDispatchMessageFormatter
        {
            public object DeserializeReply(Message message, object[] parameters) => message;

            public void DeserializeRequest(Message message, object[] parameters)
            {
                parameters[0] = message;
            }

            public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result) => result as Message;

            public Message SerializeRequest(MessageVersion messageVersion, object[] parameters) => parameters[0] as Message;
        }
    }
}
