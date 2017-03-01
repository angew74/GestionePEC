using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Business.Base;
using SendMail.Business.Contracts;
using SendMail.Model;
using SendMail.Model.RubricaMapping;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.DataContracts.Interfaces;
using SendMail.Model.Wrappers;

namespace SendMail.Business
{
    public class RubricaEntitaService : BaseSingletonService<RubricaEntitaService>, IRubricaEntitaService
    {

        #region IRubricaEntitaService Membri di

        public RubricaEntita GetEntitaById(long idEntita)
        {
            using (IRubricaEntitaDao dao = getDaoContext().DaoImpl.RubricaEntitaDao)
            {
                return dao.GetById(idEntita);
            }
        }

        public ResultList<RubricaEntita> GetEntitaByParams(IList<EntitaType> tEnt, IDictionary<SendMail.Model.FastIndexedAttributes, IList<string>> pars, int da, int per)
        {
            using (IRubricaEntitaDao dao = getDaoContext().DaoImpl.RubricaEntitaDao)
            {
                return dao.LoadEntitaByParams(tEnt, pars, da, per);
            }
        }

        public ResultList<RubricaEntita> GetEntitaByParamsSimilarity(IList<EntitaType> tEnt, IDictionary<SendMail.Model.FastIndexedAttributes, IList<string>> pars, int da, int per)
        {
            using (IRubricaEntitaDao dao = getDaoContext().DaoImpl.RubricaEntitaDao)
            {
                return dao.LoadSimilarityEntitaByParams(tEnt, pars, da, per);
            }
        }

        public ResultList<RubricaEntita> GetEntitaByMailDomain(IList<EntitaType> tEnt, string mail, int da, int per)
        {
            using (IRubricaEntitaDao dao = getDaoContext().DaoImpl.RubricaEntitaDao)
            {
                return dao.LoadEntitaByMailDomain(tEnt, mail, da, per);
            }
        }

        public ResultList<RubricaEntita> GetEntitaIPAByMailDomain(string mail, int da, int per)
        {
            using (IRubricaEntitaDao dao = getDaoContext().DaoImpl.RubricaEntitaDao)
            {
                return dao.LoadEntitaIPAByMailDomain(mail, da, per);
            }
        }

        public List<RubricaEntita> GetEntitaIPAbyIPAid(string IPAid)
        {
            using (IRubricaEntitaDao dao = getDaoContext().DaoImpl.RubricaEntitaDao)
            {
                return dao.LoadEntitaIPAbyIPAid(IPAid);
            }
        }

        public List<RubricaEntita> GetEntitaByPartitaIVA(string partitaIVA)
        {
            using (IRubricaEntitaDao dao = getDaoContext().DaoImpl.RubricaEntitaDao)
            {
                return dao.LoadEntitaByPartitaIVA(partitaIVA);
            }
        }

        public RubricaEntita GetRubricaEntitaCompleteById(Int64 idEntita)
        {
            using (IRubricaEntitaDao dao = getDaoContext().DaoImpl.RubricaEntitaDao)
            {
                return dao.LoadEntitaCompleteById(idEntita);
            }
        }

        public List<RubricaEntita> GetEntitaByName(IList<EntitaType> tEnt, string name)
        {
            using (IRubricaEntitaDao dao = getDaoContext().DaoImpl.RubricaEntitaDao)
            {
                return dao.LoadEntitaByName(tEnt, name);
            }
        }

        public IList<SimpleTreeItem> GetRubricaEntitaTree(Int64 idEntita)
        {
            using (IRubricaEntitaDao dao = getDaoContext().DaoImpl.RubricaEntitaDao)
            {
                return dao.LoadRubricaEntitaTree(idEntita);
            }
        }

        public IList<SimpleTreeItem> GetRubricaEntitaTreeByIdPadre(Int64 idPadre)
        {
            using (IRubricaEntitaDao dao = getDaoContext().DaoImpl.RubricaEntitaDao)
            {
                return dao.LoadRubricaEntitaTreeByIdPadre(idPadre);
            }
        }

        public void Update(RubricaEntita r)
        {
            using (IRubricaEntitaDao dao = getDaoContext().DaoImpl.RubricaEntitaDao)
            {
                dao.Update(r);
            }
        }

        //ATTENZIONE!!! Attualmente si possono inserire solo gruppi e uffici interni a organizzazioni
        public void Insert(RubricaEntita r)
        {
            using (IRubricaEntitaDao dao = getDaoContext().DaoImpl.RubricaEntitaDao)
            {
                dao.Insert(r);
            }
        }

        #endregion
    }
}
