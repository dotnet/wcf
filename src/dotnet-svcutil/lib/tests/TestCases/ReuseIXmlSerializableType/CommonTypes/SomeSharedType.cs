using System.Runtime.Serialization;

namespace CommonTypes
{
    [DataContract]
    public struct SomeSharedType
    {
        [DataMember]
        public int SomeProperty { get; set; }
    }
}
