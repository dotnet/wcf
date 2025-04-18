// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public partial class ClientBaseTests : ConditionalWcfTest
{
    [WcfFact]
    [OuterLoop]
    public static void DefaultSettings_Echo_Cookie()
    {
        // *** SETUP *** \\
        var factory = new ChannelFactory<IWcfAspNetCompatibleService>(
                 new BasicHttpBinding()
                 {
                     AllowCookies = true
                 },
                 new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));

        IWcfAspNetCompatibleService serviceProxy = factory.CreateChannel();

        IHttpCookieContainerManager cookieManager = ((IChannel)serviceProxy).GetProperty<IHttpCookieContainerManager>();
        Assert.True(cookieManager != null, "cookieManager was null.");
        Assert.True(cookieManager.CookieContainer != null, "cookieManager.CookieContainer was null.");
        string cookieName = "cookieName";
        string cookieValue = "cookieValue";
        string cookieSentOut = string.Format("{0}={1}", cookieName, cookieValue);
        cookieManager.CookieContainer.Add(new Uri(Endpoints.HttpBaseAddress_Basic_Text), new System.Net.Cookie(cookieName, cookieValue));

        try
        {
            // *** EXECUTE *** \\
            string cookieSentBack = serviceProxy.EchoCookie();

            // *** VALIDATE *** \\
            Assert.True(cookieSentOut == cookieSentBack,
                string.Format("The expected cookie sent back from the server was '{0}', but the actual cookie was '{1}'", cookieSentOut, cookieSentBack));

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void DefaultSettings_SetCookieOnServerSide()
    {
        // *** SETUP *** \\
        Uri uri = new Uri(Endpoints.HttpBaseAddress_Basic_Text);
        var factory = new ChannelFactory<IWcfAspNetCompatibleService>(
                 new BasicHttpBinding()
                 {
                     AllowCookies = true
                 },
                 new EndpointAddress(uri));

        IWcfAspNetCompatibleService serviceProxy = factory.CreateChannel();

        IHttpCookieContainerManager cookieManager = ((IChannel)serviceProxy).GetProperty<IHttpCookieContainerManager>();
        Assert.True(cookieManager != null, "cookieManager was null.");
        Assert.True(cookieManager.CookieContainer != null, "cookieManager.CookieContainer was null.");

        try
        {
            string cookieName = "cookie_time";


            // *** EXECUTE *** \\
            // EchoTimeAndSetCookie returns the current time and also sets the cookie named 'cookieName' to be the same time returned.
            string timeReturned = serviceProxy.EchoTimeAndSetCookie(cookieName);

            CookieCollection cookies = cookieManager.CookieContainer.GetCookies(uri);
            Assert.True(cookies != null, "cookies was null.");

            Cookie cookie = cookies[cookieName];
            Assert.True(cookie != null, "cookie was null.");

            string timeSetInCookie = cookie.Value;

            // *** VALIDATE *** \\
            Assert.True(timeReturned != null, "timeReturned != null");
            Assert.True(timeSetInCookie != null, "timeSetInCookie != null");
            Assert.True(timeReturned == timeSetInCookie,
                string.Format("Expected cookie named '{0}' to be set to '{1}', but the actual value was '{2}'", cookieName, timeReturned, timeSetInCookie));

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}

