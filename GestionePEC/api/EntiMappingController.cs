using SendMail.BusinessEF;
using SendMail.Model;
using SendMail.Model.ContactApplicationMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;

namespace GestionePEC.api
{

    [DataContract(Namespace = "http://Delta.cdr.mailservice/entimappings")]
    public class Titolo
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Nome { get; set; }

        public static explicit operator Titolo(SendMail.Model.Titolo t)
        {
            Titolo titolo = new Titolo();
            titolo.Id = (int)t.Id;
            titolo.Code = t.AppCode;
            titolo.Nome = t.Nome;
            return titolo;
        }
    }

    [DataContract(Namespace = "http://Delta.cdr.mailservice/entimappings")]
    public class Contatto
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "mail")]
        public string Mail { get; set; }
    }

    [DataContract(Namespace = "http://Delta.cdr.mailservice/entimappings")]
    public class Backend
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string BackendCode { get; set; }
    }

    [DataContract(Namespace = "http://Delta.cdr.mailservice/entimappings")]
    public class Mapping
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public Titolo Titolo { get; set; }
        [DataMember]
        public Contatto Contatto { get; set; }
    }

    [DataContract(Namespace = "http://Delta.cdr.mailservice/entimappings")]
    public class EnteMapping
    {
        [DataMember]
        public Backend Backend { get; set; }
        [DataMember]
        public List<Mapping> Mapping { get; set; }
    }

    [DataContract(Namespace = "http://Delta.cdr.mailservice/entimappings")]
    public class EntiMappings
    {
        [DataMember]
        public int IdCanale { get; set; }
        [DataMember]
        public List<EnteMapping> EnteMapping { get; set; }

        public static explicit operator EntiMappings(ContactsBackendMap map)
        {
            if (map == null) return null;
            EntiMappings e = new EntiMappings();
            e.IdCanale = (int)map.Canale;
            if (map.Id >= 0)
            {
                e.EnteMapping = new List<EnteMapping>();
                EnteMapping em = new EnteMapping();
                e.EnteMapping.Add(em);
                if (map.Backend != null)
                {
                    em.Backend = new Backend
                    {
                        Id = (int)map.Backend.Id,
                        BackendCode = map.Backend.Codice
                    };
                    if (string.IsNullOrEmpty(map.Backend.DescrizionePlus))
                        em.Backend.Name = map.Backend.Descrizione;
                    else
                        em.Backend.Name = string.Format("{0} ({1})", map.Backend.Descrizione, map.Backend.DescrizionePlus);
                };
                em.Mapping = new List<Mapping>();
                Mapping m = new Mapping { Id = map.Id };
                em.Mapping.Add(m);
                if (map.Titolo != null)
                    m.Titolo = new Titolo
                    {
                        Id = (int)map.Titolo.Id,
                        Nome = map.Titolo.Nome
                    };
                if (map.Contatto != null)
                    m.Contatto = new Contatto
                    {
                        Id = (int)map.Contatto.IdContact.Value,
                        Mail = map.Contatto.Mail
                    };
            }
            return e;
        }

        public static explicit operator ContactsBackendMap(EntiMappings em)
        {
            if (em == null) return null;
            ContactsBackendMap map = new ContactsBackendMap();
            if (em.IdCanale >= 0)
                map.Canale = (SendMail.Model.TipoCanale)em.IdCanale;
            if (em.EnteMapping != null && em.EnteMapping.Count > 0)
            {
                var enteMap = em.EnteMapping.First();
                if (enteMap.Backend != null)
                    map.Backend = new BackEndRefCode
                    {
                        Id = em.EnteMapping.First().Backend.Id
                    };
                if (enteMap.Mapping != null && enteMap.Mapping.Count > 0)
                {
                    var m = enteMap.Mapping.First();
                    if (m.Contatto != null)
                        map.Contatto = new SendMail.Model.RubricaMapping.RubricaContatti
                        {
                            IdContact = m.Contatto.Id
                        };
                    if (m.Titolo != null)
                        map.Titolo = new SendMail.Model.Titolo
                        {
                            Id = m.Titolo.Id
                        };
                    map.Id = m.Id;
                }
            }
            return map;
        }
    }

    [DataContract(Namespace = "http://Delta.cdr.mailservice/entimappings")]
    public class Response<T>
    {
        [DataMember(Name = "totalCount")]
        public int Totale { get; set; }
        [DataMember(Name = "message")]
        public string Message { get; set; }
        [DataMember(Name = "data")]
        public List<T> Data { get; set; }
    }

    public class EntiMappingController : ApiController
    {

        ContactsBackendService s = new ContactsBackendService();

       

        [Authorize]
        [Route("api/EntiMappingController/GetEntiMappings")]
        public HttpResponseMessage GetEntiMappings(string id)
        {
            EntiMappings model = new EntiMappings();
            int idMap = -1;
            if (!int.TryParse(id, out idMap))
            { model = new api.EntiMappings();}
            else
            {model = (EntiMappings)s.FindByIdMap(idMap);}
            return this.Request.CreateResponse<EntiMappings>(HttpStatusCode.OK, model);
        }

        [Authorize]
        [Route("api/EntiMappingController/GetAllEntiMappings")]
        public HttpResponseMessage  GetAllEntiMappings()
        {
            List<EntiMappings> l = new List<api.EntiMappings>();
            var collContacts = s.FindAll();
           l= MapToEntiMappings(collContacts);
            return this.Request.CreateResponse<List<EntiMappings>>(HttpStatusCode.OK, l);
        }

        [Authorize]
        [Route("api/EntiMappingController/GetMappingsByEntita")]

        public HttpResponseMessage GetMappingsByEntita(string idEntita)
        {
            List<EntiMappings> l = new List<api.EntiMappings>();
            int idEnt = -1;
            if (!int.TryParse(idEntita, out idEnt))
                return null;

            var collContacts = s.FindPerEntita(idEnt);
            l= MapToEntiMappings(collContacts);
            return this.Request.CreateResponse<List<EntiMappings>>(HttpStatusCode.OK, l);
        }

        [Authorize]
        [Route("api/EntiMappingController/GetMappingsByTitoloEntita")]
        public HttpResponseMessage GetMappingsByTitoloEntita(string idTitolo, string idEntita)
        {
            List<EntiMappings> l = new List<api.EntiMappings>();
            int idTit, idEnt;
            idTit = -1;
            idEnt = -1;
            if (!int.TryParse(idTitolo, out idTit) ||
                !int.TryParse(idEntita, out idEnt))
                return null;

            var collContacts = s.FindPerTitoloEntita(idTit, idEnt);
            l= MapToEntiMappings(collContacts);
            return this.Request.CreateResponse<List<EntiMappings>>(HttpStatusCode.OK, l);
        }

        [Authorize]
        [Route("api/EntiMappingController/GetMappingsByCanaleTitoloEntita")]
        public HttpResponseMessage GetMappingsByCanaleTitoloEntita(string idCanale, string idTitolo, string idEntita)
        {
            List<EntiMappings> l = new List<api.EntiMappings>();
            int idCan, idTit, idEnt;
            idCan = idTit = idEnt = -1;
            if (!int.TryParse(idCanale, out idCan) ||
                !int.TryParse(idTitolo, out idTit) ||
                !int.TryParse(idEntita, out idEnt))
                return null;
            var collContacts = s.FindPerCanaleTitoloEntita(idCan, idTit, idEnt);
            l= MapToEntiMappings(collContacts);
            return this.Request.CreateResponse<List<EntiMappings>>(HttpStatusCode.OK, l);
        }

        [Authorize]
        [Route("api/EntiMappingController/GetContattiEntita")]
        public HttpResponseMessage GetContattiEntita(string idEntita, int start, int limit)
        {
            Response<Contatto> contatto = new api.Response<Contatto>();
            long idEnt = -1;
            if (start == 0) start++;
            if (!long.TryParse(idEntita, out idEnt))
                return null;
            ContattoService cs = new ContattoService();
            var cont = cs.GetContattiByIdEntita(idEnt, true, true, false, false);
            contatto = new Response<Contatto>
            {
                Totale = cont.Count,
                Data = cont.Select(rc =>
                {
                    Contatto c = new Contatto
                    {
                        Id = -1,
                        Mail = rc.Mail
                    };
                    if (rc.IdContact.HasValue)
                        c.Id = (int)rc.IdContact.Value;
                    return c;
                }).Skip(start - 1).Take(limit).ToList()
            };
            return this.Request.CreateResponse<Response<Contatto>>(HttpStatusCode.OK, contatto);

        }

        [Authorize]
        [Route("api/EntiMappingController/GetAllTitoli")]
        public HttpResponseMessage GetAllTitoli()
        {
            List<Titolo> risp = new List<Titolo>();
            TitolarioService<SendMail.Model.Titolo> titoloservice = new TitolarioService<SendMail.Model.Titolo>();         
            IList<SendMail.Model.Titolo> titoli = titoloservice.GetAll(null);
            risp = null;
            if (titoli != null)
            {
                risp = (from t in titoli
                        select (Titolo)t).ToList();
            }
            return this.Request.CreateResponse<List<Titolo>>(HttpStatusCode.OK, risp);         
        }

        [Authorize]
        [Route("api/EntiMappingController/CreateMapping")]
        public HttpResponseMessage CreateMapping(EntiMappings mapping)
        {
            Response<EntiMappings> resp = new Response<EntiMappings>();
            try
            {
                ContactsBackendMap bMap = (ContactsBackendMap)mapping;
                s.CreateMapping(bMap);
                mapping.EnteMapping.ElementAt(0).Mapping.ElementAt(0).Id = bMap.Id;
                List<EntiMappings> list = new List<EntiMappings>();
                list.Add(mapping);
                resp.Totale = 1;
                resp.Message = "Operazione effettuata";
                resp.Data = list;
            }
            catch
            {
                resp.Totale = 0;
                resp.Message = "Errore nell'inserimento del contatto";
            }
            return this.Request.CreateResponse<Response<EntiMappings>>(HttpStatusCode.OK, resp);
        }

        [Authorize]
        [Route("api/EntiMappingController/PutMapping")]
        public HttpResponseMessage PutMapping(EntiMappings mapping)
        {
            Response<EntiMappings> response = new Response<EntiMappings>();
            try
            {
                ContactsBackendMap bMap = (ContactsBackendMap)mapping;
                s.UpdateMapping(bMap);
                List<EntiMappings> list = new List<EntiMappings>();
                list.Add(mapping);
                response.Totale = 1;
                response.Message = "Operazione effettuata";
                response.Data = list;
            }
            catch
            {
                response.Totale = 0;
                response.Message = "Errore nell'aggiornamento del contatto";
            }
            return this.Request.CreateResponse<Response<EntiMappings>>(HttpStatusCode.OK, response);
        }

        [Authorize]
        [Route("api/EntiMappingController/DeleteMapping")]
        public void DeleteMapping(string id)
        {
            int idMap = -1;
            if (!int.TryParse(id, out idMap))
                return;
            s.DeleteMapping(idMap);
        }
       

        #region Private methods
        private List<EntiMappings> MapToEntiMappings(ICollection<ContactsBackendMap> collContacts)
        {
            List<EntiMappings> risp = null;
            if (collContacts != null)
            {
                risp = (from c in collContacts
                        group c by c.Canale into grCanale
                        select new EntiMappings
                        {
                            IdCanale = (int)grCanale.Key,
                            EnteMapping = (from g in grCanale
                                           group g by g.Backend.Codice into grBackend
                                           select new EnteMapping
                                           {
                                               Backend = grBackend.Where(x => x.Backend.Codice == grBackend.Key)
                                               .Select(x =>
                                               {
                                                   Backend b = new Backend
                                                   {
                                                       Id = (int)x.Backend.Id,
                                                       BackendCode = x.Backend.Codice
                                                   };
                                                   if (String.IsNullOrEmpty(x.Backend.DescrizionePlus))
                                                       b.Name = x.Backend.Descrizione;
                                                   else
                                                       b.Name = String.Format("{0} ({1})", x.Backend.Descrizione, x.Backend.DescrizionePlus);
                                                   return b;
                                               }).FirstOrDefault(),
                                               Mapping = (from m in grBackend
                                                          select new Mapping
                                                          {
                                                              Id = m.Id,
                                                              Contatto = (m.Contatto == null) ? null : new Contatto
                                                              {
                                                                  Id = (int)m.Contatto.IdContact.Value,
                                                                  Mail = m.Contatto.Mail
                                                              },
                                                              Titolo = new Titolo
                                                              {
                                                                  Id = (int)m.Titolo.Id,
                                                                  Code = m.Titolo.CodiceProtocollo,
                                                                  Nome = m.Titolo.Nome
                                                              }
                                                          }).ToList()
                                           }).ToList()
                        }).ToList();

            }

            return risp;
        }
        #endregion
    }
}
