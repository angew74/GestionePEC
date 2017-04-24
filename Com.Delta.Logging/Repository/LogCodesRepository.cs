using Com.Delta.Logging.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Delta.Logging.Context;
using System.Linq.Expressions;

namespace Com.Delta.Logging.Repository
{
    public class LogCodesRepository : ILogCodesRepository
    {
        public LOG_LOG_CODES Add(LOG_LOG_CODES entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(LOG_LOG_CODES entity)
        {
            throw new NotImplementedException();
        }

        public LOG_LOG_CODES Get(Expression<Func<LOG_LOG_CODES, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public List<LOG_LOG_CODES> GetAll()
        {
            List<LOG_LOG_CODES> list = null;
            using (var dbcontext = new MailLogEntities())
            {
                var result = dbcontext.LOG_LOG_CODES.ToList();
            }
            return list;
        }

        public List<LOG_LOG_CODES> GetAllByRows(int da, int a)
        {
            throw new NotImplementedException();
        }

        public LOG_LOG_CODES GetRowById(string id)
        {
            throw new NotImplementedException();
        }

        public List<LOG_LOG_CODES> GetRowsByDate(DateTime d, DateTime df)
        {
            throw new NotImplementedException();
        }

        public List<LOG_LOG_CODES> GetRowsByUser(string user)
        {
            throw new NotImplementedException();
        }

        public List<LOG_LOG_CODES> GetRowsByUserDate(string user, DateTime d, DateTime df, int da, int per)
        {
            throw new NotImplementedException();
        }

        public bool Update(LOG_LOG_CODES entity)
        {
            throw new NotImplementedException();
        }
    }
}
