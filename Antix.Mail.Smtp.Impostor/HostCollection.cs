//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>A collection of Host objects</para>
    /// </summary>
    public class HostCollection : List<Host> {
        /// <summary>
        ///   <para>Checks for a host with the ip port combo in the collection</para>
        /// </summary>
        /// <param name = "ipAddress">IP address</param>
        /// <param name = "port">Port number</param>
        /// <returns>True if host exists</returns>
        public bool Contains(IPAddress ipAddress, int port) {
            return this.Any(i => i.IPAddress.Equals(ipAddress) && i.Port.Equals(port));
        }
    }
}