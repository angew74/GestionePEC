using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Business.Base;
using SendMail.DataContracts.Interfaces;
using SendMail.Model;

namespace SendMail.Business
{

    public sealed class TitolarioService<T> : BaseSingletonService<TitolarioService<T>>, ITitolarioService<T> where T : Titolo
    {
        public IList<T> GetAll(decimal? idPadre)
        {
            Com.Delta.Data.IDaoBaseSession<ISessionModel> provider = getDaoContext();
            if (idPadre.HasValue)
            {
                using (ITitoloDao dao = provider.DaoImpl.TitoloDao)
                {
                    return dao.GetSottotitoliByIdPadre(idPadre.Value) as IList<T>;
                }
            }
            else
            {
                if (typeof(T) == typeof(Titolo))
                    using (ITitoloDao dao = provider.DaoImpl.TitoloDao)
                    {
                        return dao.GetAll() as IList<T>;
                    }
                else
                    using (ISottoTitoloDao dao = provider.DaoImpl.SottoTitoloDao)
                    {
                        return dao.GetAll() as IList<T>;
                    }
            }
        }

        public T LoadTitoloById(decimal id)
        {
            Com.Delta.Data.IDaoBaseSession<ISessionModel> provider = getDaoContext();
            if(typeof(T)==typeof(Titolo))
                using (ITitoloDao dao = provider.DaoImpl.TitoloDao)
                {
                    return dao.GetById(id) as T;
                }
            else
                using (ISottoTitoloDao dao = provider.DaoImpl.SottoTitoloDao)
                {
                    return dao.GetById(id) as T;
                }
        }

        public IList<T> FindByTitolo(decimal tkey)
        {
            Com.Delta.Data.IDaoBaseSession<ISessionModel> provider = getDaoContext();            
                using (ISottoTitoloDao dao = provider.DaoImpl.SottoTitoloDao)
                {
                    return dao.FindByTitolo(tkey) as IList<T>;
                }
        }

        public void deleteTitolo(decimal id, decimal? idPadre)
        {
            if (typeof(T) == typeof(Titolo))
            {
                using (ITitoloDao dao = getDaoContext().DaoImpl.TitoloDao)
                {
                    dao.Delete(id);
                }
            }
            else
            {
                using (ISottoTitoloDao dao = getDaoContext().DaoImpl.SottoTitoloDao)
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
                using (ITitoloDao dao = getDaoContext().DaoImpl.TitoloDao)
                {
                    dao.Update(e);
                    return e;
                }
            }
            else
            {
                using (ISottoTitoloDao dao = getDaoContext().DaoImpl.SottoTitoloDao)
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
                using (ITitoloDao dao = getDaoContext().DaoImpl.TitoloDao)
                {
                    dao.Insert(e);
                    return e;
                }
            }
            else
            {
                using (ISottoTitoloDao dao = getDaoContext().DaoImpl.SottoTitoloDao)
                {
                    dao.Insert(e as SottoTitolo);
                    return e;
                }
            }
        }

        public T LoadByComCode(string code)
        {
            using (ISottoTitoloDao dao = getDaoContext().DaoImpl.SottoTitoloDao)
            {
                return (T)(Titolo)dao.GetAll().FirstOrDefault(s => string.Equals(s.ComCode, code, StringComparison.OrdinalIgnoreCase));
            }
        }

        public T LoadByCode(string code)
        {
            if (typeof(T) == typeof(Titolo))
            {
                using (ITitoloDao dao = getDaoContext().DaoImpl.TitoloDao)
                {
                    return (T)dao.GetAll().SingleOrDefault(t => string.Equals(t.AppCode, code, StringComparison.OrdinalIgnoreCase));
                }
            }
            else if (typeof(T) == typeof(SottoTitolo))
            {
                using (ISottoTitoloDao dao = getDaoContext().DaoImpl.SottoTitoloDao)
                {
                    return (T)(Titolo)dao.GetAll().FirstOrDefault(s => string.Equals(s.ComCode, code, StringComparison.OrdinalIgnoreCase));
                }
            }
            else throw new InvalidOperationException("Tipo non gestito.");
        }
    }
}
