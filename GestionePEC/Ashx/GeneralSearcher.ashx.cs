using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace GestionePEC.Ashx
{
    /// <summary>
    /// Descrizione di riepilogo per GeneralSearcher
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class GeneralSearcher : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    [Serializable()]
    public class ExtJSGridDataSource<T>
    {
        public virtual string TotalCount { get; set; }

        public virtual String Message { get; set; }

        public virtual List<T> Data { get; set; }
    }
}