#region Copyright & License
//
// Copyright 2001-2005 The Apache Software Foundation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Messaging;
using System.Threading;
using log4net.Core;
using System.Xml;
using log4net;
using Com.Delta.Logging.Errors;

namespace Com.Delta.Logging
{
    
    /// <summary>
    /// Classe che estende AppenderSkeleton di log4net per la scrittura nel logger queue 
    /// delle informazioni presenti nelle code configurate
    /// </summary>
	public class MsmqAppender : log4net.Appender.AppenderSkeleton
	{
        private static readonly ILog log = LogManager.GetLogger("MsmqAppender");
        /// <summary>
        /// Costruttore generico
        /// </summary>
        public MsmqAppender()
		{
		}

		private static string  m_queueName;
        private static bool notify;
        private static string backupQueueName;
        private static string eventLog;
        private static string appName;

        /// <summary>
        /// Proprietà pubblica che espone il nome della coda di backup
        /// </summary>
        public string BackupQueueName
        {
        get { return backupQueueName; }
        set { backupQueueName = value; }
        }
         
        /// <summary>
        /// Nome dell'applicazione legata alla coda
        /// </summary>
        public string AppName
        {
        get { return appName; }
        set { appName = value; }
        }
 
        /// <summary>
        /// Nome dell'event log da scrivere nella coda 
        /// </summary>
        public string EventLogName
        {
            get { return eventLog; }
            set { eventLog = value; }
        }

        /// <summary>
        /// Nome della coda
        /// </summary>
		public string QueueName
		{
			get { return m_queueName; }
			set { m_queueName = value; }
		}

        /// <summary>
        /// Notifica dell'errore di scrittura
        /// </summary>
        public bool NotifyError
		{
			get { return notify; }
			set { notify = value; }
		}

        /// <summary>
        /// Metodo che appende nel logger queue la coda configurata 
        /// nel web.config dell'applicazione
        /// </summary>
        /// <param name="loggingEvent">Evento (coda) da scrivere nel logger queue </param>
		override protected void Append(LoggingEvent loggingEvent) 
		{
            if (loggingEvent.Level.Name != "DEBUG")
            {
                ((BaseLogInfo)loggingEvent.MessageObject).loggingTime = loggingEvent.TimeStamp;
                Thread myThread;
                myThread = new System.Threading.Thread(new ParameterizedThreadStart(Com.Delta.Logging.MsmqAppender.AsyncAppend));
                myThread.Start(loggingEvent);
            }
        }

        /// <summary>
        /// Append asynchronous
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        public static void AsyncAppend(object loggingEvent)
        {
            LoggingEvent Event = (LoggingEvent)loggingEvent;
            try
            {
                if (loggingEvent != null)
                {
                    //scrivo il messaggio nella coda configurata
                    System.Messaging.MessageQueue mq = new System.Messaging.MessageQueue();
                    System.Messaging.Message mqMessage = new Message();
                    //System.Messaging.Message mqMessage = new System.Messaging.Message(((BaseLogInfo)Event.MessageObject).Serialize().InnerXml); 
                    { 
                        mqMessage.AttachSenderId = false; 
                        mqMessage.Recoverable = true;
                        mqMessage.Label = appName +"-"+ Event.Level.DisplayName; 
                        mqMessage.TimeToReachQueue = System.Messaging.Message.InfiniteTimeout; 
                        mqMessage.TimeToBeReceived = System.Messaging.Message.InfiniteTimeout;
                        mqMessage.Body = ((BaseLogInfo)Event.MessageObject).Serialize().DocumentElement;
                    } 
                    
                    try 
                    { 
                        mq.Path = "FormatName:" + m_queueName;
                        mq.Formatter = new XmlMessageFormatter();
                        mq.Send(mqMessage, MessageQueueTransactionType.None); 
                    } 
                    catch (Exception)
                    { 
                         if ((backupQueueName != null && !backupQueueName.Equals("")))
                         {
                             try
                             {
                                    mq.Path = "FormatName:" + backupQueueName; 
                                    mq.Send(mqMessage, MessageQueueTransactionType.None);
                             }
                             catch (Exception e1)
                             {
                                 ErrorLog error = new ErrorLog();
                                 error.freeTextDetails = e1.Message;
                                 error.logCode = "ERR003";
                                 error.loggingAppCode = "LOG";
                                 error.loggingTime = System.DateTime.Now;
                                 error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                                 log.Error(error);
                                 throw e1;
                             }    
                         }       
                    }
                }
            }
            catch(Exception e2)
            {
                 //log di 2° backup//
                 string error = e2.Message;
                 log4net.ILog log = log4net.LogManager.GetLogger(typeof(Com.Delta.Logging.MsmqAppender));
                 log.Info(Event.MessageObject);
                 
                 //notifico il sistema del problema
                 if (notify)
                 {
                     if (System.Diagnostics.EventLog.SourceExists(eventLog))
                     {
                         System.Diagnostics.EventLog myEventLog=null;
                         try
                         {
                             string msg="Errore durante la scrittura nella coda:"+
                                        m_queueName +
                                        "\\r\\n Dettaglio:\\r\\n+"+
                                        Event.MessageObject.ToString()+
                                        "\\r\\n Errore:\\r\\n"+
                                        e2.Message+
                                        "\\r\\n\\r\\n Dettaglio Errore:\\r\\n"+
                                        e2.StackTrace;
                             
                             myEventLog= new System.Diagnostics.EventLog();
                             myEventLog.Source = eventLog;
                             myEventLog.WriteEntry(msg, System.Diagnostics.EventLogEntryType.Error);
                             myEventLog.Close();
                         }
                         finally
                         {
                             if (myEventLog!=null)myEventLog.Dispose();
                         }
                     }
                }
            }
        }
    }
}