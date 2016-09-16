//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Host Event Arguments</para>
    /// </summary>
    public class HostEventArgs : EventArgs {
        /// <summary>
        ///   <para>Create object</para>
        /// </summary>
        internal HostEventArgs(
            HostEventTypes type,
            Session session) {
            Type = type;
            Session = session;
        }

        #region Properties

        /// <summary>
        ///   <para>Type of event</para>
        /// </summary>
        public HostEventTypes Type { get; private set; }

        /// <summary>
        ///   <para>Session object</para>
        /// </summary>
        public Session Session { get; private set; }

        #endregion
    }
}