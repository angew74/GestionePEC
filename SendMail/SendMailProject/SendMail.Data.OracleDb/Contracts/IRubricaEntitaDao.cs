using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model.RubricaMapping;
using SendMail.DataContracts.Interfaces;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.Model.Wrappers;
using SendMail.Model;

namespace SendMail.DataContracts.Interfaces
{
    public interface IRubricaEntitaDao:IDao<RubricaEntita, Int64>
    {
        ResultList<RubricaEntita> LoadEntitaByParams(IList<SendMail.Model.EntitaType> tEnt, IDictionary<SendMail.Model.FastIndexedAttributes, IList<string>> pars, int da, int per);
        ResultList<RubricaEntita> LoadSimilarityEntitaByParams(IList<SendMail.Model.EntitaType> tEnt, IDictionary<SendMail.Model.FastIndexedAttributes, IList<string>> pars, int da, int per);
        RubricaEntita LoadEntitaCompleteById(long idEntita);

        List<RubricaEntita> LoadEntitaByName(IList<EntitaType> tEnt, string name);

        IList<SimpleTreeItem> LoadRubricaEntitaTree(Int64 idEntita);
        IList<SimpleTreeItem> LoadRubricaEntitaTreeByIdPadre(Int64 idPadre);
        ResultList<RubricaEntita> LoadEntitaByMailDomain(IList<EntitaType> tEnt, string mail, int da, int per);
        ResultList<RubricaEntita> LoadEntitaIPAByMailDomain(string mail, int da, int per);
        List<RubricaEntita> LoadEntitaIPAbyIPAid(string IPAid);
        List<RubricaEntita> LoadEntitaByPartitaIVA(string partitaIVA);
    }
}
