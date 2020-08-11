
using System.ServiceModel;
using TypesLib;
using BinLib;

namespace TypeReuseService
{
    [ServiceContract]
    public interface ITypeReuseSvc
    {
        [OperationContract]
        BinLibrary GetData(int value);

        [OperationContract]
        TypeReuseCompositeType GetDataUsingDataContract(TypeReuseCompositeType composite);
    }
}
