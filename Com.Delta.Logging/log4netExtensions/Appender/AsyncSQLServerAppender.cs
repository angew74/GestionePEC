using log4net.Core;
using log4netExtensions.Appender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace log4netExtensions.Appender
{
    public class AsyncSQLServerAppender : SQLServerAppender
    {
        private Queue<LoggingEvent> pendingTasks;
        private readonly object lockObject = new object();
        private readonly ManualResetEvent manualResetEvent;
        private bool onClosing;

        public AsyncSQLServerAppender()
        {
            pendingTasks = new Queue<LoggingEvent>();
            manualResetEvent = new ManualResetEvent(false);
            Start();
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            foreach (LoggingEvent ev in loggingEvents)
                Append(ev);
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (FilterEvent(loggingEvent))
                Enqueue(loggingEvent);
        }

        private void Start()
        {
            if (!onClosing)
            {
                Thread thread = new Thread(LogMessages);
                thread.Start();
            }
        }

        private void LogMessages()
        {
            LoggingEvent ev;
            while (!onClosing)
            {
                while (!Dequeue(out ev))
                {
                    Thread.Sleep(10);
                    if (onClosing)
                        break;
                }
                if (ev != null)
                {
                    base.Append(ev);
                }
            }
            manualResetEvent.Set();
        }

        private void Enqueue(LoggingEvent ev)
        {
            lock (lockObject)
            {
                pendingTasks.Enqueue(ev);
            }
        }

        private bool Dequeue(out LoggingEvent ev)
        {
            lock (lockObject)
            {
                if (pendingTasks.Count > 0)
                {
                    ev = pendingTasks.Dequeue();
                    return true;
                }
                else
                {
                    ev = null;
                    return false;
                }
            }
        }

        protected override void OnClose()
        {
            onClosing = true;
            manualResetEvent.WaitOne(TimeSpan.FromSeconds(10));
            base.OnClose();
        }
    }
}
