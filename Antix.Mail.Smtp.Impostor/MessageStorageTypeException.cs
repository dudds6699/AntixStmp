//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Runtime.Serialization;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Thrown when a type is not and expected to be a message storage type</para>
    /// </summary>
    [Serializable]
    public class MessageStorageTypeException : Exception {
        /// <summary>
        ///   <para>Create Exception</para>
        /// </summary>
        public MessageStorageTypeException(Type type)
            : base(string.Format("Type '{0}' is not a valid IMessageStorage Type", type)) {}

        protected MessageStorageTypeException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) {}
    }
}