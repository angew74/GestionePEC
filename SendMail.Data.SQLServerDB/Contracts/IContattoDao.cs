using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Data.SQLServerDB;
using SendMail.Model.RubricaMapping;
using SendMail.Model.Wrappers;
using ActiveUp.Net.Mail.DeltaExt;

namespace SendMail.Data.SQLServerDB
{
    public interface IContattoDao : IDao<RubricaContatti, Int64>
    {
        List<RubricaContatti> LoadContattiOrgByOrgId(long identita, bool startFromOrg, bool includeDescendant, bool includeIPA, bool includeAppMappings);
        IList<RubricaContatti> LoadContattiOrgByName(string identita, bool startFromOrg, bool includeDescendant, bool includeIPA);
        IEnumerable<RubricaContatti> LoadContattiByTitoloAndBackendCode(string titolo, string backendCode);

        ResultList<RubricaContatti> LoadContattiByParams(List<SendMail.Model.EntitaType> tEnt, Dictionary<SendMail.Model.FastIndexedAttributes, List<string>> pars, int da, int per, bool withEntita);
        ResultList<RubricaContatti> LoadContattiIPAByParams(List<SendMail.Model.EntitaType> tEnt, Dictionary<SendMail.Model.FastIndexedAttributes, List<string>> pars, int da, int per, bool withEntita);

        ResultList<SimpleResultItem> LoadFieldsByParams(SendMail.Model.IndexedCatalogs ctg, IList<SendMail.Model.EntitaType> tEnt, KeyValuePair<SendMail.Model.FastIndexedAttributes, string> par, int da, int per);
        ResultList<SimpleResultItem> LoadSimilarityFieldsByParams(SendMail.Model.IndexedCatalogs ctg, IList<SendMail.Model.EntitaType> tEnt, KeyValuePair<SendMail.Model.FastIndexedAttributes, string> par, int da, int per);
    }
}
