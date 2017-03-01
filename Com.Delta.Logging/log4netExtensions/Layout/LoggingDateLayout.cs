using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Layout;
using Com.Delta.Logging;

namespace log4netExtensions.Layout
{
    /// <summary>
    /// 
    /// </summary>
    public class LoggingDateLayout : RawTimeStampLayout
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingDateLayout"/> class.
        /// </summary>
        public LoggingDateLayout() { }

        /// <summary>
        /// Formats the specified logging event.
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        /// <returns></returns>
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
