// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Infrastructure.Common;
using Xunit;

public static class CommunicationObjectTest
{
    [WcfFact]
    public static void CommunicationObject_Sync_Open_Close_Methods_Called()
    {
        MockCommunicationObject mco = new MockCommunicationObject();
        List<string> openMethodsCalled = new List<string>();
        List<string> closeMethodsCalled = new List<string>();
        TimeSpan timeout = TimeSpan.FromSeconds(30);

        // *** SETUP *** \\
        MockCommunicationObject.InterceptAllOpenMethods(mco, openMethodsCalled);
        MockCommunicationObject.InterceptAllCloseMethods(mco, closeMethodsCalled);

        // *** EXECUTE *** \\
        mco.Open(timeout);
        mco.Close(timeout);

        // *** VALIDATE *** \\
        string expectedOpens = "OnOpening,OnOpen,OnOpened";
        string actualOpens = String.Join(",", openMethodsCalled);
        Assert.True(String.Equals(expectedOpens, actualOpens, StringComparison.Ordinal),
               String.Format("Expected open methods to be '{0}' but actual was '{1}'.",
                             expectedOpens, actualOpens));

        string expectedCloses = "OnClosing,OnClose,OnClosed";
        string actualCloses = String.Join(",", closeMethodsCalled);
        Assert.True(String.Equals(expectedCloses, actualCloses, StringComparison.Ordinal),
               String.Format("Expected close methods to be '{0}' but actual was '{1}'.",
                             expectedCloses, actualCloses));
    }

    [WcfFact]

    public static void CommunicationObject_Async_Open_Close_Methods_Called()
    {
        MockCommunicationObject mco = new MockCommunicationObject();
        List<string> openMethodsCalled = new List<string>();
        List<string> closeMethodsCalled = new List<string>();
        TimeSpan timeout = TimeSpan.FromSeconds(30);

        // *** SETUP *** \\
        MockCommunicationObject.InterceptAllOpenMethods(mco, openMethodsCalled);
        MockCommunicationObject.InterceptAllCloseMethods(mco, closeMethodsCalled);

        // *** EXECUTE *** \\
        IAsyncResult openAr = mco.BeginOpen(timeout, callback: null, state: null);
        mco.OpenAsyncResult.Complete();
        mco.EndOpen(openAr);

        IAsyncResult closeAr = mco.BeginClose(timeout, callback: null, state: null);
        mco.CloseAsyncResult.Complete();
        mco.EndClose(closeAr);

        // *** VALIDATE *** \\
        string expectedOpens = "OnOpening,OnBeginOpen,OnOpened";
        string actualOpens = String.Join(",", openMethodsCalled);
        Assert.True(String.Equals(expectedOpens, actualOpens, StringComparison.Ordinal),
               String.Format("Expected open methods to be '{0}' but actual was '{1}'.",
                             expectedOpens, actualOpens));

        string expectedCloses = "OnClosing,OnBeginClose,OnClosed";
        string actualCloses = String.Join(",", closeMethodsCalled);
        Assert.True(String.Equals(expectedCloses, actualCloses, StringComparison.Ordinal),
               String.Format("Expected close methods to be '{0}' but actual was '{1}'.",
                             expectedCloses, actualCloses));
    }

    [WcfFact]
    public static void CommunicationObject_Abort_Close_Methods_Called()
    {
        MockCommunicationObject mco = new MockCommunicationObject();
        List<string> closeMethodsCalled = new List<string>();
        TimeSpan timeout = TimeSpan.FromSeconds(30);

        // *** SETUP *** \\
        MockCommunicationObject.InterceptAllCloseMethods(mco, closeMethodsCalled);

        // *** EXECUTE *** \\
        mco.Open(timeout);
        mco.Abort();

        // *** VALIDATE *** \\
        string expectedCloses = "OnClosing,OnAbort,OnClosed";
        string actualCloses = String.Join(",", closeMethodsCalled);
        Assert.True(String.Equals(expectedCloses, actualCloses, StringComparison.Ordinal),
               String.Format("Expected close methods to be '{0}' but actual was '{1}'.",
                             expectedCloses, actualCloses));

        Assert.True(mco.State == CommunicationState.Closed,
                String.Format("Expected final state to be 'Closed' but actual was '{0}", mco.State));

    }

    [WcfFact]
    public static void CommunicationObject_Sync_Open_Close_Events_Fire()
    {
        MockCommunicationObject mco = new MockCommunicationObject();
        List<string> openEventsFired = new List<string>();
        List<string> closeEventsFired = new List<string>();
        TimeSpan timeout = TimeSpan.FromSeconds(30);

        // *** SETUP *** \\
        MockCommunicationObject.InterceptAllOpenEvents(mco, openEventsFired);
        MockCommunicationObject.InterceptAllCloseEvents(mco, closeEventsFired);

        // *** EXECUTE *** \\
        mco.Open(timeout);
        mco.Close(timeout);

        // *** VALIDATE *** \\
        string expectedOpens = "Opening,Opened";
        string actualOpens = String.Join(",", openEventsFired);
        Assert.True(String.Equals(expectedOpens, actualOpens, StringComparison.Ordinal),
               String.Format("Expected open events to be '{0}' but actual was '{1}'.",
                             expectedOpens, actualOpens));

        string expectedCloses = "Closing,Closed";
        string actualCloses = String.Join(",", closeEventsFired);
        Assert.True(String.Equals(expectedCloses, actualCloses, StringComparison.Ordinal),
               String.Format("Expected close events to be '{0}' but actual was '{1}'.",
                             expectedCloses, actualCloses));
    }

    [WcfFact]
    public static void CommunicationObject_Async_Open_Close_Events_Fire()
    {
        MockCommunicationObject mco = new MockCommunicationObject();
        List<string> openEventsFired = new List<string>();
        List<string> closeEventsFired = new List<string>();
        TimeSpan timeout = TimeSpan.FromMinutes(30);

        // *** SETUP *** \\
        MockCommunicationObject.InterceptAllOpenEvents(mco, openEventsFired);
        MockCommunicationObject.InterceptAllCloseEvents(mco, closeEventsFired);

        // *** EXECUTE *** \\
        IAsyncResult openAr = mco.BeginOpen(timeout, callback: null, state: null);
        mco.OpenAsyncResult.Complete();
        mco.EndOpen(openAr);

        IAsyncResult closeAr = mco.BeginClose(timeout, callback: null, state: null);
        mco.CloseAsyncResult.Complete();
        mco.EndClose(closeAr);

        // *** VALIDATE *** \\
        string expectedOpens = "Opening,Opened";
        string actualOpens = String.Join(",", openEventsFired);
        Assert.True(String.Equals(expectedOpens, actualOpens, StringComparison.Ordinal),
               String.Format("Expected open events to be '{0}' but actual was '{1}'.",
                             expectedOpens, actualOpens));

        string expectedCloses = "Closing,Closed";
        string actualCloses = String.Join(",", closeEventsFired);
        Assert.True(String.Equals(expectedCloses, actualCloses, StringComparison.Ordinal),
               String.Format("Expected close events to be '{0}' but actual was '{1}'.",
                             expectedCloses, actualCloses));
    }

    [WcfFact]
    public static void CommunicationObject_Sync_Open_Close_States_Transition()
    {
        MockCommunicationObject mco = new MockCommunicationObject();
        TimeSpan timeout = TimeSpan.FromSeconds(30);
        CommunicationStateData data = new CommunicationStateData();

        // *** SETUP *** \\
        MockCommunicationObject.InterceptAllStateChanges(mco, data);

        // *** EXECUTE *** \\
        mco.Open(timeout);
        mco.Close(timeout);

        // *** VALIDATE *** \\
        Assert.True(data.StateAfterCreate == CommunicationState.Created,
                    String.Format("CommunicationState after creation was '{0}' but expected 'Created'",
                        data.StateAfterCreate));

        Assert.True(data.StateEnterOnOpening == CommunicationState.Opening,
                    String.Format("CommunicationState entering OnOpening was '{0}' but expected 'Opening'",
                        data.StateEnterOnOpening));
        Assert.True(data.StateLeaveOnOpening == CommunicationState.Opening,
                    String.Format("CommunicationState leaving OnOpening was '{0}' but expected 'Opening'",
                        data.StateLeaveOnOpening));

        Assert.True(data.StateEnterOnOpen == CommunicationState.Opening,
                    String.Format("CommunicationState entering OnOpen was '{0}' but expected 'Opening'",
                        data.StateEnterOnOpen));
        Assert.True(data.StateLeaveOnOpen == CommunicationState.Opening,
                    String.Format("CommunicationState leaving OnOpen was '{0}' but expected 'Opening'",
                        data.StateLeaveOnOpen));

        Assert.True(data.StateEnterOnOpened == CommunicationState.Opening,
                    String.Format("CommunicationState entering OnOpened was '{0}' but expected 'Opening'",
                        data.StateEnterOnOpened));
        Assert.True(data.StateLeaveOnOpened == CommunicationState.Opened,
                    String.Format("CommunicationState leaving OnOpened was '{0}' but expected 'Opened'",
                        data.StateLeaveOnOpened));

        Assert.True(data.StateEnterOnClosing == CommunicationState.Closing,
                    String.Format("CommunicationState entering OnClosing was '{0}' but expected 'Closing'",
                        data.StateEnterOnClosing));
        Assert.True(data.StateLeaveOnClosing == CommunicationState.Closing,
                    String.Format("CommunicationState leaving OnClosing was '{0}' but expected 'Closing'",
                        data.StateLeaveOnClosing));

        Assert.True(data.StateEnterOnClose == CommunicationState.Closing,
                    String.Format("CommunicationState entering OnClose was '{0}' but expected 'Closing'",
                        data.StateEnterOnClose));
        Assert.True(data.StateLeaveOnClose == CommunicationState.Closing,
                    String.Format("CommunicationState leaving OnClose was '{0}' but expected 'Closing'",
                        data.StateLeaveOnClose));

        Assert.True(data.StateEnterOnClosed == CommunicationState.Closing,
                    String.Format("CommunicationState entering OnClosed was '{0}' but expected 'Closing'",
                        data.StateEnterOnClosed));
        Assert.True(data.StateLeaveOnClosed == CommunicationState.Closed,
                    String.Format("CommunicationState leaving OnClosed was '{0}' but expected 'Closed'",
                        data.StateLeaveOnClosed));
    }

    [WcfFact]
    public static void CommunicationObject_Async_Open_Close_States_Transition()
    {
        MockCommunicationObject mco = new MockCommunicationObject();
        TimeSpan timeout = TimeSpan.FromMinutes(5);
        CommunicationStateData data = new CommunicationStateData();

        // *** SETUP *** \\
        MockCommunicationObject.InterceptAllStateChanges(mco, data);

        // *** EXECUTE *** \\
        IAsyncResult openAr = mco.BeginOpen(timeout, callback: null, state: null);
        mco.OpenAsyncResult.Complete();
        mco.EndOpen(openAr);

        IAsyncResult closeAr = mco.BeginClose(timeout, callback: null, state: null);
        mco.CloseAsyncResult.Complete();
        mco.EndClose(closeAr);

        // *** VALIDATE *** \\
        Assert.True(data.StateAfterCreate == CommunicationState.Created,
                    String.Format("CommunicationState after creation was '{0}' but expected 'Created'",
                        data.StateAfterCreate));

        Assert.True(data.StateEnterOnOpening == CommunicationState.Opening,
                    String.Format("CommunicationState entering OnOpening was '{0}' but expected 'Opening'",
                        data.StateEnterOnOpening));
        Assert.True(data.StateLeaveOnOpening == CommunicationState.Opening,
                    String.Format("CommunicationState leaving OnOpening was '{0}' but expected 'Opening'",
                        data.StateLeaveOnOpening));

        Assert.True(data.StateEnterOnBeginOpen == CommunicationState.Opening,
                    String.Format("CommunicationState entering OnBeginOpen was '{0}' but expected 'Opening'",
                        data.StateEnterOnBeginOpen));
        Assert.True(data.StateLeaveOnBeginOpen == CommunicationState.Opening,
                    String.Format("CommunicationState leaving OnBeginOpen was '{0}' but expected 'Opening'",
                        data.StateLeaveOnBeginOpen));

        Assert.True(data.StateEnterOnOpened == CommunicationState.Opening,
                    String.Format("CommunicationState entering OnOpened was '{0}' but expected 'Opening'",
                        data.StateEnterOnOpened));
        Assert.True(data.StateLeaveOnOpened == CommunicationState.Opened,
                    String.Format("CommunicationState leaving OnOpened was '{0}' but expected 'Opened'",
                        data.StateLeaveOnOpened));

        Assert.True(data.StateEnterOnClosing == CommunicationState.Closing,
                    String.Format("CommunicationState entering OnClosing was '{0}' but expected 'Closing'",
                        data.StateEnterOnClosing));
        Assert.True(data.StateLeaveOnClosing == CommunicationState.Closing,
                    String.Format("CommunicationState leaving OnClosing was '{0}' but expected 'Closing'",
                        data.StateLeaveOnClosing));

        Assert.True(data.StateEnterOnBeginClose == CommunicationState.Closing,
                    String.Format("CommunicationState entering OnBeginClose was '{0}' but expected 'Closing'",
                        data.StateEnterOnBeginClose));
        Assert.True(data.StateLeaveOnBeginClose == CommunicationState.Closing,
                    String.Format("CommunicationState leaving OnClose was '{0}' but expected 'Closing'",
                        data.StateLeaveOnBeginClose));

        Assert.True(data.StateEnterOnClosed == CommunicationState.Closing,
                    String.Format("CommunicationState entering OnClosed was '{0}' but expected 'Closing'",
                        data.StateEnterOnClosed));
        Assert.True(data.StateLeaveOnClosed == CommunicationState.Closed,
                    String.Format("CommunicationState leaving OnClosed was '{0}' but expected 'Closed'",
                        data.StateLeaveOnClosed));
    }

    [WcfFact]
    public static void CommunicationObject_Sync_Open_Propagates_Exception()
    {
        MockCommunicationObject mco = new MockCommunicationObject();
        TimeSpan timeout = TimeSpan.FromSeconds(30);
        string exceptionMessage = "Expected exception";

        // *** SETUP *** \\
        mco.OnOpenOverride = (t) =>
        {
            throw new InvalidOperationException(exceptionMessage);
        };

        // *** EXECUTE *** \\
        InvalidOperationException actualException = Assert.Throws<InvalidOperationException>(() =>
        {
           mco.Open(timeout);
        });

        // *** VALIDATE *** \\
        Assert.True(String.Equals(exceptionMessage, actualException.Message),
                    String.Format("Expected exception message '{0}' but actual was '{1}'",
                                  exceptionMessage, actualException.Message));
    }

    [WcfFact]
    public static void CommunicationObject_Sync_Close_Propagates_Exception()
    {
        MockCommunicationObject mco = new MockCommunicationObject();
        TimeSpan timeout = TimeSpan.FromSeconds(30);
        string exceptionMessage = "Expected exception";

        // *** SETUP *** \\
        mco.OnCloseOverride = (t) =>
        {
            throw new InvalidOperationException(exceptionMessage);
        };

        // *** EXECUTE *** \\
        mco.Open(timeout);

        InvalidOperationException actualException = Assert.Throws<InvalidOperationException>(() =>
        {
            mco.Close(timeout);
        });

        // *** VALIDATE *** \\
        Assert.True(String.Equals(exceptionMessage, actualException.Message),
                    String.Format("Expected exception message '{0}' but actual was '{1}'",
                                  exceptionMessage, actualException.Message));
    }

    [WcfFact]
    public static void CommunicationObject_Async_Open_Propagates_Exception()
    {
        MockCommunicationObject mco = new MockCommunicationObject();
        TimeSpan timeout = TimeSpan.FromSeconds(30);
        string exceptionMessage = "Expected exception";

        // *** SETUP *** \\
        mco.OnBeginOpenOverride = (TimeSpan t, AsyncCallback c, object s) =>
        {
            throw new InvalidOperationException(exceptionMessage);
        };

        // *** EXECUTE *** \\
        InvalidOperationException actualException = Assert.Throws<InvalidOperationException>(() =>
        {
            IAsyncResult openAr = mco.BeginOpen(timeout, callback: null, state: null);
            mco.OpenAsyncResult.Complete();
            mco.EndOpen(openAr);
        });

        // *** VALIDATE *** \\
        Assert.True(String.Equals(exceptionMessage, actualException.Message),
                    String.Format("Expected exception message '{0}' but actual was '{1}'",
                                  exceptionMessage, actualException.Message));
    }

    [WcfFact]
    public static void CommunicationObject_Async_Close_Propagates_Exception()
    {
        MockCommunicationObject mco = new MockCommunicationObject();
        TimeSpan timeout = TimeSpan.FromSeconds(30);
        string exceptionMessage = "Expected exception";

        // *** SETUP *** \\
        mco.OnBeginCloseOverride = (TimeSpan t, AsyncCallback c, object s) =>
        {
            throw new InvalidOperationException(exceptionMessage);
        };

        // *** EXECUTE *** \\
        IAsyncResult openAr = mco.BeginOpen(timeout, callback: null, state: null);
        mco.OpenAsyncResult.Complete();
        mco.EndOpen(openAr);

        InvalidOperationException actualException = Assert.Throws<InvalidOperationException>(() =>
        {
            IAsyncResult closeAr = mco.BeginClose(timeout, callback: null, state: null);
            mco.CloseAsyncResult.Complete();
            mco.EndClose(closeAr);
        });

        // *** VALIDATE *** \\
        Assert.True(String.Equals(exceptionMessage, actualException.Message),
                    String.Format("Expected exception message '{0}' but actual was '{1}'",
                                  exceptionMessage, actualException.Message));
    }
}
