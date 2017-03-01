using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Delta.Logging.Errors;
using System.IO;
using log4net.Core;
using log4net.Layout.Pattern;

namespace log4netExtensions.Layout.Pattern
{
    /// <summary>
    /// mappatura per userIP
    /// </summary>
    internal sealed class UserIPPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<ErrorLogInfo>(writer, loggingEvent, "userIP");
        }
    }

    internal sealed class EnanchedInfoPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<ErrorLogInfo>(writer, loggingEvent, "enanchedInfos");
        }
    }
    /// <summary>
    /// Mappatura per objectID
    /// </summary>
    internal sealed class ObjectIDPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<ErrorLogInfo>(writer, loggingEvent, "objectID");
        }
    }
    /// <summary>
    /// Mappatura per objectGroupID
    /// </summary>
    internal sealed class ObjectGroupIDPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<ErrorLogInfo>(writer, loggingEvent, "objectGroupID");
        }
    }
    /// <summary>
    /// Mappatura per objectAppID
    /// </summary>
    internal sealed class ObjectAppIDPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<ErrorLogInfo>(writer, loggingEvent, "objectAppID");
        }
    }
    /// <summary>
    /// Mappatura per objectParentcode
    /// </summary>
    internal sealed class ObjectParentcodePatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<ErrorLogInfo>(writer, loggingEvent, "objectParentcode");
        }
    }
    /// <summary>
    /// Mappatura per passiveobjectID
    /// </summary>
    internal sealed class PassiveobjectIDPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<ErrorLogInfo>(writer, loggingEvent, "passiveobjectID");
        }
    }
    /// <summary>
    /// Mappatura per passiveobjectGroupID
    /// </summary>
    internal sealed class PassiveobjectGroupIDPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<ErrorLogInfo>(writer, loggingEvent, "passiveobjectGroupID");
        }
    }
    /// <summary>
    /// Mappatura per passiveparentcodeobjectID
    /// </summary>
    internal sealed class PassiveparentcodeobjectIDPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<ErrorLogInfo>(writer, loggingEvent, "passiveparentcodeobjectID");
        }
    }
    /// <summary>
    /// Mappatura per passiveapplicationID
    /// </summary>
    internal sealed class PassiveapplicationIDPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<ErrorLogInfo>(writer, loggingEvent, "passiveapplicationID");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ModulePatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<ErrorLogInfo>(writer, loggingEvent, "module");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal sealed class FunctionPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<ErrorLogInfo>(writer, loggingEvent, "function");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ActionPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            GenericPatternConverter.Convert<ErrorLogInfo>(writer, loggingEvent, "action");
        }
    }
}
