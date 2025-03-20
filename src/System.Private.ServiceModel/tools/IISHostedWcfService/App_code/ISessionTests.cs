// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System.Runtime.Serialization;
using System.ServiceModel;
#endif

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

[ServiceContract(CallbackContract = typeof(ISessionTestsDuplexCallback), SessionMode = SessionMode.Required)]
public interface ISessionTestsDuplexService
{
    [OperationContract(IsInitiating = true, IsTerminating = false)]
    int NonTerminatingMethodCallingDuplexCallbacks(
       int callsToClientCallbackToMake,
       int callsToTerminatingClientCallbackToMake,
       int callsToClientSideOnlyTerminatingClientCallbackToMake,
       int callsToNonTerminatingMethodToMakeInsideClientCallback,
       int callsToTerminatingMethodToMakeInsideClientCallback);

    [OperationContract(IsInitiating = true, IsTerminating = true)]
    int TerminatingMethodCallingDuplexCallbacks(
        int callsToClientCallbackToMake,
        int callsToTerminatingClientCallbackToMake,
        int callsToClientSideOnlyTerminatingClientCallbackToMake,
        int callsToNonTerminatingMethodToMakeInsideClientCallback,
        int callsToTerminatingMethodToMakeInsideClientCallback);

    [OperationContract]
    int NonTerminatingMethod();

    [OperationContract]
    int TerminatingMethod();
}

[ServiceContract(SessionMode = SessionMode.Required)]
public interface ISessionTestsDuplexCallback
{
    [OperationContract(IsInitiating = true, IsTerminating = false)]
    int ClientCallback(int callsToNonTerminatingMethodToMake, int callsToTerminatingMethodToMake);

    [OperationContract(IsInitiating = true, IsTerminating = true)]
    int TerminatingClientCallback(int callsToNonTerminatingMethodToMake, int callsToTerminatingMethodToMake);

    // note IsTerminating = false while the client callback has IsTerminating = true
    [OperationContract(IsInitiating = true, IsTerminating = false)]
    int ClientSideOnlyTerminatingClientCallback(int callsToNonTerminatingMethodToMake, int callsToTerminatingMethodToMake);
}
