using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model;

namespace SendMail.Business
{
    public interface ITitolarioService<T> where T : Titolo
    {
        IList<T> GetAll(decimal? idPadre);
        T LoadTitoloById(decimal id);
        T updateTitolo(T e);
        T insertTitolo(T e);
        void deleteTitolo(decimal id, decimal? idPadre);
        IList<T> FindByTitolo(decimal tkey);
        T LoadByCode(string code);
        T LoadByComCode(string code);
    }
}
