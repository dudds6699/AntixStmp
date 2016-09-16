//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System.Runtime.Serialization;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Information on a host</para>
    /// </summary>
    [DataContract]
    public class HostInfo {
        /// <summary>
        ///   <para>IP and Port</para>
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        ///   <para>Current Status</para>
        /// </summary>
        [DataMember]
        public HostStates Status { get; set; }

        /// <summary>
        ///   <para>Message Count</para>
        /// </summary>
        [DataMember]
        public int MessageCount { get; set; }
    }
}