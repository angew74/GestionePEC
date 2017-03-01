using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Business.Contracts;
using SendMail.DataContracts.Interfaces;
using SendMail.Business.Base;
using SendMail.Model;
using Com.Delta.Web.Cache;

namespace SendMail.Business
{
    public sealed class BackEndDictionaryService: BaseSingletonService<BackEndDictionaryService>, IBackEndDictionaryService
    {

        #region IBackEndDictionaryService Membri di

        public IList<SendMail.Model.BackEndRefCode> GetAll(bool refresh)
        {
            if (refresh)
            {
                using (IBackEndCodeDao dao = getDaoContext().DaoImpl.BackEndCodeDao)
                {
                    CacheManager<List<BackEndRefCode>>.set(CacheKeys.DATI_MEMO, (List<BackEndRefCode>)dao.GetAll());
                    return CacheManager<List<BackEndRefCode>>.get(CacheKeys.DATI_MEMO,Com.Delta.Web.Cache.VincoloType.NONE);
                }
            }
            else
            { 
                if(CacheManager<List<SendMail.Model.BackEndRefCode>>.get(CacheKeys.DATI_MEMO,Com.Delta.Web.Cache.VincoloType.NONE)!=null)
                    return CacheManager<List<SendMail.Model.BackEndRefCode>>.get(CacheKeys.DATI_MEMO,Com.Delta.Web.Cache.VincoloType.NONE);
                else 
                return GetAll(true);
            }
        }
            

        public SendMail.Model.BackEndRefCode Load(decimal id)
        {
            using (IBackEndCodeDao dao = getDaoContext().DaoImpl.BackEndCodeDao)
            {
                return dao.GetById(id);
            }
           
        }

        public SendMail.Model.BackEndRefCode Update(SendMail.Model.BackEndRefCode e)
        {
            throw new NotImplementedException();
        }

        public SendMail.Model.BackEndRefCode insertTitolo(SendMail.Model.BackEndRefCode e)
        {
            throw new NotImplementedException();
        }

        public void deleteTitolo(decimal id)
        {
            throw new NotImplementedException();
        }

        public BackEndRefCode GetByCode(string idCodeBackEnd)
        {
            using (IBackEndCodeDao dao = this.getDaoContext().DaoImpl.BackEndCodeDao)
                return dao.GetByCode(idCodeBackEnd);
        }

        public List<BackEndRefCode> GetByDescr(string descrBackEnd)
        {
            using (IBackEndCodeDao dao = this.getDaoContext().DaoImpl.BackEndCodeDao)
                return dao.GetByDescr(descrBackEnd);
        }

        public void Insert(BackEndRefCode entity)
        {
            using (IBackEndCodeDao dao = this.getDaoContext().DaoImpl.BackEndCodeDao)
                dao.Insert(entity);
        }

        public void Delete(decimal id)
        {
            using (IBackEndCodeDao dao = this.getDaoContext().DaoImpl.BackEndCodeDao)
                dao.Delete(id);
        }

        #endregion
    }
}
