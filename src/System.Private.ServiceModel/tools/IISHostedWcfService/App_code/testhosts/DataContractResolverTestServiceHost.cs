using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace WcfService
{

    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "DataContractResolver.svc")]
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
        public DataContractResolverTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(DataContractResolverService), baseAddresses)
        {
        }
    }
}
