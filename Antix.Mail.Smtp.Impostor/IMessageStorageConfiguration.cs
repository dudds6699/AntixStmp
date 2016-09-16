//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>MessageStorageConfiguration interface </para>
    /// </summary>
    public interface IMessageStorageConfiguration {
        /// <summary>
        ///   <para>Create a message store based on this config</para>
        /// </summary>
        /// <param name = "hostConfig">The HostConfiguration that the message store is being created for</param>
        /// <returns>A new IMessageStorage</returns>
        IMessageStorage Create(HostConfiguration hostConfig);
    }
}