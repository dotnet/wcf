// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Reflection;
using Xunit.Sdk;

namespace Infrastructure.Common
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class WcfCurrentCultureAttribute : BeforeAfterTestAttribute
    {
        private readonly CultureInfo _cultureInfo;
        private CultureInfo _savedCultureInfo;

        public WcfCurrentCultureAttribute(string name)
        {
            _cultureInfo = new CultureInfo(name);
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            _savedCultureInfo = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = _cultureInfo;
        }

        public override void After(MethodInfo methodUnderTest)
        {
            CultureInfo.CurrentCulture = _savedCultureInfo;
        }
    }
}
