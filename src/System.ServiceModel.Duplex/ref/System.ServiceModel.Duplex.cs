// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.ServiceModel
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(4))]
    public sealed partial class CallbackBehaviorAttribute : System.Attribute, System.ServiceModel.Description.IEndpointBehavior
    {
        public CallbackBehaviorAttribute() { }
        public bool AutomaticSessionShutdown { get { return default(bool); } set { } }
        public bool UseSynchronizationContext { get { return default(bool); } set { } }
        void System.ServiceModel.Description.IEndpointBehavior.AddBindingParameters(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Channels.BindingParameterCollection parameters) { }
        void System.ServiceModel.Description.IEndpointBehavior.ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime) { }
        void System.ServiceModel.Description.IEndpointBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher) { }
        void System.ServiceModel.Description.IEndpointBehavior.Validate(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint) { }
    }
    public partial class DuplexChannelFactory<TChannel> : System.ServiceModel.ChannelFactory<TChannel>
    {
        public DuplexChannelFactory(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding) : base(default(System.Type)) { }
        public DuplexChannelFactory(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(default(System.Type)) { }
        public DuplexChannelFactory(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, string remoteAddress) : base(default(System.Type)) { }
        public DuplexChannelFactory(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : base(default(System.Type)) { }
        public DuplexChannelFactory(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(default(System.Type)) { }
        public override TChannel CreateChannel(System.ServiceModel.EndpointAddress address, System.Uri via) { return default(TChannel); }
        public TChannel CreateChannel(System.ServiceModel.InstanceContext callbackInstance) { return default(TChannel); }
        public TChannel CreateChannel(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.EndpointAddress address) { return default(TChannel); }
        public virtual TChannel CreateChannel(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.EndpointAddress address, System.Uri via) { return default(TChannel); }
    }
    public abstract partial class DuplexClientBase<TChannel> : System.ServiceModel.ClientBase<TChannel> where TChannel : class
    {
        protected DuplexClientBase(System.ServiceModel.InstanceContext callbackInstance) { }
        protected DuplexClientBase(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) { }
        protected DuplexClientBase(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) { }
        protected DuplexClientBase(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) { }
        protected DuplexClientBase(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) { }
    }
    public sealed partial class InstanceContext : System.ServiceModel.Channels.CommunicationObject, System.ServiceModel.IExtensibleObject<System.ServiceModel.InstanceContext>
    {
        public InstanceContext(object implementation) { }
        protected override System.TimeSpan DefaultCloseTimeout { get { return default(System.TimeSpan); } }
        protected override System.TimeSpan DefaultOpenTimeout { get { return default(System.TimeSpan); } }
        public System.Threading.SynchronizationContext SynchronizationContext { get { return default(System.Threading.SynchronizationContext); } set { } }
        System.ServiceModel.IExtensionCollection<System.ServiceModel.InstanceContext> System.ServiceModel.IExtensibleObject<System.ServiceModel.InstanceContext>.Extensions { get { return default(System.ServiceModel.IExtensionCollection<System.ServiceModel.InstanceContext>); } }
        public object GetServiceInstance(System.ServiceModel.Channels.Message message) { return default(object); }
        protected override void OnAbort() { }
        protected override System.IAsyncResult OnBeginClose(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default(System.IAsyncResult); }
        protected override System.IAsyncResult OnBeginOpen(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default(System.IAsyncResult); }
        protected override void OnClose(System.TimeSpan timeout) { }
        protected override void OnClosed() { }
        protected override void OnEndClose(System.IAsyncResult result) { }
        protected override void OnEndOpen(System.IAsyncResult result) { }
        protected override void OnFaulted() { }
        protected override void OnOpen(System.TimeSpan timeout) { }
        protected override void OnOpened() { }
        protected override void OnOpening() { }
    }
}
