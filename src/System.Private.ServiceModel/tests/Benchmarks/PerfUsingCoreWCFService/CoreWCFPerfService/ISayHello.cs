using CoreWCF;
using System.Threading.Tasks;

namespace WCFCorePerfService
{
    [ServiceContract]
    public interface ISayHello
    {
        [OperationContract]
        Task<string> HelloAsync(string name);
    }   
}
