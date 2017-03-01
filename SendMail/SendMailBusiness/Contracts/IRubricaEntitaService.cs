using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model.RubricaMapping;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.Model;
using SendMail.Model.Wrappers;

namespace SendMail.Business.Contracts
{
    public interface IRubricaEntitaService
    {
        //Entità
        ResultList<RubricaEntita> GetEntitaByParams(IList<EntitaType> tEnt, IDictionary<FastIndexedAttributes, IList<string>> pars, int da, int per);
        ResultList<RubricaEntita> GetEntitaByParamsSimilarity(IList<EntitaType> tEnt, IDictionary<FastIndexedAttributes, IList<string>> pars, int da, int per);
        ResultList<RubricaEntita> GetEntitaByMailDomain(IList<EntitaType> tEnt, string mail, int da, int per);
        ResultList<RubricaEntita> GetEntitaIPAByMailDomain(string mail, int da, int per);
        List<RubricaEntita> GetEntitaIPAbyIPAid(string IPAid);
        List<RubricaEntita> GetEntitaByPartitaIVA(string partitaIVA);

        RubricaEntita GetEntitaById(long idEntita);
        RubricaEntita GetRubricaEntitaCompleteById(Int64 idEntita);
        List<RubricaEntita> GetEntitaByName(IList<EntitaType> tEnt, string name);

        IList<SimpleTreeItem> GetRubricaEntitaTree(Int64 idEntita);
        IList<SimpleTreeItem> GetRubricaEntitaTreeByIdPadre(Int64 idPadre);
        void Update(RubricaEntita r);
        void Insert(RubricaEntita r);
    }
}
