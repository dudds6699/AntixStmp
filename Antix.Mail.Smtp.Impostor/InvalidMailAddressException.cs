//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Runtime.Serialization;

namespace Antix.Mail.Smtp.Impostor {
    [Serializable]
    public class InvalidMailAddressException : Exception {
        public InvalidMailAddressException(string address)
            : base(string.Format("{0} not recognised as a valid e-mail address", address)) {
            Address = address;
        }

        protected InvalidMailAddressException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) {}

        public string Address { get; private set; }
    }
}