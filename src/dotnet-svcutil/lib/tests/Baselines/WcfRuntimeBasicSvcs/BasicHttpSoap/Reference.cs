//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BasicHttpSoap_NS
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BasicHttpSoap_NS.IWcfSoapService")]
    public interface IWcfSoapService
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWcfService/CombineStringXmlSerializerFormatSoap", ReplyAction="http://tempuri.org/IWcfSoapService/CombineStringXmlSerializerFormatSoapResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        System.Threading.Tasks.Task<string> CombineStringXmlSerializerFormatSoapAsync(string message1, string message2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWcfService/EchoComositeTypeXmlSerializerFormatSoap", ReplyAction="http://tempuri.org/IWcfSoapService/EchoComositeTypeXmlSerializerFormatSoapRespons" +
            "e")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(AdditionalData))]
        System.Threading.Tasks.Task<BasicHttpSoap_NS.SoapComplexType> EchoComositeTypeXmlSerializerFormatSoapAsync(BasicHttpSoap_NS.SoapComplexType c);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWcfService/ProcessCustomerData", ReplyAction="http://tempuri.org/IWcfSoapService/ProcessCustomerDataResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(AdditionalData))]
        [return: System.ServiceModel.MessageParameterAttribute(Name="ProcessCustomerDataReturn")]
        System.Threading.Tasks.Task<string> ProcessCustomerDataAsync(BasicHttpSoap_NS.CustomerObject CustomerData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWcfService/Ping", ReplyAction="http://tempuri.org/IWcfSoapService/PingResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(AdditionalData))]
        [return: System.ServiceModel.MessageParameterAttribute(Name="Return")]
        System.Threading.Tasks.Task<int> PingAsync(string Pinginfo);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="http://tempuri.org/encoded")]
    public partial class SoapComplexType
    {
        
        private bool boolValueField;
        
        private string stringValueField;
        
        /// <remarks/>
        public bool BoolValue
        {
            get
            {
                return this.boolValueField;
            }
            set
            {
                this.boolValueField = value;
            }
        }
        
        /// <remarks/>
        public string StringValue
        {
            get
            {
                return this.stringValueField;
            }
            set
            {
                this.stringValueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="WcfService")]
    public partial class AdditionalData
    {
        
        private string fieldField;
        
        /// <remarks/>
        public string Field
        {
            get
            {
                return this.fieldField;
            }
            set
            {
                this.fieldField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="WcfService")]
    public partial class CustomerObject
    {
        
        private string nameField;
        
        private object dataField;
        
        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public object Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    public interface IWcfSoapServiceChannel : BasicHttpSoap_NS.IWcfSoapService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    public partial class WcfSoapServiceClient : System.ServiceModel.ClientBase<BasicHttpSoap_NS.IWcfSoapService>, BasicHttpSoap_NS.IWcfSoapService
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public WcfSoapServiceClient() : 
                base(WcfSoapServiceClient.GetDefaultBinding(), WcfSoapServiceClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.Basic_IWcfSoapService.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public WcfSoapServiceClient(EndpointConfiguration endpointConfiguration) : 
                base(WcfSoapServiceClient.GetBindingForEndpoint(endpointConfiguration), WcfSoapServiceClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public WcfSoapServiceClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(WcfSoapServiceClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public WcfSoapServiceClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(WcfSoapServiceClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public WcfSoapServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<string> CombineStringXmlSerializerFormatSoapAsync(string message1, string message2)
        {
            return base.Channel.CombineStringXmlSerializerFormatSoapAsync(message1, message2);
        }
        
        public System.Threading.Tasks.Task<BasicHttpSoap_NS.SoapComplexType> EchoComositeTypeXmlSerializerFormatSoapAsync(BasicHttpSoap_NS.SoapComplexType c)
        {
            return base.Channel.EchoComositeTypeXmlSerializerFormatSoapAsync(c);
        }
        
        public System.Threading.Tasks.Task<string> ProcessCustomerDataAsync(BasicHttpSoap_NS.CustomerObject CustomerData)
        {
            return base.Channel.ProcessCustomerDataAsync(CustomerData);
        }
        
        public System.Threading.Tasks.Task<int> PingAsync(string Pinginfo)
        {
            return base.Channel.PingAsync(Pinginfo);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.Basic_IWcfSoapService))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        public static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.Basic_IWcfSoapService))
            {
                return new System.ServiceModel.EndpointAddress("http://wcfcoresrv53.westus3.cloudapp.azure.com/WcfTestService1/BasicHttpSoap.svc/" +
                        "Basic");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        public static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return WcfSoapServiceClient.GetBindingForEndpoint(EndpointConfiguration.Basic_IWcfSoapService);
        }
        
        public static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return WcfSoapServiceClient.GetEndpointAddress(EndpointConfiguration.Basic_IWcfSoapService);
        }
        
        public enum EndpointConfiguration
        {
            
            Basic_IWcfSoapService,
        }
    }
}