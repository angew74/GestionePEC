using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActiveUp.Net.Common.DeltaExt
{
   
   public class Action
    {
            #region IDomainObject Membri di

        public bool IsValid
        {
            get { return (Id > 0 && NomeAzione != null); }
        }

        public bool IsPersistent
        {
            get { return Id > 0; }
        }

        #endregion

        #region "C.tor"

        public Action()
        {

        }

        public Action(int id, String nomeAzione,decimal idDestinazione, string tipoDestinazione,string tipoAzione,string nuovoStatus,string tipoFolder,int idfolderdestinazione)
        {
            this.Id = id;
            this.NomeAzione = nomeAzione;
            this.TipoAzione = tipoAzione;
            this.TipoDestinazione = tipoDestinazione;
            this.NuovoStatus = nuovoStatus;
            this.IdDestinazione = IdDestinazione;
            this.IdComp = id.ToString() + tipoFolder;
            this.IdFolderDestinazione = idfolderdestinazione;
        }

        #endregion

        #region "Properties"

     
        public virtual decimal Id { get; set; }

        public virtual decimal IdDestinazione { get; set; }

        public virtual string TipoDestinazione { get; set; }

        public virtual string TipoAzione { get; set; }

        public virtual string NuovoStatus { get; set; }
      
        public virtual string NomeAzione { get; set; }

        public virtual string TipoFolder { get; set; }

        public virtual int IdFolderDestinazione { get; set; }

        public virtual string IdComp
        {
            get { return Id.ToString() + TipoFolder; }
            set { ;}
        }

        #endregion
    }
}
