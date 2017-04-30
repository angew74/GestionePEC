using Com.Delta.Logging;
using Com.Delta.Logging.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;


namespace Com.Delta.Logging
{
    public class MailLogRepository : IMailLogRepository
    {


        public List<LOG_ACTIONS> GetByLogCode(string logCode)
        {
            List<LOG_ACTIONS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                var result = dbcontext.LOG_ACTIONS.Where(x => x.LOG_CODE.ToUpper() == logCode.ToUpper()).ToList();
            }
            return list;
        }
        public List<LOG_ACTIONS> GetByLogCodeDate(string logCode, DateTime d, DateTime df, int da, int per)
        {
            List<LOG_ACTIONS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                var result = dbcontext.LOG_ACTIONS.Where(x => x.LOG_CODE.ToUpper() == logCode.ToUpper()).Skip(da).Take(per).ToList();
            }
            return list;
        }

        public List<LOG_ACTIONS> GetByAppCode(string appCode)
        {
            List<LOG_ACTIONS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                var result = dbcontext.LOG_ACTIONS.Where(x => x.APP_CODE.ToUpper() == appCode.ToUpper()).ToList();
            }
            return list;
        }

        public List<LOG_ACTIONS> GetByAppCodeDate(string appCode, DateTime d, DateTime df,int da, int per)
        {
            List<LOG_ACTIONS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                var result = dbcontext.LOG_ACTIONS.Where(x => x.APP_CODE.ToUpper() == appCode.ToUpper() && x.LOG_DATE >= d && x.LOG_DATE <= df).Skip(da).Take(per).ToList();
            }
            return list;
        }


        public List<LOG_ACTIONS> GetByUserMail(string userMail)
        {
            List<LOG_ACTIONS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                var result = dbcontext.LOG_ACTIONS.Where(x => x.USER_MAIL.ToUpper() == userMail.ToUpper()).ToList();
            }
            return list;
        }

        public List<LOG_ACTIONS> GetByUserMailDate(string userMail, DateTime d, DateTime df, int da,int per)
        {
            List<LOG_ACTIONS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                var result = dbcontext.LOG_ACTIONS.Where(x => x.USER_MAIL.ToUpper() == userMail.ToUpper() && x.LOG_DATE >= d && x.LOG_DATE <= df).Skip(da).Take(per).ToList();
            }
            return list;
        }

        public List<LOG_ACTIONS> GetAll()
        {
            throw new NotImplementedException();
        }
        public List<LOG_ACTIONS> GetAllByRows(int da, int a)
        {
            List<LOG_ACTIONS> list = null;
            using (var dbcontext = new  MailLogEntities())
            {
                var result = (from p in dbcontext.LOG_ACTIONS
                          select p);
                list = result.Skip(da).Take(a).ToList();
            }
            return list;
        }


        
        public List<LOG_ACTIONS> GetRowsByDate(DateTime d, DateTime df)
        {
            List<LOG_ACTIONS> list = null;
            using (var dbcontext = new MailLogEntities())
            {

                list = (from p in dbcontext.LOG_ACTIONS
                            where p.LOG_DATE >= d && p.LOG_DATE <= df
                            select p).ToList();
            }
            return list;
        }

        public List<LOG_ACTIONS> GetRowsByDate(DateTime d, DateTime df,int da, int per)
        {
            List<LOG_ACTIONS> list = null;
            using (var dbcontext = new MailLogEntities())
            {

                list = (from p in dbcontext.LOG_ACTIONS
                        where p.LOG_DATE >= d && p.LOG_DATE <= df
                        select p).Skip(da).Take(per).ToList();
            }
            return list;
        }

        public List<LOG_ACTIONS> GetRowsByUser(string user)
        {
            List<LOG_ACTIONS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                 list = (from p in dbcontext.LOG_ACTIONS
                            where p.USER_ID.ToUpper() == user.ToUpper()
                            select p).ToList();
            }
            return list;
        }
        public List<LOG_ACTIONS> GetRowsByUserDate(string user, DateTime d, DateTime df, int da, int per)
        {
            List<LOG_ACTIONS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                list = (from p in dbcontext.LOG_ACTIONS
                        where p.USER_ID.ToUpper() == user.ToUpper()
                        && p.LOG_DATE >= d && p.LOG_DATE <= df
                        select p).Skip(da).Take(per).ToList();
            }
            return list;
        }
        public LOG_ACTIONS GetRowById(string id)
        {
            LOG_ACTIONS result = null;
            using (var dbcontext = new MailLogEntities())
            {
                 result = (from p in dbcontext.LOG_ACTIONS
                              where p.LOG_UID == id
                              select p).FirstOrDefault();
            }
            return result;

        }
        public LOG_ACTIONS Add(LOG_ACTIONS entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(LOG_ACTIONS entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(LOG_ACTIONS entity)
        {
            throw new NotImplementedException();
        }

        public LOG_ACTIONS Get(Expression<Func<LOG_ACTIONS, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public List<LOG_ACTIONS> GetVariables(string user, string codapp, string codlog, string usermail, DateTime d, DateTime df, int da, int per,ref int tot)
        {
            List<LOG_ACTIONS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                var Querable = from e in dbcontext.LOG_ACTIONS select e;
                var QuerableCount = from e in dbcontext.LOG_ACTIONS select e;
                if (!string.IsNullOrEmpty(user))
                {
                    Querable = Querable.Where(x => x.USER_ID.ToUpper() == user.ToUpper());
                    QuerableCount = QuerableCount.Where(x => x.USER_ID.ToUpper() == user.ToUpper());
                }
                if (!string.IsNullOrEmpty(codapp))
                {
                    Querable = Querable.Where(x => x.APP_CODE.ToUpper() == codapp.ToUpper());
                    QuerableCount = QuerableCount.Where(x => x.APP_CODE.ToUpper() == codapp.ToUpper());
                }
                if (!string.IsNullOrEmpty(codlog))
                {
                    Querable = Querable.Where(x => x.LOG_CODE.ToUpper() == codlog.ToUpper());
                    QuerableCount = QuerableCount.Where(x => x.LOG_CODE.ToUpper() == codlog.ToUpper());
                }
                if (!string.IsNullOrEmpty(usermail))
                {
                    Querable = Querable.Where(x => x.USER_MAIL.ToUpper()==usermail.ToUpper());
                    QuerableCount = QuerableCount.Where(x => x.USER_MAIL.ToUpper() == usermail.ToUpper());
                }
                list = Querable.Where(p => p.LOG_DATE >= d && p.LOG_DATE <= df).OrderByDescending(u=>u.LOG_DATE).Skip(da).Take(per).ToList();
                tot = QuerableCount.Where(p => p.LOG_DATE >= d && p.LOG_DATE <= df).OrderByDescending(f => f.LOG_DATE).Count();
            }

            return list;
        }
    }
}