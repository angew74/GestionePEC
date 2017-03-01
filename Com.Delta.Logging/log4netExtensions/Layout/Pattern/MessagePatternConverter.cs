using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Layout.Pattern;

namespace log4netExtensions.Layout.Pattern
{
    internal sealed class MessagePatternConverter : PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            loggingEvent.WriteRenderedMessage(writer);
        }
    }
}
