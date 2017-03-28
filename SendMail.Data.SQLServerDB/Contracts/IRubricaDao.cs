using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SendMail.Model;
using SendMail.Model.RubricaMapping;
using SendMail.Model.Wrappers;

namespace SendMail.Data.SQLServerDB
{
    public interface IRubricaDao : IDao<Rubrica, Int64>
    {
       Rubrica GetRubricaItem(Int64 IdRub);
       ICollection<Rubrica> GetRubricaItems(string[] codici);
       Dictionary<string,SimpleTreeItem> LoadTree(Int64? startNode, SendMail.Model.IndexedCatalogs catalog, int? levels);
    }
}
