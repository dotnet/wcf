// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using System.IO;
using System.ServiceModel;
using System.Runtime.Serialization;
using System;

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

        [OperationContract(Name = "GetData", AsyncPattern = true)]
        IAsyncResult BeginGetData(int value, AsyncCallback callback, object state);
        string EndGetData(IAsyncResult iar);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        private bool _boolValue = true;
        private string _stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return _boolValue; }
            set { _boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return _stringValue; }
            set { _stringValue = value; }
        }
    }

    [ServiceContract(CallbackContract = typeof(IDuplexCallback))]
    public interface IDuplexService
    {
        [OperationContract]
        int SetData(int value, int callbackCallsToMake);

        [OperationContract(Name = "SetData")]
        Task<int> SetDataAsync(int value, int callbackCallsToMake);

        [OperationContract]
        int GetAsyncCallbackData(int value, int asyncCallbacksToMake);
        [OperationContract(Name = "GetAsyncCallbackData")]
        Task<int> GetAsyncCallbackDataAsync(int value, int asyncCallbacksToMake);
    }

    public interface IDuplexCallback
    {
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

        [OperationContract(Action = "http://tempuri.org/IStreamingService/EchoStream", ReplyAction = "http://tempuri.org/IStreamingService/EchoStreamResponse")]
        Stream EchoStream(Stream stream);


        [OperationContract(Name = "GetStreamFromInt")]
        Task<Stream> GetStreamFromIntAsync(int data);

        [OperationContract(Name = "GetIntFromStream")]
        Task<int> GetIntFromStreamAsync(Stream stream);

        [OperationContract(Name = "EchoStream", Action = "http://tempuri.org/IStreamingService/EchoStream", ReplyAction = "http://tempuri.org/IStreamingService/EchoStreamResponse")]
        Task<Stream> EchoStreamAsync(Stream stream);
    }

    [ServiceContract(CallbackContract = typeof(IDuplexStreamingCallback))]
    public interface IDuplexStreamingService : IStreamingService
    {
    }

    public interface IDuplexStreamingCallback
    {
        [OperationContract]
        Stream GetStreamFromInt(int data);

        [OperationContract]
        int GetIntFromStream(Stream stream);

        [OperationContract]
        Stream EchoStream(Stream stream);
    }
}