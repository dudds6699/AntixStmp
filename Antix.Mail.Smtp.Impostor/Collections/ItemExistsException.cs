//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Runtime.Serialization;

namespace Antix.Mail.Smtp.Impostor.Collections {
    /// <summary>
    ///   <para>Exception for duplicate instance of an smtp host</para>
    /// </summary>
    [Serializable]
    public class ItemExistsException : Exception {
        /// <summary>
        ///   <para>Create Exception</para>
        /// </summary>
        public ItemExistsException(string key) : base(key) {}

        /// <summary>
        ///   <para>Create Exception</para>
        /// </summary>
        protected ItemExistsException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) {}
    }
}