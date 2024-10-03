//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BasicHttpDocLitSingleNs_NS
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.Runtime.Serialization.DataContractAttribute(Name="IntParams", Namespace="http://contoso.com/calc")]
    public partial class IntParams : object
    {
        
        private int P1Field;
        
        private int P2Field;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int P1
        {
            get
            {
                return this.P1Field;
            }
            set
            {
                this.P1Field = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int P2
        {
            get
            {
                return this.P2Field;
            }
            set
            {
                this.P2Field = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FloatParams", Namespace="http://contoso.com/calc")]
    public partial class FloatParams : object
    {
        
        private float P1Field;
        
        private float P2Field;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public float P1
        {
            get
            {
                return this.P1Field;
            }
            set
            {
                this.P1Field = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public float P2
        {
            get
            {
                return this.P2Field;
            }
            set
            {
                this.P2Field = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ByteParams", Namespace="http://contoso.com/calc")]
    public partial class ByteParams : object
    {
        
        private byte P1Field;
        
        private byte P2Field;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public byte P1
        {
            get
            {
                return this.P1Field;
            }
            set
            {
                this.P1Field = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public byte P2
        {
            get
            {
                return this.P2Field;
            }
            set
            {
                this.P2Field = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://contoso.com/calc", ConfigurationName="BasicHttpDocLitSingleNs_NS.ICalculatorDocLit")]
    public interface ICalculatorDocLit
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://contoso.com/calc/ICalculatorDocLit/Sum2", ReplyAction="http://contoso.com/calc/ICalculatorDocLit/Sum2Response")]
        System.Threading.Tasks.Task<int> Sum2Async(int i, int j);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://contoso.com/calc/ICalculatorDocLit/Sum", ReplyAction="http://contoso.com/calc/ICalculatorDocLit/SumResponse")]
        System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.SumResponse> SumAsync(BasicHttpDocLitSingleNs_NS.SumRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://contoso.com/calc/ICalculatorDocLit/Divide", ReplyAction="http://contoso.com/calc/ICalculatorDocLit/DivideResponse")]
        System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.DivideResponse> DivideAsync(BasicHttpDocLitSingleNs_NS.DivideRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://contoso.com/calc/ICalculatorDocLit/Concatenate", ReplyAction="http://contoso.com/calc/ICalculatorDocLit/ConcatenateResponse")]
        System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.ConcatenateResponse> ConcatenateAsync(BasicHttpDocLitSingleNs_NS.ConcatenateRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://contoso.com/calc/ICalculatorDocLit/AddIntParams", ReplyAction="http://contoso.com/calc/ICalculatorDocLit/AddIntParamsResponse")]
        System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.AddIntParamsResponse> AddIntParamsAsync(BasicHttpDocLitSingleNs_NS.AddIntParamsRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://contoso.com/calc/ICalculatorDocLit/GetAndRemoveIntParams", ReplyAction="http://contoso.com/calc/ICalculatorDocLit/GetAndRemoveIntParamsResponse")]
        System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.GetAndRemoveIntParamsResponse> GetAndRemoveIntParamsAsync(BasicHttpDocLitSingleNs_NS.GetAndRemoveIntParamsRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://contoso.com/calc/ICalculatorDocLit/ReturnInputDateTime", ReplyAction="http://contoso.com/calc/ICalculatorDocLit/ReturnInputDateTimeResponse")]
        System.Threading.Tasks.Task<System.DateTime> ReturnInputDateTimeAsync(System.DateTime dt);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://contoso.com/calc/ICalculatorDocLit/CreateSet", ReplyAction="http://contoso.com/calc/ICalculatorDocLit/CreateSetResponse")]
        System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.CreateSetResponse> CreateSetAsync(BasicHttpDocLitSingleNs_NS.CreateSetRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SumRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Sum", Namespace="http://contoso.com/calc", Order=0)]
        public BasicHttpDocLitSingleNs_NS.SumRequestBody Body;
        
        public SumRequest()
        {
        }
        
        public SumRequest(BasicHttpDocLitSingleNs_NS.SumRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://contoso.com/calc")]
    public partial class SumRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public BasicHttpDocLitSingleNs_NS.IntParams par;
        
        public SumRequestBody()
        {
        }
        
        public SumRequestBody(BasicHttpDocLitSingleNs_NS.IntParams par)
        {
            this.par = par;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SumResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="SumResponse", Namespace="http://contoso.com/calc", Order=0)]
        public BasicHttpDocLitSingleNs_NS.SumResponseBody Body;
        
        public SumResponse()
        {
        }
        
        public SumResponse(BasicHttpDocLitSingleNs_NS.SumResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://contoso.com/calc")]
    public partial class SumResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public int SumResult;
        
        public SumResponseBody()
        {
        }
        
        public SumResponseBody(int SumResult)
        {
            this.SumResult = SumResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class DivideRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Divide", Namespace="http://contoso.com/calc", Order=0)]
        public BasicHttpDocLitSingleNs_NS.DivideRequestBody Body;
        
        public DivideRequest()
        {
        }
        
        public DivideRequest(BasicHttpDocLitSingleNs_NS.DivideRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://contoso.com/calc")]
    public partial class DivideRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public BasicHttpDocLitSingleNs_NS.FloatParams par;
        
        public DivideRequestBody()
        {
        }
        
        public DivideRequestBody(BasicHttpDocLitSingleNs_NS.FloatParams par)
        {
            this.par = par;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class DivideResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="DivideResponse", Namespace="http://contoso.com/calc", Order=0)]
        public BasicHttpDocLitSingleNs_NS.DivideResponseBody Body;
        
        public DivideResponse()
        {
        }
        
        public DivideResponse(BasicHttpDocLitSingleNs_NS.DivideResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://contoso.com/calc")]
    public partial class DivideResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public float DivideResult;
        
        public DivideResponseBody()
        {
        }
        
        public DivideResponseBody(float DivideResult)
        {
            this.DivideResult = DivideResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ConcatenateRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Concatenate", Namespace="http://contoso.com/calc", Order=0)]
        public BasicHttpDocLitSingleNs_NS.ConcatenateRequestBody Body;
        
        public ConcatenateRequest()
        {
        }
        
        public ConcatenateRequest(BasicHttpDocLitSingleNs_NS.ConcatenateRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://contoso.com/calc")]
    public partial class ConcatenateRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public BasicHttpDocLitSingleNs_NS.IntParams par;
        
        public ConcatenateRequestBody()
        {
        }
        
        public ConcatenateRequestBody(BasicHttpDocLitSingleNs_NS.IntParams par)
        {
            this.par = par;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ConcatenateResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="ConcatenateResponse", Namespace="http://contoso.com/calc", Order=0)]
        public BasicHttpDocLitSingleNs_NS.ConcatenateResponseBody Body;
        
        public ConcatenateResponse()
        {
        }
        
        public ConcatenateResponse(BasicHttpDocLitSingleNs_NS.ConcatenateResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://contoso.com/calc")]
    public partial class ConcatenateResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string ConcatenateResult;
        
        public ConcatenateResponseBody()
        {
        }
        
        public ConcatenateResponseBody(string ConcatenateResult)
        {
            this.ConcatenateResult = ConcatenateResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class AddIntParamsRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="AddIntParams", Namespace="http://contoso.com/calc", Order=0)]
        public BasicHttpDocLitSingleNs_NS.AddIntParamsRequestBody Body;
        
        public AddIntParamsRequest()
        {
        }
        
        public AddIntParamsRequest(BasicHttpDocLitSingleNs_NS.AddIntParamsRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://contoso.com/calc")]
    public partial class AddIntParamsRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string guid;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public BasicHttpDocLitSingleNs_NS.IntParams par;
        
        public AddIntParamsRequestBody()
        {
        }
        
        public AddIntParamsRequestBody(string guid, BasicHttpDocLitSingleNs_NS.IntParams par)
        {
            this.guid = guid;
            this.par = par;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class AddIntParamsResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="AddIntParamsResponse", Namespace="http://contoso.com/calc", Order=0)]
        public BasicHttpDocLitSingleNs_NS.AddIntParamsResponseBody Body;
        
        public AddIntParamsResponse()
        {
        }
        
        public AddIntParamsResponse(BasicHttpDocLitSingleNs_NS.AddIntParamsResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class AddIntParamsResponseBody
    {
        
        public AddIntParamsResponseBody()
        {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetAndRemoveIntParamsRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetAndRemoveIntParams", Namespace="http://contoso.com/calc", Order=0)]
        public BasicHttpDocLitSingleNs_NS.GetAndRemoveIntParamsRequestBody Body;
        
        public GetAndRemoveIntParamsRequest()
        {
        }
        
        public GetAndRemoveIntParamsRequest(BasicHttpDocLitSingleNs_NS.GetAndRemoveIntParamsRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://contoso.com/calc")]
    public partial class GetAndRemoveIntParamsRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string guid;
        
        public GetAndRemoveIntParamsRequestBody()
        {
        }
        
        public GetAndRemoveIntParamsRequestBody(string guid)
        {
            this.guid = guid;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetAndRemoveIntParamsResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetAndRemoveIntParamsResponse", Namespace="http://contoso.com/calc", Order=0)]
        public BasicHttpDocLitSingleNs_NS.GetAndRemoveIntParamsResponseBody Body;
        
        public GetAndRemoveIntParamsResponse()
        {
        }
        
        public GetAndRemoveIntParamsResponse(BasicHttpDocLitSingleNs_NS.GetAndRemoveIntParamsResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://contoso.com/calc")]
    public partial class GetAndRemoveIntParamsResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public BasicHttpDocLitSingleNs_NS.IntParams GetAndRemoveIntParamsResult;
        
        public GetAndRemoveIntParamsResponseBody()
        {
        }
        
        public GetAndRemoveIntParamsResponseBody(BasicHttpDocLitSingleNs_NS.IntParams GetAndRemoveIntParamsResult)
        {
            this.GetAndRemoveIntParamsResult = GetAndRemoveIntParamsResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class CreateSetRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="CreateSet", Namespace="http://contoso.com/calc", Order=0)]
        public BasicHttpDocLitSingleNs_NS.CreateSetRequestBody Body;
        
        public CreateSetRequest()
        {
        }
        
        public CreateSetRequest(BasicHttpDocLitSingleNs_NS.CreateSetRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://contoso.com/calc")]
    public partial class CreateSetRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public BasicHttpDocLitSingleNs_NS.ByteParams par;
        
        public CreateSetRequestBody()
        {
        }
        
        public CreateSetRequestBody(BasicHttpDocLitSingleNs_NS.ByteParams par)
        {
            this.par = par;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class CreateSetResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="CreateSetResponse", Namespace="http://contoso.com/calc", Order=0)]
        public BasicHttpDocLitSingleNs_NS.CreateSetResponseBody Body;
        
        public CreateSetResponse()
        {
        }
        
        public CreateSetResponse(BasicHttpDocLitSingleNs_NS.CreateSetResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://contoso.com/calc")]
    public partial class CreateSetResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public byte[] CreateSetResult;
        
        public CreateSetResponseBody()
        {
        }
        
        public CreateSetResponseBody(byte[] CreateSetResult)
        {
            this.CreateSetResult = CreateSetResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    public interface ICalculatorDocLitChannel : BasicHttpDocLitSingleNs_NS.ICalculatorDocLit, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "99.99.99")]
    public partial class CalculatorDocLitClient : System.ServiceModel.ClientBase<BasicHttpDocLitSingleNs_NS.ICalculatorDocLit>, BasicHttpDocLitSingleNs_NS.ICalculatorDocLit
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public CalculatorDocLitClient() : 
                base(CalculatorDocLitClient.GetDefaultBinding(), CalculatorDocLitClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.Basic_ICalculatorDocLit.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public CalculatorDocLitClient(EndpointConfiguration endpointConfiguration) : 
                base(CalculatorDocLitClient.GetBindingForEndpoint(endpointConfiguration), CalculatorDocLitClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public CalculatorDocLitClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(CalculatorDocLitClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public CalculatorDocLitClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(CalculatorDocLitClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public CalculatorDocLitClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<int> Sum2Async(int i, int j)
        {
            return base.Channel.Sum2Async(i, j);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.SumResponse> BasicHttpDocLitSingleNs_NS.ICalculatorDocLit.SumAsync(BasicHttpDocLitSingleNs_NS.SumRequest request)
        {
            return base.Channel.SumAsync(request);
        }
        
        public System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.SumResponse> SumAsync(BasicHttpDocLitSingleNs_NS.IntParams par)
        {
            BasicHttpDocLitSingleNs_NS.SumRequest inValue = new BasicHttpDocLitSingleNs_NS.SumRequest();
            inValue.Body = new BasicHttpDocLitSingleNs_NS.SumRequestBody();
            inValue.Body.par = par;
            return ((BasicHttpDocLitSingleNs_NS.ICalculatorDocLit)(this)).SumAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.DivideResponse> BasicHttpDocLitSingleNs_NS.ICalculatorDocLit.DivideAsync(BasicHttpDocLitSingleNs_NS.DivideRequest request)
        {
            return base.Channel.DivideAsync(request);
        }
        
        public System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.DivideResponse> DivideAsync(BasicHttpDocLitSingleNs_NS.FloatParams par)
        {
            BasicHttpDocLitSingleNs_NS.DivideRequest inValue = new BasicHttpDocLitSingleNs_NS.DivideRequest();
            inValue.Body = new BasicHttpDocLitSingleNs_NS.DivideRequestBody();
            inValue.Body.par = par;
            return ((BasicHttpDocLitSingleNs_NS.ICalculatorDocLit)(this)).DivideAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.ConcatenateResponse> BasicHttpDocLitSingleNs_NS.ICalculatorDocLit.ConcatenateAsync(BasicHttpDocLitSingleNs_NS.ConcatenateRequest request)
        {
            return base.Channel.ConcatenateAsync(request);
        }
        
        public System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.ConcatenateResponse> ConcatenateAsync(BasicHttpDocLitSingleNs_NS.IntParams par)
        {
            BasicHttpDocLitSingleNs_NS.ConcatenateRequest inValue = new BasicHttpDocLitSingleNs_NS.ConcatenateRequest();
            inValue.Body = new BasicHttpDocLitSingleNs_NS.ConcatenateRequestBody();
            inValue.Body.par = par;
            return ((BasicHttpDocLitSingleNs_NS.ICalculatorDocLit)(this)).ConcatenateAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.AddIntParamsResponse> BasicHttpDocLitSingleNs_NS.ICalculatorDocLit.AddIntParamsAsync(BasicHttpDocLitSingleNs_NS.AddIntParamsRequest request)
        {
            return base.Channel.AddIntParamsAsync(request);
        }
        
        public System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.AddIntParamsResponse> AddIntParamsAsync(string guid, BasicHttpDocLitSingleNs_NS.IntParams par)
        {
            BasicHttpDocLitSingleNs_NS.AddIntParamsRequest inValue = new BasicHttpDocLitSingleNs_NS.AddIntParamsRequest();
            inValue.Body = new BasicHttpDocLitSingleNs_NS.AddIntParamsRequestBody();
            inValue.Body.guid = guid;
            inValue.Body.par = par;
            return ((BasicHttpDocLitSingleNs_NS.ICalculatorDocLit)(this)).AddIntParamsAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.GetAndRemoveIntParamsResponse> BasicHttpDocLitSingleNs_NS.ICalculatorDocLit.GetAndRemoveIntParamsAsync(BasicHttpDocLitSingleNs_NS.GetAndRemoveIntParamsRequest request)
        {
            return base.Channel.GetAndRemoveIntParamsAsync(request);
        }
        
        public System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.GetAndRemoveIntParamsResponse> GetAndRemoveIntParamsAsync(string guid)
        {
            BasicHttpDocLitSingleNs_NS.GetAndRemoveIntParamsRequest inValue = new BasicHttpDocLitSingleNs_NS.GetAndRemoveIntParamsRequest();
            inValue.Body = new BasicHttpDocLitSingleNs_NS.GetAndRemoveIntParamsRequestBody();
            inValue.Body.guid = guid;
            return ((BasicHttpDocLitSingleNs_NS.ICalculatorDocLit)(this)).GetAndRemoveIntParamsAsync(inValue);
        }
        
        public System.Threading.Tasks.Task<System.DateTime> ReturnInputDateTimeAsync(System.DateTime dt)
        {
            return base.Channel.ReturnInputDateTimeAsync(dt);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.CreateSetResponse> BasicHttpDocLitSingleNs_NS.ICalculatorDocLit.CreateSetAsync(BasicHttpDocLitSingleNs_NS.CreateSetRequest request)
        {
            return base.Channel.CreateSetAsync(request);
        }
        
        public System.Threading.Tasks.Task<BasicHttpDocLitSingleNs_NS.CreateSetResponse> CreateSetAsync(BasicHttpDocLitSingleNs_NS.ByteParams par)
        {
            BasicHttpDocLitSingleNs_NS.CreateSetRequest inValue = new BasicHttpDocLitSingleNs_NS.CreateSetRequest();
            inValue.Body = new BasicHttpDocLitSingleNs_NS.CreateSetRequestBody();
            inValue.Body.par = par;
            return ((BasicHttpDocLitSingleNs_NS.ICalculatorDocLit)(this)).CreateSetAsync(inValue);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.Basic_ICalculatorDocLit))
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
            if ((endpointConfiguration == EndpointConfiguration.Basic_ICalculatorDocLit))
            {
                return new System.ServiceModel.EndpointAddress("http://wcfcoresrv53.westus3.cloudapp.azure.com/WcfTestService1/BasicHttpDocLitSin" +
                        "gleNs.svc/Basic");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        public static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return CalculatorDocLitClient.GetBindingForEndpoint(EndpointConfiguration.Basic_ICalculatorDocLit);
        }
        
        public static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return CalculatorDocLitClient.GetEndpointAddress(EndpointConfiguration.Basic_ICalculatorDocLit);
        }
        
        public enum EndpointConfiguration
        {
            
            Basic_ICalculatorDocLit,
        }
    }
}