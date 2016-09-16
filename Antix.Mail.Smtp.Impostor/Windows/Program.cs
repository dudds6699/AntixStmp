//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System.ServiceProcess;

namespace Antix.Mail.Smtp.Impostor.Windows {
    /// <summary>
    ///   <para>Program Class</para>
    /// </summary>
    public class Program {
        /// <summary>
        ///   <para>Entry point for Windows Service</para>
        /// </summary>
        /// <param name = "args"></param>
        private static void Main(string[] args) {
            ServiceBase.Run(
                new ServiceBase[]
                {
                    new ServerService()
                });
        }
    }
}