Add E-mail send testing

        [TestMethod]
        public void SendAMessage() {

            var server = new Server();
            using(var host = server.CreateHost(new HostConfiguration
                                    {
                                        IPAddressString = "127.0.0.1",
			                Port = 25                                           
                                    }))
            {
                host.Start();
                host.Messages.DeleteAll();

                new SmtpClient()
                    .Send(new MailMessage(
                    "tester@example.co.uk",
                    "tester@example.co.uk", 
                    "hiya", 
                    "howya?"));

                Assert.AreEqual(1, host.Messages.Count);

            }

        }