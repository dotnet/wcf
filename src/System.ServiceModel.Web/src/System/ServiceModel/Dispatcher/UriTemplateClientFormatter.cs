// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable 1634, 1691
namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Text;
    using System.Xml;
    using System.ServiceModel.Web;

    internal class UriTemplateClientFormatter : IClientMessageFormatter
    {
        internal Dictionary<int, string> pathMapping;
        internal Dictionary<int, KeyValuePair<string, Type>> queryMapping;
        Uri baseUri;
        IClientMessageFormatter inner;
        bool innerIsUntypedMessage;
        bool isGet;
        string method;
        QueryStringConverter qsc;
        int totalNumUTVars;
        UriTemplate uriTemplate;

        public UriTemplateClientFormatter(OperationDescription operationDescription, IClientMessageFormatter inner, QueryStringConverter qsc, Uri baseUri, bool innerIsUntypedMessage, string contractName)
        {
            this.inner = inner;
            this.qsc = qsc;
            this.baseUri = baseUri;
            this.innerIsUntypedMessage = innerIsUntypedMessage;
            Populate(out this.pathMapping,
                out this.queryMapping,
                out this.totalNumUTVars,
                out this.uriTemplate,
                operationDescription,
                qsc,
                contractName);
            this.method = WebHttpBehavior.GetWebMethod(operationDescription);
            isGet = this.method == WebHttpBehavior.GET;
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            // The URI template formatter is purely a request-side concern: it binds
            // operation parameters into the outgoing URI. Reply deserialization is
            // delegated to the inner formatter.
            if (inner != null)
            {
                return inner.DeserializeReply(message, parameters);
            }
            return null;
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            object[] innerParameters = new object[parameters.Length - this.totalNumUTVars];
            NameValueCollection nvc = new NameValueCollection();
            int j = 0;
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (this.pathMapping.ContainsKey(i))
                {
                    nvc[this.pathMapping[i]] = parameters[i] as string;
                }
                else if (this.queryMapping.ContainsKey(i))
                {
                    if (parameters[i] != null)
                    {
                        nvc[this.queryMapping[i].Key] = this.qsc.ConvertValueToString(parameters[i], this.queryMapping[i].Value);
                    }
                }
                else
                {
                    innerParameters[j] = parameters[i];
                    ++j;
                }
            }
            Message m = inner.SerializeRequest(messageVersion, innerParameters);
            bool userSetTheToOnMessage = (this.innerIsUntypedMessage && m.Headers.To != null);
            bool userSetTheToOnOutgoingHeaders = (OperationContext.Current != null && OperationContext.Current.OutgoingMessageHeaders.To != null);
            if (!userSetTheToOnMessage && !userSetTheToOnOutgoingHeaders)
            {
                m.Headers.To = this.uriTemplate.BindByName(this.baseUri, nvc);
            }
            // dotnet/wcf's HttpRequestMessageProperty exposes SuppressEntityBody and Method
            // (server-only HttpResponseMessageProperty does not). Set them here rather than
            // routing through WebOperationContext (which is optional and may not be set up).
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
            if (isGet)
            {
                hrmp.SuppressEntityBody = true;
            }
            if (this.method != WebHttpBehavior.WildcardMethod)
            {
                hrmp.Method = this.method;
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
            //sb.Append("/*"); // note: not + "/*", see 8988 and 9653
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
