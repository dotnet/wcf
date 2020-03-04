
using System;
using TypesLib;
using BinLib;

namespace TypeReuseService
{
    public class TypeReuseSvc : ITypeReuseSvc
    {
        public BinLibrary GetData(int value)
        {
            return new BinLibrary { Value = string.Format("You entered: {0}", value) };
        }

        public TypeReuseCompositeType GetDataUsingDataContract(TypeReuseCompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
