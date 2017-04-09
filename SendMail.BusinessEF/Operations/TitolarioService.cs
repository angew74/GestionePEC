using System;
using System.Collections.Generic;
using System.Linq;
using SendMail.BusinessEF.Base;
using SendMail.Model;
using SendMail.Data.SQLServerDB.Repository;

namespace SendMail.BusinessEF
{

    public sealed class TitolarioService<T> : BaseSingletonService<TitolarioService<T>>, ITitolarioService<T> where T : Titolo
    {
        public IList<T> GetAll(decimal? idPadre)
        {           
            if (idPadre.HasValue)
            {
                using (TitoloSQLDb dao = new TitoloSQLDb())
                {
                    return dao.GetSottotitoliByIdPadre(idPadre.Value) as IList<T>;
                }
            }
            else
            {
                if (typeof(T) == typeof(Titolo))
                    using (TitoloSQLDb dao = new TitoloSQLDb())
                    {
                        return dao.GetAll() as IList<T>;
                    }
                else
                    using (SottoTitoloSQLDb dao = new SottoTitoloSQLDb())
                    {
                        return dao.GetAll() as IList<T>;
                    }
            }
        }

        public T LoadTitoloById(decimal id)
        {
            if(typeof(T)==typeof(Titolo))
                using (TitoloSQLDb dao = new TitoloSQLDb())
                {
                    return dao.GetById(id) as T;
                }
            else
                using (SottoTitoloSQLDb dao = new SottoTitoloSQLDb())
                {
                    return dao.GetById(id) as T;
                }
        }

        public IList<T> FindByTitolo(decimal tkey)
        {

            using (SottoTitoloSQLDb dao = new SottoTitoloSQLDb())
            {
                    return dao.FindByTitolo(tkey) as IList<T>;
                }
        }

        public void deleteTitolo(decimal id, decimal? idPadre)
        {
            if (typeof(T) == typeof(Titolo))
            {
                using (TitoloSQLDb dao = new TitoloSQLDb())
                {
                    dao.Delete(id);
                }
            }
            else
            {
                using (SottoTitoloSQLDb dao = new SottoTitoloSQLDb())
                {
                    {
                        dao.Delete(id);
                    }
                    
                }
            }
        }


        public T updateTitolo(T e)
        {
            if (typeof(T) == typeof(Titolo))
            {
                using (TitoloSQLDb dao = new TitoloSQLDb())
                {
                    dao.Update(e);
                    return e;
                }
            }
            else
            {
                using (SottoTitoloSQLDb dao = new SottoTitoloSQLDb())
                {
                    //return (IList<Titolo>)dao.GetAll();
                    return null;
                }
            }
        }

        public T insertTitolo(T e)
        {
            if (typeof(T) == typeof(Titolo))
            {
                using (TitoloSQLDb dao = new TitoloSQLDb())
                {
                    dao.Insert(e);
                    return e;
                }
            }
            else
            {
                using (SottoTitoloSQLDb dao = new SottoTitoloSQLDb())
                {
                    dao.Insert(e as SottoTitolo);
                    return e;
                }
            }
        }

        public T LoadByComCode(string code)
        {
            using (SottoTitoloSQLDb dao = new SottoTitoloSQLDb())
            {
                return (T)(Titolo)dao.GetAll().FirstOrDefault(s => string.Equals(s.ComCode, code, StringComparison.OrdinalIgnoreCase));
            }
        }

        public T LoadByCode(string code)
        {
            if (typeof(T) == typeof(Titolo))
            {
                using (TitoloSQLDb dao = new TitoloSQLDb())
                {
                    return (T)dao.GetAll().SingleOrDefault(t => string.Equals(t.AppCode, code, StringComparison.OrdinalIgnoreCase));
                }
            }
            else if (typeof(T) == typeof(SottoTitolo))
            {
                using (SottoTitoloSQLDb dao = new SottoTitoloSQLDb())
                {
                    return (T)(Titolo)dao.GetAll().FirstOrDefault(s => string.Equals(s.ComCode, code, StringComparison.OrdinalIgnoreCase));
                }
            }
            else throw new InvalidOperationException("Tipo non gestito.");
        }
    }
}
