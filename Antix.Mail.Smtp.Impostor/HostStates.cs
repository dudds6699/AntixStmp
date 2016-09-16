//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Possible states of a host</para>
    /// </summary>
    public enum HostStates {
        /// <summary>
        ///   Stopped
        /// </summary>
        Stopped,

        /// <summary>
        ///   Error, attempt to start failed
        /// </summary>
        Error,

        /// <summary>
        ///   Started
        /// </summary>
        Started
    }
}