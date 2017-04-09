using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model;

namespace SendMail.BusinessEFEF.Contracts
{
  public interface IRubricaService
  {
      Rubrica GetRubricaItem(Int64 idRub);
     // ICollection<Rubrica> GetRubricaItems(string[] codici);
    //  ICollection<RubricaBean> GetRubricaMapperItems(string[] codice);
     
  }
}
