// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using System.IO;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.CodeDom.Compiler;

namespace WcfService1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract(Name = "GetData")]
        Task<string> GetDataAsync(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }

    [ServiceContract (CallbackContract=typeof(IDuplexCallback))]
    public interface IDuplexService
    {
        //[OperationContract(IsOneWay = true)]
        [OperationContract]
        int SetData(int value, int callbackCallsToMake);

        [OperationContract(Name = "SetData")]
        Task<int> SetDataAsync(int value, int callbackCallsToMake);

        //
        [OperationContract]
        int GetAsyncCallbackData(int value, int asyncCallbacksToMake);
        [OperationContract(Name = "GetAsyncCallbackData")]
        Task<int> GetAsyncCallbackDataAsync(int value, int asyncCallbacksToMake);
    }

    public interface IDuplexCallback
    {
        //[OperationContract(IsOneWay = true)]
        [OperationContract]
        int EchoSetData(int value);

        [OperationContract]
        Task<int> EchoGetAsyncCallbackData(int value);
    }



    [ServiceContract]
    public interface IStreamingService
    {
        [OperationContract]
        Stream GetStreamFromInt(int data);

        [OperationContract]
        int GetIntFromStream(Stream stream);

        [OperationContractAttribute(Action = "http://tempuri.org/IStreamingService/EchoStream", ReplyAction = "http://tempuri.org/IStreamingService/EchoStreamResponse")]
        Stream EchoStream(Stream stream);

        [OperationContractAttribute(Action = "http://tempuri.org/IStreamingService/EchoStream", ReplyAction = "http://tempuri.org/IStreamingService/EchoStreamResponse")]
        Task<Stream> EchoStreamAsync(Stream stream);
    }
}
