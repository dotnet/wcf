#if disabled
// using System.Security.Permissions;
using System.Threading.Tasks;

namespace Microsoft.Xml {
				using System;
				

    // [PermissionSetAttribute(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public partial class XmlSecureResolver : XmlResolver {
        public override Task<object> GetEntityAsync(Uri absoluteUri, string role, Type ofObjectToReturn) {
            permissionSet.PermitOnly();
            return resolver.GetEntityAsync(absoluteUri, role, ofObjectToReturn);
        }
    }
}
#endif 