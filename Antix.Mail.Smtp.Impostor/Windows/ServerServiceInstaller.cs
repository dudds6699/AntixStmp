//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System.ComponentModel;
using System.Configuration.Install;

namespace Antix.Mail.Smtp.Impostor.Windows {
    [RunInstaller(true)]
    public partial class ServerServiceInstaller : Installer {
        public ServerServiceInstaller() {
            InitializeComponent();
        }
    }
}