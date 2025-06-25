using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AtCommon.Utilities
{
    /// <summary>
    ///  Extension methods for Exception class.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        ///  Provides full stack trace for the exception that occurred.
        /// </summary>
        /// <param name="exception">Exception object.</param>
        /// <param name="environmentStackTrace">Environment stack trace, for pulling additional stack frames.</param>
        public static string ToLogString(this Exception exception, string environmentStackTrace)
        {
            var environmentStackTraceLines = GetUserStackTraceLines(environmentStackTrace);
            if (environmentStackTraceLines.Count > 2)
            {
                environmentStackTraceLines.RemoveAt(0);
                environmentStackTraceLines.RemoveAt(0);
            }

            var stackTraceLines = GetStackTraceLines(exception.StackTrace);
            stackTraceLines.AddRange(environmentStackTraceLines);

            var fullStackTrace = string.Join(Environment.NewLine, stackTraceLines);

            return fullStackTrace;
        }

        /// <summary>
        ///  Gets a list of stack frame lines, as strings.
        /// </summary>
        /// <param name="stackTrace">Stack trace string.</param>
        private static List<string> GetStackTraceLines(string stackTrace)
        {
            return stackTrace.SplitLines();
        }

        /// <summary>
        ///  Gets a list of stack frame lines, as strings, only including those for which line number is known.
        /// </summary>
        /// <param name="fullStackTrace">Full stack trace, including external code.</param>
        private static List<string> GetUserStackTraceLines(string fullStackTrace)
        {
            var regex = new Regex(@"([^\)]*\)) in (.*):line (\d)*$");

            var stackTraceLines = GetStackTraceLines(fullStackTrace);

            return stackTraceLines.Where(stackTraceLine => regex.IsMatch(stackTraceLine)).ToList();
        }

        public static T CheckForNull<T>(this T obj, string message = null)
        {
            if (obj != null) return obj;
            if (message == null) throw new NullReferenceException();
           
            throw new NullReferenceException(message);
            
        }
    }
}
