using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Business.Contracts;
using Com.Delta.Data;
using SendMail.DataContracts.Interfaces;
using SendMail.Business.Base;
using SendMail.Model;


namespace SendMail.Business
{
    public class RubricaService : BaseSingletonService<RubricaService>, IRubricaService
    {

        public Rubrica GetRubricaItem(long idRub)
        {
            using (IRubricaDao dao = this.getDaoContext().DaoImpl.RubricaDao)
            {
                return dao.GetRubricaItem(idRub);
            }
        }

        //public ICollection<RubricaBean> GetRubricaMapperItems(string[] codici)
        //{
        //    List<RubricaBean> tem = new List<RubricaBean>();

        //    //TODO aggiungere la gestione delle mancate mappature
        //    MailServiceOra ora = new MailServiceOra();
        //    tem = ora.GetRubricaAll(codici);


        //    return tem;

        //}


        //public ICollection<Rubrica> GetRubricaItems(string[] codici)
        //{

        //    //TODO aggiungere la gestione delle mancate mappature
        //    MailServiceOra ora = new MailServiceOra();
        //    List<RubricaBean> tem = ora.GetRubricaAll(codici);
        //    codici = tem.Select(x => x.CodRubrica).ToArray();
        //    ICollection<Rubrica> temp;
        //    using (IRubricaDao dao = this.getDaoContext().DaoImpl.RubricaDao)
        //    {
        //        temp = dao.GetRubricaItems(codici);

        //        codici.Where(x => !(temp.Select<Rubrica, Int64>(r => (Int64)r.IdRubrica))
        //                                .Contains(Int64.Parse(x)))
        //              .ToList()
        //              .ForEach(c => temp.Add(new Rubrica
        //                                    {
        //                                        IdRubrica = 0,
        //                                        Mail = "SCONOSCIUTA",
        //                                        Ragione_Sociale = c
        //                                    }));
        //    }
        //    return temp;
        //}
       
    }
}
