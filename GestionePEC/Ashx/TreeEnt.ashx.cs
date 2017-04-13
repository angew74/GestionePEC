using Delta.Utilities.LinqExtensions;
using SendMail.BusinessEF;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Services;

namespace GestionePEC.Ashx
{

    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class TreeEnt : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        internal class tNode
        {
            internal int nodeId;
            internal string catalog;
            internal int virtualPager;
            internal bool virtualNode;

            internal tNode(string n)
            {
                string nId = n;
                nodeId = -1;
                virtualPager = -1;

                if (n.EndsWith("]"))
                {
                    virtualNode = true;
                    int.TryParse(n.Substring(n.LastIndexOf('[')).Replace("[", "").Replace("]", ""), out virtualPager);
                    nId = n.Substring(0, n.LastIndexOf('['));
                }
                if (nId.Contains('@'))
                {
                    catalog = nId.Split('@')[0];
                    int.TryParse(nId.Split('@')[1], out nodeId);
                }
                else
                    int.TryParse(nId, out nodeId);
            }
        }

        [DataContract]
        [Serializable]
        internal class JSONTreeNode
        {
            [DataMember]
            internal string itemId { get; set; }
            [DataMember]
            internal string text { get; set; }
            [DataMember]
            internal string cls { get; set; }
            [DataMember]
            internal bool leaf { get; set; }
            [DataMember(IsRequired = false)]
            internal JSONTreeNode[] children { get; set; }
            [DataMember]
            internal string icon { get; set; }
            [DataMember]
            internal string nodeType { get; set; }
        }

        private const string TREE_KEY = "treeEnt";
        private const int PACK_SIZE = 25;
        private const string ROOT_NODE = "ROOT";

        public void ProcessRequest(HttpContext context)
        {
            string nod = context.Request.Params["node"]; // IPA@123 RUBR@123 IPA@123[1]  RUBR@435653[3]
            tNode node = new tNode(nod);
            if (node.nodeId == -1 || node.nodeId == 0)
                return;
            string nodeRaw = context.Request.Form["node"];

            if (context.Application[TREE_KEY] == null)
                InitializeTreeStore(node.nodeId.ToString());

            List<SimpleTreeItem> xDoc = (List<SimpleTreeItem>)context.Application[TREE_KEY];

            if (xDoc == null || xDoc.Count == 0)
                throw new InvalidOperationException("Errore nel recupero dell'ente");

            if (xDoc.SingleOrDefault(e => e.Value.Equals(node.nodeId.ToString())) == null)
            {
                InitializeTreeStore(node.nodeId.ToString());
                xDoc = (List<SimpleTreeItem>)context.Application[TREE_KEY];
            }

            IEnumerable<HierarchyNode<SimpleTreeItem>> hl = null;
            IEnumerable<SimpleTreeItem> ll = null;
            Func<List<SimpleTreeItem>, SimpleTreeItem> getRoot = l => l.FirstOrDefault(x => !l.Select(y => y.Value).Contains(x.Padre));
            try
            {
                //provo prima a prendere gli item dalla memoria
                if (node.nodeId > -1)
                    ll = xDoc.Where(e => e.Padre.Equals(node.nodeId.ToString()));
                else
                    ll = xDoc.Where(e => String.IsNullOrEmpty(e.Padre));

                if (node.virtualNode)
                {
                    ll = ll.Skip((node.virtualPager - 1) * PACK_SIZE).Take(PACK_SIZE);
                }

                JSONTreeNode[] nodes = null;
                //se non trova figli prova a caricarli dalla banca dati
                if (ll.Count() == 0)
                {
                    if (nodeRaw == ROOT_NODE)
                    {
                        hl = xDoc.AsHierarchy(e => e.Value, e => e.Padre, e => e.Text, getRoot(xDoc).Value);
                        nodes = ConvertToJSON(hl);
                    }
                    else
                    {
                        RubricaEntitaService rubrentitaService = new RubricaEntitaService();
                        List<SimpleTreeItem> tree = (List<SimpleTreeItem>)rubrentitaService.GetRubricaEntitaTreeByIdPadre(node.nodeId);
                        if (tree != null && tree.Count > 0)
                        {
                            AppendChildren(tree);
                        }
                        xDoc = (List<SimpleTreeItem>)context.Application[TREE_KEY];
                        hl = xDoc.AsHierarchy(e => e.Value, e => e.Padre, e => e.Text, node.nodeId.ToString());
                        nodes = ConvertToJSON(hl.ElementAt(0).ChildNodes);
                    }
                }
                else
                {
                    if (node.nodeId > -1)
                    {
                        ll = ll.Union(xDoc.Where(x => x.Value.Equals(node.nodeId.ToString())));
                        hl = ll.AsHierarchy(e => e.Value, e => e.Padre, e => e.Text, node.nodeId.ToString()).ToList();
                        if (nodeRaw == ROOT_NODE)
                            nodes = ConvertToJSON(hl);
                        else
                            nodes = ConvertToJSON(hl.ElementAt(0).ChildNodes);
                    }
                    else
                    {
                        nodes = ConvertToJSON(ll);
                    }
                }

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(JSONTreeNode[]));
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                ser.WriteObject(ms, nodes);
                string json = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();

                context.Response.ContentType = "application/json";
                context.Response.AppendHeader("Content-type", "text/json");
                context.Response.Write(json);
            }
            catch (Exception e)
            {
                throw new Exception("Retry Please!!");
            }

        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private static void AppendChildren(List<SimpleTreeItem> newChildren)
        {
            if (System.Web.HttpContext.Current.Application[TREE_KEY] != null)
            {
                HttpContext.Current.Application.Lock();
                // lock the current application object

                List<SimpleTreeItem> xDoc = (List<SimpleTreeItem>)HttpContext.Current.Application[TREE_KEY];
                foreach (SimpleTreeItem n in newChildren)
                {
                    if (xDoc.FirstOrDefault(i => i.Value.Equals(n.Value)) == null)
                        xDoc.Add(n);
                }
                // unlock the application object
                HttpContext.Current.Application.UnLock();
            }
        }

        private static void InitializeTreeStore(string idNode)
        {
            List<SimpleTreeItem> indice = null;
            if (String.IsNullOrEmpty(idNode))
            {
                indice = new List<SimpleTreeItem>();
            }
            else
            {
                RubricaEntitaService rubrEntitaService = new RubricaEntitaService();
                indice = (List<SimpleTreeItem>)rubrEntitaService.GetRubricaEntitaTree(long.Parse(idNode));
            }
            // get the current httpContext
            HttpContext context = HttpContext.Current;
            // get the current application object
            if (System.Web.HttpContext.Current.Application[TREE_KEY] != null)
            {
                HttpContext.Current.Application.Lock();
                // lock the current application object
                System.Web.HttpContext.Current.Application.Remove(TREE_KEY);
                System.Web.HttpContext.Current.Application.Add(TREE_KEY, indice);
                // unlock the application object
                HttpContext.Current.Application.UnLock();
            }
            else System.Web.HttpContext.Current.Application.Add(TREE_KEY, indice);
        }

        private static void VirtualizeTree(IEnumerable<HierarchyNode<SimpleTreeItem>> tree)
        {
            foreach (HierarchyNode<SimpleTreeItem> item in tree)
            {
                VirtualizeTree(item);
            }
        }

        private static void VirtualizeTree(HierarchyNode<SimpleTreeItem> item)
        {
            IEnumerable<HierarchyNode<SimpleTreeItem>> children = item.ChildNodes.OrderBy(x => x.Entity.Text).AsEnumerable();
            //children = VirtualizeTree(children);
            if (item.ChildNodes != null && item.ChildNodes.Count() > PACK_SIZE)
            {
                List<HierarchyNode<SimpleTreeItem>> l = null;
                if (children.Count() > PACK_SIZE)
                {
                    l = new List<HierarchyNode<SimpleTreeItem>>();
                    for (int j = 0; j < ((children.Count() / PACK_SIZE) + 1); j++)
                    {
                        HierarchyNode<SimpleTreeItem> hn = new HierarchyNode<SimpleTreeItem>()
                        {
                            Entity = new SimpleTreeItem()
                            {
                                Padre = item.Entity.Value,
                                Text = "Da: " + children.ElementAt(j * PACK_SIZE).Entity.Text,
                                Source = item.Entity.Source,
                                Value = String.Format("{0}[{1}]", item.Entity.Value, (j + 1)),
                                SubType = item.Entity.SubType
                            },
                            Parent = item.Entity,
                            ChildNodes = children.Skip(j * PACK_SIZE).Take(PACK_SIZE)
                        };
                        l.Add(hn);
                    }

                    item.ChildNodes = l;
                }
            }
        }

        private static JSONTreeNode[] ConvertToJSON(IEnumerable<HierarchyNode<SimpleTreeItem>> tree)
        {
            List<JSONTreeNode> list = new List<JSONTreeNode>();

            if (tree.Count() > PACK_SIZE)
            {
                List<SimpleTreeItem> xDoc = (List<SimpleTreeItem>)HttpContext.Current.Application[TREE_KEY];
                if (xDoc != null && xDoc.Count != 0)
                {
                    tree = xDoc.AsHierarchy(e => e.Value, e => e.Padre, e => e.Text, tree.ElementAt(0).Entity.Padre);
                    if (tree != null && tree.Count() > 0)
                    {
                        var it = tree.ElementAt(0);
                        VirtualizeTree(it);
                        tree = it.ChildNodes;
                    }
                }
            }

            foreach (HierarchyNode<SimpleTreeItem> item in tree)
            {
                JSONTreeNode node = new JSONTreeNode()
                {
                    itemId = item.Entity.ExtendedValue,
                    text = item.Entity.Text,
                    leaf = !Convert.ToBoolean(int.Parse(item.Entity.SubType)),
                    nodeType = "async",
                    cls = item.Entity.Description
                };
                if (node.leaf)
                {
                    node.icon = "../../App_Themes/Delta/images/tree/folder.png";
                }
                if (item.ChildNodes != null && item.ChildNodes.Count() > 0)
                {
                    if (item.ChildNodes.Count() > PACK_SIZE)
                        VirtualizeTree(item);
                    node.children = ConvertToJSON(item.ChildNodes);
                }
                list.Add(node);
            }

            return list.ToArray();
        }

        private static JSONTreeNode[] ConvertToJSON(IEnumerable<SimpleTreeItem> items)
        {
            List<JSONTreeNode> list = new List<JSONTreeNode>();
            foreach (SimpleTreeItem item in items)
            {
                JSONTreeNode node = new JSONTreeNode()
                {
                    itemId = item.Value,
                    text = item.Text,
                    leaf = !Convert.ToBoolean(int.Parse(item.SubType)),
                    nodeType = "async"
                };
                if (node.leaf)
                {
                    node.icon = "App_Themes/Delta/images/default/tree/folder.gif";
                }

                list.Add(node);
            }
            return list.ToArray();
        }
    }

}
