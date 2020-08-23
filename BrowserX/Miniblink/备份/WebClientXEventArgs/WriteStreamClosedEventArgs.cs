using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MiniBlink.WebClientXEventArgs
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class WriteStreamClosedEventArgs : EventArgs
    {
        [Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Exception Error => null;

        [Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public WriteStreamClosedEventArgs()
        {
        }
    }
}
