// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WcfService
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface ISessionTestsDefaultService
    {
        [OperationContract]
        int MethodAInitiating(int a);

        [OperationContract]
        int MethodBNonInitiating(int b);

        [OperationContract]
        SessionTestsCompositeType MethodCTerminating();
    }

    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface ISessionTestsShortTimeoutService : ISessionTestsDefaultService
    {
    }
}

[DataContract]
public class SessionTestsCompositeType
{
    [DataMember]
    public int MethodAValue { get; set; }
    [DataMember]
    public int MethodBValue { get; set; }
}