using System.ServiceModel;
using System.Threading.Tasks;

namespace WCFPerfClient
{
    [ServiceContract]
    public interface ISayHello
    {
        [OperationContract]
        Task<string> HelloAsync(string name);
    }

    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        Task<string> GetDataAsync(int value);      
    }
}
