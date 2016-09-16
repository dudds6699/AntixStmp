//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;

namespace Antix.Mail.Smtp.Impostor.Client {
    internal static class Extensions {
        #region ToVersionString

        /// <summary>
        ///   <para>Gets a standard version string</para>
        ///   <para>Only shows the revision if greater than 0</para>
        /// </summary>
        internal static string ToVersionString(this Version version) {
            return string.Format("{0}.{1}.{2}{3}",
                                 version.Major, version.Minor,
                                 version.Build,
                                 version.Revision > 0 ? string.Concat(".", version.Revision) : string.Empty);
        }

        #endregion
    }
}