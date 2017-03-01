using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActiveUp.Net.Common.DeltaExt
{
   
    public class Folder 
    {
        #region IDomainObject Membri di

        public bool IsValid
        {
            get { return (Id > 0 && Nome != null); }
        }

        public bool IsPersistent
        {
            get { return Id > 0; }
        }

        #endregion

        #region "C.tor"

        public Folder()
        {

        }

        public Folder(int id, String nome, String abilitata,String tipo,string idNome)
        {
            this.Id = id;
            this.Nome = nome;
            this.Abilitata = abilitata;
            this.TipoFolder = tipo;
            this.IdNome = idNome;

        }

        #endregion

        #region "Properties"

      
        public virtual decimal Id { get; set; }

      
        public virtual string Nome { get; set; }

       
        public virtual string Abilitata { get; set; }

        public virtual string TipoFolder { get; set; }

        public virtual string IdNome { get; set; }

        public virtual List<ActiveUp.Net.Common.DeltaExt.Action> Azioni
        { get; set; }


        #endregion

    }
}
