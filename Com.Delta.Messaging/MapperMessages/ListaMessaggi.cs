using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Com.Delta.Messaging.MapperMessages
{
    [Serializable]
    public class ListaMessaggi
    {
        #region Members

        public LivelloMessaggio mostCriticalLevel = LivelloMessaggio.INFO;
        public LivelloMessaggio lessCriticalLevel = LivelloMessaggio.CRITICAL;

        public int messageNumber;
        public int InfoNumber = 0;
        public int WarningNumber = 0;
        public int CriticalNumber = 0;
        public int AltriLivelliNumber = 0;
        public int ErrorNumber = 0;
        public int ApplicationNumber = 0;
        public int MapperErrorNumber = 0;
        public int DetailsNumber = 0;


        #endregion


        #region Constructors

        public List<Messaggio> messaggi = new List<Messaggio>();

        

        public ListaMessaggi()
        { 
        }


        /* temporaneo da mettere a posto sui crab*/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabMessage"></param>
        public ListaMessaggi(System.Data.DataTable tabMessage)
        {
            fill(tabMessage.Select());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowsMessaggi"></param>
        public ListaMessaggi(System.Data.DataRow[] rowsMessaggi) 
        {
            fill(rowsMessaggi);
        }

        /*---------------------------------------*/

        #endregion

        
        #region Protected Methods

       
        /* temporaneo da mettere a posto sui crab*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowsMessaggi"></param>
        protected internal void fill(System.Data.DataRow[] rowsMessaggi)
        {
            messageNumber = rowsMessaggi.Length;
            List<Messaggio> resp = new List<Messaggio>();
            foreach (System.Data.DataRow r in rowsMessaggi)
            {
                Messaggio temp = new Messaggio(r);
                resp.Add(new Messaggio(r));
                setLevel(temp);
            }
            this.messaggi = resp;
        }

        /*---------------------------------------*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowsMessaggi"></param>
        protected void fill(object[] rowMessaggi) 
        {
            messageNumber = rowMessaggi.Length;
            this.messaggi  = new List<Messaggio>();
            foreach(object r in rowMessaggi)
            {
                Messaggio temp = buildMessaggio(r);
                this.messaggi.Add(temp);
                setLevel(temp); 
            }
        }

        
        protected virtual Messaggio buildMessaggio(object obj)
        {
            throw new Exception("Must be overridden");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="temp"></param>
       private void setLevel(Messaggio temp)
        {
            switch (temp.Livello)
            {
                case LivelloMessaggio.CRITICAL:
                    CriticalNumber++;
                    break;
                case LivelloMessaggio.WARNING:
                    WarningNumber++;
                    break;
                case LivelloMessaggio.INFO:
                    InfoNumber++;
                    break;
                case LivelloMessaggio.ERROR:
                    ErrorNumber++;
                    break;
                case LivelloMessaggio.APPLICATION:
                    ApplicationNumber++;
                    break;
                case LivelloMessaggio.MAPPER_ERROR:
                    MapperErrorNumber++;
                    break;
                case LivelloMessaggio.DETAILS:
                    DetailsNumber++;
                    break;
                default:
                    throw new Exception("Errore Nella codifica del messaggio");
            }
            if (temp.Livello != LivelloMessaggio.MAPPER_ERROR && temp.Livello != LivelloMessaggio.APPLICATION)
            {
                if (mostCriticalLevel > temp.Livello) mostCriticalLevel = temp.Livello;
                if (lessCriticalLevel < temp.Livello) mostCriticalLevel = temp.Livello;
            }
            if (temp.Livello == LivelloMessaggio.APPLICATION)
            {
                if (mostCriticalLevel > temp.Livello) mostCriticalLevel = LivelloMessaggio.CRITICAL;
                if (lessCriticalLevel < temp.Livello) mostCriticalLevel = LivelloMessaggio.CRITICAL;
            }
        }


        #endregion


        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="listaMessaggi"></param>
        /// <returns></returns>
        public bool getMessage(LivelloMessaggio tipo, out IList<Messaggio> listaMessaggi)
        {
            listaMessaggi = null;
            IList<Messaggio> r = new List<Messaggio>();
            foreach (Messaggio m in messaggi)
            {
                if (m.Livello == tipo) r.Add(m);
            }
            if (r.Count > 0)
            {
                listaMessaggi = r;
                return true;
            }
            else return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="codice"></param>
        /// <param name="listaMessaggi"></param>
        /// <returns></returns>
        public bool getMessage(string codice, out IList<Messaggio> listaMessaggi)
        {
            listaMessaggi = null;
            IList<Messaggio> r = new List<Messaggio>();
            foreach (Messaggio m in messaggi)
            {
                if (m.Codice == codice) r.Add(m);
            }
            if (r.Count > 0)
            {
                listaMessaggi = r;
                return true;
            }
            else return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="codice"></param>
        /// <param name="messaggio"></param>
        /// <returns></returns>
        public bool getFirstMessage(string codice, out Messaggio messaggio)
        {
            messaggio = null;
            foreach (Messaggio m in messaggi)
            {
                if (m.Codice == codice)
                {
                    messaggio = m;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="messaggio"></param>
        /// <returns></returns>
        public bool getFirstMessage(out Messaggio messaggio)
        {
            messaggio = null;
            foreach (Messaggio m in messaggi)
            {
                if (!(m.Codice == "000373" && m.Livello == LivelloMessaggio.APPLICATION))
                {
                    messaggio = m;
                    return true;
                }
            }
            return false;
           
        }

        public void Clear()
        {
            if (messaggi != null)
                messaggi.Clear();
        }
    }

        #endregion

}
