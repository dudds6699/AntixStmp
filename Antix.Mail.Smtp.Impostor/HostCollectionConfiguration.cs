//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Holds the configuration information for a host</para>
    /// </summary>
    [Serializable]
    [XmlRoot("Hosts")]
    public class HostCollectionConfiguration
        : List<HostConfiguration>, IXmlSerializable {
        #region IXmlSerializable Members

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            var serializer = new XmlSerializer(typeof(HostConfiguration));

            reader.Read();
            var item = default(HostConfiguration);
            while ((item = (HostConfiguration) serializer.Deserialize(reader)) != null) {
                Add(item);
            }
        }

        public void WriteXml(XmlWriter writer) {
            var serializer = new XmlSerializer(typeof(HostConfiguration));

            foreach (var item in this) {
                serializer.Serialize(writer, item);
            }
        }

        #endregion
        }
}