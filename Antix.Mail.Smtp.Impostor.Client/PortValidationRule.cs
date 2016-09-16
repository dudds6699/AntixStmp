//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System.Globalization;
using System.Windows.Controls;

using Antix.Mail.Smtp.Impostor.Client.Properties;

namespace Antix.Mail.Smtp.Impostor.Client {
    /// <summary>
    ///   <para>Validate the Port</para>
    /// </summary>
    public class PortValidationRule : ValidationRule {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            int port;
            if (int.TryParse(value as string, out port)) {
                if (port > 0 && port <= 65535) {
                    return new ValidationResult(true, null);
                }
            }

            return new ValidationResult(
                false, Resources.Port_Invalid_Message);
        }
    }
}