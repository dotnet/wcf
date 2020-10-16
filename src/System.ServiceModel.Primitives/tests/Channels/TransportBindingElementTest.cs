// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static class TransportBindingElementTest
{
    [WcfFact]
    public static void GetSet_MaxBufferPoolSize()
    {
        MyBindingElement element = new MyBindingElement();
        Assert.Equal(512 * 1024, element.MaxBufferPoolSize);

        element.MaxBufferPoolSize = 1024;
        Assert.Equal(1024, element.MaxBufferPoolSize);
    }

    public class MyBindingElement : TransportBindingElement
    {
        public override string Scheme => throw new NotImplementedException();

        public override BindingElement Clone()
        {
            throw new NotImplementedException();
        }
    }
}
