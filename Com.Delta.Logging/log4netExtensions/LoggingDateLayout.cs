using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Layout;

namespace Com.Delta.Logging.log4netExtensions
{
    public class LoggingDateLayout : RawTimeStampLayout
    {
        public LoggingDateLayout() { }

        public override object Format(log4net.Core.LoggingEvent loggingEvent)
        {
            BaseLogInfo message = loggingEvent.MessageObject as BaseLogInfo;
            if (message != null)
            {
                return message.loggingTime;
            }
            return base.Format(loggingEvent);
        }
    }
}
