//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using Antix.Mail.Smtp.Impostor;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antix.Mail.Tests {
    ///<summary>
    ///  This is a test class for FileMessageStorageTest and is intended
    ///  to contain all FileMessageStorageTest Unit Tests
    ///</summary>
    [TestClass]
    public class FileMessageStorageTests {
        private const int MESSAGE_COUNT = 100;

        ///<summary>
        ///  A test for Retrieve
        ///</summary>
        [TestMethod]
        public void RetrieveTest() {
            const int port = 25;

            var dropPath = Path.Combine(Path.GetTempPath(), "RetrieveTest");
            if (Directory.Exists(dropPath)) Directory.Delete(dropPath, true);

            var server = new Server();
            try {
                var host = server.CreateHost(new HostConfiguration
                                             {
                                                 MessageStorage = new FileMessageStorageConfiguration
                                                                  {
                                                                      DropPath = dropPath
                                                                  }
                                             });
                var messageStorage = host.Messages;
                host.Start();

                ImpostorTests.SendEmails(server, MESSAGE_COUNT);

                Assert.AreEqual(MESSAGE_COUNT, messageStorage.Count);

                var firstId = messageStorage.First().Id;
                var message = messageStorage.Retrieve(firstId);

                Assert.IsNotNull(message);
                Assert.IsNotNull(message.Id);
                Assert.IsNotNull(message.From);
                Assert.IsNotNull(message.To);
                Assert.IsNotNull(message.Subject);
                Assert.IsNotNull(message.Headers);
                Assert.IsNotNull(message.Data);
            }
            finally {
                server.Dispose();
            }
        }

        ///<summary>
        ///  A test for Store
        ///</summary>
        [TestMethod]
        public void StoreTest() {
            var dropPath = Path.Combine(Path.GetTempPath(), "StoreTest");
            if (Directory.Exists(dropPath)) Directory.Delete(dropPath, true);

            var expectedId = Guid.NewGuid().ToString();
            var expectedPath = Path.Combine(dropPath, string.Format("{0}.xxx", expectedId));

            var storage = new FileMessageStorageConfiguration
                          {
                              DropPath = dropPath,
                              FileExtension = ".xxx"
                          }.Create(new HostConfiguration());

            var addedItems = default(IEnumerable<MessageInfo>);
            storage.CollectionChanged += (sender, e) => addedItems = e.NewItems.Cast<MessageInfo>();

            try {
                storage.Store(new Message {Id = expectedId});

                Assert.IsTrue(File.Exists(expectedPath),
                              string.Format("expected path {0} not found", expectedPath));

                Assert.IsNotNull(addedItems);
                Assert.AreEqual(1, addedItems.Count());
                Assert.AreEqual(expectedId, addedItems.First().Id);
            }
            finally {
                storage.Dispose();
            }
        }

        ///<summary>
        ///  A test for GetEnumerator
        ///</summary>
        [TestMethod]
        public void GetEnumeratorTest() {
            const int port = 25;

            var dropPath = Path.Combine(Path.GetTempPath(), "GetEnumeratorTest");
            if (Directory.Exists(dropPath)) Directory.Delete(dropPath, true);

            var server = new Server();

            try {
                var host = server.CreateHost(new HostConfiguration
                                             {
                                                 Port = port,
                                                 MessageStorage = new FileMessageStorageConfiguration
                                                                  {
                                                                      DropPath = dropPath
                                                                  }
                                             });
                host.Start();

                ImpostorTests.SendEmails(server, MESSAGE_COUNT);

                Assert.AreEqual(MESSAGE_COUNT, host.Messages.Count);
                foreach (var info in host.Messages) {
                    Assert.IsNotNull(info.Subject);
                }
            }
            finally {
                server.Dispose();
            }
        }

        [TestMethod]
        public void DeleteTest() {
            const int port = 25;

            var dropPath = Path.Combine(Path.GetTempPath(), "DeleteTest");
            if (Directory.Exists(dropPath)) Directory.Delete(dropPath, true);

            var server = new Server();

            try {
                var host = server.CreateHost(new HostConfiguration
                                             {
                                                 Port = port,
                                                 MessageStorage = new FileMessageStorageConfiguration
                                                                  {
                                                                      DropPath = dropPath
                                                                  }
                                             });
                host.Start();

                ImpostorTests.SendEmails(server, MESSAGE_COUNT);

                Assert.AreEqual(MESSAGE_COUNT, host.Messages.Count);
                var firstId = host.Messages.First().Id;
                host.Messages.Delete(firstId);
                Assert.AreEqual(MESSAGE_COUNT - 1, host.Messages.Count);

                Assert.IsFalse(host.Messages.Contains(firstId));
            }
            finally {
                server.Dispose();
            }
        }

        [TestMethod]
        public void DeleteFileTest() {
            const int port = 25;

            var dropPath = Path.Combine(Path.GetTempPath(), "DeleteTest");
            var fileExtension = ".xxx";
            if (Directory.Exists(dropPath)) Directory.Delete(dropPath, true);

            var server = new Server();

            try {
                var host = server.CreateHost(new HostConfiguration
                                             {
                                                 Port = port,
                                                 MessageStorage = new FileMessageStorageConfiguration
                                                                  {
                                                                      DropPath = dropPath,
                                                                      FileExtension = fileExtension
                                                                  }
                                             });
                host.Start();

                ImpostorTests.SendEmails(server, MESSAGE_COUNT);

                Assert.AreEqual(MESSAGE_COUNT, host.Messages.Count);
                var firstId = host.Messages.First().Id;
                File.Delete(Path.Combine(dropPath, string.Concat(firstId, fileExtension)));
                Thread.Sleep(1000); // give time for the watcher to catch up

                Assert.AreEqual(MESSAGE_COUNT - 1, host.Messages.Count);

                Assert.IsFalse(host.Messages.Any(m => m.Id.Equals(firstId)));
            }
            finally {
                server.Dispose();
            }
        }

        [TestMethod]
        public void DeleteAllTest() {
            const int port = 25;

            var dropPath = Path.Combine(Path.GetTempPath(), "DeleteAllTest");
            if (Directory.Exists(dropPath)) Directory.Delete(dropPath, true);

            var server = new Server();

            try {
                var host = server.CreateHost(new HostConfiguration
                                             {
                                                 Port = port,
                                                 MessageStorage = new FileMessageStorageConfiguration
                                                                  {
                                                                      DropPath = dropPath
                                                                  }
                                             });
                host.Start();

                ImpostorTests.SendEmails(server, MESSAGE_COUNT);

                Assert.AreEqual(MESSAGE_COUNT, host.Messages.Count);
                host.Messages.DeleteAll();
                Assert.AreEqual(0, host.Messages.Count);
            }
            finally {
                server.Dispose();
            }
        }
    }
}