using System.ServiceModel;
using System.Threading.Tasks;

namespace WCFCorePerfClient
{
    [ServiceContract]
    public interface ISayHello
    {
        [OperationContract]
        Task<string> HelloAsync(string name);

        [OperationContract]
        string Hello(string name);
    }   
}
