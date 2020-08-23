using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniBlink.WebClientX
{
    internal class SystemNetworkCredential : NetworkCredential
    {
        internal static readonly SystemNetworkCredential defaultCredential = new SystemNetworkCredential();

        private SystemNetworkCredential()
            : base(string.Empty, string.Empty, string.Empty)
        {
        }
    }
}
