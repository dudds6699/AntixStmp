//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Antix.Mail.Smtp.Impostor;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antix.Mail.Tests {
    [TestClass]
    public class HostConfigurationTests {
        [TestMethod]
        public void SaveLoadConfiguration() {
            var name = "Test Host";
            var ipAddress = IPAddress.Any;
            var port = 25;
            var dropPath = Path.GetTempPath();
            var path = Path.Combine(dropPath, "SaveConfigurationItem.xml");

            var config = new HostCollectionConfiguration();
            config.Add(new HostConfiguration
                       {
                           Name = name,
                           IPAddress = ipAddress,
                           Port = port,
                           MessageStorage = new FileMessageStorageConfiguration
                                            {
                                                DropPath = dropPath
                                            }
                       });
            config.Add(new HostConfiguration
                       {
                           Name = name,
                           IPAddress = ipAddress,
                           Port = port + 1,
                           MessageStorage = new FileMessageStorageConfiguration
                                            {
                                                DropPath = dropPath
                                            }
                       });

            using (var writer = new XmlTextWriter(path, Encoding.UTF8)) {
                var serializer = new XmlSerializer(typeof(HostCollectionConfiguration));
                serializer.Serialize(writer, config);
            }

            // deserializer
            using (var reader = new XmlTextReader(path)) {
                var serializer = new XmlSerializer(typeof(HostCollectionConfiguration));
                config = (HostCollectionConfiguration) serializer.Deserialize(reader);
            }

            Assert.AreEqual(2, config.Count);
            foreach (var item in config) {
                Assert.AreEqual(ipAddress, item.IPAddress);
                Assert.AreEqual(port, item.Port);
                Assert.IsNotNull(item.MessageStorage);
                Assert.IsInstanceOfType(item.MessageStorage, typeof(FileMessageStorageConfiguration));

                var messageStorage = (FileMessageStorageConfiguration) item.MessageStorage;
                Assert.AreEqual(dropPath, messageStorage.DropPath);

                port++;
            }
        }
    }
}