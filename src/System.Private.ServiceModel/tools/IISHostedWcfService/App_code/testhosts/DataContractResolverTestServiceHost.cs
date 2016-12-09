using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace WcfService
{
    public class DataContractResolverTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            DataContractResolverTestServiceHost serviceHost = new DataContractResolverTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class DataContractResolverTestServiceHost : TestServiceHostBase<IDataContractResolverService>
    {
        protected override string Address { get { return "DataContractResolver"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            this.AddServiceEndpoint(typeof(IDataContractResolverService), new BasicHttpBinding(), "");
            foreach (ServiceEndpoint endpoint in this.Description.Endpoints)
            {
                foreach (OperationDescription operation in endpoint.Contract.Operations)
                {
                    DataContractSerializerOperationBehavior behavior =
                        operation.OperationBehaviors.FirstOrDefault(
                            x => x.GetType() == typeof(DataContractSerializerOperationBehavior)) as DataContractSerializerOperationBehavior;
                    behavior.DataContractResolver = new ManagerDataContractResolver();
                }
            }
        }
        public DataContractResolverTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}