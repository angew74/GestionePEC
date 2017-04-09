﻿using System;
using System.Collections.Generic;
using System.Linq;
using SendMail.BusinessEF.Base;
using SendMail.BusinessEF.Contracts;
using SendMail.Model;
using SendMail.Model.RubricaMapping;
using SendMail.Model.Wrappers;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.Data.SQLServerDB;
using SendMail.Data.SQLServerDB.Repository;

namespace SendMail.BusinessEF
{
    public class ContattoService : BaseSingletonService<ContattoService>, IContattoService
    {

        #region IContattoService Membri di

        public RubricaContatti GetContattoById(long idContact, bool withMappings)
        {
            using (ContattoSQLDb dao = new ContattoSQLDb())
            {
                return dao.GetById(idContact);
            }
        }

        public IList<RubricaContatti> GetContattiByIdEntita(long identita, bool startFromOrg, bool includeDescendant, bool includeIPA, bool includeAppMappings)
        {
            //TODO:aggiungere le mappature
            using (ContattoSQLDb dao = new ContattoSQLDb())
            {
                return dao.LoadContattiOrgByOrgId(identita, startFromOrg, includeDescendant, includeIPA, includeAppMappings);
            }
        }

        public IList<RubricaContatti> GetContattiByIdEntita(string identita, bool startFromOrg, bool includeDescendant, bool includeIPA)
        {
            using (ContattoSQLDb dao = new ContattoSQLDb())
            {
                return dao.LoadContattiOrgByName(identita, startFromOrg, includeDescendant, includeIPA);
            }
        }

        public IEnumerable<RubricaContatti> GetContattiByTitoloAndBackendCode(string titolo, string backendCode)
        {
            using (ContattoSQLDb dao = new ContattoSQLDb())
            {
                return dao.LoadContattiByTitoloAndBackendCode(titolo, backendCode);
            }
        }

        //TODO:inserire il parametro applicazione invece del booleano
        public void UpdateRubrContatti(RubricaContatti cont, bool setAppMappings)
        {
            using (ContattoSQLDb dao = new ContattoSQLDb())
            {
                dao.Update(cont);
                if (setAppMappings)
                    UpdateContattoAppMappings(cont);
            }
        }

        public void InsertRubrContatti(RubricaContatti cont, bool setAppMappings)
        {
            using (ContattoSQLDb dao = new ContattoSQLDb())
            {
                dao.Insert(cont);
                if (setAppMappings)
                    UpdateContattoAppMappings(cont);
            }
        }

        public void UpdateContattoAppMappings(RubricaContatti cont)
        {
            using (ContactsApplicationMappingSQLDb dao = new ContactsApplicationMappingSQLDb())
            {
                long refOrg = default(long);
                if (cont.Entita == null)
                    refOrg = cont.RefIdReferral ?? default(long);
                else
                {
                    refOrg = cont.Entita.RefOrg ?? cont.Entita.IdReferral.Value;
                }
                dao.setDefaultContact(cont.T_MappedAppID, refOrg, cont.IdContact.Value);
            }
        }

        //confluito in getbyparams
        //public IList<RubricaContatti> GetContattiOrgByName(string nomeEntita)
        //{
        //    throw new NotImplementedException();
        //}

        public ResultList<RubricaContatti> GetContattiByParams(List<EntitaType> tEnt, Dictionary<SendMail.Model.FastIndexedAttributes, List<string>> pars, int da, int per, bool withEntita, bool withIPA)
        {
            ResultList<RubricaContatti> res = null;
            using (ContattoSQLDb dao = new ContattoSQLDb())
            {
                res = dao.LoadContattiByParams(tEnt, pars, da, per, withEntita);
                if (withIPA)
                {
                    ResultList<RubricaContatti> resIPA = dao.LoadContattiIPAByParams(tEnt, pars, da, per, withEntita);
                    if (resIPA != null)
                    {
                        if (res == null) res = new ResultList<RubricaContatti>();
                        res.Totale += resIPA.Totale;
                        res.Per = (per > res.Totale) ? res.Totale : per;
                    }
                }
            }
            return res;
        }

        public ResultList<RubricaContatti> GetContattiByParams(EntitaType tEnt, Dictionary<SendMail.Model.FastIndexedAttributes, string> pars, int da, int per, bool withEntita, bool withIPA)
        {
            List<EntitaType> l = new List<EntitaType>();
            l.Add(tEnt);
            Dictionary<FastIndexedAttributes, List<string>> d = new Dictionary<FastIndexedAttributes, List<string>>();
            foreach (KeyValuePair<SendMail.Model.FastIndexedAttributes, string> v in pars)
            {
                List<string> ltemp = new List<string>();
                ltemp.Add(v.Value);
                d.Add(v.Key, ltemp);
            }
            return this.GetContattiByParams(l, d, da, per, withEntita, withIPA);
        }

        public ResultList<RubricaContatti> GetContattiByParams(List<EntitaType> tEnt, SendMail.Model.FastIndexedAttributes attribute, string pars, int da, int per, bool withEntita, bool withIPA)
        {
            Dictionary<FastIndexedAttributes, List<string>> d = new Dictionary<FastIndexedAttributes, List<string>>();
            List<string> ltemp = new List<string>();
            ltemp.Add(pars);
            d.Add(attribute, ltemp);
            return this.GetContattiByParams(tEnt, d, da, per, withEntita, withIPA);
        }

        public ResultList<RubricaContatti> GetContattiByParams(EntitaType tEnt, SendMail.Model.FastIndexedAttributes attribute, string pars, int da, int per, bool withEntita, bool withIPA)
        {
            List<EntitaType> l = new List<EntitaType>();
            l.Add(tEnt);
            Dictionary<FastIndexedAttributes, List<string>> d = new Dictionary<FastIndexedAttributes, List<string>>();
            List<string> ltemp = new List<string>();
            ltemp.Add(pars);
            d.Add(attribute, ltemp);
            return this.GetContattiByParams(l, d, da, per, withEntita, withIPA);
        }

        public ResultList<SimpleResultItem> GetFieldsByParams(SendMail.Model.IndexedCatalogs ctg, IList<SendMail.Model.EntitaType> tEnt, KeyValuePair<SendMail.Model.FastIndexedAttributes, string> par, int da, int per)
        {
            using (ContattoSQLDb dao = new ContattoSQLDb())
            {
                return dao.LoadFieldsByParams(ctg, tEnt, par, da, per);
            }
        }

        public ResultList<SimpleResultItem> GetSimilarityFieldsByParams(SendMail.Model.IndexedCatalogs ctg, IList<SendMail.Model.EntitaType> tEnt, KeyValuePair<SendMail.Model.FastIndexedAttributes, string> par, int da, int per)
        {
            using (ContattoSQLDb dao = new ContattoSQLDb())
            {
                return dao.LoadSimilarityFieldsByParams(ctg, tEnt, par, da, per);
            }
        }

        public Dictionary<string, SimpleTreeItem> LoadRubricaIndex(IndexedCatalogs catalog, int? levels)
        {
            Dictionary<string, SimpleTreeItem> list = new Dictionary<string, SimpleTreeItem>();
            using (RubricaSQLDb dao = new RubricaSQLDb())
            {
                switch (catalog)
                {
                    case IndexedCatalogs.ALL:
                        list = list.Concat(dao.LoadTree(null, IndexedCatalogs.RUBR, levels)).ToDictionary(e => e.Key, e => e.Value);
                       // list = list.Concat(dao.LoadTree(null, IndexedCatalogs.IPA, levels)).ToDictionary(e => e.Key, e => e.Value);
                        break;
                    default:
                        list = dao.LoadTree(null, catalog, levels);
                        break;
                }
            }
            return list;
        }

        public Dictionary<string, SimpleTreeItem> LoadRubricaIndex(Int64 startNode, IndexedCatalogs catalog, int? levels)
        {
            Dictionary<string, SimpleTreeItem> list = new Dictionary<string, SimpleTreeItem>();
            using (RubricaSQLDb dao = new RubricaSQLDb())
            {
                switch (catalog)
                {
                    case IndexedCatalogs.ALL:
                        throw new Exception("il parametro ALL non è supportato,la ricerca potrebbe essere inconsistente");
                    default:
                        list = dao.LoadTree(startNode, catalog, levels);
                        break;
                }
            }
            return list;
        }

        #endregion
    }
}
