// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Xunit;
using Infrastructure.Common;

public static class ScenarioTestHelpers
{
    private const string testString = "Hello";

    //WebSocket constants
    public const int SixtyFourMB = 64 * 1024 * 1024;
    public const string ContentToReplace = "ContentToReplace";
    public const string ResponseReplaceThisContent = "ResponseReplaceThisContent";
    public const string ReplacedContent = "ReplacedContent";
    public const string LastMessage = "LastMessage";
    public const string RemoteEndpointMessagePropertyFailure = "RemoteEndpointMessageProperty did not contain the address of this machine.";
    public const string CertificateIssuerName = "DO_NOT_TRUST_WcfBridgeRootCA";

    public static TimeSpan TestTimeout
    {
        get
        {
            // Let any exception throw to the test that asked for this so it is reported
            string timeSpanAsString = TestProperties.GetProperty(TestProperties.MaxTestTimeSpan_PropertyName);
            return TimeSpan.Parse(timeSpanAsString);
        }
    }

    // Returns true only if the test services are accessed via "localhost".
    // This test is not intended to be used to determine whether test services 
    // are running on the same machine as the tests.
    public static bool IsLocalHost()
    {
        string serviceUri = TestProperties.GetProperty(TestProperties.ServiceUri_PropertyName);
        return String.Equals("localhost", serviceUri, StringComparison.OrdinalIgnoreCase);
    }

    public static string GenerateStringValue(int length)
    {
        // There's no great reason why we use this set of characters - we just want to be able to generate a longish string
        uint firstCharacter = 0x41; // A

        StringBuilder builder = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            builder.Append((char)(firstCharacter + i % 25));
        }

        return builder.ToString();
    }

    public static bool RunBasicEchoTest(Binding binding, string address, string variation, Action<ChannelFactory> factorySettings = null)
    {
        Logger.LogInformation("Starting basic echo test.\nTest variation:...\n{0}\nUsing address: '{1}'", variation, address);
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        bool success = false;

        try
        {
            // *** SETUP *** \\
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(address));
            if (factorySettings != null)
            {
                factorySettings(factory);
            }
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.True(String.Equals(result, testString), String.Format("    Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }

        Logger.LogInformation("  Result: {0} ", success ? "PASS" : "FAIL");
        return success;
    }

    public static bool RunComplexEchoTest(Binding binding, string address, string variation, StringBuilder errorBuilder, Action<ChannelFactory> factorySettings = null)
    {
        bool success = false;
        try
        {
            ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(address));

            if (factorySettings != null)
            {
                factorySettings(factory);
            }

            IWcfService serviceProxy = factory.CreateChannel();

            ComplexCompositeType compositeObject = new ComplexCompositeType()
            {
                BoolValue = true,
                ByteArrayValue = new byte[] { 0x60, 0x61, 0x62 },
                CharArrayValue = new char[] { 'a', 'b', 'c' },
                CharValue = 'a',
                DateTimeValue = new DateTime(2000, 1, 1),
                DayOfWeekValue = DayOfWeek.Sunday,
                DoubleValue = 3.14159265,
                FloatValue = 2.71828183f,
                GuidValue = new Guid("EFEA21A0-F59A-4F43-B5D3-B2C667CA6FB6"),
                IntValue = int.MinValue,
                LongerStringValue = GenerateStringValue(2048),
                LongValue = long.MaxValue,
                SbyteValue = (sbyte)'a',
                ShortValue = short.MaxValue,
                StringValue = "the quick brown fox jumps over the lazy dog",
                TimeSpanValue = TimeSpan.MinValue,
                UintValue = uint.MaxValue,
                UlongValue = ulong.MaxValue,
                UshortValue = ushort.MaxValue
            };

            ComplexCompositeType result = serviceProxy.EchoComplex(compositeObject);
            success = compositeObject.Equals(result);

            if (!success)
            {
                errorBuilder.AppendLine(String.Format("    Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));
            }
        }
        catch (Exception ex)
        {
            Logger.LogInformation("    {0}", ex.Message);
            errorBuilder.AppendLine(String.Format("    Error: Unexpected exception was caught while doing the basic echo test for variation...\n'{0}'\nException: {1}", variation, ex.ToString()));
        }

        return success;
    }

    public static ComplexCompositeType GetInitializedComplexCompositeType()
    {
        ComplexCompositeType compositeObject = new ComplexCompositeType()
        {
            BoolValue = true,
            ByteArrayValue = new byte[] { 0x60, 0x61, 0x62 },
            CharArrayValue = new char[] { 'a', 'b', 'c' },
            CharValue = 'a',
            DateTimeValue = new DateTime(2000, 1, 1),
            DayOfWeekValue = DayOfWeek.Sunday,
            DoubleValue = 3.14159265,
            FloatValue = 2.71828183f,
            GuidValue = new Guid("EFEA21A0-F59A-4F43-B5D3-B2C667CA6FB6"),
            IntValue = int.MinValue,
            LongerStringValue = GenerateStringValue(2048),
            LongValue = long.MaxValue,
            SbyteValue = (sbyte)'a',
            ShortValue = short.MaxValue,
            StringValue = "the quick brown fox jumps over the lazy dog",
            TimeSpanValue = TimeSpan.MinValue,
            UintValue = uint.MaxValue,
            UlongValue = ulong.MaxValue,
            UshortValue = ushort.MaxValue
        };

        return compositeObject;
    }

    /// <summary>
    /// Closes com objects in the order passed in if not already closed.
    /// If Close fails for Timeout or Communication exception then Aborts.
    /// </summary>
    /// <param name="objects">Any communication objects that need to be cleaned up.
    /// In the order in which they need to be cleaned up.</param>
    public static void CloseCommunicationObjects(params ICommunicationObject[] objects)
    {
        foreach (ICommunicationObject comObj in objects)
        {
            try
            {
                if (comObj == null)
                {
                    continue;
                }
                // Only want to call Close if it is in the Opened state
                if (comObj.State == CommunicationState.Opened)
                {
                    comObj.Close();
                }
                // Anything not closed by this point should be aborted
                if (comObj.State != CommunicationState.Closed)
                {
                    comObj.Abort();
                }
            }
            catch (TimeoutException)
            {
                comObj.Abort();
            }
            catch (CommunicationException)
            {
                comObj.Abort();
            }
        }
    }

    public static string CreateInterestingString(int length)
    {
        char[] chars = new char[length];
        int index = 0;

        // Arrays of odd length will start with a single char.
        // The rest of the entries will be surrogate pairs.
        if (length % 2 == 1)
        {
            chars[index] = 'a';
            index++;
        }

        // Fill remaining entries with surrogate pairs
        int seed = DateTime.Now.Millisecond;
        Random rand = new Random(seed);
        char highSurrogate;
        char lowSurrogate;

        while (index < length)
        {
            highSurrogate = Convert.ToChar(rand.Next(0xD800, 0xDC00));
            lowSurrogate = Convert.ToChar(rand.Next(0xDC00, 0xE000));

            chars[index] = highSurrogate;
            ++index;
            chars[index] = lowSurrogate;
            ++index;
        }

        return new string(chars, 0, chars.Length);
    }
}

public static class Logger
{
    public static void Log(string category, string message)
    {
#if LOG_TO_CONSOLE
        Console.WriteLine(String.Format("{0}: {1}", category, message));
#endif // LOG_TO_CONSOLE
    }

    public static void LogInformation(string message)
    {
#if LOG_TO_CONSOLE
        Console.WriteLine(message);
#endif // LOG_TO_CONSOLE
    }

    public static void LogInformation(string message, params object[] args)
    {
        LogInformation(String.Format(message, args));
    }

    public static void LogWarning(string message, params object[] args)
    {
        LogWarning(String.Format(message, args));
    }

    public static void LogWarning(string message)
    {
        Log("Warning", message);
    }

    public static void LogError(string message, params object[] args)
    {
        LogError(String.Format(message, args));
    }

    public static void LogError(string message)
    {
        Log("Error", message);
    }
}
