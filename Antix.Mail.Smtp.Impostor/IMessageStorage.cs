//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Message Store interface</para>
    /// </summary>
    public interface IMessageStorage
        : IEnumerable<MessageInfo>, INotifyCollectionChanged, IDisposable {
        /// <summary>
        ///   <para>Gets the number of messages in the store</para>
        /// </summary>
        int Count { get; }

        /// <summary>
        ///   <para>Gets whether the store contains a message by id</para>
        /// </summary>
        /// <param name = "id">Message Id</param>
        /// <returns>True if found</returns>
        bool Contains(string id);

        /// <summary>
        ///   <para>Store a message</para>
        /// </summary>
        /// <param name = "message">Message to store</param>
        void Store(Message message);

        /// <summary>
        ///   <para>Retrieve a message by id</para>
        /// </summary>
        /// <param name = "id">Message id</param>
        /// <returns>Message</returns>
        Message Retrieve(string id);

        /// <summary>
        ///   <para>Get MessageInfo by id</para>
        /// </summary>
        /// <param name = "id">Message id</param>
        /// <returns>MessageInfo</returns>
        MessageInfo RetrieveInfo(string id);

        /// <summary>
        ///   <para>Delete a message by id</para>
        /// </summary>
        /// <param name = "id">Message Id</param>
        void Delete(string id);

        /// <summary>
        ///   <para>Delete all messages</para>
        /// </summary>
        void DeleteAll();
        }
}