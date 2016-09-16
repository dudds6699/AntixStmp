//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System.ServiceProcess;

using Antix.Mail.Smtp.Impostor.Properties;

namespace Antix.Mail.Smtp.Impostor.Windows {
    internal partial class ServerService : ServiceBase {
        public static Server Server;

        public ServerService() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            if (Server == null) {
                Server = new Server();
                //Server.MessageReceivedEvent += Server.SaveMessageReceivedToDropPath;
            }

            foreach (var hostConfig in Settings.Default.Hosts) {
                var host = Server.CreateHost(hostConfig);

                host.Start();
            }
        }

        protected override void OnStop() {
            if (Server != null) Server.Dispose();
        }
    }
}