using System.Threading.Tasks;
using CoreWCF;

namespace WCFCorePerfService
{
    [ServiceContract]
    public interface ISayHello
    {
        [OperationContract]
        Task<string> HelloAsync(string name);
    }   
}
