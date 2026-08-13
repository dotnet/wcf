// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

// Contracts shared by the System.ServiceModel.Web unit tests. They are never
// dispatched against a real service - they only exist so the tests can build
// ServiceEndpoint / ContractDescription graphs and drive WebHttpBehavior,
// UriTemplateClientFormatter and WebChannelFactory in-memory.

[ServiceContract]
public interface IWebHttpBindingTestService
{
    [OperationContract]
    [WebGet(UriTemplate = "EchoWithGet?message={message}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Xml)]
    string EchoWithGet(string message);

    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "EchoWithPost",
               BodyStyle = WebMessageBodyStyle.Bare,
               RequestFormat = WebMessageFormat.Xml,
               ResponseFormat = WebMessageFormat.Xml)]
    string EchoWithPost(string message);
}

// Contract whose operations carry no [WebGet]/[WebInvoke] attributes at all.
// WebHttpBehavior has to synthesise the default POST/"*" mapping for these.
[ServiceContract]
public interface IWebHttpUnattributedTestService
{
    [OperationContract]
    string Echo(string message);
}

// Raw / application-octet-stream contract used by the encoder tests.
[ServiceContract]
public interface IWebHttpRawTestService
{
    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "EchoStream", BodyStyle = WebMessageBodyStyle.Bare)]
    Stream EchoStream(Stream stream);
}

[ServiceContract]
public interface IWebHttpJsonTestService
{
    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "EchoJson",
        BodyStyle = WebMessageBodyStyle.Bare,
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
    string EchoJson(string message);

    [OperationContract]
    [WebInvoke(Method = "*", UriTemplate = "EchoWildcardJson",
        BodyStyle = WebMessageBodyStyle.Bare,
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
    string EchoWildcardJson(string message);
}

[ServiceContract]
public interface IWebHttpWrappedReplyTestService
{
    [OperationContract]
    [WebGet(UriTemplate = "EchoWrapped",
        BodyStyle = WebMessageBodyStyle.WrappedResponse,
        ResponseFormat = WebMessageFormat.Xml)]
    string EchoWrapped();
}

// Contract with a parameter type the default QueryStringConverter cannot
// convert, used to prove WebHttpBehavior.Validate rejects it.
[ServiceContract]
public interface IWebHttpUnconvertibleParameterService
{
    [OperationContract]
    [WebGet(UriTemplate = "Get?value={value}")]
    string Get(UnconvertibleParameter value);
}

public class UnconvertibleParameter
{
}
