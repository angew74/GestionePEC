using System;
using System.Collections.Generic;
using SendMail.BusinessEF.Contracts;
using SendMail.BusinessEF.Base;
using SendMail.Model;
using SendMail.Data.SQLServerDB.Repository;
using Com.Delta.Web.Cache;

namespace SendMail.BusinessEF
{
    public sealed class BackEndDictionaryService: BaseSingletonService<BackEndDictionaryService>, IBackEndDictionaryService
    {

        #region IBackEndDictionaryService Membri di

        public IList<SendMail.Model.BackEndRefCode> GetAll(bool refresh)
        {
            if (refresh)
            {
                using (BackEndCodeSQLDb dao = new BackEndCodeSQLDb())
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
            using (BackEndCodeSQLDb dao = new BackEndCodeSQLDb())
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
            using (BackEndCodeSQLDb dao = new BackEndCodeSQLDb())
                return dao.GetByCode(idCodeBackEnd);
        }

        public List<BackEndRefCode> GetByDescr(string descrBackEnd)
        {
            using (BackEndCodeSQLDb dao = new BackEndCodeSQLDb())
                return dao.GetByDescr(descrBackEnd);
        }

        public void Insert(BackEndRefCode entity)
        {
            using (BackEndCodeSQLDb dao = new BackEndCodeSQLDb())
                dao.Insert(entity);
        }

        public void Delete(decimal id)
        {
            using (BackEndCodeSQLDb dao = new BackEndCodeSQLDb())
                dao.Delete(id);
        }

        #endregion
    }
}
