using System;
using System.Data;
using System.Collections.Generic;

using Com.Delta.Messaging.MapperMessages;
using log4net;
using Com.Delta.Web.Cache.Schemes;
using Com.Delta.Web.Cache;
using Com.Delta.Logging.Errors;

namespace Com.Delta.Messaging.WebMessages
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Info
    {
        private static readonly ILog log = LogManager.GetLogger("Info");
        protected IList<message> messageList = new List<message>();
        //private IList<message> messageList = new List<message>(); //test SISC
        Com.Delta.Web.Cache.CacheKeys messaggi = Com.Delta.Web.Cache.CacheKeys.NULL;

         
        public Info(Com.Delta.Web.Cache.CacheKeys messageKey)
        {
            messaggi = messageKey;
        }
        

        /// <summary>
        /// 
        /// </summary>
        //private class message
        [Serializable]
        protected class message
        {
            public message(string message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio tipo)
            {
                this.tipo = tipo;
                this.msg = message;
            }
            public Com.Delta.Messaging.MapperMessages.LivelloMessaggio tipo;
            public string msg;
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string decodeMapperMessage(Messaggio msg)
        {
            string sRet = string.Empty;
            sRet = string.Format("Livello:{0}, Codice:{1}, la decodifica del messaggio è assente!", msg.Livello, msg.Codice);
            if (messaggi != CacheKeys.NULL)
            {
                ListaDecodificaMessaggi el =
                    CacheManager<ListaDecodificaMessaggi>.get(
                        messaggi, VincoloType.FILESYSTEM);
                ListaDecodificaMessaggi.ErroreRow _errorerow =
                    el.Errore.FindBycod(msg.Codice);
                

                if (_errorerow != null)
                {
                    sRet = _errorerow.desc;
                    if (msg.IsVariable)
                    {
                        foreach (KeyValuePair<string, string[]> item in msg.Variabili)
                            if (item.Value != null)
                            {
                                sRet = sRet.Replace("\\" + item.Key + "\\", string.Join("||", item.Value));
                            }
                            else sRet = sRet.Replace("\\" + item.Key + "\\", "");
                    }
                }
            }
            return sRet;
        }

        
        public string decodeMapperMessage(DataTable msgDt)
        {
            string sRet = string.Empty;
            sRet = string.Format("La lista per decodificare i messaggi è assente!");
            if (messaggi != CacheKeys.NULL)
            {
                string decod = String.Empty;
                decod = "<ol>";

                foreach (DataRow item in msgDt.Rows)
                {
                    if ((item.ItemArray[1] as string) != "2") decod += "<li>Codice: " + item.ItemArray[0] + " Descrizione: " + item.ItemArray[2] + "</li>";

                }
                decod += "</ol>";

                return decod;
            }
            else return sRet;
        }


        public string decodeMapperMessageList(IList<Messaggio> mList)
        {
            string sRet = string.Empty;
            sRet = string.Format("La lista per decodificare i messaggi è assente!");
            if (messaggi != CacheKeys.NULL)
            {
                string decod = String.Empty;
                decod = "<ol>";
                foreach (Messaggio m in mList)
                {
                    if (m.Livello != LivelloMessaggio.INFO ) decod += "<li>Codice: " + m.Codice + " Descrizione: " + m.Descrizione + "</li>";
                }
                decod += "</ol>";

                return decod;
            }
            else return sRet;
        }
       
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static string GetDescrizione(Object row, Com.Delta.Web.Cache.CacheKeys listaMessaggi)
        {
            DataRow r = (DataRow)row;
            return new Info(listaMessaggi).decodeMapperMessage(new Messaggio(r));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void AddError(string message)
        {
            messageList.Add(new message(message, LivelloMessaggio.ERROR));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void AddInfo(string message)
        {
            messageList.Add(new message(message, LivelloMessaggio.INFO));
        }


        public void AddMessage(int codice, int livello)
        {
            try
            {
                Messaggio m = new Messaggio(codice.ToString(), (LivelloMessaggio)livello);

                messageList.Add(new message(decodeMapperMessage(m), m.Livello));

            }
            catch(Exception ex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = ex.Message;
                error.logCode = "ERR_INF_001";
                log.Error(error);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="t"></param>
        public void AddMessage(string message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio t)
        {
            if (t == Com.Delta.Messaging.MapperMessages.LivelloMessaggio.APPLICATION)
                throw new Exception("I messaggi di livello APPLICATION non sono per l'utente finale");
            if (t == Com.Delta.Messaging.MapperMessages.LivelloMessaggio.MAPPER_ERROR)
                throw new Exception("I messaggi di livello MAPPER_MESSAGES non sono per l'utente finale");
            messageList.Add(new message(message, t));
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TableMessaggi"></param>
        /// <param name="l"></param>
        public void AddMessage(DataTable TableMessaggi, Com.Delta.Messaging.MapperMessages.LivelloMessaggio l)
        {
            Com.Delta.Messaging.MapperMessages.ListaMessaggi lm =
                new Com.Delta.Messaging.MapperMessages.ListaMessaggi(TableMessaggi);

            foreach (Messaggio m in lm.messaggi)
                if (m.Livello != LivelloMessaggio.APPLICATION)
                messageList.Add(new message(decodeMapperMessage(m), l));
        }

        public void AddMessage(DataTable TableMessaggi)
        {
            Com.Delta.Messaging.MapperMessages.ListaMessaggi lm =
                new Com.Delta.Messaging.MapperMessages.ListaMessaggi(TableMessaggi);

            foreach (Messaggio m in lm.messaggi)
                if(m.Livello!= LivelloMessaggio.APPLICATION)
                messageList.Add(new message(decodeMapperMessage(m),m.Livello));
        }

        public void AddMessage(IList<Messaggio> messaggi)
        {
            foreach (Messaggio m in messaggi)
                if (m.Livello != LivelloMessaggio.APPLICATION)
                messageList.Add(new message(decodeMapperMessage(m), m.Livello));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RowsMessaggi"></param>
        /// <param name="l"></param>
        public void AddMessage(DataRow[] RowsMessaggi, Com.Delta.Messaging.MapperMessages.LivelloMessaggio l)
        {
            Com.Delta.Messaging.MapperMessages.ListaMessaggi lm =
                new Com.Delta.Messaging.MapperMessages.ListaMessaggi(RowsMessaggi);

            foreach (Messaggio m in lm.messaggi)
                if (m.Livello != LivelloMessaggio.APPLICATION)
                messageList.Add(new message(decodeMapperMessage(m), l));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RowMessaggio"></param>
        public void AddMessage(DataRow RowMessaggio)
        {
            Com.Delta.Messaging.MapperMessages.Messaggio m = new Messaggio(RowMessaggio);
            messageList.Add(new message(decodeMapperMessage(m), m.Livello));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RowMessaggio"></param>
        /// <param name="l"></param>
        public void AddMessage(DataRow RowMessaggio, Com.Delta.Messaging.MapperMessages.LivelloMessaggio l)
        {
            Com.Delta.Messaging.MapperMessages.Messaggio m = new Messaggio(RowMessaggio);
            messageList.Add(new message(decodeMapperMessage(m), l));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        public void AddMessage(Com.Delta.Messaging.MapperMessages.Messaggio m)
        {
            messageList.Add(new message(decodeMapperMessage(m), m.Livello));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="l"></param>
        public void AddMessage(Com.Delta.Messaging.MapperMessages.Messaggio m, Com.Delta.Messaging.MapperMessages.LivelloMessaggio l)
        {
            messageList.Add(new message(decodeMapperMessage(m), l));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public string renderMessage(System.Web.UI.Page page)  //test SISC
        public virtual string renderMessage(System.Web.UI.Page page)
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder("<ul>");

            foreach (message m in messageList)
            {
                string icon = string.Empty;
                string style = string.Empty;  
                switch(m.tipo)
                { 
                    case LivelloMessaggio.ERROR:
                        icon = "<img src='" + page.ResolveUrl("~/App_Themes/Delta/images/message/error.png") + "' alt='' />";
                        style = "style='font-size:medium; font-weight:bold; color:Red'";
                        break;
                    case LivelloMessaggio.WARNING:
                        icon = "<img src='" + page.ResolveUrl("~/App_Themes/Delta/images/message/alert.png") + "' alt='' />";
                        style = "style='font-size:medium; font-weight:bold; color:#000'";
                        break;
                    case LivelloMessaggio.INFO:
                        icon = "<img src='" + page.ResolveUrl("~/App_Themes/Delta/images/message/info.png") + "' alt='' />";
                        style = "style='font-size:medium; font-weight:bold; color:Blue'";
                        break;
                    case LivelloMessaggio.DETAILS:
                        icon = "<img src='" + page.ResolveUrl("~/App_Themes/Delta/images/message/info.png") + "' alt='' />";
                        style = "style='font-size:medium; font-weight:bold; color:Blue'";
                        break;
                    case LivelloMessaggio.OK:
                        icon = "<img src='" + page.ResolveUrl("~/App_Themes/Delta/images/message/accept.png") + "' alt='' />";
                        style = "style='font-size:medium; font-weight:bold; color:#000'";
                        break;
                    default:
                        break; 
                }
                s.Append("<li " + style + ">").Append(icon + " " + m.msg).Append("</li>");
            }
            
            s.Append("</ul>");
            return s.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int messageCount()
        {
            return messageList.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exm"></param>
        public void AddMessage(Com.Delta.Logging.ManagedException exm)
        {
            if (exm.Message == null || exm.Message.Equals(""))
            {
                Messaggio m = new Messaggio(exm.CodiceEccezione, LivelloMessaggio.ERROR);
                messageList.Add(new message(decodeMapperMessage(m), LivelloMessaggio.ERROR));
                if(exm.EnanchedInfos != null && !exm.EnanchedInfos.Equals(""))
                messageList.Add(new message(exm.EnanchedInfos,LivelloMessaggio.DETAILS));
            }
            else
            {
                AddMessage(exm.Message,LivelloMessaggio.ERROR);
                if(exm.EnanchedInfos!=null && !exm.EnanchedInfos.Equals(""))
                messageList.Add(new message(exm.EnanchedInfos, LivelloMessaggio.DETAILS));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exm"></param>
        /// <param name="livello"></param>
        public void AddMessage(Com.Delta.Logging.ManagedException exm, Com.Delta.Messaging.MapperMessages.LivelloMessaggio livello)
        {
            if (exm.Message == null || exm.Message.Equals(""))
            {
                Messaggio m = new Messaggio(exm.CodiceEccezione, livello);
                messageList.Add(new message(decodeMapperMessage(m), livello));
                if(exm.EnanchedInfos != null && !exm.EnanchedInfos.Equals(""))
                messageList.Add(new message(exm.EnanchedInfos,LivelloMessaggio.DETAILS));
            }
            else
            {
                AddMessage(exm.Message, livello);
                if(exm.EnanchedInfos != null && !exm.EnanchedInfos.Equals(""))
                messageList.Add(new message(exm.EnanchedInfos, LivelloMessaggio.DETAILS));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static void TranslateDescription(Messaggio AMessage, CacheKeys AKey)
        {
            if ((AMessage != null) && (AKey != CacheKeys.NULL))
            {
                ListaDecodificaMessaggi FList = CacheManager<ListaDecodificaMessaggi>.get(AKey, VincoloType.FILESYSTEM);
                if (FList != null)
                {
                    ListaDecodificaMessaggi.ErroreRow FRow = FList.Errore.FindBycod(AMessage.Codice);

                    if (FRow != null)
                    {
                        AMessage.Descrizione = FRow.desc;
                        if (AMessage.IsVariable)
                        {
                            foreach (KeyValuePair<string, string[]> item in AMessage.Variabili)
                                if (item.Value != null)
                                {
                                    AMessage.Descrizione = AMessage.Descrizione.Replace("\\" + item.Key + "\\", string.Join("||", item.Value));
                                }
                        }
                    }
                }
            }
        }

        public void ClearMessage()
        {
            messageList.Clear();
        }
    }
}
