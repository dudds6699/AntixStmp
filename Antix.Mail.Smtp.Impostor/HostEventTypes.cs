//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Types of host events</para>
    /// </summary>
    public enum HostEventTypes {
        /// <summary>
        ///   Host is started
        /// </summary>
        Started,

        /// <summary>
        ///   Host is stopped
        /// </summary>
        Stopped,

        /// <summary>
        ///   A host has connected
        /// </summary>
        SessionConnected,

        /// <summary>
        ///   A host has been identified
        /// </summary>
        SessionIdentified,

        /// <summary>
        ///   A host has received a message
        /// </summary>
        SessionMessageReceived,

        /// <summary>
        ///   A host has disconnected
        /// </summary>
        SessionDisconnected
    }
}