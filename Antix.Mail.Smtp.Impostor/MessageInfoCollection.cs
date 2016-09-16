//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System.Collections.Generic;

using Antix.Mail.Smtp.Impostor.Collections;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>A collection of MessageInfos</para>
    /// </summary>
    public class MessageInfoCollection : ItemCollection<MessageInfo> {
        internal MessageInfoCollection(IEnumerable<MessageInfo> items)
            : base(items) {}

        /// <summary>
        ///   <para>Gets a key given the item</para>
        /// </summary>
        public override string GetKey(MessageInfo item) {
            return item.Id;
        }
    }
}