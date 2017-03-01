using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Com.Delta.Messaging
{
    public class DataBox<T>
    {

       
        /* temporaneo da mettere a posto sui crab*/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabMessage"></param>
        //public DataBox(object o)
        //{
                
        //}
        public DataBox(ICollection<T> i)
        {
            this.documenti = i;
        }

        private ICollection<T> documenti;

        public ICollection<T> Documenti
        {
            get { return documenti; }
            set { documenti = value; }
        }

        private IList<Messaging.MapperMessages.ListaMessaggi> messaggi;

        public IList<Messaging.MapperMessages.ListaMessaggi> Messaggi
        {
            get { return messaggi; }
            set { messaggi = value; }
        }

        private T payLoad;

        public T PayLoad
        {
            get { return payLoad; }
            set { payLoad = value; }
        }

      
    }
}
 