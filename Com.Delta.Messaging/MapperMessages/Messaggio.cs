using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Com.Delta.Messaging.MapperMessages
{
	/// <summary>
	/// 
	/// </summary>
    /// 
    [Serializable]
    public class Messaggio 
	{
        private string _codice;
        private LivelloMessaggio _livello;
        private string _descrizione;
        private bool _isVariable;
        private int _varNumber;
        private IDictionary<string, string[]> _variabili;

        public string Codice
        {
            get { return _codice; }
          
        }

        public LivelloMessaggio Livello
        {
            get { return _livello; }
            set { _livello = value; }
        }

        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

        public bool IsVariable
        {
            get { return _isVariable; }
        }

        public int VarNumber
        {
            get { return _varNumber; }
        }

        public IDictionary<string, string[]> Variabili
        {
            get { return _variabili; }
        }

        public Messaggio(string codice, LivelloMessaggio livello) 
        {
            _codice = codice;
            _livello = livello;
            _isVariable = false;
        }

        public Messaggio()
        {

        }
		
		public Messaggio(System.Data.DataRow r)
		{
			_livello = (LivelloMessaggio)Enum.Parse(typeof(LivelloMessaggio), r["Livello"].ToString());
			_codice = r["Codice"].ToString();
			_descrizione = r["Descrizione"].ToString();
            splitMessaggio(); 
		}


        public Messaggio(string codice, string livello, string descrizione)
        {
            _codice = codice;
            _livello = (LivelloMessaggio)Enum.Parse(typeof(LivelloMessaggio), livello);
            _descrizione = descrizione;
            splitMessaggio(); 
        }

        protected internal void splitMessaggio()
        {
            _variabili = new Dictionary<string, string[]>();
            if (_descrizione != null && _descrizione.Length > 0)
            {
                _isVariable = true;
                string[] vars = _descrizione.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                if (vars == null || vars.Length == 0)
                    throw new Exception("Il messaggio MAPPER non rispetta la codifica standard");

                _varNumber = vars.Length;
                for (int i = 0; i < VarNumber; i++)
                {
                    string[] temp = vars[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (temp == null || temp.Length == 0)
                        throw new Exception("Il messaggio MAPPER non rispetta la codifica standard");

                    int l = temp.Length;
                    if (l == 1)
                        Variabili.Add(temp[0], null);

                    else if (l == 2)
                    {   //separa la lista di variabili
                        string[] temp1 = temp[1].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                        if (temp1.Length == 0)
                            throw new Exception("Il messaggio MAPPER non rispetta la codifica standard");

                        Variabili.Add(temp[0], temp1);
                    }
                }
            }
        }
	}
}
