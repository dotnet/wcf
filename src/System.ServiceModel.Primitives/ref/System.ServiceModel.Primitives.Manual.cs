// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.ServiceModel.Channels
{
    public partial class BindingParameterCollection
    {
        protected override Type GetKeyForItem(object item) { return default(Type); }
        protected override void SetItem(int index, object item) { }
        protected override void InsertItem(int index, object item) { }
    }
}