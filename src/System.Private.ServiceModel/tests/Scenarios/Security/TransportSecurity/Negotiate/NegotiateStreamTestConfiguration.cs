// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Infrastructure.Common;

public class NegotiateStreamTestConfiguration
{
    //private static readonly string Default_NegotiateTestRealm = "DOMAIN.CONTOSO.COM";
    //private static readonly string Default_NegotiateTestDomain = "DOMAIN";
    //private static readonly string Default_NegotiateTestUserName = "testuser";
    //private static readonly string Default_NegotiateTestPassword = "hunter2";
    //private static readonly string Default_NegotiateTestSpn = "testhost.domain.contoso.com";
    //private static readonly string Default_NegotiateTestUpn = "testuser@DOMAIN.CONTOSO.COM";

    // These property names must match the names used in TestProperties because
    // that is the set of name/value pairs from which this type is created.
    private const string NegotiateTestRealm_PropertyName = "NegotiateTestRealm";
    private const string NegotiateTestDomain_PropertyName = "NegotiateTestDomain";
    private const string NegotiateTestUserName_PropertyName = "NegotiateTestUserName";
    private const string NegotiateTestPassword_PropertyName = "NegotiateTestPassword";
    private const string NegotiateTestSpn_PropertyName = "NegotiateTestSpn";
    private const string NegotiateTestUpn_PropertyName = "NegotiateTestUpn";

    public string NegotiateTestRealm { get; set; }
    public string NegotiateTestDomain { get; set; }
    public string NegotiateTestUserName { get; set; }
    public string NegotiateTestPassword { get; set; }
    public string NegotiateTestSpn { get; set; }
    public string NegotiateTestUpn { get; set; }

    private static NegotiateStreamTestConfiguration s_instance;

    private NegotiateStreamTestConfiguration()
    {
        NegotiateTestRealm = TestProperties.GetProperty(NegotiateTestRealm_PropertyName);
        NegotiateTestDomain = TestProperties.GetProperty(NegotiateTestDomain_PropertyName);
        NegotiateTestUserName = TestProperties.GetProperty(NegotiateTestUserName_PropertyName);
        NegotiateTestPassword = TestProperties.GetProperty(NegotiateTestPassword_PropertyName);
        NegotiateTestSpn = TestProperties.GetProperty(NegotiateTestSpn_PropertyName);
        NegotiateTestUpn = TestProperties.GetProperty(NegotiateTestUpn_PropertyName);
    }

    public static NegotiateStreamTestConfiguration Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new NegotiateStreamTestConfiguration();
            }
            return s_instance;
        }
    }

    public NegotiateStreamTestConfiguration(Dictionary<string, string> properties)
        : this(new NegotiateStreamTestConfiguration(), properties)
    {
    }

    // This ctor accepts an existing Configuration and a set of name/value pairs.
    // It will create a new Configuration instance that is a clone of the existing
    // one and will overwrite any properties with corresponding entries found in the name/value pairs.
    public NegotiateStreamTestConfiguration(NegotiateStreamTestConfiguration configuration, Dictionary<string, string> properties)
    {
        NegotiateTestRealm = configuration.NegotiateTestRealm;
        NegotiateTestDomain = configuration.NegotiateTestDomain;
        NegotiateTestUserName = configuration.NegotiateTestUserName;
        NegotiateTestPassword = configuration.NegotiateTestPassword;
        NegotiateTestSpn = configuration.NegotiateTestSpn;
        NegotiateTestUpn = configuration.NegotiateTestUpn;

        if (properties != null)
        {
            string propertyValue = null;

            if (properties.TryGetValue(NegotiateTestRealm_PropertyName, out propertyValue))
            {
                NegotiateTestRealm = propertyValue;
            }

            if (properties.TryGetValue(NegotiateTestDomain_PropertyName, out propertyValue))
            {
                NegotiateTestDomain = propertyValue;
            }

            if (properties.TryGetValue(NegotiateTestUserName_PropertyName, out propertyValue))
            {
                NegotiateTestUserName = propertyValue;
            }

            if (properties.TryGetValue(NegotiateTestPassword_PropertyName, out propertyValue))
            {
                NegotiateTestPassword = propertyValue;
            }

            if (properties.TryGetValue(NegotiateTestSpn_PropertyName, out propertyValue))
            {
                NegotiateTestSpn = propertyValue;
            }

            if (properties.TryGetValue(NegotiateTestUpn_PropertyName, out propertyValue))
            {
                NegotiateTestUpn = propertyValue;
            }
        }
    }

    public Dictionary<string, string> ToDictionary()
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

    public override string ToString()
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
