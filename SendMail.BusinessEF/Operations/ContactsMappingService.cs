using System;
using System.Collections.Generic;
using System.Linq;

using SendMail.BusinessEF.Base;
using SendMail.BusinessEF.Contracts;
using SendMail.Model;
using Com.Delta.Data.QueryModel;
using System.Reflection;
using SendMail.Data.SQLServerDB.Repository;

namespace SendMail.BusinessEF
{
    public class ContactsMappingService : BaseSingletonService<ContactsMappingService>, IContactsMappingService
    {
        #region IContactsMappingService Membri di
        public ICollection<ContactsApplicationMapping> FindByDatiGenerici(ContactsApplicationMapping contact)
        {
            List<ContactsApplicationMapping> listContacts = null;
            using (ContactsApplicationMappingSQLDb dao = new ContactsApplicationMappingSQLDb())
            {
                listContacts = (List<ContactsApplicationMapping>)dao.GetContactsByCriteria(BuildQueryByData(contact));
            }
            return listContacts;
        }

        public ICollection<ContactsApplicationMapping> FindByDatiGenerici(ICollection<ContactsApplicationMapping> contacts)
        {
            List<ContactsApplicationMapping> listContacts = new List<ContactsApplicationMapping>();
            using (ContactsApplicationMappingSQLDb dao = new ContactsApplicationMappingSQLDb())
            {
                foreach (ContactsApplicationMapping m in contacts)
                {
                    ICollection<ContactsApplicationMapping> cm = dao.GetContactsByCriteria(BuildQueryByData(m));
                    if (cm != null)
                        listContacts.AddRange(cm);
                }
            }
            if (listContacts.Count == 0)
                listContacts = null;
            return listContacts;
        }

        public ICollection<ContactsApplicationMapping> FindByBackendCodeAndCodComunicazione(IEnumerable<string> codes, string codCom)
        {
            using (ContactsApplicationMappingSQLDb dao = new ContactsApplicationMappingSQLDb())
            {
                return dao.FindByBackendCodeAndCodComunicazione(codes, codCom);
            }
        }

        public void SetContattoAsDefault(long idTitolo, long idEntita, long idContatto)
        {
            using (ContactsApplicationMappingSQLDb dao = new ContactsApplicationMappingSQLDb())
            {
                dao.setDefaultContact(idTitolo, idEntita, idContatto);
            }
        }

        public ICollection<ContactsApplicationMapping> FindByIdContact(long idContact)
        {
            using (ContactsApplicationMappingSQLDb dao = new ContactsApplicationMappingSQLDb())
            {
                return dao.GetContactsByIdContact(idContact);
            }
        }
        #endregion

        #region Private methods
        private QueryCmp BuildQueryByData(ContactsApplicationMapping contact)
        {
            QueryCmp q = new QueryCmp();
            //prendo tutte le properties mappate su database
            IEnumerable<PropertyInfo> pi = contact.GetType()
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
                .Where(x => (x.GetCustomAttributes(typeof(DatabaseFieldAttribute), true) != null
                    && x.GetCustomAttributes(typeof(DatabaseFieldAttribute), true).Length > 0));

            if (pi.All(x =>
            {
                Type pt = x.PropertyType;
                if (pt.IsValueType)
                {
                    return x.GetValue(contact, null).Equals(Activator.CreateInstance(pt));
                }
                else return x.GetValue(contact, null) == null;
            }))
            {
                return q;
            }

            var validProps = pi.Where(x =>
            {
                Type pt = x.PropertyType;
                if (pt.IsValueType)
                    return !x.GetValue(contact, null).Equals(Activator.CreateInstance(pt));
                else return x.GetValue(contact, null) != null;
            });

            CombinedClause cc = new CombinedClause();
            q.WhereClause = cc;
            List<IClause> lc = new List<IClause>();
            cc.SubClauses = lc;

            if (validProps.Count() == 1)
            {
                cc.ClauseType = QueryOperator.Or;
                lc.Add(BuildSingleClause(contact, validProps.Single()));
                lc.Add(BuildNullClause(validProps.Single()));
            }
            else
            {
                cc.ClauseType = QueryOperator.And;
                foreach (var p in validProps)
                {
                    CombinedClause cc0 = new CombinedClause();
                    cc0.ClauseType = QueryOperator.Or;
                    List<IClause> lc0 = new List<IClause>();
                    cc0.SubClauses = lc0;
                    lc0.Add(BuildSingleClause(contact, p));
                    lc0.Add(BuildNullClause(p));
                    lc.Add(cc0);
                }
            }

            return q;
        }

        private SingleClause BuildSingleClause(ContactsApplicationMapping c, PropertyInfo pi)
        {
            SingleClause sc = new SingleClause();

            object[] pa = pi.GetCustomAttributes(typeof(DatabaseFieldAttribute), true);
            if (pa != null && pa.Length == 1)
            {
                DatabaseFieldAttribute dba = (DatabaseFieldAttribute)(pa[0]);
                if (dba != null && dba.FieldName.Length > 0)
                {
                    sc.PropertyName = dba.FieldName;
                }
            }

            sc.Value = pi.GetValue(c, null);

            switch (sc.PropertyName)
            {
                case "APP_CODE":
                case "MAIL":
                case "FAX":
                case "TELEFONO":
                case "CODICE":
                case "BACKEND_CODE":
                case "CATEGORY":
                case "COM_CODE":
                case "SOTTOTITOLO_ACTIVE":
                case "TITOLO_ACTIVE":
                    sc.Operator = CriteriaOperator.Equal;
                    break;

                case "CONTACT_REF":
                    sc.Operator = CriteriaOperator.StartsWith;
                    break;

                case "BACKEND_DESCR":
                case "DESCR_PLUS":
                    sc.Operator = CriteriaOperator.Like;
                    break;
            }

            return sc;
        }

        private SingleClause BuildSingleClause(KeyValuePair<string, string> kv)
        {
            SingleClause sc = new SingleClause();
            sc.PropertyName = kv.Key;
            sc.Value = kv.Value;

            switch (kv.Key)
            {
                case "appCode":
                case "mail":
                case "fax":
                case "telefono":
                case "codiceCanale":
                case "codiceBackend":
                case "categoria":
                    sc.Operator = CriteriaOperator.Equal;
                    break;

                case "contactRef":
                    sc.Operator = CriteriaOperator.StartsWith;
                    break;

                case "descrBackend":
                case "descrBackendPlus":
                    sc.Operator = CriteriaOperator.Like;
                    break;
            }
            return sc;
        }

        private SingleClause BuildNullClause(PropertyInfo pi)
        {
            SingleClause sc = new SingleClause();

            object[] pa = pi.GetCustomAttributes(typeof(DatabaseFieldAttribute), true);
            if (pa != null && pa.Length == 1)
            {
                DatabaseFieldAttribute dba = (DatabaseFieldAttribute)(pa[0]);
                if (dba != null && dba.FieldName.Length > 0)
                {
                    sc.PropertyName = dba.FieldName;
                }
            }
            sc.Operator = CriteriaOperator.IsNull;
            return sc;
        }

        private SingleClause BuildNullClause(KeyValuePair<string, string> kv)
        {
            SingleClause sc = new SingleClause();
            sc.PropertyName = kv.Key;
            sc.Operator = CriteriaOperator.IsNull;
            return sc;
        }

        private SingleClause NuildNotNullClause(PropertyInfo pi)
        {
            SingleClause sc = new SingleClause();

            object[] pa = pi.GetCustomAttributes(typeof(DatabaseFieldAttribute), true);
            if (pa != null && pa.Length == 1)
            {
                DatabaseFieldAttribute dba = (DatabaseFieldAttribute)(pa[0]);
                if (dba != null && dba.FieldName.Length > 0)
                {
                    sc.PropertyName = dba.FieldName;
                }
            }
            sc.Operator = CriteriaOperator.IsNotNull;
            return sc;
        }

        private SingleClause NuildNotNullClause(KeyValuePair<string, string> kv)
        {
            SingleClause sc = new SingleClause();
            sc.PropertyName = kv.Key;
            sc.Operator = CriteriaOperator.IsNotNull;
            return sc;
        }
        #endregion  
    }
}
