using ActiveUp.Net.Mail.DeltaExt;
using SendMail.BusinessEF;
using SendMail.Model;
using SendMail.Model.WebserviceMappings;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GestionePEC.api
{

    public class ResponseType<T>
    {
        
        public int totale { get; set; }       
        public string message { get; set; }        
        public string success { get; set; }     
        public List<T> Items { get; set; }
    }

    public class GeneralSearchController : ApiController
    {
        [Authorize]
        [Route("api/GeneralSearchController/GetEntitaByPartialName")]
        public HttpResponseMessage GetEntitaByPartialName(string text, string type, int start, int limit)
        {
            ResponseType<Item> response = new ResponseType<Item>();
            if (String.IsNullOrEmpty(text) || string.IsNullOrEmpty(type))
            { this.Request.CreateResponse<ResponseType<Item>>(HttpStatusCode.OK, response); }
            start = (start == 0) ? ++start : start;           
            IList<EntitaType> filtro = new List<EntitaType>();
            KeyValuePair<FastIndexedAttributes, string> matchingString = new KeyValuePair<FastIndexedAttributes, string>();
            IndexedCatalogs catalogo;
            try
            {
                List<string> types = type.Split(':').ToList();
                string tipoRicerca = types.First();
                catalogo = (IndexedCatalogs)Enum.Parse(typeof(IndexedCatalogs), types.Last());

                var f = from t in types
                        where types.IndexOf(t) != 0 &&
                        types.IndexOf(t) != (types.Count - 1)
                        select (EntitaType)Enum.Parse(typeof(EntitaType), t);
                if (f.Count() != 0)
                    filtro = f.ToList();
                matchingString = new KeyValuePair<FastIndexedAttributes, string>(
                    (FastIndexedAttributes)Enum.Parse(typeof(FastIndexedAttributes),
                    tipoRicerca), text);
            }
            catch
            {
                response.message = "Errore: la stringa di ricerca è mal formata";
                response.totale = 0;
                response.Items = null;
                return this.Request.CreateResponse<ResponseType<Item>>(HttpStatusCode.OK, response);
            }

            ResultList<SimpleResultItem> res = null;
            try
            {
                ContattoService service = new ContattoService();
                res = service.GetFieldsByParams(catalogo, filtro, matchingString, start, limit);
                if (res == null || res.List.Count == 0)
                {
                    response.message = "la ricerca non ha prodotto risultati";
                    response.totale = 0;
                    response.Items = null;
                    return this.Request.CreateResponse<ResponseType<Item>>(HttpStatusCode.OK, response);
                }
                response.totale = res.Totale;
                response.Items = (from i in res.List
                                 select new SendMail.Model.WebserviceMappings.Item
                                 {
                                     Id = string.Format("{0}#{1}", i.Value, (res.List as List<SimpleResultItem>).IndexOf(i)),
                                     Title = i.Text,
                                     Text = i.Description,
                                     Subtype = i.SubType
                                 }).ToList();
                response.success = bool.TrueString;
                response.message = "";
            }
            catch (Exception e)
            {
                response.message = "Errore: " + e.Message;
            }
            return this.Request.CreateResponse<ResponseType<Item>>(HttpStatusCode.OK, response);
        }

    }
}