﻿using System;
using System.IO;
using System.ComponentModel;

namespace Microsoft.Xml {
				using System;
				
    [Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IApplicationResourceStreamResolver
    {
        // Methods
        [Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Stream GetApplicationResourceStream(Uri relativeUri);
    }
}
