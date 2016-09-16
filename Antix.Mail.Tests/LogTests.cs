//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using Antix.Mail.Smtp.Impostor;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antix.Mail.Tests {
    /// <summary>
    ///   Summary description for LogTests
    /// </summary>
    [TestClass]
    public class LogTests {
        private const string EXCEPTION_TEXT = "A test error occurred";

        [TestMethod]
        public void LogException() {
            var log = new StringBuilder();

            Trace.Listeners.Clear();
            Trace.Listeners.Add(new TextWriterTraceListener(new StringWriter(log)));


            try {
                throw new Exception(EXCEPTION_TEXT);
            }
            catch (Exception ex) {
                Log.Error("LogException {0}", ex);
            }

            Console.Write(@"LOG OUTPUT--------
{0}
----------------------", log);

            Assert.IsTrue(log.Length > 0);
            Assert.IsTrue(log.ToString().Contains(EXCEPTION_TEXT));

            Trace.Listeners.Clear();
        }

        [TestMethod]
        public void LogExceptionOverridenAction() {
            var log = new StringBuilder();

            Log.LogErrorAction = (f, a) => {
                                     log.AppendLine(DateTime.Now.ToString());
                                     log.AppendLine(string.Format(f, a));
                                 };

            try {
                throw new Exception(EXCEPTION_TEXT);
            }
            catch (Exception ex) {
                Log.Error("LogException {0}", ex);
            }

            Assert.IsTrue(log.Length > 0);
            Assert.IsTrue(log.ToString().Contains(EXCEPTION_TEXT));
        }
    }
}