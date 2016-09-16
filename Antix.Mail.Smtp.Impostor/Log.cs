//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Diagnostics;
using System.Linq;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Logging class</para>
    ///   <para>Simple wrapper allowing for change of logging in one place, should I decide to use log4net</para>
    /// </summary>
    public sealed class Log {

        static Log() {
            LogInformationAction = (f, a) => Trace.TraceInformation(f, a);
            LogWarningAction = (f, a) => Trace.TraceWarning(f, a);
            LogErrorAction = (f, a) => Trace.TraceError(f, a);
        }

        #region actions

        public static Action<string, object[]> LogInformationAction { private get; set; }
        public static Action<string, object[]> LogWarningAction { private get; set; }
        public static Action<string, object[]> LogErrorAction { private get; set; }

        #endregion

        #region methods

        public static void Information(string format, params object[] args) {
            LogInformationAction(format, args);
        }

        public static void Warning(string format, params object[] args) {
            LogWarningAction(format, args);
        }

        public static void Error(string format, params object[] args) {
            LogErrorAction(format, args);
        }

        public static void Error(Exception ex) {
            if (ex == null) return;

            LogErrorAction(ex.ToString(), null);
        }

        #endregion

    }
}