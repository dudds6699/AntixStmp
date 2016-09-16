using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Antix.Mail.Smtp.Impostor.Properties;

namespace Antix.Mail.Smtp.Impostor {

    /// <summary>
    /// <para>Message Store for the File System</para>
    /// </summary>
    public class FileSystemMessageStore : IMessageStore {

        /// <summary>
        /// <para>Create object</para>
        /// </summary>
        /// <param name="path">Path to store messages at</param>
        public FileSystemMessageStore(string path) {
            _path = path;
        }

        /// <summary>
        /// <para>Create object</para>
        /// <para>Store messages in configured drop path, or temp if not set</para>
        /// </summary>
        public FileSystemMessageStore()
            : this(string.IsNullOrEmpty(Settings.Default.DropPath)
                    ? Path.GetTempPath()
                    : Settings.Default.DropPath) { }

        #region constants/properties

        private string _path;
        private const string FILE_EXTN = ".eml";
        private const string FILE_EXTN_PATTERN = "*.eml";

        #endregion

        #region IMessageStore Members

        /// <summary>
        /// <para>Number of messages</para>
        /// </summary>
        public int Count {
            get { return new DirectoryInfo(_path).GetFiles(FILE_EXTN_PATTERN).Length; }
        }

        /// <summary>
        /// <para>Store a message</para>
        /// </summary>
        /// <param name="message"></param>
        public void Store(Message message) {
            throw new NotImplementedException();
        }

        public Message Retrieve(string id) {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<MessageInfo> Members

        public IEnumerator<MessageInfo> GetEnumerator() {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        #endregion
    }
}
