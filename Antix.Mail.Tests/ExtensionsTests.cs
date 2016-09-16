//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

using Antix.Mail.Smtp.Impostor;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antix.Mail.Tests {
    /// <summary>
    ///   <para>Test the extensions</para>
    /// </summary>
    [TestClass]
    public class ExtensionsTests {
        ///<summary>
        ///  A test for ReadTo
        ///</summary>
        [TestMethod]
        public void ReadToTest() {
            var path = Path.GetTempFileName();
            var writer = File.CreateText(path);
            var file = writer.BaseStream;
            try {
                var expected = new string('x', 1022);
                var toFind = "abcde";
                writer.Write(expected);
                writer.Write(toFind);
                writer.Write(expected);
                writer.Close();

                file = File.OpenRead(path);

                Assert.AreEqual(expected, file.ReadTo(toFind, Encoding.UTF8));
            }
            finally {
                file.Dispose();
                File.Delete(path);
            }
        }

        /// <summary>
        ///   <para>Test string.FromEncodedWord()</para>
        /// </summary>
        [TestMethod]
        [Description("Test string.FromEncodedWord()")]
        public void DecodeFromEncodedWord() {
            var nullValue = default(string);

            Assert.AreEqual(null, nullValue.FromEncodedWord(Encoding.UTF8));
            Assert.AreEqual(string.Empty, string.Empty.FromEncodedWord(Encoding.UTF8));

            Assert.AreEqual("From Mail Åddress", "From_Mail_=C3=85ddress".FromEncodedWord(Encoding.UTF8));
        }

        /// <summary>
        ///   <para>Test string.ToMailAddress[es]()</para>
        /// </summary>
        [TestMethod]
        [Description("Test string.ToMailAddress[es]()")]
        public void ToEmail() {
            const string email = "tester@antix.co.uk";
            const string name = "Antix Tester";

            Assert.AreEqual(new MailAddress(email), email.ToMailAddress());
            Assert.AreEqual(new MailAddress(email), string.Format("<{0}>", email).ToMailAddress());
            Assert.AreEqual(new MailAddress(email, name), string.Format("{1}<{0}>", email, name).ToMailAddress());
            Assert.AreEqual(new MailAddress(email, name), string.Format("\"{1}\"<{0}>", email, name).ToMailAddress());
            Assert.AreEqual(new MailAddress(email, name), string.Format("'{1}'<{0}>", email, name).ToMailAddress());

            var mailAddresses = string.Format("{1}<{0}>,\"{1}\" <{0}>,  \t'{1}'<{0}>", email, name).ToMailAddresses();
            Assert.AreEqual(3, mailAddresses.Count());
            foreach (var mailAddress in mailAddresses) {
                Assert.AreEqual(new MailAddress(email, name), mailAddress);
            }
        }
    }
}