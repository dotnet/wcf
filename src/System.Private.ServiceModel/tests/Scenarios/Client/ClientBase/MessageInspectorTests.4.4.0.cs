// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Net;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using ClientMessageInspector;
using TestTypes;
using Xunit;

public static class MessageInspectorTests
{
    [WcfFact]
    [OuterLoop]
    public static void MessageInspector_RoundtripCustomHeaders_ComplexType()
    {
        MI_ClientBase_ClientAuth mi_ClientBase_ClientAuth1 = null;
        MI_ClientBase_ClientAuth mi_ClientBase_ClientAuth2 = null;
        BasicHttpBinding binding = null;
        string accessToken;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding((BasicHttpSecurityMode)BasicHttpSecurityMode.None);
            AuthenticationType authType = AuthenticationType.Live;
            EndpointAddress endPoint = new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text);
            

            // *** EXECUTE FIRST VARIATION *** \\
            accessToken = "Not Allowed";
            mi_ClientBase_ClientAuth1 = new MI_ClientBase_ClientAuth(authType.ToString(), accessToken, binding, endPoint);
            ResultObject<string> result1 = mi_ClientBase_ClientAuth1.GetAuthToken();

            // *** EXECUTE SECOND VARIATION *** \\
            accessToken = "Allow";
            mi_ClientBase_ClientAuth2 = new MI_ClientBase_ClientAuth(authType.ToString(), accessToken, binding, endPoint);
            ResultObject<string> result2 = mi_ClientBase_ClientAuth2.GetAuthToken();

            // *** VALIDATE FIRST VARIATION *** \\
            string expectedErrorDescription = TestTypes.ResultMessage.GetErrorDescription(TestTypes.ErrorCode.UserNotAuthenticated);
            Assert.True(String.Equals(result1.Result, expectedErrorDescription), String.Format("Expected Error Description: {0}/Actual Error Description: {1}", expectedErrorDescription, result2.Result));
            TestTypes.ErrorCode returnedErrorCode = (TestTypes.ErrorCode)result1.ErrorCode;
            Assert.True(returnedErrorCode.Equals(TestTypes.ErrorCode.UserNotAuthenticated), String.Format("Expected Error Code: {0}/nActual Error Code: {1}", TestTypes.ErrorCode.UserNotAuthenticated, returnedErrorCode));
            Assert.True(result1.HttpStatusCode.Equals(System.Net.HttpStatusCode.Unauthorized), String.Format("Expected HttpStatusCode: {0}/nActual HttpStatusCode: {1}", System.Net.HttpStatusCode.Unauthorized, result2.HttpStatusCode));

            // *** VALIDATE SECOND VARIATION *** \\
            expectedErrorDescription = TestTypes.ResultMessage.GetErrorDescription(TestTypes.ErrorCode.Ok);
            Assert.True(String.Equals(result2.Result, expectedErrorDescription), String.Format("Expected Error Description: {0}/nActual Error Description: {1}", expectedErrorDescription, result2.Result));
            returnedErrorCode = (TestTypes.ErrorCode)result2.ErrorCode;
            Assert.True(returnedErrorCode.Equals(TestTypes.ErrorCode.Ok), String.Format("Expected Error Code: {0}/nActual Error Code: {1}", TestTypes.ErrorCode.Ok, returnedErrorCode));
            Assert.True(result2.HttpStatusCode.Equals(System.Net.HttpStatusCode.OK), String.Format("Expected HttpStatusCode: {0}/nActual HttpStatusCode: {1}", System.Net.HttpStatusCode.OK, result2.HttpStatusCode));

            // *** CLEANUP *** \\
            ((ICommunicationObject)mi_ClientBase_ClientAuth1).Close();
            ((ICommunicationObject)mi_ClientBase_ClientAuth2).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)mi_ClientBase_ClientAuth1, (ICommunicationObject)mi_ClientBase_ClientAuth2);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void MessageInspector_RoundtripCustomHeaders()
    {
        MI_ClientBase_ClientAuth mi_ClientBase_ClientAuth = null;
        BasicHttpBinding binding = null;
        string accessToken = "Allow";

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding((BasicHttpSecurityMode)BasicHttpSecurityMode.None);

            AuthenticationType authType = AuthenticationType.None;

            EndpointAddress endPoint = new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text);

            mi_ClientBase_ClientAuth = new MI_ClientBase_ClientAuth(authType.ToString(), accessToken, binding, endPoint);

            // *** EXECUTE *** \\
            Dictionary<string, string> headers = mi_ClientBase_ClientAuth.ValidateHeaders();

            // *** VALIDATE *** \\
            string authorizationHeaderValue, authTypeHeaderValue = "";
            if (headers.TryGetValue("Authorization", out authorizationHeaderValue) && (headers.TryGetValue("authType", out authTypeHeaderValue)))
            {
                Assert.True(String.Equals(authorizationHeaderValue, accessToken), String.Format("Expected Authorization Header value: {0}/nActual Authorization Header value: {1}", accessToken, authorizationHeaderValue));
                Assert.True(String.Equals(authTypeHeaderValue, authType.ToString()), String.Format("Expected AuthType Header value: {0}/nActual AuthType Header value: {1}", authType.ToString(), authTypeHeaderValue));
            }
            else
            {
                Assert.Fail(String.Format("One or both of the expected headers were not found.\nHeader: \"authType\" had value: {0}\nHeader: \"HttpRequestHeader.Authorization\" had value: {1}", authorizationHeaderValue, authTypeHeaderValue));
            }

            // *** CLEANUP *** \\
            ((ICommunicationObject)mi_ClientBase_ClientAuth).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)mi_ClientBase_ClientAuth);
        }

    }
}

