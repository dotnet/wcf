using System.ServiceModel;
using System.Threading.Tasks;

namespace WCFCorePerf
{
    [ServiceContract]
    public interface ISayHello
    {
        [OperationContract]
        Task<string> HelloAsync(string name);
    }   
}
