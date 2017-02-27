// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.ServiceModel;

using Infrastructure.Common;
using Xunit;

public class ExtensionCollectionTest
{
    [WcfFact]
    public static void ExtensionCollectionPublicMembersTest()
    {
        ExtensionCollection<IMyExtensibleObject> collection = new ExtensionCollection<IMyExtensibleObject>(new MyExtensibleObject(), "syncRoot");

        collection.Add(new MyExtension1());
        collection.Add(new MyExtension2());

        Assert.True(collection.Count == 2, $"Expected the collection to contain 2 items, instead it contained '{collection.Count}' items.");

        IMyExtension result = collection.Find<IMyExtension>();
        Assert.NotNull(result);

        Collection<IMyExtension> myCollection = collection.FindAll<IMyExtension>();
        Assert.True(myCollection.Count == 2, $"Expected the collection to contain 2 items of type 'IMyExtension', instead it contained: '{myCollection.Count}' items.");
    }

    public interface IMyExtensibleObject : IExtensibleObject<IMyExtensibleObject> { }

    public class MyExtensibleObject : IMyExtensibleObject
    {
        public IExtensionCollection<IMyExtensibleObject> Extensions
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    public interface IMyExtension : IExtension<IMyExtensibleObject> { }

    public class MyExtension1 : IMyExtension
    {
        private IMyExtensibleObject _ownerAttached;
        private IMyExtensibleObject _ownerDetached;

        public void Attach(IMyExtensibleObject owner)
        {
            _ownerAttached = owner;
        }

        public void Detach(IMyExtensibleObject owner)
        {
            _ownerDetached = owner;
        }
    }

    public class MyExtension2 : MyExtension1
    {
    }
}
