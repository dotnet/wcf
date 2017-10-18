// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;
using Infrastructure.Common;
using Xunit;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public static partial class XmlSerializerFormatTests
{
    [WcfFact]
    [OuterLoop]
    public static void XmlSerializerFormatAttribute_SupportFaults()
    {
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IXmlSFAttribute> factory = null;
        IXmlSFAttribute serviceProxy = null;

        // *** SETUP *** \\
        binding = new BasicHttpBinding();
        endpointAddress = new EndpointAddress(Endpoints.XmlSFAttribute_Address);
        factory = new ChannelFactory<IXmlSFAttribute>(binding, endpointAddress);
        serviceProxy = factory.CreateChannel();

        // *** EXECUTE 1st Variation *** \\
        try
        {
            // Calling the Operation Contract overload with "SupportFaults" not set, default is "false"
            serviceProxy.TestXmlSerializerSupportsFaults_False();
        }
        catch (FaultException<FaultDetailWithXmlSerializerFormatAttribute> fException)
        {
            // In this variation the Fault message should have been returned using the Data Contract Serializer.
            Assert.True(fException.Detail.UsedDataContractSerializer, "The returning Fault Detail should have used the Data Contract Serializer.");
            Assert.True(fException.Detail.UsedXmlSerializer == false, "The returning Fault Detail should NOT have used the Xml Serializer.");
        }
        catch (Exception exception)
        {
            Assert.True(false, $"Test Failed, caught unexpected exception.\nException: {exception.ToString()}\nException Message: {exception.Message}");
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy);
        }

        // *** EXECUTE 2nd Variation *** \\
        try
        {
            serviceProxy = factory.CreateChannel();
            serviceProxy.TestXmlSerializerSupportsFaults_True();
        }
        catch (FaultException<FaultDetailWithXmlSerializerFormatAttribute> fException)
        {
            // In this variation the Fault message should have been returned using the Xml Serializer.
            Assert.True(fException.Detail.UsedDataContractSerializer == false, "The returning Fault Detail should NOT have used the Data Contract Serializer.");
            Assert.True(fException.Detail.UsedXmlSerializer, "The returning Fault Detail should have used the Xml Serializer.");
        }
        catch (Exception exception)
        {
            Assert.True(false, $"Test Failed, caught unexpected exception.\nException: {exception.ToString()}\nException Message: {exception.Message}");
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

#if SVCUTILTESTS
    [WcfFact]
    [OuterLoop]
    public static void XmlSFAttributeWsdlTest()
    {
        //single contract namespace
        Assert.True(RunVariation(Endpoints.BasciHttpRpcEncSingleNs_Address));
        Assert.True(RunVariation(Endpoints.BasicHttpRpcLitSingleNs_Address));
        Assert.True(RunVariation(Endpoints.BasicHttpDocLitSingleNs_Address));

        //multiple binding namespaces : not implement test

        //multiple contract namesapaces : not implement negative test, svcutil on ?singlewsdl should fail
    }

    private static bool RunVariation(string serviceAddress)
    {
        string wsdlGeneratedFile = $"{new Guid()}\\wsdlGened.cs";
        string singleWsdlGeneratedFile = $"{new Guid()}\\SingleWsdlGened.cs";

        try
        {    
            string wsdlArguments = $"{serviceAddress}?wsdl /syncOnly /noConfig /ser:XmlSerializer /o:{wsdlGeneratedFile}";
            string singleWsdlArguments = $"{serviceAddress}?singleWsdl /syncOnly /noConfig /ser:XmlSerializer /o:{singleWsdlGeneratedFile}";

            Tool.Main(wsdlArguments.ToString().Split(new Char[] { ' ' }));
            Tool.Main(singleWsdlArguments.ToString().Split(new Char[] { ' ' }));
            return CompareFile(wsdlGeneratedFile, singleWsdlGeneratedFile, null, false);
        }

        catch
        {
            return false;
        }

        finally
        {
            if (File.Exists(wsdlGeneratedFile))
            {
                File.Delete(wsdlGeneratedFile);
            }

            if (File.Exists(singleWsdlGeneratedFile))
            {
                File.Delete(singleWsdlGeneratedFile);
            }
        }
    }

#region Result validator
    static string[] FilteredWords = new string[]
        {
            "System.Xml.Serialization.XmlSerializerVersionAttribute",
            "System.Reflection.AssemblyVersionAttribute",
            @"//------------------",
            @"\[Microsoft",
            @"^//.*:[0-9]\.[0-9]\.[0-9][0-9][0-9][0-9][0-9]\.[0-9][0-9]",
            @",^'.*:[0-9]\.[0-9]\.[0-9][0-9][0-9][0-9][0-9]\.[0-9][0-9]",
            "Runtime Version",
            "System.CodeDom.Compiler.GeneratedCodeAttribute",
            "userPrincipalName",
            "^[0-9][0-9]*.[0-9][0-9]*",
            "^---",
            @"^//",
            "^'"
        };
    static Regex filter = new Regex(string.Join("|", FilteredWords));
    static Regex bracketFilter = new Regex(@"[\d\w\@\(\)]+[\s]*\{$");

    private static bool CompareFile(string expectedFile, string generatedFile, string toolsBinDir, bool runFilter)
    {
        string[] expected;
        string[] generated;

        try
        {
            expected = File.ReadAllLines(expectedFile);
        }
        catch (FileNotFoundException)
        {
            return false;
        }
        catch (DirectoryNotFoundException)
        {
            return false;
        }

        try
        {
            generated = File.ReadAllLines(generatedFile);
        }
        catch (FileNotFoundException)
        {
            return false;
        }
        catch (DirectoryNotFoundException)
        {            
            return false;
        }

        if (runFilter)
        {
            expected = RunWordFilter(expected);
            generated = RunWordFilter(generated);
        }

        if (!string.IsNullOrEmpty(toolsBinDir))
        {
            // Saving the filtered versions of the code for debugging purposes
            File.WriteAllLines(string.Format(@"{0}\generated.txt", toolsBinDir), generated);
            File.WriteAllLines(string.Format(@"{0}\expected.txt", toolsBinDir), expected);
        }

        // Validating generatedFile
        bool filesMatch = true;

        // Sort lines to have a normalized version
        Array.Sort(generated);
        Array.Sort(expected);

        if (!string.IsNullOrEmpty(toolsBinDir))
        {
            // Saving the sorted versions of the code for debugging purposes
            File.WriteAllLines(string.Format(@"{0}\generatedSorted.txt", toolsBinDir), generated);
            File.WriteAllLines(string.Format(@"{0}\expectedSorted.txt", toolsBinDir), expected);
        }

        // don't bother comparing the files
        if (expected.Length != generated.Length)
        {
            return false;
        }

        for (int i = 0; i < expected.Length; i++)
        {
            if (string.Compare(expected[i], generated[i], StringComparison.CurrentCulture) != 0)
            {
                filesMatch = false;
            }
        }

        return filesMatch;
    }

    private static string[] RunWordFilter(string[] text)
    {
        List<string> result = new List<string>();
        foreach (string line in text)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                // get rid of the non-relevant sections that can cause conflicts
                if (!filter.Match(line).Success)
                {
                    // Turn lines like this: 
                    // class MyClass {
                    // into this:
                    // class MyClass
                    // {
                    if (!bracketFilter.Match(line).Success)
                    {
                        result.Add(line);
                    }

                    else
                    {
                        result.Add(line.Remove(line.IndexOf('{')).TrimEnd());
                        StringBuilder builder = new StringBuilder();
                        foreach (char c in line.ToCharArray())
                        {
                            if (Char.IsWhiteSpace(c))
                            {
                                builder.Append(c);
                            }
                            else
                            {
                                break;
                            }
                        }

                        builder.Append('{');
                        result.Add(builder.ToString());
                    }
                }
            }
        }

        return result.ToArray();
    }
    #endregion

#endif
}
