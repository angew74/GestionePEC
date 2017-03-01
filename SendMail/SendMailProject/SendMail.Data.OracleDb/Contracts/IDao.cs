using System;
using System.Collections.Generic;
using System.Text;

namespace SendMail.DataContracts.Interfaces
{
    public interface IDao<T,K>: IDisposable
    {
        ICollection<T> GetAll();
        T GetById(K id);
        void Insert(T entity);
        
        void Update(T entity);
        void Delete(K id);
    }
}

       