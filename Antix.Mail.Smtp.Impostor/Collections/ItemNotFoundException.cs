//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Runtime.Serialization;

namespace Antix.Mail.Smtp.Impostor.Collections {
    /// <summary>
    ///   <para>Exception for an smtp host not found</para>
    /// </summary>
    [Serializable]
    public class ItemNotFoundException : Exception {
        /// <summary>
        ///   <para>Create Exception</para>
        /// </summary>
        public ItemNotFoundException(string key) : base(key) {}

        /// <summary>
        ///   <para>Create Exception</para>
        /// </summary>
        protected ItemNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) {}
    }
}