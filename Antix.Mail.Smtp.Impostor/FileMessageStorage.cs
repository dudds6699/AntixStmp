//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

using Antix.Mail.Smtp.Impostor.Collections;
using Antix.Mail.Smtp.Impostor.Properties;

namespace Antix.Mail.Smtp.Impostor {

    #region configuration class

    /// <summary>
    ///   <para>FileMessageStorage Configuration</para>
    /// </summary>
    public class FileMessageStorageConfiguration : MessageStorageConfiguration<FileMessageStorage> {
        /// <summary>
        ///   <para>Create object</para>
        /// </summary>
        public FileMessageStorageConfiguration() {
            if (Settings.Default == null) return;

            // set default values from the config file
            DropPath = Settings.Default.FileMessageStorage_DropPath;
            if (string.IsNullOrWhiteSpace(DropPath)) {
                DropPath = string.Concat(Path.GetTempPath().TrimEnd('\\'), "\\{0}_{1}");
            }
            FileExtension = Settings.Default.FileMessageStorage_FileExtension;
        }

        /// <summary>
        /// </summary>
        [XmlElement]
        public string DropPath { get; set; }

        [XmlElement]
        public string FileExtension { get; set; }

        /// <summary>
        ///   <para>Create storage from this config</para>
        /// </summary>
        public override FileMessageStorage Create(HostConfiguration hostConfig) {
            if (hostConfig == null) throw new ArgumentNullException("hostConfig");

            var messageStorage = new FileMessageStorage
                                 {
                                     DropPath = string.Format(DropPath, hostConfig.IPAddress, hostConfig.Port),
                                     FileExtension = FileExtension
                                 };
            messageStorage.Reset();

            return messageStorage;
        }
    }

    #endregion

    /// <summary>
    ///   <para>Message Store for the File System</para>
    ///   <para>Can only be created using FileMessageStorageConfiguration.Create</para>
    /// </summary>
    public class FileMessageStorage : IMessageStorage {
        private bool disposed;

        /// <summary>
        ///   <para>Create object</para>
        /// </summary>
        internal FileMessageStorage() {
            _startItemsWatcher = true;
        }

        #region IMessageStorage Members

        /// <summary>
        ///   <para>Number of messages</para>
        /// </summary>
        public int Count {
            get { return _items.Count; }
        }

        /// <summary>
        ///   <para>Id Indexer</para>
        /// </summary>
        /// <param name = "id">Id</param>
        /// <returns>MessageInfo</returns>
        public MessageInfo RetrieveInfo(string id) {
            return _items.FirstOrDefault(
                i => i.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///   <para>Check for id in the Items collection</para>
        /// </summary>
        /// <param name = "id">Id</param>
        /// <returns>True if found, else False</returns>
        public bool Contains(string id) {
            return _items.Any(
                i => i.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///   <para>Store a message</para>
        /// </summary>
        /// <param name = "message">Message to store</param>
        public void Store(Message message) {
            if (message == null) throw new ArgumentNullException("message");

            message.Path = GetFilePath(message.Id);

            // append a number to uniquely id the item
            var i = 0;
            while (Contains(message.Id)) {
                message.Id = string.Format("{0}({1})",
                                           message.Id.Head(string.Format("({0})", i)),
                                           ++i);
            }

            Add(message.ToInfo());

            // Save the message
            var file = File.CreateText(message.Path);
            try {
                file.Write(message.Data);
            }
            catch (Exception) {
                throw;
            }
            finally {
                file.Close();
            }
        }

        /// <summary>
        ///   <para>Retrieve a message by id</para>
        /// </summary>
        /// <param name = "id">Message Id</param>
        /// <returns>Message</returns>
        public Message Retrieve(string id) {
            var path = GetFilePath(id);

            return File.Exists(path)
                       ? GetMessage(path)
                       : null;
        }

        /// <summary>
        ///   <para>Delete a message by id</para>
        /// </summary>
        /// <param name = "id">Message Id</param>
        public void Delete(string id) {
            // remove from items collection            
            var item = RetrieveInfo(id);
            if (item != null) Remove(item);

            // delete the file if it exists
            var path = GetFilePath(id);
            if (File.Exists(path)) File.Delete(path);
        }

        /// <summary>
        ///   <para>Delete all messages</para>
        /// </summary>
        public void DeleteAll() {
            if (Directory.Exists(DropPath)) {
                if (_itemsWatcher != null) _itemsWatcher.EnableRaisingEvents = false;
                foreach (var path in Directory.GetFiles(DropPath, GetFileFilter())) {
                    File.Delete(path);
                }
                if (_itemsWatcher != null) _itemsWatcher.EnableRaisingEvents = true;
            }

            _items.Clear();
        }

        /// <summary>
        ///   <para>Enumerate the messages</para>
        /// </summary>
        /// <returns>Enumeration of MessageInfo's</returns>
        public IEnumerator<MessageInfo> GetEnumerator() {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        ///   <para>Dispose</para>
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        /// <summary>
        ///   <para>Finalize object</para>
        /// </summary>
        ~FileMessageStorage() {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                if (disposing) {
                    if (_itemsWatcher != null) {
                        _itemsWatcher.Dispose();
                        _itemsWatcher = null;
                        _items = null;
                    }
                }
            }
            disposed = true;
        }

        #region constants/properties

        private const string FILE_EXTN_PATTERN = "*{0}";

        private readonly bool _startItemsWatcher;
        private ItemCollection<MessageInfo> _items;
        private FileSystemWatcher _itemsWatcher;

        /// <summary>
        ///   <para>Path where recieved e-mails are stored</para>
        ///   <para>Defaults to DropPath in config or the temp directory</para>
        ///   <list type = "table">
        ///     <listheader>Placeholders</listheader>
        ///     <item><term>{0}</term><description>host ip</description></item>
        ///     <item><term>{1}</term><description>host port</description></item>
        ///   </list>
        /// </summary>
        public string DropPath { get; internal set; }

        /// <summary>
        ///   <para>File extension used for files containing received e-mails</para>
        ///   <para>Defaults to .eml</para>
        /// </summary>
        public string FileExtension { get; internal set; }

        #endregion

        #region methods

        /// <summary>
        ///   <para>Reset the items</para>
        /// </summary>
        public void Reset() {
            if (string.IsNullOrEmpty(DropPath)) return;

            // Check permission
            new FileIOPermission(FileIOPermissionAccess.Write, DropPath)
                .Assert();
            Directory.CreateDirectory(DropPath);

            // load items into memory
            _items = new MessageInfoCollection(
                Directory.GetFiles(DropPath, GetFileFilter())
                    .Select(f => GetMessageInfo(f))
                );

            if (_startItemsWatcher) StartDropPathWatcher();

            if (CollectionChanged != null)
                CollectionChanged(this,
                                  new NotifyCollectionChangedEventArgs(
                                      NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        ///   <para>Start Watching the files in the drop path</para>
        /// </summary>
        public void StartDropPathWatcher() {
            StopDropPathWatcher();

            _itemsWatcher = new FileSystemWatcher(DropPath)
                            {
                                Filter = GetFileFilter(),
                                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
                            };

            _itemsWatcher.Created += (sender, e) => {
                                         if (FileExtension.Equals(
                                             Path.GetExtension(e.Name), StringComparison.CurrentCultureIgnoreCase)) {
                                             if (!Contains(Path.GetFileNameWithoutExtension(e.Name)))
                                                 Add(GetMessageInfo(e.FullPath));
                                         }
                                     };
            _itemsWatcher.Deleted += (sender, e) => {
                                         var item = RetrieveInfo(Path.GetFileNameWithoutExtension(e.Name));
                                         if (item != null) Remove(item);
                                     };

            _itemsWatcher.EnableRaisingEvents = true;
        }


        /// <summary>
        ///   <para>Stop Watching the files in the drop path</para>
        /// </summary>
        public void StopDropPathWatcher() {
            if (_itemsWatcher != null) {
                _itemsWatcher.Dispose();
                _itemsWatcher = null;
            }
        }

        /// <summary>
        ///   <para>Gets the File Filter</para>
        /// </summary>
        public string GetFileFilter() {
            return string.Format(FILE_EXTN_PATTERN, FileExtension);
        }

        /// <summary>
        ///   <para>Gets the path to a file given the id</para>
        /// </summary>
        /// <param name = "id">Message Id</param>
        /// <returns>Full path</returns>
        public string GetFilePath(string id) {
            return Path.Combine(
                DropPath,
                string.Concat(id, FileExtension));
        }

        /// <summary>
        ///   <para>Get a Message from a file</para>
        /// </summary>
        /// <param name = "path">File Path</param>
        /// <returns>Message</returns>
        private Message GetMessage(string path) {
            var fileCreationTime = File.GetCreationTime(path);
            var fileStream = OpenFileForReading(path);

            if (fileStream == null) return null;

            try {
                var reader = new StreamReader(fileStream);
                var data = reader.ReadToEnd();
                var headers = Session.GetHeaders(data);

                return new Message
                       {
                           Id = Path.GetFileNameWithoutExtension(path),
                           From =
                               headers.ContainsKey(Server.HEADER_FROM)
                                   ? headers[Server.HEADER_FROM].ToMailAddress()
                                   : null,
                           To =
                               headers.ContainsKey(Server.HEADER_TO)
                                   ? headers[Server.HEADER_TO].ToMailAddresses()
                                   : null,
                           Subject = headers.ContainsKey(Server.HEADER_SUBJECT) ? headers[Server.HEADER_SUBJECT] : null,
                           Headers = headers,
                           ReceivedOn = fileCreationTime,
                           Path = path,
                           Data = data
                       };
            }
            finally {
                fileStream.Close();
            }
        }

        /// <summary>
        ///   <para>Get a MessageInfo from the file</para>
        /// </summary>
        /// <param name = "path">File Path</param>
        /// <returns>MessageInfo</returns>
        private MessageInfo GetMessageInfo(string path) {
            var fileCreationTime = File.GetCreationTime(path);
            var fileStream = OpenFileForReading(path);

            if (fileStream == null) return null;

            try {
                var headers = Session.GetHeaders(
                    fileStream.ReadTo(Server.HEADERS_TERMINATOR, Encoding.UTF8));

                return new MessageInfo
                       {
                           Id = Path.GetFileNameWithoutExtension(path),
                           From = headers.ContainsKey(Server.HEADER_FROM) ? headers[Server.HEADER_FROM] : null,
                           To = headers.ContainsKey(Server.HEADER_TO) ? headers[Server.HEADER_TO] : null,
                           Subject = headers.ContainsKey(Server.HEADER_SUBJECT) ? headers[Server.HEADER_SUBJECT] : null,
                           ReceivedOn = fileCreationTime,
                           Path = path
                       };
            }
            finally {
                fileStream.Close();
            }
        }

        private FileStream OpenFileForReading(string path) {
            var fileStream = default(FileStream);
            while (fileStream == null) {
                try {
                    fileStream = File.OpenRead(path);
                }
                catch (FileNotFoundException) {
                    return null;
                }
                catch (IOException) {
                    Thread.Sleep(100);
                }
            }

            return fileStream;
        }

        #region add/remove raise events

        /// <summary>
        ///   <para>Add an item</para>
        /// </summary>
        /// <param name = "item">Item to add</param>
        private void Add(MessageInfo item) {
            _items.Add(item);

            if (CollectionChanged != null)
                CollectionChanged(this,
                                  new NotifyCollectionChangedEventArgs(
                                      NotifyCollectionChangedAction.Add, item));
        }

        /// <summary>
        ///   <para>Remove an item</para>
        /// </summary>
        /// <param name = "item">Item to remove</param>
        private void Remove(MessageInfo item) {
            _items.Remove(item);

            if (CollectionChanged != null)
                CollectionChanged(this,
                                  new NotifyCollectionChangedEventArgs(
                                      NotifyCollectionChangedAction.Remove, item));
        }

        #endregion

        #endregion
    }
}