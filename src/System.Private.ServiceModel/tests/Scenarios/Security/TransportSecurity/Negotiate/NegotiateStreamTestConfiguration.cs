// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public static class NegotiateStreamTestConfiguration
{
    private static readonly string Default_NegotiateTestRealm = "DOMAIN.CONTOSO.COM";
    private static readonly string Default_NegotiateTestDomain = "DOMAIN";
    private static readonly string Default_NegotiateTestUserName = "testuser";
    private static readonly string Default_NegotiateTestPassword = "hunter2";
    private static readonly string Default_NegotiateTestSpn = "testhost.domain.contoso.com";
    private static readonly string Default_NegotiateTestUpn = "testuser@DOMAIN.CONTOSO.COM";

    // These property names must match the names used in TestProperties because
    // that is the set of name/value pairs from which this type is created.
    private const string NegotiateTestRealm_PropertyName = "NegotiateTestRealm";
    private const string NegotiateTestDomain_PropertyName = "NegotiateTestDomain";
    private const string NegotiateTestUserName_PropertyName = "NegotiateTestUserName";
    private const string NegotiateTestPassword_PropertyName = "NegotiateTestPassword";
    private const string NegotiateTestSpn_PropertyName = "NegotiateTestSpn";
    private const string NegotiateTestUpn_PropertyName = "NegotiateTestUpn";

    public static string NegotiateTestRealm { get; set; }
    public static string NegotiateTestDomain { get; set; }
    public static string NegotiateTestUserName { get; set; }
    public static string NegotiateTestPassword { get; set; }
    public static string NegotiateTestSpn { get; set; }
    public static string NegotiateTestUpn { get; set; }

    static NegotiateStreamTestConfiguration()
    {
        NegotiateTestRealm = Default_NegotiateTestRealm;
        NegotiateTestDomain = Default_NegotiateTestDomain;
        NegotiateTestUserName = Default_NegotiateTestUserName;
        NegotiateTestPassword = Default_NegotiateTestPassword;
        NegotiateTestSpn = Default_NegotiateTestSpn;
        NegotiateTestUpn = Default_NegotiateTestUpn;
    }

    //public NegotiateStreamTestConfiguration(Dictionary<string, string> properties) 
    //    : this(new NegotiateStreamTestConfiguration(), properties)
    //{
    //}

    // This ctor accepts an existing BridgeConfiguration and a set of name/value pairs.
    // It will create a new BridgeConfiguration instance that is a clone of the existing
    // one and will overwrite any properties with corresponding entries found in the name/value pairs.
    //public NegotiateStreamTestConfiguration(NegotiateStreamTestConfiguration configuration, Dictionary<string, string> properties)
    //{
    //    NegotiateTestRealm = configuration.NegotiateTestRealm;
    //    NegotiateTestDomain = configuration.NegotiateTestDomain;
    //    NegotiateTestUsername = configuration.NegotiateTestUsername;
    //    NegotiateTestPassword = configuration.NegotiateTestPassword;
    //    NegotiateTestSpn = configuration.NegotiateTestSpn;
    //    NegotiateTestUpn = configuration.NegotiateTestUpn;

    //    if (properties != null)
    //    {
    //        string propertyValue = null;

    //        if (properties.TryGetValue(NegotiateTestRealm_PropertyName, out propertyValue))
    //        {
    //            NegotiateTestRealm = propertyValue;
    //        }

    //        if (properties.TryGetValue(NegotiateTestDomain_PropertyName, out propertyValue))
    //        {
    //            NegotiateTestDomain = propertyValue;
    //        }

    //        if (properties.TryGetValue(NegotiateTestUsername_PropertyName, out propertyValue))
    //        {
    //            NegotiateTestUsername = propertyValue;
    //        }

    //        if (properties.TryGetValue(NegotiateTestPassword_PropertyName, out propertyValue))
    //        {
    //            NegotiateTestPassword = propertyValue;
    //        }

    //        if (properties.TryGetValue(NegotiateTestSpn_PropertyName, out propertyValue))
    //        {
    //            NegotiateTestSpn = propertyValue;
    //        }

    //        if (properties.TryGetValue(NegotiateTestUpn_PropertyName, out propertyValue))
    //        {
    //            NegotiateTestUpn = propertyValue;
    //        }
    //    }
    //}

    public static Dictionary<string, string> ToDictionary()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        result[NegotiateTestRealm_PropertyName] = NegotiateTestRealm;
        result[NegotiateTestDomain_PropertyName] = NegotiateTestDomain;
        result[NegotiateTestUserName_PropertyName] = NegotiateTestUserName;
        result[NegotiateTestPassword_PropertyName] = NegotiateTestPassword;
        result[NegotiateTestSpn_PropertyName] = NegotiateTestSpn;
        result[NegotiateTestUpn_PropertyName] = NegotiateTestUpn;
        
        return result;
    }

    public static new string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("  {0} : '{1}'{2}", NegotiateTestRealm_PropertyName, NegotiateTestRealm, Environment.NewLine)
          .AppendFormat("  {0} : '{1}'{2}", NegotiateTestDomain_PropertyName, NegotiateTestDomain, Environment.NewLine)
          .AppendFormat("  {0} : '{1}'{2}", NegotiateTestUserName_PropertyName, NegotiateTestUserName, Environment.NewLine)
          .AppendFormat("  {0} : '{1}'{2}", NegotiateTestPassword_PropertyName, NegotiateTestPassword, Environment.NewLine)
          .AppendFormat("  {0} : '{1}'{2}", NegotiateTestSpn_PropertyName, NegotiateTestSpn, Environment.NewLine)
          .AppendFormat("  {0} : '{1}'{2}", NegotiateTestUpn_PropertyName, NegotiateTestUpn, Environment.NewLine);
        return sb.ToString();
    }
}
