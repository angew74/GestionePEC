using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;



namespace Com.Delta.Logging
{
   public interface ILogRepository<TType> 
    {

        List<TType> GetAll();
        List<TType> GetAllByRows(int da, int a);
        List<TType> GetRowsByDate(DateTime d, DateTime df);
        List<TType> GetRowsByUser(string user);
        List<TType> GetRowsByUserDate(string user, DateTime d, DateTime df,int da, int per);
        TType GetRowById(string id);
        TType Add(TType entity);
       bool Update(TType entity);
       bool Delete(TType entity);
     
      
    }
}
