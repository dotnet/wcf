using System.Runtime.Serialization;

namespace CommonTypes
{
    [DataContract]
    public struct AnotherSharedType
    {
        [DataMember]
        public int SomeProperty { get; set; }

        [DataMember]
        public CustomSerializableType SomeXmlSerializableType { get; set; }
    }
}
