// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace System.ServiceModel.Dispatcher
{
    internal class UriTemplateClientFormatter : IClientMessageFormatter
    {
        internal Dictionary<int, string> _pathMapping;
        internal Dictionary<int, KeyValuePair<string, Type>> _queryMapping;
        private readonly Uri _baseUri;
        private readonly IClientMessageFormatter _inner;
        private readonly bool _innerIsUntypedMessage;
        private readonly bool _isGet;
        private readonly string _method;
        private readonly QueryStringConverter _qsc;
        private readonly int _totalNumUTVars;
        private readonly UriTemplate _uriTemplate;

        public UriTemplateClientFormatter(OperationDescription operationDescription, IClientMessageFormatter inner, QueryStringConverter qsc, Uri baseUri, bool innerIsUntypedMessage, string contractName)
        {
            _inner = inner;
            _qsc = qsc;
            _baseUri = baseUri;
            _innerIsUntypedMessage = innerIsUntypedMessage;
            Populate(out _pathMapping,
                out _queryMapping,
                out _totalNumUTVars,
                out _uriTemplate,
                operationDescription,
                qsc,
                contractName);
            _method = WebHttpBehavior.GetWebMethod(operationDescription);
            _isGet = _method == WebHttpBehavior.GET;
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            // The URI template formatter is purely a request-side concern: it binds
            // operation parameters into the outgoing URI. Reply deserialization is
            // delegated to the inner formatter.
            if (_inner != null)
            {
                return _inner.DeserializeReply(message, parameters);
            }
            return null;
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            object[] innerParameters = new object[parameters.Length - _totalNumUTVars];
            NameValueCollection nvc = new NameValueCollection();
            int j = 0;
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (_pathMapping.ContainsKey(i))
                {
                    nvc[_pathMapping[i]] = parameters[i] as string;
                }
                else if (_queryMapping.ContainsKey(i))
                {
                    if (parameters[i] != null)
                    {
                        nvc[_queryMapping[i].Key] = _qsc.ConvertValueToString(parameters[i], _queryMapping[i].Value);
                    }
                }
                else
                {
                    innerParameters[j] = parameters[i];
                    ++j;
                }
            }
            Message m = _inner.SerializeRequest(messageVersion, innerParameters);
            bool userSetTheToOnMessage = (_innerIsUntypedMessage && m.Headers.To != null);
            bool userSetTheToOnOutgoingHeaders = (OperationContext.Current != null && OperationContext.Current.OutgoingMessageHeaders.To != null);
            if (!userSetTheToOnMessage && !userSetTheToOnOutgoingHeaders)
            {
                m.Headers.To = _uriTemplate.BindByName(_baseUri, nvc);
            }
            // Set Method / SuppressEntityBody on the HttpRequestMessageProperty. When a
            // WebOperationContext is active we must write through its OutgoingRequest so the
            // ambient property (which ServiceChannel.AddMessageProperties later copies over the
            // message) carries these values; otherwise a caller-supplied ambient
            // HttpRequestMessageProperty would clobber what we set directly on the message.
            // When there's no context, set them on the message property directly. This mirrors
            // the .NET Framework UriTemplateClientFormatter.
            if (WebOperationContext.Current != null)
            {
                if (_isGet)
                {
                    WebOperationContext.Current.OutgoingRequest.SuppressEntityBody = true;
                }
                if (_method != WebHttpBehavior.WildcardMethod)
                {
                    WebOperationContext.Current.OutgoingRequest.Method = _method;
                }
            }
            else
            {
                HttpRequestMessageProperty hrmp;
                if (m.Properties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    hrmp = m.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
                }
                else
                {
                    hrmp = new HttpRequestMessageProperty();
                    m.Properties.Add(HttpRequestMessageProperty.Name, hrmp);
                }
                if (_isGet)
                {
                    hrmp.SuppressEntityBody = true;
                }
                if (_method != WebHttpBehavior.WildcardMethod)
                {
                    hrmp.Method = _method;
                }
            }
            return m;
        }

        internal static string GetUTStringOrDefault(OperationDescription operationDescription)
        {
            string utString = WebHttpBehavior.GetWebUriTemplate(operationDescription);
            if (utString == null && WebHttpBehavior.GetWebMethod(operationDescription) == WebHttpBehavior.GET)
            {
                utString = MakeDefaultGetUTString(operationDescription);
            }

            if (utString == null)
            {
                utString = operationDescription.Name; // note: not + "/*", see 8988 and 9653
            }

            return utString;
        }

        internal static void Populate(out Dictionary<int, string> pathMapping,
            out Dictionary<int, KeyValuePair<string, Type>> queryMapping,
            out int totalNumUTVars,
            out UriTemplate uriTemplate,
            OperationDescription operationDescription,
            QueryStringConverter qsc,
            string contractName)
        {
            pathMapping = new Dictionary<int, string>();
            queryMapping = new Dictionary<int, KeyValuePair<string, Type>>();
            string utString = GetUTStringOrDefault(operationDescription);
            uriTemplate = new UriTemplate(utString);
            List<string> neededPathVars = new List<string>(uriTemplate.PathSegmentVariableNames);
            List<string> neededQueryVars = new List<string>(uriTemplate.QueryValueVariableNames);
            Dictionary<string, byte> alreadyGotVars = new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
            totalNumUTVars = neededPathVars.Count + neededQueryVars.Count;
            for (int i = 0; i < operationDescription.Messages[0].Body.Parts.Count; ++i)
            {
                MessagePartDescription mpd = operationDescription.Messages[0].Body.Parts[i];
                string parameterName = XmlConvert.DecodeName(mpd.Name);
                if (alreadyGotVars.ContainsKey(parameterName))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        SR.Format(SR.UriTemplateVarCaseDistinction, XmlConvert.DecodeName(operationDescription.Name), contractName, parameterName)));
                }

                List<string> neededPathCopy = new List<string>(neededPathVars);
                foreach (string pathVar in neededPathCopy)
                {
                    if (string.Compare(parameterName, pathVar, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (mpd.Type != typeof(string))
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                                SR.Format(SR.UriTemplatePathVarMustBeString, XmlConvert.DecodeName(operationDescription.Name), contractName, parameterName)));
                        }
                        pathMapping.Add(i, parameterName);
                        alreadyGotVars.Add(parameterName, 0);
                        neededPathVars.Remove(pathVar);
                    }
                }

                List<string> neededQueryCopy = new List<string>(neededQueryVars);
                foreach (string queryVar in neededQueryCopy)
                {
                    if (string.Compare(parameterName, queryVar, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (!qsc.CanConvert(mpd.Type))
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                                SR.Format(SR.UriTemplateQueryVarMustBeConvertible, XmlConvert.DecodeName(operationDescription.Name), contractName, parameterName, mpd.Type, qsc.GetType().Name)));
                        }
                        queryMapping.Add(i, new KeyValuePair<string, Type>(parameterName, mpd.Type));
                        alreadyGotVars.Add(parameterName, 0);
                        neededQueryVars.Remove(queryVar);
                    }
                }
            }

            if (neededPathVars.Count != 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SR.Format(SR.UriTemplateMissingVar, XmlConvert.DecodeName(operationDescription.Name), contractName, neededPathVars[0])));
            }

            if (neededQueryVars.Count != 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SR.Format(SR.UriTemplateMissingVar, XmlConvert.DecodeName(operationDescription.Name), contractName, neededQueryVars[0])));
            }
        }

        private static string MakeDefaultGetUTString(OperationDescription od)
        {
            StringBuilder sb = new StringBuilder(XmlConvert.DecodeName(od.Name));
            if (!WebHttpBehavior.IsUntypedMessage(od.Messages[0]))
            {
                sb.Append("?");
                foreach (MessagePartDescription mpd in od.Messages[0].Body.Parts)
                {
                    string parameterName = XmlConvert.DecodeName(mpd.Name);
                    sb.Append(parameterName);
                    sb.Append("={");
                    sb.Append(parameterName);
                    sb.Append("}&");
                }
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }
    }
}
