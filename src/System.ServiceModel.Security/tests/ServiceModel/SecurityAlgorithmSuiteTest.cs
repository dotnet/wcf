// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Infrastructure.Common;
using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Security;
using Xunit;

public static class SecurityAlgorithmSuiteTest
{
    [WcfTheory]
    [InlineData(nameof(TripleDesSecurityAlgorithmSuite.DefaultCanonicalizationAlgorithm), "http://www.w3.org/2001/10/xml-exc-c14n#")]
    [InlineData(nameof(TripleDesSecurityAlgorithmSuite.DefaultDigestAlgorithm), "http://www.w3.org/2000/09/xmldsig#sha1")]
    [InlineData(nameof(TripleDesSecurityAlgorithmSuite.DefaultEncryptionAlgorithm), "http://www.w3.org/2001/04/xmlenc#tripledes-cbc")]
    [InlineData(nameof(TripleDesSecurityAlgorithmSuite.DefaultEncryptionKeyDerivationLength), "192")]
    [InlineData(nameof(TripleDesSecurityAlgorithmSuite.DefaultSymmetricKeyWrapAlgorithm), "http://www.w3.org/2001/04/xmlenc#kw-tripledes")]
    [InlineData(nameof(TripleDesSecurityAlgorithmSuite.DefaultAsymmetricKeyWrapAlgorithm), "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p")]
    [InlineData(nameof(TripleDesSecurityAlgorithmSuite.DefaultSymmetricSignatureAlgorithm), "http://www.w3.org/2000/09/xmldsig#hmac-sha1")]
    [InlineData(nameof(TripleDesSecurityAlgorithmSuite.DefaultAsymmetricSignatureAlgorithm), "http://www.w3.org/2000/09/xmldsig#rsa-sha1")]
    [InlineData(nameof(TripleDesSecurityAlgorithmSuite.DefaultSignatureKeyDerivationLength), "192")]
    [InlineData(nameof(TripleDesSecurityAlgorithmSuite.DefaultSymmetricKeyLength), "192")]
    public static void Property_SecurityAlgorithmSuite_TripleDes(string propertyName, string expectedValue)
    {
        // Use reflection to get the default value of the properties provided in the inline data.
        // Verify the default values are the same as on the full framework.
        SecurityAlgorithmSuite suite = SecurityAlgorithmSuite.TripleDes;
        PropertyInfo info = suite.GetType().GetProperty(propertyName);
        Assert.Equal(info.GetValue(suite).ToString(), expectedValue);
    }

    [WcfTheory]
    [InlineData(nameof(TripleDesSecurityAlgorithmSuite.IsSymmetricKeyLengthSupported), true, 192)]
    [InlineData(nameof(TripleDesSecurityAlgorithmSuite.IsSymmetricKeyLengthSupported), false, 257)]
    [InlineData(nameof(TripleDesSecurityAlgorithmSuite.IsAsymmetricKeyLengthSupported), true, 1024)]
    [InlineData(nameof(TripleDesSecurityAlgorithmSuite.IsAsymmetricKeyLengthSupported), false, 4097)]
    public static void Method_SecurityAlgorithmSuite_TripleDes(string propertyName, bool expectedValue, int argVal)
    {
        // Use reflection to call the method with the specified arguments based on the inline data.
        // This test verifies the called methods return true or false based on the values passed in.
        Type objType = typeof(TripleDesSecurityAlgorithmSuite);
        Type tripleDesSecurityAlgorithmSuite = Type.GetType(objType.AssemblyQualifiedName);
        object desClassObject = SecurityAlgorithmSuite.TripleDes;
        MethodInfo method = tripleDesSecurityAlgorithmSuite.GetMethod(propertyName);
        object defaultValue = method.Invoke(desClassObject, new object[] { argVal });
        Assert.Equal(defaultValue, expectedValue);
    }
}
