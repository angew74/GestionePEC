using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;
using SendMail.Data.SQLServerDB.Mapping;
using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Repository
{
   public class MailRefsSQLDb : IMailRefsDao
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MailRefsSQLDb));

        public ICollection<Model.MailRefs> GetMailRefsOfAMail(long idMail)
        {
            List<MailRefs> mailRefsList = null;
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    List<MAIL_REFS_NEW> listrefs = dbcontext.MAIL_REFS_NEW.Where(x => x.REF_ID_MAIL == idMail).ToList();
                    mailRefsList = new List<MailRefs>();
                    foreach (MAIL_REFS_NEW r in listrefs)
                    {
                        MailRefs mrefsnew = AutoMapperConfiguration.FromMailRefsNewToModel(r);
                        mailRefsList.Add(mrefsnew);
                    }
                }
            }
            catch
            {
                mailRefsList = null;
            }
            return mailRefsList;
        }

        public ICollection<Model.MailRefs> GetAll()
        {
            throw new NotImplementedException();
        }

        public Model.MailRefs GetById(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(Model.MailRefs entity)
        {
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    MAIL_REFS_NEW refs = AutoMapperConfiguration.FromMailRefsNewToDto(entity);
                    dbcontext.MAIL_REFS_NEW.Add(refs);
                    dbcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                //Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(ex.Message, "ERR_INS001", string.Empty, string.Empty, ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    er.objectID = entity.IdRef.ToString();
                    log.Error(er);
                    throw mEx;
                }
                else
                    throw ex;
            }
        }

        public void Update(Model.MailRefs entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        public int Save(MailRefs entity)
        {
            int ins = 0;
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    MAIL_REFS_NEW refs = AutoMapperConfiguration.FromMailRefsNewToDto(entity);
                    dbcontext.MAIL_REFS_NEW.Add(refs);
                    ins = dbcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(ex.Message, "ERR_INS002", string.Empty, string.Empty, ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    er.objectID = entity.IdRef.ToString();
                    log.Error(er);
                    throw mEx;
                }
                else
                    throw ex;
            }
            return ins;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

