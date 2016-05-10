// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections;
using System.Collections.Generic;

namespace System.ServiceModel.Tests.Common
{
    public class MemberDataSet<TParam> : MemberDataSet
    {
        public void Add(TParam p)
        {
            AddItem(p);
        }
    }

    public class MemberDataSet<TParam1, TParam2> : MemberDataSet
    {
        public void Add(TParam1 p1, TParam2 p2)
        {
            AddItem(p1, p2);
        }
    }

    public class MemberDataSet<TParam1, TParam2, TParam3> : MemberDataSet
    {
        public void Add(TParam1 p1, TParam2 p2, TParam3 p3)
        {
            AddItem(p1, p2, p3);
        }
    }

    public abstract class MemberDataSet : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>();

        protected void AddItem(params object[] values)
        {
            _data.Add(values);
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
