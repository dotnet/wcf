// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET
using System.Configuration;
using System.ServiceModel.Configuration;

namespace WcfService
{
    public class ServiceActivationsBuilder : ConfigurationBuilder
    {
        public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
        {
            configSection = base.ProcessConfigurationSection(configSection);
            if (configSection is ServiceHostingEnvironmentSection)
            {
                UpdateServiceActivations(configSection as ServiceHostingEnvironmentSection);
            }
            return configSection;
        }

        private void UpdateServiceActivations(ServiceHostingEnvironmentSection serviceHostingEnvironmentSection)
        {
            var serviceHostTypes = TestDefinitionHelper.GetAttributedServiceHostTypes();
            foreach(var sht in serviceHostTypes)
            {
                foreach (TestServiceDefinitionAttribute attr in sht.GetCustomAttributes(typeof(TestServiceDefinitionAttribute), false))
                {
                    var sae = new ServiceActivationElement(attr.BasePath, sht.AssemblyQualifiedName, "WcfService.ActivationServiceHostFactory, __Code");
                    serviceHostingEnvironmentSection.ServiceActivations.Add(sae);
                }
            }
        }
    }

}
#endif
