//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Runtime.Serialization;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Message Info</para>
    /// </summary>
    [DataContract]
    public class MessageInfo : IEquatable<MessageInfo> {
        #region IEquatable<MessageInfo> Members

        /// <summary>
        ///   <para>Get whether equal by id</para>
        /// </summary>
        /// <param name = "other">Other MessageInfo</param>
        /// <returns>True if the id is the same as this one</returns>
        public bool Equals(MessageInfo other) {
            return Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        /// <summary>
        ///   <para>String representation</para>
        /// </summary>
        /// <returns>Subject, From</returns>
        public override string ToString() {
            return string.Format("{0}, {1}", Subject, From);
        }

        #region properies

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public string From { get; set; }

        [DataMember]
        public DateTime ReceivedOn { get; set; }

        /// <summary>
        ///   <para>Delimited list of To recipients</para>
        /// </summary>
        [DataMember]
        public string To { get; set; }

        [DataMember]
        public string CC { get; set; }

        /// <summary>
        ///   <para>Path to the file where the message is stored</para>
        ///   <para>This is not passed over services</para>
        /// </summary>
        public string Path { get; set; }

        #endregion
    }
}