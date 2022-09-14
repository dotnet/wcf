// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace System.Runtime
//{
//    internal static partial class TaskHelpers
//    {
//        public static TResult WaitForCompletion<TResult>(this ValueTask<TResult> valueTask)
//        {
//            Fx.Assert(valueTask.IsCompleted || !IsRunningOnIOThread, "Waiting on an IO Thread might cause problems");
//            // Waiting on an IO Thread can cause performance problems as we might block the IOThreadScheduler
//            // dequeuing loop.
//            return valueTask.GetAwaiter().GetResult();
//        }

//        public static void WaitForCompletion(this ValueTask valueTask)
//        {
//            Fx.Assert(valueTask.IsCompleted || !IsRunningOnIOThread, "Waiting on an IO Thread might cause problems");
//            // Waiting on an IO Thread can cause performance problems as we might block the IOThreadScheduler
//            // dequeuing loop.
//            valueTask.GetAwaiter().GetResult();
//        }
//    }
//}
