//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System.Globalization;
using System.Net;
using System.Windows.Controls;

using Antix.Mail.Smtp.Impostor.Client.Properties;

namespace Antix.Mail.Smtp.Impostor.Client {
    /// <summary>
    ///   <para>Validate the IP</para>
    /// </summary>
    public class IPAddressValidationRule : ValidationRule {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            IPAddress ipAddress;
            return new ValidationResult(
                IPAddress.TryParse(value as string, out ipAddress), Resources.IP_Invalid_Message);
        }
    }
}