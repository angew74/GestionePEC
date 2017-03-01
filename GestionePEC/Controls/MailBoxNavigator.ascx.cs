using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Mail.MailMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class MailBoxNavigator : System.Web.UI.UserControl
    {
        #region "Public Properties"
        public bool EnableNewMail { get; set; }
        #endregion

        public string Json = "[]";
        public bool Pec = false;
        public string Account = "Casella Mail";

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (WebMailClientManager.AccountIsValid())
            {
                MailUser m = WebMailClientManager.getAccount();
                Account = m.Casella;
                //if (m.IsPec) Pec = true;
                Json = createJson();
            }
        }

        private string createJson()
        {
            StringBuilder stBuilder = new StringBuilder();
            MailUser m = WebMailClientManager.getAccount();
            if (m.FlgManaged == 1 || m.FlgManaged == 2)
            {
                string leaf = "true";
                List<Folder> Folders = m.Folders;
                if (Folders != null)
                {
                    stBuilder.Append("[");
                    var TreeReceived = (from r in Folders
                                        where r.TipoFolder == "I"
                                        orderby r.Id
                                        select r).ToList();
                    var TreeSent = (from r in Folders
                                    where r.TipoFolder == "O"
                                    orderby r.Id
                                    select r).ToList();
                    var TreeArchivedSent = (from r in Folders
                                            where (r.TipoFolder.Equals("A")
                                            && r.IdNome == "2")
                                            || r.TipoFolder == "D"
                                            orderby r.Id
                                            select r).ToList();
                    var TreeArchivedReceived = (from r in Folders
                                                where (r.TipoFolder.Equals("A")
                                                && (r.IdNome == "1" || r.IdNome == "3"))
                                                || (r.TipoFolder == "E")
                                                orderby r.Id
                                                select r).ToList();
                    var TreeCancelled = (from r in Folders
                                         where r.TipoFolder.Equals("C")
                                         orderby r.Id
                                         select r).ToList();

                    stBuilder.Append(TreeBuilder("Ricevuta", "I", "I", TreeReceived));
                    stBuilder.Append(",");
                    stBuilder.Append(TreeBuilder("Inviata", "O", "O", TreeSent));
                    stBuilder.Append(",");
                    stBuilder.Append(TreeBuilder("Archivio Ricevuta", "A", "I", TreeArchivedReceived));
                    stBuilder.Append(",");
                    stBuilder.Append(TreeBuilder("Archivio Inviata", "AO", "O", TreeArchivedSent));
                    stBuilder.Append(",");
                    stBuilder.Append(TreeCustomBuilder("Cestino", TreeCancelled));
                    stBuilder.Append("]");
                }
                else
                {
                    string InboxUnManaged = "{ 'text': 'Posta Ricevuta','leaf': true, 'cls': 'folder', id:'1' }";
                    string OutBox = "{ 'text': 'Posta Inviata','leaf': true, 'expanded':false, 'cls': 'folder', id:'3' }";
                    stBuilder.Append(String.Format("[{0},{1}]", InboxUnManaged, OutBox));
                }

            }
            else
            {
                string InboxUnManaged = "{ 'text': 'Posta Ricevuta','leaf': true,'expanded':false, 'cls': 'folder', id:'1' }";
                string OutBox = "{ 'text': 'Posta Inviata','leaf': true, 'expanded':false, 'cls': 'folder', id:'2' }";
                stBuilder.Append(String.Format("[{0},{1}]", InboxUnManaged, OutBox));
            }
            return stBuilder.ToString();
        }

        private string TreeBuilder(string root, string id, string parent, List<Folder> list)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{'text':'" + root + "','leaf': false, 'expanded':false, 'cls': 'folder', id:'" + id + "'");
            builder.Append(",children:[");
            for (int k = 0; k < list.Count; k++)
            {
                if (k > 0)
                {
                    builder.Append(",");
                }
                builder.Append("{'text':'" + list[k].Nome + "','leaf': true, 'expanded':false, 'cls': 'folder', id:'" + list[k].Id.ToString() + "', 'parentid':'" + parent + "'}");
            }
            builder.Append("]");
            builder.Append("}");
            return builder.ToString();
        }

        private string TreeCustomBuilder(string root, List<Folder> list)
        {
            StringBuilder builder = new StringBuilder();
            string id = string.Empty;
            builder.Append("{'text':'" + root + "','leaf': false, 'expanded':false, 'cls': 'folder', id:'C'");
            builder.Append(",children:[");
            for (int k = 0; k < list.Count; k++)
            {
                if (k > 0)
                {
                    builder.Append(",");
                }
                switch (list[k].IdNome)
                {
                    case "1":
                    case "3":
                        id = "I";
                        break;
                    case "2":
                        id = "O";
                        break;
                    default:
                        switch (list[k].TipoFolder)
                        {
                            case "D":
                                id = "O";
                                break;
                            case "E":
                                id = "I";
                                break;
                        }
                        break;
                }

                builder.Append("{'text':'" + list[k].Nome + "','leaf': true, 'expanded':false, 'cls': 'folder', id:'" + list[k].Id.ToString() + "', 'parentid':'" + id + "'}");
            }
            builder.Append("]");
            builder.Append("}");
            return builder.ToString();
        }

    }
}