using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model.ComunicazioniMapping
{
    [Serializable]
  public class DataItem<K,V>
    {
        public K Key
        {
            get;
            set;
        }
        public V Value
        {
            get;
            set;
        }
    }
}
