using System;
using System.Runtime.Serialization;

namespace BinLib
{
    [DataContract]
    public class BinLibrary
    {
        [DataMember]
        public string Value { get; set; }

        public BinLibrary()
        {
        }
    }
}
