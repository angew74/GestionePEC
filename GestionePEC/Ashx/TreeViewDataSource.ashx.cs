using SendMail.Locator;
using SendMail.Model;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;

namespace GestionePEC.Ashx
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class TreeViewDataSource : IHttpHandler
    {
        private const string JSON_ELEMENT = ",{{\"text\":\"{0}\",\"itemId\":\"{1}\",cls:\"{2}\"}}";
        private const string JSON_ELEMENT_FIRST = "{{\"text\":\"{0}\",\"itemId\":\"{1}\",cls:\"{2}\"}}";

        public void ProcessRequest(HttpContext context)
        {

            //  string nodeRaw = context.Request.Form["idNode"]; // IPA@123 RUBR@123 [1]IPA@123  [3]RUBR@435653
            string nodeRaw = context.Request.Params["node"];
            string node = nodeRaw;
            string catalog = null;
            string virtualPager = null;
            bool virtualNode = false;
            if (nodeRaw.Contains(']'))
            {
                node = nodeRaw.Substring(nodeRaw.LastIndexOf(']') + 1);
                virtualPager = nodeRaw.Substring(1, nodeRaw.LastIndexOf(']') - 1);
                virtualNode = true;
            }
            Int64? nodeId = null;
            if (node.Contains('@'))
            {
                nodeId = Int64.Parse(node.Split('@')[1]);
                catalog = node.Split('@')[0];
            }


            if (context.Application["tree"] == null)
                InitializeTreeStore();

            Dictionary<string, SimpleTreeItem> xDoc = (Dictionary<string, SimpleTreeItem>)context.Application["tree"];

            StringBuilder sb = null;

            if (node == null)
                throw new ArgumentNullException("nodo richiesto non valido");

            List<SimpleTreeItem> ll = null;
            try
            {
                //provo prima a prendere gli item dalla memoria

                if (virtualNode)
                {
                    ll = xDoc.Where(e => e.Value.ExtendedPadre.Equals(node)).Select(e => e.Value).ToList();
                    ll.Sort((x, y) => string.Compare(x.Text, y.Text));
                    if (ll.Count < int.Parse(virtualPager) + 100)
                        ll = ll.GetRange(int.Parse(virtualPager), ll.Count - int.Parse(virtualPager));
                    else
                        ll = ll.GetRange(int.Parse(virtualPager), 100);

                }
                else if (nodeId.HasValue)
                    ll = xDoc.Where(e => e.Value.ExtendedPadre.Equals(node)).Select(e => e.Value).ToList();
                else
                    ll = xDoc.Where(e => string.IsNullOrEmpty(e.Value.Padre)).Select(e => e.Value).ToList();

                //se non trova figli prova a caricarli dalla banca dati
                if (ll.Count() == 0)
                {
                    Dictionary<string, SimpleTreeItem> newChildren = (Dictionary<string, SimpleTreeItem>)ServiceLocator.GetServiceFactory().ContattoService.LoadRubricaIndex(nodeId.Value, (IndexedCatalogs)Enum.Parse(typeof(IndexedCatalogs), catalog), 2);
                    if (newChildren != null && newChildren.Count > 1)
                    {
                        AppendChildren(newChildren);
                        ll = xDoc.Where(e => e.Value.ExtendedPadre.Equals(node)).Select(e => e.Value).ToList();
                    }
                }

                if (ll.Count() > 100)
                {
                    ll.Sort((x, y) => string.Compare(x.Text, y.Text));
                    int nn = ll.Count;
                    int gruppi = nn / 100;
                    for (int i = 0; i < nn - 1; i = i + 100)
                    {
                        if (sb == null)
                        {
                            sb = new StringBuilder();
                            sb.Append("[");
                            sb.Append(string.Format(JSON_ELEMENT_FIRST, "Da:" + ll[i].Text, "[" + i.ToString() + "]" + nodeRaw, "GRP"));
                        }
                        else
                            sb.Append(string.Format(JSON_ELEMENT, "Da:" + ll[i].Text, "[" + i.ToString() + "]" + nodeRaw, "GRP"));
                    }
                }
                else if (ll != null && ll.Count() > 0)
                {
                    ll.Sort((x, y) => string.Compare(x.Text, y.Text));
                    foreach (SimpleTreeItem xnode in ll)
                    {
                        if (sb == null)
                        {
                            sb = new StringBuilder();
                            sb.Append("[");
                            sb.Append(string.Format(JSON_ELEMENT_FIRST, xnode.Text, xnode.ExtendedValue, xnode.SubType));
                        }
                        else
                            sb.Append(string.Format(JSON_ELEMENT, xnode.Text, xnode.ExtendedValue, xnode.SubType));
                    }
                }

                if (sb != null)
                {
                    sb.Append("]");
                    context.Response.ContentType = "application/json";
                    context.Response.AppendHeader("Content-type", "text/json");
                    context.Response.Write(sb.ToString());
                }
            }
            catch { throw new Exception("Retry Please!!"); }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public static void addNewVirtualGroup(SimpleTreeItem item)
        {

        }

        public static void AddToVirtualGroup(SimpleTreeItem item, SimpleTreeItem VirtualGroup)
        {

        }

        public static void AppendChildren(Dictionary<string, SimpleTreeItem> newChildren)
        {
            if (System.Web.HttpContext.Current.Application["tree"] != null)
            {
                HttpContext.Current.Application.Lock();
                // lock the current application object

                Dictionary<string, SimpleTreeItem> xDoc = (Dictionary<string, SimpleTreeItem>)HttpContext.Current.Application["tree"];
                foreach (KeyValuePair<string, SimpleTreeItem> n in newChildren)
                {
                    if (!xDoc.Keys.Contains(n.Key))
                        xDoc.Add(n.Key, n.Value);
                }

                // unlock the application object
                HttpContext.Current.Application.UnLock();
            }
        }

        public static void InitializeTreeStore()
        {
            Dictionary<string, SimpleTreeItem> indice = (Dictionary<string, SimpleTreeItem>)ServiceLocator.GetServiceFactory().ContattoService.LoadRubricaIndex(IndexedCatalogs.ALL, 1);
            // get the current httpContext
            HttpContext context = HttpContext.Current;
            // get the current application object
            if (System.Web.HttpContext.Current.Application["tree"] != null)
            {
                HttpContext.Current.Application.Lock();
                // lock the current application object
                System.Web.HttpContext.Current.Application.Remove("tree");
                System.Web.HttpContext.Current.Application.Add("tree", indice);
                // unlock the application object
                HttpContext.Current.Application.UnLock();
            }
            else System.Web.HttpContext.Current.Application.Add("tree", indice);
        }
    }
}