//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Message storage configuration interface</para>
    ///   <para>Must be used as a base for all message storage configuration</para>
    /// </summary>
    [Serializable]
    [XmlRoot("MessageStorage")]
    public abstract class MessageStorageConfiguration<TMessageStorage>
        : IMessageStorageConfiguration
        where TMessageStorage : IMessageStorage {
        public MessageStorageConfiguration() {
            TypeName = GetType().FullName;
        }

        /// <summary>
        ///   <para>Type of Message Store as a serializable string</para>
        ///   <para>This value is visible to the serializer, but hidden from intellisense</para>
        /// </summary>
        [XmlAttribute]
        [Browsable(false)]
        public string TypeName { get; set; }

        #region IMessageStorageConfiguration Members

        /// <summary>
        ///   <para>Create message storage object</para>
        ///   <para>Explicit implementation</para>
        /// </summary>
        IMessageStorage IMessageStorageConfiguration
            .Create(HostConfiguration hostConfig) {
            return Create(hostConfig);
        }

        #endregion

        /// <summary>
        ///   <para>Create message storage object</para>
        /// </summary>
        public abstract TMessageStorage Create(HostConfiguration hostConfig);

        /// <summary>
        ///   <para>String Representation</para>
        /// </summary>
        public override string ToString() {
            return TypeName;
        }
        }
}