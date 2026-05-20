using System;
using System.Runtime.Serialization;

namespace TypesLib
{
    [DataContract]
    public class TypeReuseCompositeType
    {
        [DataMember]
        public bool BoolValue { get; set; } = true;

        [DataMember]
        public string StringValue { get; set; } = "Hello ";
    }
}
