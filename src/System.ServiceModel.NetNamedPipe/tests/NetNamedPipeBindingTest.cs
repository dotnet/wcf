// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Reflection;
using System.Runtime.Versioning;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static class NetNamedPipeBindingTest
{
    [WcfFact]
    [SupportedOSPlatform("windows")]
    public static void AppContextSwitch_useSha1InPipeConnectionGetHashAlgorithm()
    {
        Type t = Assembly.GetAssembly(typeof(NamedPipeTransportBindingElement))
                            .GetType(typeof(NamedPipeTransportBindingElement).Namespace + ".PipeUri");
        MethodInfo m = t.GetMethod(
            "BuildSharedMemoryName",
            BindingFlags.Static | BindingFlags.Public,
            binder: null,
            types: new Type[] { typeof(string), typeof(string), typeof(bool) },
            modifiers: null
        );

        //swtich on
        AppContext.SetSwitch("Switch.System.ServiceModel.UseSha1InPipeConnectionGetHashAlgorithm", true);
        string result = (string)m.Invoke(t, new object[] { "hostname", new string('a', 128), true });
        Assert.Equal(45, result.Length);

        //switch off
        FieldInfo f = t.GetField("s_useSha1InPipeConnectionGetHashAlgorithm", BindingFlags.Static | BindingFlags.NonPublic);
        f.SetValue(t, false);
        result = (string)m.Invoke(t, new object[] { "hostname", new string('a', 128), true });
        Assert.Equal(61, result.Length);
    }
}
