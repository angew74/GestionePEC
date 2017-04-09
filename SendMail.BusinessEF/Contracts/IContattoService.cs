using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model;
using SendMail.Model.RubricaMapping;
using SendMail.Model.Wrappers;
using ActiveUp.Net.Mail.DeltaExt;

namespace SendMail.BusinessEF.Contracts
{
    public interface IContattoService
    {
        RubricaContatti GetContattoById(Int64 idContact, bool withAppMappings);
        IList<RubricaContatti> GetContattiByIdEntita(Int64 identita, bool startFromOrg, bool includeDescendant, bool includeIPA,bool includeAppMappings);
        IList<RubricaContatti> GetContattiByIdEntita(string identita, bool startFromOrg, bool includeDescendant, bool includeIPA);
        IEnumerable<RubricaContatti> GetContattiByTitoloAndBackendCode(string titolo, string backendCode);
        void UpdateRubrContatti(RubricaContatti cont, bool setAppMappings);
        void InsertRubrContatti(RubricaContatti cont, bool setAppMappings);
        void UpdateContattoAppMappings(RubricaContatti cont);
        //IList<RubricaContatti> GetContattiOrgByName(string nomeEntita);

        ResultList<RubricaContatti> GetContattiByParams(List<EntitaType> tEnt, Dictionary<SendMail.Model.FastIndexedAttributes, List<string>> pars, int da, int per, bool withEntita, bool withIPA);
        ResultList<RubricaContatti> GetContattiByParams(EntitaType tEnt, Dictionary<SendMail.Model.FastIndexedAttributes, string> pars, int da, int per, bool withEntita, bool withIPA);
        ResultList<RubricaContatti> GetContattiByParams(List<EntitaType> tEnt, SendMail.Model.FastIndexedAttributes attribute, string pars, int da, int per, bool withEntita, bool withIPA);
        ResultList<RubricaContatti> GetContattiByParams(EntitaType tEnt, SendMail.Model.FastIndexedAttributes attribute, string pars, int da, int per, bool withEntita, bool withIPA);
        
        Dictionary<string, SimpleTreeItem> LoadRubricaIndex(IndexedCatalogs catalog, int? levels);
        Dictionary<string, SimpleTreeItem> LoadRubricaIndex(Int64 startNode, IndexedCatalogs catalog, int? levels);

        //Ricerche
        ResultList<SimpleResultItem> GetFieldsByParams(SendMail.Model.IndexedCatalogs ctg, IList<SendMail.Model.EntitaType> tEnt, KeyValuePair<SendMail.Model.FastIndexedAttributes, string> par, int da, int per);
        ResultList<SimpleResultItem> GetSimilarityFieldsByParams(SendMail.Model.IndexedCatalogs ctg, IList<SendMail.Model.EntitaType> tEnt, KeyValuePair<SendMail.Model.FastIndexedAttributes, string> par, int da, int per);
    }
}
