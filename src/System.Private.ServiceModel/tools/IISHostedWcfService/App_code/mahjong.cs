// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET
using System.Runtime.Serialization;
#endif

namespace WcfService
{
    public static class MessageInspector_CustomHeaderAuthentication
    {
        [DataContract(Name = "ResultOf{0}", Namespace = "http://www.contoso.com/wcfnamespace")]
        public class ResultObject<TEntity>
        {
            private string _resultMessage;

            public static ResultObject<T> CreateSuccessObject<T>()
            {
                return new ResultObject<T>
                {
                    Result = default(T),
                    ErrorCode = (int)MessageInspector_CustomHeaderAuthentication.ErrorCode.Ok,
                    HttpStatusCode = System.Net.HttpStatusCode.OK,
                    ResultMessage = MessageInspector_CustomHeaderAuthentication.ResultMessage.GetErrorDescription(MessageInspector_CustomHeaderAuthentication.ErrorCode.Ok)
                };
            }

            public static ResultObject<T> CreateFailureObject<T>()
            {
                return new ResultObject<T>
                {
                    Result = default(T),
                    ErrorCode = (int)MessageInspector_CustomHeaderAuthentication.ErrorCode.UserNotAuthenticated,
                    HttpStatusCode = System.Net.HttpStatusCode.Unauthorized,
                    ResultMessage = MessageInspector_CustomHeaderAuthentication.ResultMessage.GetErrorDescription(MessageInspector_CustomHeaderAuthentication.ErrorCode.UserNotAuthenticated)
                };
            }

            [DataMember]
            public int ErrorCode { get; set; }

            [DataMember]
            public string ResultMessage
            {
                get
                {
                    return _resultMessage;
                }
                set
                {
                    _resultMessage = value;
                }
            }

            [DataMember]
            public System.Net.HttpStatusCode HttpStatusCode { get; set; }

            [DataMember(Name = "Result")]
            public TEntity Result { get; set; }
        }

        public static class ResultMessage
        {
            public static string GetErrorDescription(ErrorCode errorCode)
            {
                switch (errorCode)
                {
                    case ErrorCode.Ok:
                        return "Authentication Succeeded";

                    case ErrorCode.UserNotAuthenticated:
                        return "Authentication Failed";
                }
                return "Unexpected exception";
            }
        }

        public enum ErrorCode
        {
            Ok = 0,
            UserNotAuthenticated = 0x191
        }
    }
}
