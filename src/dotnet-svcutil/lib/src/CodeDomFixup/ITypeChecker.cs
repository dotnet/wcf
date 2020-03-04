// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    // This interface is implemented by the project flavor to check if a type can be reused in the current profile.
    internal interface ITypeChecker
    {
        bool IsPresent(string typeName);
    }
}
