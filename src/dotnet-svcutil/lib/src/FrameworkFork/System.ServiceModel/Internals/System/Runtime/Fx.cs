// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Diagnostics;
using System.Security;
using System.ServiceModel;

namespace System.Runtime
{
    public static class Fx
    {
        private const string defaultEventSource = "System.Runtime";

#if DEBUG
        private const string AssertsFailFastName = "AssertsFailFast";
        private const string BreakOnExceptionTypesName = "BreakOnExceptionTypes";
        private const string FastDebugName = "FastDebug";
        private const string StealthDebuggerName = "StealthDebugger";

        private static bool s_breakOnExceptionTypesRetrieved;
        private static Type[] s_breakOnExceptionTypesCache;
#endif

        private static ExceptionTrace s_exceptionTrace;
        private static EtwDiagnosticTrace s_diagnosticTrace;

        [Fx.Tag.SecurityNote(Critical = "This delegate is called from within a ConstrainedExecutionRegion, must not be settable from PT code")]
        [SecurityCritical]
        private static ExceptionHandler s_asynchronousThreadExceptionHandler;

        internal static ExceptionTrace Exception
        {
            get
            {
                if (s_exceptionTrace == null)
                {
                    // don't need a lock here since a true singleton is not required
                    s_exceptionTrace = new ExceptionTrace(defaultEventSource, Trace);
                }

                return s_exceptionTrace;
            }
        }
        internal static EtwDiagnosticTrace Trace
        {
            get
            {
                if (s_diagnosticTrace == null)
                {
                    s_diagnosticTrace = InitializeTracing();
                }

                return s_diagnosticTrace;
            }
        }

        [Fx.Tag.SecurityNote(Critical = "Accesses SecurityCritical field EtwProvider",
            Safe = "Doesn't leak info\\resources")]
        [SecuritySafeCritical]
        [SuppressMessage(FxCop.Category.ReliabilityBasic, FxCop.Rule.UseNewGuidHelperRule,
            Justification = "This is a method that creates ETW provider passing Guid Provider ID.")]
        private static EtwDiagnosticTrace InitializeTracing()
        {
            EtwDiagnosticTrace trace = new EtwDiagnosticTrace(defaultEventSource, EtwDiagnosticTrace.DefaultEtwProviderId);

            return trace;
        }

        public static ExceptionHandler AsynchronousThreadExceptionHandler
        {
            [Fx.Tag.SecurityNote(Critical = "access critical field", Safe = "ok for get-only access")]
            [SecuritySafeCritical]
            get
            {
                return Fx.s_asynchronousThreadExceptionHandler;
            }

            [Fx.Tag.SecurityNote(Critical = "sets a critical field")]
            [SecurityCritical]
            set
            {
                Fx.s_asynchronousThreadExceptionHandler = value;
            }
        }

        // Do not call the parameter "message" or else FxCop thinks it should be localized.
        [Conditional("DEBUG")]
        public static void Assert(bool condition, string description)
        {
            if (!condition)
            {
                Assert(description);
            }
        }

        [Conditional("DEBUG")]
        public static void Assert(string description)
        {
            AssertHelper.FireAssert(description);
        }

        public static void AssertAndThrow(bool condition, string description)
        {
            if (!condition)
            {
                AssertAndThrow(description);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Exception AssertAndThrow(string description)
        {
            Fx.Assert(description);
            TraceCore.ShipAssertExceptionMessage(Trace, description);
            throw new InternalException(description);
        }

        public static void AssertAndThrowFatal(bool condition, string description)
        {
            if (!condition)
            {
                AssertAndThrowFatal(description);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Exception AssertAndThrowFatal(string description)
        {
            Fx.Assert(description);
            TraceCore.ShipAssertExceptionMessage(Trace, description);
            throw new FatalInternalException(description);
        }

        public static void AssertAndFailFast(bool condition, string description)
        {
            if (!condition)
            {
                AssertAndFailFast(description);
            }
        }

        // This never returns.  The Exception return type lets you write 'throw AssertAndFailFast()' which tells the compiler/tools that
        // execution stops.
        [Fx.Tag.SecurityNote(Critical = "Calls into critical method Environment.FailFast",
            Safe = "The side affect of the app crashing is actually intended here")]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Exception AssertAndFailFast(string description)
        {
            Fx.Assert(description);
            string failFastMessage = InternalSR.FailFastMessage(description);

            // The catch is here to force the finally to run, as finallys don't run until the stack walk gets to a catch.  
            // The catch makes sure that the finally will run before the stack-walk leaves the frame, but the code inside is impossible to reach.
            try
            {
                try
                {
                    Fx.Exception.TraceFailFast(failFastMessage);
                }
                finally
                {
                    Environment.FailFast(failFastMessage);
                }
            }
            catch
            {
                throw;
            }

            return null; // we'll never get here since we've just fail-fasted
        }

        public static bool IsFatal(Exception exception)
        {
            while (exception != null)
            {
                if (exception is FatalException ||
                    exception is OutOfMemoryException ||
                    exception is FatalInternalException)
                {
                    return true;
                }

                // These exceptions aren't themselves fatal, but since the CLR uses them to wrap other exceptions,
                // we want to check to see whether they've been used to wrap a fatal exception.  If so, then they
                // count as fatal.
                if (exception is TypeInitializationException ||
                    exception is TargetInvocationException)
                {
                    exception = exception.InnerException;
                }
                else if (exception is AggregateException)
                {
                    // AggregateExceptions have a collection of inner exceptions, which may themselves be other
                    // wrapping exceptions (including nested AggregateExceptions).  Recursively walk this
                    // hierarchy.  The (singular) InnerException is included in the collection.
                    ReadOnlyCollection<Exception> innerExceptions = ((AggregateException)exception).InnerExceptions;
                    foreach (Exception innerException in innerExceptions)
                    {
                        if (IsFatal(innerException))
                        {
                            return true;
                        }
                    }

                    break;
                }
                else
                {
                    break;
                }
            }

            return false;
        }

        // This method should be only used for debug build.
        internal static bool AssertsFailFast
        {
            get
            {
                return false;
            }
        }

        // This property should be only used for debug build.
        internal static Type[] BreakOnExceptionTypes
        {
            get
            {
#if DEBUG
                if (!Fx.s_breakOnExceptionTypesRetrieved)
                {
                    object value;
                    if (TryGetDebugSwitch(Fx.BreakOnExceptionTypesName, out value))
                    {
                        string[] typeNames = value as string[];
                        if (typeNames != null && typeNames.Length > 0)
                        {
                            List<Type> types = new List<Type>(typeNames.Length);
                            for (int i = 0; i < typeNames.Length; i++)
                            {
                                types.Add(Type.GetType(typeNames[i], false));
                            }
                            if (types.Count != 0)
                            {
                                Fx.s_breakOnExceptionTypesCache = types.ToArray();
                            }
                        }
                    }
                    Fx.s_breakOnExceptionTypesRetrieved = true;
                }
                return Fx.s_breakOnExceptionTypesCache;
#else
                return null;
#endif
            }
        }

        // This property should be only used for debug build.
        internal static bool StealthDebugger
        {
            get
            {
                return false;
            }
        }

#if DEBUG
        private static bool TryGetDebugSwitch(string name, out object value)
        {
            value = null;
            return false;
        }
#endif

        public static AsyncCallback ThunkCallback(AsyncCallback callback)
        {
            return (new AsyncThunk(callback)).ThunkFrame;
        }

        [SuppressMessage(FxCop.Category.ReliabilityBasic, FxCop.Rule.UseNewGuidHelperRule,
            Justification = "These are the core methods that should be used for all other Guid(string) calls.")]
        public static Guid CreateGuid(string guidString)
        {
            bool success = false;
            Guid result = Guid.Empty;

            try
            {
                result = new Guid(guidString);
                success = true;
            }
            finally
            {
                if (!success)
                {
                    AssertAndThrow("Creation of the Guid failed.");
                }
            }

            return result;
        }

        [SuppressMessage(FxCop.Category.ReliabilityBasic, FxCop.Rule.UseNewGuidHelperRule,
            Justification = "These are the core methods that should be used for all other Guid(string) calls.")]
        public static bool TryCreateGuid(string guidString, out Guid result)
        {
            bool success = false;
            result = Guid.Empty;

            try
            {
                result = new Guid(guidString);
                success = true;
            }
            catch (ArgumentException)
            {
            }
            catch (FormatException)
            {
            }
            catch (OverflowException)
            {
            }

            return success;
        }

        public static byte[] AllocateByteArray(int size)
        {
            try
            {
                // Safe to catch OOM from this as long as the ONLY thing it does is a simple allocation of a primitive type (no method calls).
                return new byte[size];
            }
            catch (OutOfMemoryException exception)
            {
                // Desktop wraps the OOM inside a new InsufficientMemoryException, traces, and then throws it.
                // Project N and K trace and throw the original OOM.  InsufficientMemoryException does not exist in N and K.
                Fx.Exception.AsError(exception);
                throw;
            }
        }

        public static char[] AllocateCharArray(int size)
        {
            try
            {
                // Safe to catch OOM from this as long as the ONLY thing it does is a simple allocation of a primitive type (no method calls).
                return new char[size];
            }
            catch (OutOfMemoryException exception)
            {
                // Desktop wraps the OOM inside a new InsufficientMemoryException, traces, and then throws it.
                // Project N and K trace and throw the original OOM.  InsufficientMemoryException does not exist in N and K.
                Fx.Exception.AsError(exception);
                throw;
            }
        }

        [SuppressMessage(FxCop.Category.Design, FxCop.Rule.DoNotCatchGeneralExceptionTypes,
            Justification = "Don't want to hide the exception which is about to crash the process.")]
        [Fx.Tag.SecurityNote(Miscellaneous = "Must not call into PT code as it is called within a CER.")]
        private static void TraceExceptionNoThrow(Exception exception)
        {
            try
            {
                // This call exits the CER.  However, when still inside a catch, normal ThreadAbort is prevented.
                // Rude ThreadAbort will still be allowed to terminate processing.
                Fx.Exception.TraceUnhandledException(exception);
            }
            catch
            {
                // This empty catch is only acceptable because we are a) in a CER and b) processing an exception
                // which is about to crash the process anyway.
            }
        }

        [SuppressMessage(FxCop.Category.Design, FxCop.Rule.DoNotCatchGeneralExceptionTypes,
            Justification = "Don't want to hide the exception which is about to crash the process.")]
        [SuppressMessage(FxCop.Category.ReliabilityBasic, FxCop.Rule.IsFatalRule,
            Justification = "Don't want to hide the exception which is about to crash the process.")]
        [Fx.Tag.SecurityNote(Miscellaneous = "Must not call into PT code as it is called within a CER.")]
        private static bool HandleAtThreadBase(Exception exception)
        {
            // This area is too sensitive to do anything but return.
            if (exception == null)
            {
                Fx.Assert("Null exception in HandleAtThreadBase.");
                return false;
            }

            TraceExceptionNoThrow(exception);

            try
            {
                ExceptionHandler handler = Fx.AsynchronousThreadExceptionHandler;
                return handler == null ? false : handler.HandleException(exception);
            }
            catch (Exception secondException)
            {
                // Don't let a new exception hide the original exception.
                TraceExceptionNoThrow(secondException);
            }

            return false;
        }

        private static void UpdateLevel(EtwDiagnosticTrace trace)
        {
            if (trace == null)
            {
                return;
            }

            if (TraceCore.ActionItemCallbackInvokedIsEnabled(trace) ||
                TraceCore.ActionItemScheduledIsEnabled(trace))
            {
                trace.SetEnd2EndActivityTracingEnabled(true);
            }
        }

        private static void UpdateLevel()
        {
            UpdateLevel(Fx.Trace);
        }

        public abstract class ExceptionHandler
        {
            [Fx.Tag.SecurityNote(Miscellaneous = "Must not call into PT code as it is called within a CER.")]
            public abstract bool HandleException(Exception exception);
        }

        public static class Tag
        {
            public enum CacheAttrition
            {
                None,
                ElementOnTimer,

                // A finalizer/WeakReference based cache, where the elements are held by WeakReferences (or hold an
                // inner object by a WeakReference), and the weakly-referenced object has a finalizer which cleans the
                // item from the cache.
                ElementOnGC,

                // A cache that provides a per-element token, delegate, interface, or other piece of context that can
                // be used to remove the element (such as IDisposable).
                ElementOnCallback,

                FullPurgeOnTimer,
                FullPurgeOnEachAccess,
                PartialPurgeOnTimer,
                PartialPurgeOnEachAccess,
            }

            public enum ThrottleAction
            {
                Reject,
                Pause,
            }

            public enum ThrottleMetric
            {
                Count,
                Rate,
                Other,
            }

            public enum Location
            {
                InProcess,
                OutOfProcess,
                LocalSystem,
                LocalOrRemoteSystem, // as in a file that might live on a share
                RemoteSystem,
            }

            public enum SynchronizationKind
            {
                LockStatement,
                MonitorWait,
                MonitorExplicit,
                InterlockedNoSpin,
                InterlockedWithSpin,

                // Same as LockStatement if the field type is object.
                FromFieldType,
            }

            [Flags]
            public enum BlocksUsing
            {
                MonitorEnter,
                MonitorWait,
                ManualResetEvent,
                AutoResetEvent,
                AsyncResult,
                IAsyncResult,
                PInvoke,
                InputQueue,
                ThreadNeutralSemaphore,
                PrivatePrimitive,
                OtherInternalPrimitive,
                OtherFrameworkPrimitive,
                OtherInterop,
                Other,

                NonBlocking, // For use by non-blocking SynchronizationPrimitives such as IOThreadScheduler
            }

            public static class Strings
            {
                internal const string ExternallyManaged = "externally managed";
                internal const string AppDomain = "AppDomain";
                internal const string DeclaringInstance = "instance of declaring class";
                internal const string Unbounded = "unbounded";
                internal const string Infinite = "infinite";
            }

            [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Class,
                AllowMultiple = true, Inherited = false)]
            [Conditional("DEBUG")]
            public sealed class FriendAccessAllowedAttribute : Attribute
            {
                public FriendAccessAllowedAttribute(string assemblyName) :
                    base()
                {
                    this.AssemblyName = assemblyName;
                }

                public string AssemblyName { get; set; }
            }

            public static class Throws
            {
                [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor,
                    AllowMultiple = true, Inherited = false)]
                [Conditional("CODE_ANALYSIS_CDF")]
                public sealed class TimeoutAttribute : ThrowsAttribute
                {
                    public TimeoutAttribute() :
                        this("The operation timed out.")
                    {
                    }

                    public TimeoutAttribute(string diagnosis) :
                        base(typeof(TimeoutException), diagnosis)
                    {
                    }
                }
            }

            [AttributeUsage(AttributeTargets.Field)]
            [Conditional("CODE_ANALYSIS_CDF")]
            public sealed class CacheAttribute : Attribute
            {
                private readonly Type _elementType;
                private readonly CacheAttrition _cacheAttrition;

                public CacheAttribute(Type elementType, CacheAttrition cacheAttrition)
                {
                    Scope = Strings.DeclaringInstance;
                    SizeLimit = Strings.Unbounded;
                    Timeout = Strings.Infinite;

                    if (elementType == null)
                    {
                        throw Fx.Exception.ArgumentNull("elementType");
                    }

                    _elementType = elementType;
                    _cacheAttrition = cacheAttrition;
                }

                public Type ElementType
                {
                    get
                    {
                        return _elementType;
                    }
                }

                public CacheAttrition CacheAttrition
                {
                    get
                    {
                        return _cacheAttrition;
                    }
                }

                public string Scope { get; set; }
                public string SizeLimit { get; set; }
                public string Timeout { get; set; }
            }

            [AttributeUsage(AttributeTargets.Field)]
            [Conditional("CODE_ANALYSIS_CDF")]
            public sealed class QueueAttribute : Attribute
            {
                private readonly Type _elementType;

                public QueueAttribute(Type elementType)
                {
                    Scope = Strings.DeclaringInstance;
                    SizeLimit = Strings.Unbounded;

                    if (elementType == null)
                    {
                        throw Fx.Exception.ArgumentNull("elementType");
                    }

                    _elementType = elementType;
                }

                public Type ElementType
                {
                    get
                    {
                        return _elementType;
                    }
                }

                public string Scope { get; set; }
                public string SizeLimit { get; set; }
                public bool StaleElementsRemovedImmediately { get; set; }
                public bool EnqueueThrowsIfFull { get; set; }
            }

            [AttributeUsage(AttributeTargets.Field)]
            [Conditional("CODE_ANALYSIS_CDF")]
            public sealed class ThrottleAttribute : Attribute
            {
                private readonly ThrottleAction _throttleAction;
                private readonly ThrottleMetric _throttleMetric;
                private readonly string _limit;

                public ThrottleAttribute(ThrottleAction throttleAction, ThrottleMetric throttleMetric, string limit)
                {
                    Scope = Strings.AppDomain;

                    if (string.IsNullOrEmpty(limit))
                    {
                        throw Fx.Exception.ArgumentNullOrEmpty("limit");
                    }

                    _throttleAction = throttleAction;
                    _throttleMetric = throttleMetric;
                    _limit = limit;
                }

                public ThrottleAction ThrottleAction
                {
                    get
                    {
                        return _throttleAction;
                    }
                }

                public ThrottleMetric ThrottleMetric
                {
                    get
                    {
                        return _throttleMetric;
                    }
                }

                public string Limit
                {
                    get
                    {
                        return _limit;
                    }
                }

                public string Scope
                {
                    get; set;
                }
            }

            [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Constructor,
                AllowMultiple = true, Inherited = false)]
            [Conditional("CODE_ANALYSIS_CDF")]
            public sealed class ExternalResourceAttribute : Attribute
            {
                private readonly Location _location;
                private readonly string _description;

                public ExternalResourceAttribute(Location location, string description)
                {
                    _location = location;
                    _description = description;
                }

                public Location Location
                {
                    get
                    {
                        return _location;
                    }
                }

                public string Description
                {
                    get
                    {
                        return _description;
                    }
                }
            }

            // Set on a class when that class uses lock (this) - acts as though it were on a field
            //     private object this;
            [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, Inherited = false)]
            [Conditional("CODE_ANALYSIS_CDF")]
            public sealed class SynchronizationObjectAttribute : Attribute
            {
                public SynchronizationObjectAttribute()
                {
                    Blocking = true;
                    Scope = Strings.DeclaringInstance;
                    Kind = SynchronizationKind.FromFieldType;
                }

                public bool Blocking { get; set; }
                public string Scope { get; set; }
                public SynchronizationKind Kind { get; set; }
            }

            [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
            [Conditional("CODE_ANALYSIS_CDF")]
            public sealed class SynchronizationPrimitiveAttribute : Attribute
            {
                private readonly BlocksUsing _blocksUsing;

                public SynchronizationPrimitiveAttribute(BlocksUsing blocksUsing)
                {
                    _blocksUsing = blocksUsing;
                }

                public BlocksUsing BlocksUsing
                {
                    get
                    {
                        return _blocksUsing;
                    }
                }

                public bool SupportsAsync { get; set; }
                public bool Spins { get; set; }
                public string ReleaseMethod { get; set; }
            }

            [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
            [Conditional("CODE_ANALYSIS_CDF")]
            public sealed class BlockingAttribute : Attribute
            {
                public BlockingAttribute()
                {
                }

                public string CancelMethod { get; set; }
                public Type CancelDeclaringType { get; set; }
                public string Conditional { get; set; }
            }

            // Sometime a method will call a conditionally-blocking method in such a way that it is guaranteed
            // not to block (i.e. the condition can be Asserted false).  Such a method can be marked as
            // GuaranteeNonBlocking as an assertion that the method doesn't block despite calling a blocking method.
            //
            // Methods that don't call blocking methods and aren't marked as Blocking are assumed not to block, so
            // they do not require this attribute.
            [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
            [Conditional("CODE_ANALYSIS_CDF")]
            public sealed class GuaranteeNonBlockingAttribute : Attribute
            {
                public GuaranteeNonBlockingAttribute()
                {
                }
            }

            [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
            [Conditional("CODE_ANALYSIS_CDF")]
            public sealed class NonThrowingAttribute : Attribute
            {
                public NonThrowingAttribute()
                {
                }
            }

            [SuppressMessage(FxCop.Category.Performance, "CA1813:AvoidUnsealedAttributes",
                Justification = "This is intended to be an attribute heirarchy. It does not affect product perf.")]
            [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor,
                AllowMultiple = true, Inherited = false)]
            [Conditional("CODE_ANALYSIS_CDF")]
            public class ThrowsAttribute : Attribute
            {
                private readonly Type _exceptionType;
                private readonly string _diagnosis;

                public ThrowsAttribute(Type exceptionType, string diagnosis)
                {
                    if (exceptionType == null)
                    {
                        throw Fx.Exception.ArgumentNull("exceptionType");
                    }
                    if (string.IsNullOrEmpty(diagnosis))
                    {
                        throw Fx.Exception.ArgumentNullOrEmpty("diagnosis");
                    }

                    _exceptionType = exceptionType;
                    _diagnosis = diagnosis;
                }

                public Type ExceptionType
                {
                    get
                    {
                        return _exceptionType;
                    }
                }

                public string Diagnosis
                {
                    get
                    {
                        return _diagnosis;
                    }
                }
            }

            [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
            [Conditional("CODE_ANALYSIS_CDF")]
            public sealed class InheritThrowsAttribute : Attribute
            {
                public InheritThrowsAttribute()
                {
                }

                public Type FromDeclaringType { get; set; }
                public string From { get; set; }
            }

            [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
            [Conditional("CODE_ANALYSIS_CDF")]
            public sealed class KnownXamlExternalAttribute : Attribute
            {
                public KnownXamlExternalAttribute()
                {
                }
            }

            [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
            [Conditional("CODE_ANALYSIS_CDF")]
            public sealed class XamlVisibleAttribute : Attribute
            {
                public XamlVisibleAttribute()
                    : this(true)
                {
                }

                public XamlVisibleAttribute(bool visible)
                {
                    this.Visible = visible;
                }

                public bool Visible
                {
                    get;
                    private set;
                }
            }

            [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class |
                AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method |
                AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface |
                AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
            [Conditional("CODE_ANALYSIS_CDF")]
            public sealed class SecurityNoteAttribute : Attribute
            {
                public SecurityNoteAttribute()
                {
                }

                public string Critical
                {
                    get;
                    set;
                }
                public string Safe
                {
                    get;
                    set;
                }
                public string Miscellaneous
                {
                    get;
                    set;
                }
            }
        }

        internal abstract class Thunk<T> where T : class
        {
            [Fx.Tag.SecurityNote(Critical = "Make these safe to use in SecurityCritical contexts.")]
            [SecurityCritical]
            private T _callback;

            [Fx.Tag.SecurityNote(Critical = "Accesses critical field.", Safe = "Data provided by caller.")]
            [SecuritySafeCritical]
            protected Thunk(T callback)
            {
                _callback = callback;
            }

            internal T Callback
            {
                [Fx.Tag.SecurityNote(Critical = "Accesses critical field.", Safe = "Data is not privileged.")]
                [SecuritySafeCritical]
                get
                {
                    return _callback;
                }
            }
        }

        internal sealed class ActionThunk<T1> : Thunk<Action<T1>>
        {
            public ActionThunk(Action<T1> callback) : base(callback)
            {
            }

            public Action<T1> ThunkFrame
            {
                get
                {
                    return new Action<T1>(UnhandledExceptionFrame);
                }
            }
            [Fx.Tag.SecurityNote(Critical = "Calls PrepareConstrainedRegions which has a LinkDemand",
                            Safe = "Guaranteed not to call into PT user code from the finally.")]
            [SecuritySafeCritical]

            private void UnhandledExceptionFrame(T1 result)
            {
                try
                {
                    Callback(result);
                }
                catch (Exception exception)
                {
                    if (!Fx.HandleAtThreadBase(exception))
                    {
                        throw;
                    }
                }
            }
        }

        internal sealed class AsyncThunk : Thunk<AsyncCallback>
        {
            public AsyncThunk(AsyncCallback callback) : base(callback)
            {
            }

            public AsyncCallback ThunkFrame
            {
                get
                {
                    return new AsyncCallback(UnhandledExceptionFrame);
                }
            }
            [Fx.Tag.SecurityNote(Critical = "Calls PrepareConstrainedRegions which has a LinkDemand",
                            Safe = "Guaranteed not to call into PT user code from the finally.")]
            [SecuritySafeCritical]

            private void UnhandledExceptionFrame(IAsyncResult result)
            {
                try
                {
                    Callback(result);
                }
                catch (Exception exception)
                {
                    if (!Fx.HandleAtThreadBase(exception))
                    {
                        throw;
                    }
                }
            }
        }

        internal class InternalException : Exception
        {
            public InternalException(string description)
                : base(InternalSR.ShipAssertExceptionMessage(description))
            {
            }
        }

        internal class FatalInternalException : InternalException
        {
            public FatalInternalException(string description)
                : base(description)
            {
            }
        }
    }
}
