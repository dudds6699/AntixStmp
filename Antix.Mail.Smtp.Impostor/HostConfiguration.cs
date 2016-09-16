//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Net;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Antix.Mail.Smtp.Impostor.Properties;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Configuration for a host</para>
    /// </summary>
    [Serializable]
    [XmlRoot("Host")]
    public class HostConfiguration : IXmlSerializable {
        /// <summary>
        ///   <para>Create object</para>
        /// </summary>
        public HostConfiguration() {
            // defaults
            Name = null;
            if (Settings.Default != null) {
                IPAddressString = Settings.Default.HostIPAddress;
                Port = Settings.Default.HostPort;
                MessageStorage = new FileMessageStorageConfiguration();
            }
        }

        /// <summary>
        ///   <para>Name of the </para>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///   <para>IP Address</para>
        /// </summary>
        public string IPAddressString { get; set; }

        public IPAddress IPAddress {
            get {
                IPAddress ip;
                if (IPAddress.TryParse(IPAddressString, out ip)) {
                    return ip;
                }
                else {
                    return IPAddress.Any;
                }
            }
            set { IPAddressString = value.ToString(); }
        }


        /// <summary>
        ///   <para>Port</para>
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///   <para>Message storage</para>
        /// </summary>
        public IMessageStorageConfiguration MessageStorage { get; set; }

        #region IXmlSerializable Members

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            reader.Read();

            Name = reader.ReadElementString("Name");
            IPAddressString = reader.ReadElementString("IPAddress");
            Port = int.Parse(reader.ReadElementString("Port"));

            reader.MoveToAttribute("TypeName");

            MessageStorage = (IMessageStorageConfiguration)
                             new XmlSerializer(
                                 Type.GetType(reader.Value),
                                 new XmlRootAttribute("MessageStorage"))
                                 .Deserialize(reader);

            reader.Read();
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteElementString("Name", Name);

            writer.WriteElementString("IPAddress", IPAddressString);
            writer.WriteElementString("Port", Port.ToString());

            if (MessageStorage != null) {
                new XmlSerializer(
                    MessageStorage.GetType(),
                    new XmlRootAttribute("MessageStorage"))
                    .Serialize(writer, MessageStorage,
                               new XmlSerializerNamespaces(
                                   new[]
                                   {
                                       new XmlQualifiedName("", "")
                                   }
                                   ));
            }
        }

        #endregion

        /// <summary>
        ///   <para>String Representation</para>
        /// </summary>
        public override string ToString() {
            return string.Format("{0}:{1} ({2})", IPAddress, Port, MessageStorage);
        }
    }
}