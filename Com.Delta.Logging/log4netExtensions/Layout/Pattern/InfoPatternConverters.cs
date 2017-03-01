using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;
using log4net.Layout.Pattern;
using log4net.Core;
using Com.Delta.Logging;
using System.Reflection;
using Com.Delta.Logging.Errors;
using Com.Delta.Logging.Mail;

namespace log4netExtensions.Layout.Pattern
{
    /// <summary>
    /// Mappatura per UserID
    /// </summary>
    internal sealed class UserIDPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<BaseLogInfo>(writer, loggingEvent, "userID");
        }
    }
    /// <summary>
    /// Mappatura per UserMail
    /// </summary>
    internal sealed class UserMailPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<BaseLogInfo>(writer, loggingEvent, "userMail");
        }
    }
    internal sealed class MObjectIdPaetternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<MailLogInfo>(writer, loggingEvent, "mobjectID");
        }
    }
}
