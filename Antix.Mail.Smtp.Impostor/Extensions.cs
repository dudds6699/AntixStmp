//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Extensions used by Impostor</para>
    /// </summary>
    internal static class Extensions {
        #region Head/Tail

        /// <summary>
        ///   Get the head of a string, 
        /// 
        ///   destructive, ie leaving only the body in the text variable
        /// </summary>
        /// <param name = "text">Text string</param>
        /// <param name = "uptoItem">Head cut of point (neck)</param>
        /// <param name = "comparisonType">String Comparison</param>
        /// <returns>Head only</returns>
        public static string Head(ref string text, string uptoItem, StringComparison comparisonType) {
            var position = text.IndexOf(uptoItem, comparisonType);
            string headText;

            if (position == -1) {
                headText = text;
                text = "";
            }
            else {
                headText = text.Substring(0, position);
                text = text.Substring(position + uptoItem.Length);
            }

            return headText;
        }

        /// <summary>
        ///   Get the head of a string, 
        /// 
        ///   destructive, ie leaving only the body in the text variable
        ///   case sensitive
        /// </summary>
        /// <param name = "text">Text string</param>
        /// <param name = "uptoItem">Head cut of point (neck)</param>
        /// <returns>Head only</returns>
        public static string Head(ref string text, string uptoItem) {
            return Head(ref text, uptoItem, StringComparison.CurrentCulture);
        }

        /// <summary>
        ///   Get the head of a string
        /// 
        ///   non destuctive, ie leaves the text as it was
        /// </summary>
        /// <param name = "text">Text string</param>
        /// <param name = "uptoItem">Head cut of point (neck)</param>
        /// <param name = "comparisonType">String Comparison</param>
        /// <returns>Head only</returns>
        public static string Head(this string text, string uptoItem, StringComparison comparisonType) {
            var position = text.IndexOf(uptoItem, comparisonType);
            string headText;

            if (position == -1) {
                headText = text;
            }
            else {
                headText = text.Substring(0, position);
            }

            return headText;
        }

        /// <summary>
        ///   Get the head of a string
        /// 
        ///   non destuctive, ie leaves the text as it was
        ///   case sensitive
        /// </summary>
        /// <param name = "text">Text string</param>
        /// <param name = "uptoItem">Head cut of point (neck)</param>
        /// <returns>Head only</returns>
        public static string Head(this string text, string uptoItem) {
            return Head(text, uptoItem, StringComparison.CurrentCulture);
        }

        /// <summary>
        ///   Remove the tail from text
        /// 
        ///   destructive, ie leaving only the body in the text variable
        /// </summary>
        /// <param name = "text">Text string</param>
        /// <param name = "uptoItem">Tail cut off point</param>
        /// <param name = "comparisonType">String Comparison</param>
        /// <returns>Tail only</returns>
        public static string Tail(ref string text, string uptoItem, StringComparison comparisonType) {
            var position = text.LastIndexOf(uptoItem, comparisonType);
            string tailText;

            if (position == -1) {
                tailText = text;
                text = "";
            }
            else {
                tailText = text.Substring(position + uptoItem.Length);
                text = text.Substring(0, position);
            }

            return tailText;
        }

        /// <summary>
        ///   Remove the tail from text
        /// 
        ///   destructive, ie leaving only the body in the text variable
        ///   case sensitive
        /// </summary>
        /// <param name = "text">Text string</param>
        /// <param name = "uptoItem">Tail cut off point</param>
        /// <returns>Tail only</returns>
        public static string Tail(ref string text, string uptoItem) {
            return Tail(ref text, uptoItem, StringComparison.CurrentCulture);
        }

        /// <summary>
        ///   Remove the tail from text
        /// 
        ///   non destuctive, ie leaves the text as it was
        /// </summary>
        /// <param name = "text">Text string</param>
        /// <param name = "uptoItem">Tail cut off point</param>
        /// <param name = "comparisonType">String Comparison</param>
        /// <returns>Tail only</returns>
        public static string Tail(this string text, string uptoItem, StringComparison comparisonType) {
            var position = text.LastIndexOf(uptoItem, comparisonType);
            string tailText;

            if (position == -1) {
                tailText = text;
            }
            else {
                tailText = text.Substring(position + uptoItem.Length);
            }

            return tailText;
        }

        /// <summary>
        ///   Remove the tail from text
        /// 
        ///   non destuctive, ie leaves the text as it was
        ///   case sensitive
        /// </summary>
        /// <param name = "text">Text string</param>
        /// <param name = "uptoItem">Tail cut off point</param>
        /// <returns>Tail only</returns>
        public static string Tail(this string text, string uptoItem) {
            return Tail(text, uptoItem, StringComparison.CurrentCulture);
        }

        #endregion

        #region Between

        /// <summary>
        ///   <para>Get the text between the 'startsWith' and 'endsWith' parameters</para>
        /// </summary>
        /// <param name = "text">Text</param>
        /// <param name = "startsWith">Required text starts with this</param>
        /// <param name = "endsWith">Required text ends with this</param>
        /// <param name = "comparisonType">String Comparison</param>
        /// <returns>Text between</returns>
        public static string Between(this string text, string startsWith, string endsWith,
                                     StringComparison comparisonType) {
            Head(ref text, startsWith, comparisonType);
            return Head(text, endsWith, comparisonType);
        }

        /// <summary>
        ///   <para>Get the text between the 'startsWith' and 'endsWith' parameters</para>
        ///   <para>Case sensitive</para>
        /// </summary>
        /// <param name = "text">Text</param>
        /// <param name = "startsWith">Required text starts with this</param>
        /// <param name = "endsWith">Required text ends with this</param>
        /// <returns>Text between</returns>
        public static string Between(this string text, string startsWith, string endsWith) {
            return Between(text, startsWith, endsWith, StringComparison.CurrentCulture);
        }

        #endregion

        #region Remove

        /// <summary>
        ///   <para>Remove chars from a string</para>
        /// </summary>
        internal static string Remove(this string text, char[] chars) {
            var newText = new StringBuilder();
            for (var index = 0; index < text.Length; index++) {
                if (Array.IndexOf(chars, text[index]) == -1) {
                    newText.Append(text[index]);
                }
            }
            return newText.ToString();
        }

        #endregion

        #region Decode

        /// <summary>
        ///   <para>Decode the text from Quoted word format</para>
        /// </summary>
        public static string FromEncodedWord(this string text, Encoding encoding) {
            if (string.IsNullOrEmpty(text)) return text;

            var reader = new StringReader(text.Replace("_", " "));
            var writer = new StringWriter();
            try {
                var line = default(string);
                while ((line = reader.ReadLine()) != null) {
                    if (line.EndsWith("=")) line = line.Substring(0, line.Length - 1);

                    writer.Write(
                        Regex.Replace(line, @"(\=([0-9A-F]{2}))+",
                                      m => {
                                          var bytes = new byte[m.Groups[2].Captures.Count];
                                          for (var i = 0; i < bytes.Length; i++) {
                                              bytes[i] = Convert.ToByte(m.Groups[2].Captures[i].Value, 16);
                                          }

                                          return encoding.GetString(bytes);
                                      },
                                      RegexOptions.IgnoreCase));
                }
                writer.Flush();
                return writer.ToString();
            }
            finally {
                reader.Close();
                writer.Close();
            }
        }

        #endregion

        #region MailAddress

        private static readonly Regex EMailMatcher
            = new Regex(@"([""']?(?<name>.*?)[""']?\s*<(?<email>.*?@[^\s,]*)>|(?<email>.*?@[^\s,]*))\s*,?\s*",
                        RegexOptions.Compiled);

        /// <summary>
        ///   <para>Create a MailAddress object from a string</para>
        /// </summary>
        /// <param name = "text">String to check</param>
        /// <returns>MailAddress</returns>
        /// <exception cref = "ArgumentException" />
        public static MailAddress ToMailAddress(this string text) {
            if (string.IsNullOrEmpty(text)) return null;

            var match = EMailMatcher.Match(text);
            if (!match.Success) throw new InvalidMailAddressException(text);

            return string.IsNullOrEmpty(match.Groups["name"].Value)
                       ? new MailAddress(match.Groups["email"].Value)
                       : new MailAddress(match.Groups["email"].Value, match.Groups["name"].Value);
        }

        /// <summary>
        ///   <para>Creates an array of MailAddress objects from a string</para>
        /// </summary>
        /// <param name = "text">String to check</param>
        /// <returns>Array of MailAddress</returns>
        public static IEnumerable<MailAddress> ToMailAddresses(this string text) {
            if (string.IsNullOrEmpty(text)) return null;

            var list = new List<MailAddress>();
            var match = EMailMatcher.Match(text);
            while (match.Success) {
                list.Add(new MailAddress(
                             match.Groups["email"].Value,
                             match.Groups["name"].Value));

                match = match.NextMatch();
            }

            return list.ToArray();
        }

        #endregion

        #region ReadTo

        /// <summary>
        ///   <para>Read a stream until the first instance of a stream is found</para>
        /// </summary>
        /// <param name = "stream">A stream</param>
        /// <param name = "to">String to search for</param>
        /// <param name = "encoding">Encoding</param>
        /// <returns>Data read as a string</returns>
        public static string ReadTo(this Stream stream, string to, Encoding encoding) {
            var data = new StringBuilder();
            var buffer = new byte[1024];
            var toBytes = encoding.GetBytes(to);
            var matchStart = 0;
            var matchLength = 0;
            var readCount = 0;

            while (
                (readCount = stream.Read(buffer, 0, buffer.Length)) > 0
                && matchLength < toBytes.Length) {
                data.Append(encoding.GetString(buffer));

                for (var i = 0; i < readCount; i++) {
                    if (toBytes[matchLength] == buffer[i]) {
                        matchLength++;
                        if (matchLength == toBytes.Length) break;
                    }
                    else {
                        matchStart++;
                        matchLength = 0;
                    }
                }
            }

            return data.ToString(0, matchStart);
        }

        #endregion

        #region ToInfo

        /// <summary>
        ///   <para>Get a MessageInfo given the full Message</para>
        /// </summary>
        /// <param name = "message">Message</param>
        /// <returns>MessageInfo</returns>
        public static MessageInfo ToInfo(this Message message) {
            if (message == null) throw new ArgumentNullException("message");

            return new MessageInfo
                   {
                       Id = message.Id,
                       From = message.From == null
                                  ? string.Empty
                                  : message.From.ToString(),
                       To = message.To == null
                                ? string.Empty
                                : message.To.First().ToString(),
                       Subject = message.Subject,
                       Path = message.Path,
                       ReceivedOn = message.ReceivedOn,
                       CC = message.To.Count() > 1 ? string.Join(", ", message.To.Skip(1).Select(a => a.ToString())) : string.Empty
                   };
        }

        #endregion
    }
}