using System;
using System.Collections;
using System.Collections.Generic;
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

    public class RecursiveCollection : ICollection<RecursiveCollection>
    {
        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(RecursiveCollection item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(RecursiveCollection item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(RecursiveCollection[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<RecursiveCollection> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(RecursiveCollection item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
