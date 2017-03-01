using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net.Core;
using System.Reflection;
using Com.Delta.Logging;

namespace log4netExtensions.Layout.Pattern
{
    /// <summary>
    /// Classe generica per pattern converter
    /// </summary>
    internal class GenericPatternConverter
    {
        internal static void Convert<T>(TextWriter w, LoggingEvent ev, string field)
            where T : class
        {
            T message = ev.MessageObject as T;
            if (message != null)
            {
                FieldInfo fi = message.GetType().GetField(field, BindingFlags.Public | BindingFlags.Instance);
                if (fi != null)
                {
                    w.Write(fi.GetValue(message));
                }
            }
        }
    }
}