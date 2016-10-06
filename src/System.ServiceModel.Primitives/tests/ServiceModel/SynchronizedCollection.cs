// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using Infrastructure.Common;
using Xunit;

public class SynchronizedCollectionTest
{
    [WcfFact]
    // This Unit test is based from full framework test
    public static void SynchronizedCollectionPublicMembersTest()
    {
        SynchronizedCollection<int> coll = new SynchronizedCollection<int>();
        int size = 100;
        for (int i = 0; i < size; i++)
            coll.Add(i);

        Assert.True(coll.Count == size, string.Format("collection count was wrong! Expected: {0} got: {1}", size, coll.Count));

        for (int i = 0; i < size; i++)
        {
            Assert.True(coll[i] == i, string.Format("coll element {0} was wrong! Expected: {1} got: {2} ", i, i, coll[i]));
            Assert.True(coll.IndexOf(i) == i, string.Format("coll IndexOf wasn't right! Expected: {0} got: {1}" , i, coll.IndexOf(i)));
            Assert.True(coll.Contains(i), string.Format("coll Contains failed to find the value {0}.", i));
        }

        SynchronizedCollection<int> coll2 = new SynchronizedCollection<int>(new object(), new List<int>(coll));
        for (int i = 0; i < size; i++)
        {
            Assert.True(coll2[i] == i, string.Format("coll2 element was wrong! expected: {0} got: {1} ", i, coll2[i]));
        }

        SynchronizedCollection<int> coll3 = new SynchronizedCollection<int>(new object(), 1, 2, 3, 4, 5 , 6);
        for (int i = 0; i < 5; i++)
        {
            Assert.True(coll3[i] == i + 1, string.Format("coll3 element {0} was wrong! expected: {1} got: {2}", i, i+1, coll3[i]));
        }
        int newValue = 80;
        coll3[5] = newValue;
        Assert.True(coll3[5] == newValue);

        IEnumerator <int> e = coll.GetEnumerator();
        int n = 0;
        while (e.MoveNext())
        {
            Assert.True(e.Current.Equals(n++), string.Format("Expected: {0}, got:{1}", n-1, e.Current));
        }

        Assert.True(n == 100, string.Format("Expect number of elements: {0}, got:{1}", 100, n));

        int[] array = new int[size + 1];
        coll.CopyTo(array, 1);
        for (int i = 0; i < size; i++)
        {
            Assert.True(array[i + 1] == i, string.Format("After CopyTo, Element {0} was wrong!  Expected: {1} got:  {2}", i, i+1, array[i + 1]));
        }

        coll.Add(coll.Count);
        coll.Insert(0, -1);
        coll.RemoveAt(0);
        coll.Remove(coll.Count - 1);
        Assert.True(coll.Count == size, string.Format("Expect number of elements after modification: {0}, got: {1}", size, coll.Count));

        for (int i = 0; i < size; i++)
        {
            Assert.True(coll[i] == i, string.Format("coll element was wrong after modification! Expected: {0} got: {1} ", i, coll[i]));
        }

        coll.Clear();
        Assert.True(coll.Count == 0, string.Format("Clear operation failed!, expected: 0, actual {0}", coll.Count));

        // Negative cases
        Assert.Throws<ArgumentNullException>("syncRoot", () =>
        {
            new SynchronizedCollection<int>(null);
        });

        Assert.Throws<ArgumentNullException>("list", () =>
        {
            new SynchronizedCollection<int>(new object(), null);
        });

        Assert.Throws<ArgumentNullException>("syncRoot", () =>
        {
            new SynchronizedCollection<int>(null, new List<int>());
        });

        Assert.Throws<ArgumentNullException>("syncRoot", () =>
        {
            new SynchronizedCollection<int>(null, 1, 2, 3, 4);
        });

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            coll[1000] = 5;
        });

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            coll[-1] = 5;
        });

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            coll.Insert(1000, 5);
        });

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            coll.Insert(-1, 5);
        });

        Assert.False(coll.Remove(100000));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            coll.RemoveAt(-1);
        });

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            coll.RemoveAt(10000);
        });
    }
}
