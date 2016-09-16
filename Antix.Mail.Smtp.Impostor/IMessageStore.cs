using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Antix.Mail.Smtp.Impostor {

    /// <summary>
    /// <para>Message Store interface</para>
    /// </summary>
    public interface IMessageStore : IEnumerable<MessageInfo> {

        /// <summary>
        /// <para>Number of messages in the store</para>
        /// </summary>
        int Count { get; }

        /// <summary>
        /// <para>Store a message</para>
        /// </summary>
        /// <param name="message">Message to store</param>
        void Store(Message message);

        /// <summary>
        /// <para>Retrieve a message by id</para>
        /// </summary>
        /// <param name="id">Message Id</param>
        /// <returns>Message</returns>
        Message Retrieve(string id);
    }
}
