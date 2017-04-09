using System;
using System.Collections.Generic;
using SendMail.BusinessEF.Base;
using SendMail.BusinessEF.Contracts;
using SendMail.Model;
using SendMail.Model.RubricaMapping;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.Model.Wrappers;
using SendMail.Data.SQLServerDB;
using SendMail.Data.SQLServerDB.Repository;

namespace SendMail.BusinessEF
{
    public class RubricaEntitaService : BaseSingletonService<RubricaEntitaService>, IRubricaEntitaService
    {

        #region IRubricaEntitaService Membri di

        public RubricaEntita GetEntitaById(long idEntita)
        {
            using (RubrEntitaSQLDb dao = new RubrEntitaSQLDb())
            {
                return dao.GetById(idEntita);
            }
        }

        public ResultList<RubricaEntita> GetEntitaByParams(IList<EntitaType> tEnt, IDictionary<SendMail.Model.FastIndexedAttributes, IList<string>> pars, int da, int per)
        {
            using (RubrEntitaSQLDb dao = new RubrEntitaSQLDb())
            {
                return dao.LoadEntitaByParams(tEnt, pars, da, per);
            }
        }

        public ResultList<RubricaEntita> GetEntitaByParamsSimilarity(IList<EntitaType> tEnt, IDictionary<SendMail.Model.FastIndexedAttributes, IList<string>> pars, int da, int per)
        {
            using (RubrEntitaSQLDb dao = new RubrEntitaSQLDb())
            {
                // return dao.LoadSimilarityEntitaByParams(tEnt, pars, da, per);
                throw new NotImplementedException();
            }
        }

        public ResultList<RubricaEntita> GetEntitaByMailDomain(IList<EntitaType> tEnt, string mail, int da, int per)
        {
            using (RubrEntitaSQLDb dao = new RubrEntitaSQLDb())
            {
                return dao.LoadEntitaByMailDomain(tEnt, mail, da, per);
            }
        }

        public ResultList<RubricaEntita> GetEntitaIPAByMailDomain(string mail, int da, int per)
        {
            using (RubrEntitaSQLDb dao = new RubrEntitaSQLDb())
            {
                //  return dao.LoadEntitaIPAByMailDomain(mail, da, per);
                throw new NotImplementedException();
            }
        }

        public List<RubricaEntita> GetEntitaIPAbyIPAid(string IPAid)
        {
            using (RubrEntitaSQLDb dao = new RubrEntitaSQLDb())
            {
                return dao.LoadEntitaIPAbyIPAid(IPAid);
            }
        }

        public List<RubricaEntita> GetEntitaByPartitaIVA(string partitaIVA)
        {
            using (RubrEntitaSQLDb dao = new RubrEntitaSQLDb())
            {
                return dao.LoadEntitaByPartitaIVA(partitaIVA);
            }
        }

        public RubricaEntita GetRubricaEntitaCompleteById(Int64 idEntita)
        {
            using (RubrEntitaSQLDb dao = new RubrEntitaSQLDb())
            {
                return dao.LoadEntitaCompleteById(idEntita);
            }
        }

        public List<RubricaEntita> GetEntitaByName(IList<EntitaType> tEnt, string name)
        {
            using (RubrEntitaSQLDb dao = new RubrEntitaSQLDb())
            {
                return dao.LoadEntitaByName(tEnt, name);
            }
        }

        public IList<SimpleTreeItem> GetRubricaEntitaTree(Int64 idEntita)
        {
            using (RubrEntitaSQLDb dao = new RubrEntitaSQLDb())
            {
                return dao.LoadRubricaEntitaTree(idEntita);
            }
        }

        public IList<SimpleTreeItem> GetRubricaEntitaTreeByIdPadre(Int64 idPadre)
        {
            using (RubrEntitaSQLDb dao = new RubrEntitaSQLDb())
            {
                return dao.LoadRubricaEntitaTreeByIdPadre(idPadre);
            }
        }

        public void Update(RubricaEntita r)
        {
            using (RubrEntitaSQLDb dao = new RubrEntitaSQLDb())
            {
                dao.Update(r);
            }
        }

        //ATTENZIONE!!! Attualmente si possono inserire solo gruppi e uffici interni a organizzazioni
        public void Insert(RubricaEntita r)
        {
            using (RubrEntitaSQLDb dao = new RubrEntitaSQLDb())
            {
                dao.Insert(r);
            }
        }

        #endregion
    }
}
