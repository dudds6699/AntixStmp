//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Session data class</para>
    /// </summary>
    public class Message {
        /// <summary>
        ///   <para>Create object</para>
        /// </summary>
        internal Message() {}

        /// <summary>
        ///   <para>String representation</para>
        /// </summary>
        /// <returns>Subject, From</returns>
        public override string ToString() {
            return string.Format("{0}, {1}", Subject, From);
        }

        #region Properties

        /// <summary>
        ///   <para>Gets all headers received</para>
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Headers { get; internal set; }

        /// <summary>
        ///   <para>Gets the message id header</para>
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        ///   <para>Gets the message subject header</para>
        /// </summary>
        public string Subject { get; internal set; }

        /// <summary>
        ///   <para>Gets the from address, if present in the headers then this is used in preference</para>
        /// </summary>
        public MailAddress From { get; internal set; }

        /// <summary>
        ///   <para>Gets a list of To recipients</para>
        /// </summary>
        public IEnumerable<MailAddress> To { get; internal set; }

        /// <summary>
        ///   <para>Gets the date and time the message was received</para>
        /// </summary>
        public DateTime ReceivedOn { get; internal set; }

        /// <summary>
        ///   <para>Path to the file where the message is stored</para>
        ///   <para>This is not passed over services</para>
        /// </summary>
        public string Path { get; set; }


        /// <summary>
        ///   <para>Gets all data</para>
        /// </summary>
        public string Data { get; internal set; }

        #endregion
    }
}