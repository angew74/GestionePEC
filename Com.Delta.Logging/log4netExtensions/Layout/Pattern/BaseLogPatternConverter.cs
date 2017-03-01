using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Layout.Pattern;
using log4net.Core;
using System.IO;
using Com.Delta.Logging;

namespace log4netExtensions.Layout.Pattern
{
    /// <summary>
    /// Mappatura per loggingAppCode
    /// </summary>
    internal sealed class LoggingAppCodePatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<BaseLogInfo>(writer, loggingEvent, "loggingAppCode");
        }
    }
    /// <summary>
    /// Mappatura per logCode
    /// </summary>
    internal sealed class LogCodePatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<BaseLogInfo>(writer, loggingEvent, "logCode");
        }
    }
    /// <summary>
    /// Mappatura per freeTextDetails
    /// </summary>
    internal sealed class FreeTextDetailsPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            if (loggingEvent.MessageObject is string) //per gestione dei log solo stringa su oracle
            {
                writer.Write(loggingEvent.RenderedMessage);
            }
            else GenericPatternConverter.Convert<BaseLogInfo>(writer, loggingEvent, "freeTextDetails");

        }
    }
    /// <summary>
    /// Mappatura per uniqueLogID
    /// </summary>
    internal sealed class UniqueLogIDPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            if (loggingEvent.MessageObject is string) //per la gestione dei log solo stringa su oracle
            {
                var method = typeof(BaseLogInfo).GetMethod("PlainToSHA1", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                if (method != null)
                {
                    var uniqueId = method.Invoke(null, new object[] { Guid.NewGuid().ToString() });
                    writer.Write(uniqueId);
                }
            }
            else
                GenericPatternConverter.Convert<BaseLogInfo>(writer, loggingEvent, "uniqueLogID");
        }
    }
}
