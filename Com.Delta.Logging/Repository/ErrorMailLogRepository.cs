using Com.Delta.Logging.Context;
using Com.Delta.Logging.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Com.Delta.Logging.Repository
{
    public class ErrorMailLogRepository : IErrorMailLogRepository
    {
        public List<LOG_APP_ERRORS> GetByLogCode(string logCode)
        {
            List<LOG_APP_ERRORS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                var result = dbcontext.LOG_APP_ERRORS.Where(x => x.LOG_CODE.ToUpper() == logCode.ToUpper()).ToList();
            }
            return list;
        }

        public List<LOG_APP_ERRORS> GetByAppCode(string appCode)
        {
            List<LOG_APP_ERRORS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                var result = dbcontext.LOG_APP_ERRORS.Where(x => x.APP_CODE.ToUpper() == appCode.ToUpper()).ToList();
            }
            return list;
        }


        public List<LOG_APP_ERRORS> GetByUserId(string userMail)
        {
            List<LOG_APP_ERRORS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                var result = dbcontext.LOG_APP_ERRORS.Where(x => x.USER_ID.ToUpper() == userMail.ToUpper()).ToList();
            }
            return list;
        }

        public List<LOG_APP_ERRORS> GetAll()
        {
            throw new NotImplementedException();
        }
        public List<LOG_APP_ERRORS> GetAllByRows(int da, int a)
        {
            List<LOG_APP_ERRORS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                var result = (from p in dbcontext.LOG_APP_ERRORS
                              select p);
                list = result.Skip(da).Take(a).ToList();
            }
            return list;
        }
        public List<LOG_APP_ERRORS> GetRowsByDate(DateTime d, DateTime df)
        {
            List<LOG_APP_ERRORS> list = null;
            using (var dbcontext = new MailLogEntities())
            {

                list = (from p in dbcontext.LOG_APP_ERRORS
                        where p.LOG_DATE >= d && p.LOG_DATE <= df
                        select p).ToList();
            }
            return list;
        }
        public List<LOG_APP_ERRORS> GetRowsByUser(string user)
        {
            List<LOG_APP_ERRORS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                list = (from p in dbcontext.LOG_APP_ERRORS
                        where p.USER_ID.ToUpper() == user.ToUpper()
                        select p).ToList();
            }
            return list;
        }
        public List<LOG_APP_ERRORS> GetRowsByUserDate(string user, DateTime d, DateTime df, int da, int per)
        {
            List<LOG_APP_ERRORS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                list = (from p in dbcontext.LOG_APP_ERRORS
                        where p.USER_ID.ToUpper() == user.ToUpper()
                        && p.LOG_DATE >= d && p.LOG_DATE <= df
                        select p).ToList();
            }
            return list;
        }
        public LOG_APP_ERRORS GetRowById(string id)
        {
            LOG_APP_ERRORS result = null;
            using (var dbcontext = new MailLogEntities())
            {
                result = (from p in dbcontext.LOG_APP_ERRORS
                          where p.LOG_UID == id
                          select p).FirstOrDefault();
            }
            return result;

        }
        public LOG_APP_ERRORS Add(LOG_APP_ERRORS entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(LOG_APP_ERRORS entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(LOG_APP_ERRORS entity)
        {
            throw new NotImplementedException();
        }
        public List<LOG_APP_ERRORS> GetVariables(string user, string codapp, string codlog, string details, DateTime d, DateTime df, int da, int per)
        {
            List<LOG_APP_ERRORS> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                var Querable = from e in dbcontext.LOG_APP_ERRORS select e;
                if (!string.IsNullOrEmpty(user))
                { Querable = Querable.Where(x => x.USER_ID.ToUpper() == user.ToUpper()); }
                if (!string.IsNullOrEmpty(codapp))
                { Querable = Querable.Where(x => x.APP_CODE.ToUpper() == codapp.ToUpper()); }
                if (!string.IsNullOrEmpty(codlog))
                { Querable = Querable.Where(x => x.LOG_CODE.ToUpper() == codlog.ToUpper()); }
                if (!string.IsNullOrEmpty(details))
                { Querable = Querable.Where(x => x.DETAILS.ToUpper().Contains(details.ToUpper())); }
                list = Querable.Where(p => p.LOG_DATE >= d && p.LOG_DATE <= df).Skip(da).Take(per).ToList();
            }

            return list;
        }
    }
}
