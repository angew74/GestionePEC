using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Mail.MailMessage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class InBox : System.Web.UI.UserControl
    {
        
            public class RowSelectedEventArgs : EventArgs
            {
                public string UId { get; set; }
                public string CurrentRating { get; set; }
                public string CurrentFolder { get; set; }
                public string ParentFolder { get; set; }
                public int Dimension { get; set; }


                public RowSelectedEventArgs(string uid, string currRating, string currFolder, string parFolder, int dimension)
                {
                    this.UId = uid;
                    this.CurrentRating = currRating;
                    this.CurrentFolder = currFolder;
                    this.Dimension = dimension;
                    this.ParentFolder = parFolder;
                }
            }


            public event EventHandler<RowSelectedEventArgs> RowSelected;
            protected virtual void onRowSelected(object sender, RowSelectedEventArgs e)
            {
                if (RowSelected != null)
                {
                    RowSelected(sender, e);
                }
            }

            [Bindable(true), Browsable(true), DefaultValue(""), Localizable(true),
            RefreshProperties(RefreshProperties.Repaint)]
            [UrlProperty]
            [EditorAttribute(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public string MailBoxProvider { get; set; }

            // ---------------------------------------------------------------------------------------------------- //
            private int da = 1;
            private int iPageSize = 5;
          //  private UCPaging ucPaging = null;
            // ---------------------------------------------------------------------------------------------------- //

            public string JsonCartella = "[]";

            public string JsonStatus = "[]";

            public void Initialize()
            {
                if (WebMailClientManager.AccountIsValid())
                {
                    this.Visible = true;
                    Initialize(false);
                }
                else this.Visible = false;
            }

            protected void Initialize(bool refresh)
            {
                if (WebMailClientManager.AccountIsValid())
                {
                    this.Visible = WebMailClientManager.AccountIsValid();
                    if (this.Visible)
                        BindDataViews(true);
                }
                else this.Visible = false;
            }

            protected void Page_Load(object sender, EventArgs e)
            {
                if (!Page.IsPostBack)
                    if (WebMailClientManager.AccountIsValid())
                    {
                        BindDataViews(false);
                    }
            }

            protected void Page_PreRender(object sender, EventArgs e)
            {
                if (WebMailClientManager.AccountIsValid())
                {
                    MailUser m = WebMailClientManager.getAccount();
                    hfCurrentFolder.Value = "1";
                    hfPFolder.Value = "I";
                    JsonCartella = createJsonCartella(m.IsManaged);
                    JsonStatus = createJsonStatus();
                }
            }

            protected void btnPost_Click(object sender, EventArgs e)
            {
                var uid = hfSelectedRow.Value;
                var currRating = hfCurrentRating.Value;
                var currFolder = hfCurrentFolder.Value;
                var dimension = int.Parse(hfDimension.Value);
                var parFolder = hfPFolder.Value;
                onRowSelected(sender, new RowSelectedEventArgs(uid, currRating, currFolder, parFolder, dimension));
            }

            //protected void ucPaging_Init(object sender, EventArgs e)
            //{
            //    this.ucPaging = sender as UCPaging;
            //}

            protected void BindDataViews(bool refresh)
            {
                if (WebMailClientManager.AccountIsValid())
                {
                    MailUser user = WebMailClientManager.getAccount();
                    this.Visible = true;
                }
                else this.Visible = false;
            }

            private string createJsonCartella(bool IsManaged)
            {
                StringBuilder stBuilder = new StringBuilder();
                MailUser m = WebMailClientManager.getAccount();
                string Nom = string.Empty;
                if (m == null)
                {
                    return "[]";
                }
                if (IsManaged)
                {
                    List<Folder> Folders = m.Folders;
                    stBuilder.Append("[");
                    if (Folders != null && Folders.Count > 0)
                    {
                        for (int i = 0; i < Folders.Count; i++)
                        {
                            if (i > 0)
                            {
                                stBuilder.Append(",");
                            }
                            switch (Folders[i].TipoFolder)
                            {
                                case "I":
                                    Nom = string.Empty;
                                    break;
                                case "O":
                                    Nom = string.Empty;
                                    break;
                                case "E":
                                case "A":
                                case "D":
                                    Nom = " Archivio";
                                    break;
                                case "C":
                                    Nom = " Cestino";
                                    break;

                            }
                            stBuilder.Append("['" + Folders[i].Id + "','" + Folders[i].Nome + Nom + "']");
                        }
                        stBuilder.Append("]");
                    }
                    else
                    {
                        return "[]";
                    }
                }
                return stBuilder.ToString();
            }

            private string AppendCustomFolders(List<Folder> list, string skip, string skip1, string skip2)
            {
                StringBuilder appender = new StringBuilder();
                if (list.Count > 0)
                {
                    for (int k = 0; k < list.Count; k++)
                    {
                        if (list[k].TipoFolder != skip && list[k].TipoFolder != skip1 && list[k].TipoFolder != skip2)
                        {
                            appender.Append("#" + list[k].Id + list[k].TipoFolder);
                        }
                    }
                }
                return appender.ToString();
            }

            private string createJsonStatus()
            {
                MailUser m = WebMailClientManager.getAccount();
                if (m == null)
                {
                    return "[]";
                }
                if (m.FlgManaged == 1)
                {
                    StringBuilder stBuilder = new StringBuilder();
                    List<Folder> Folders = m.Folders;
                    var CustomFolders = (from r in Folders
                                         where r.Abilitata.Contains("E")
                                         orderby r.Id
                                         select r).ToList();
                    stBuilder.Append("[");
                    stBuilder.Append("[");
                    stBuilder.Append((int)MailStatus.SCARICATA);
                    stBuilder.Append(",'Da leggere','#1I#1A#3I#3A");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "O", ""));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.LETTA);
                    stBuilder.Append(",'Letta','#1I#1A#3I#3A");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "O", ""));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.INOLTRATA);
                    stBuilder.Append(",'Inoltrata','#1I#1A");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "O", ""));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.REPLY_ONE);
                    stBuilder.Append(",'Con risposta','#1I#1A");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "O", "A"));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.REPLY_ALL);
                    stBuilder.Append(",'Con risposta a tutti','#1I#1A");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "O", "A"));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.CANCELLED);
                    stBuilder.Append(",'Cancellata dalla posta ricevuta','#1C#3C");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "I", "O", "A"));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.SCARICATA_INCOMPLETA);
                    stBuilder.Append(",'Incompleta','#1I#3I");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "O", "A"));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.INSERTED);
                    stBuilder.Append(",'Salvata','#2I");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "I", "A"));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.PROCESSING);
                    stBuilder.Append(",'In lavorazione','#-1");
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.SEND_AGAIN);
                    stBuilder.Append(",'Da reinviare','#2O");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "I", "A"));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.SENT);
                    stBuilder.Append(",'Inviata al server','#2O");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "I", "A"));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.ERROR);
                    stBuilder.Append(",'In errore','#2O");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "I", "A"));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.CANCELLED);
                    stBuilder.Append(",'Cancellata dalla posta inviata','#2O");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "I", "A"));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.ACCETTAZIONE);
                    stBuilder.Append(",'Accettata dal server','#2O");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "I", "A"));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.NON_ACCETTAZIONE);
                    stBuilder.Append(",'Non accettata dal server','#2O");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "I", "A"));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.AVVENUTA_CONSEGNA);
                    stBuilder.Append(",'Consegnata','#2O");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "I", "A"));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.ERRORE_CONSEGNA);
                    stBuilder.Append(",'Errore di consegna','#2O");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "I", "A"));
                    stBuilder.Append("'],[");
                    stBuilder.Append((int)MailStatus.ARCHIVIATA);
                    stBuilder.Append(",'Archiviata','#1A#2A#3A");
                    stBuilder.Append(AppendCustomFolders(CustomFolders, "C", "I", ""));
                    stBuilder.Append("']]");
                    return stBuilder.ToString();
                    //         fields: [{name: 'idItem', type: 'string'}, {name: 'displayText', type: 'string'}, {name: 'folder', type: 'string'}],
                    //data: [[eval('<%= (int)MailStatus.SCARICATA %>'), 'Da leggere', '#<%= (int)MailFolder.Ricevute %>#<%= (int)MailFolder.RicevutePEC %>#'],
                    //       [eval('<%= (int)MailStatus.LETTA %>'), 'Letta', '#<%= (int)MailFolder.Ricevute %>#<%= (int)MailFolder.RicevutePEC %>#'],
                    //       [eval('<%= (int)MailStatus.INOLTRATA %>'), 'Inoltrata', '#<%= (int)MailFolder.Ricevute %>#'],

                    //       [eval('<%= (int)MailStatus.REPLY_ONE %>'), 'Con risposta', '#<%= (int)MailFolder.Ricevute %>#'],
                    //       [eval('<%= (int)MailStatus.REPLY_ALL %>'), 'Con risposta a tutti', '#<%= (int)MailFolder.Ricevute %>#'],
                    //       [eval('<%= (int)MailStatus.CANCELLATA %>'), 'Cancellata dalla posta ricevuta', '#<%= (int)MailFolder.Cestino %>#'],
                    //       [eval('<%= (int)MailStatus.SCARICATA_INCOMPLETA %>'), 'Incompleta', '#<%= (int)MailFolder.Ricevute %>#'],
                    //       [eval('<%= (int)MailStatus.INSERTED %>'), 'Salvata', '#<%= (int)MailFolder.Inviate %>#'],
                    //       [eval('<%= (int)MailStatus.PROCESSING %>'), 'In lavorazione', '#-1#'],
                    //       [eval('<%= (int)MailStatus.SEND_AGAIN %>'), 'Da reinviare', '#<%= (int)MailFolder.Inviate %>#'],
                    //       [eval('<%= (int)MailStatus.SENT %>'), 'Inviata al server', '#<%= (int)MailFolder.Inviate %>#'],
                    //       [eval('<%= (int)MailStatus.ERROR %>'), 'In errore', '#<%= (int)MailFolder.Inviate %>#'],
                    //       [eval('<%= (int)MailStatus.CANCELLED %>'), 'Cancellata dalla posta inviata', '#<%= (int)MailFolder.Cestino %>#'],
                    //       [eval('<%= (int)MailStatus.ACCETTAZIONE %>'), 'Accettata dal server', '#<%= (int)MailFolder.Inviate %>#'],
                    //       [eval('<%= (int)MailStatus.NON_ACCETTAZIONE %>'), 'Non accettata dal server', '#<%= (int)MailFolder.Inviate %>#'],
                    //       [eval('<%= (int)MailStatus.AVVENUTA_CONSEGNA %>'), 'Consegnata', '#<%= (int)MailFolder.Inviate %>#'],
                    //       [eval('<%= (int)MailStatus.ERRORE_CONSEGNA %>'), 'Errore di consegna', '#<%= (int)MailFolder.Inviate %>#'],
                    //       [eval('<%= (int)MailStatus.ARCHIVIATA %>'), 'Archiviata', '#<%= (int)MailFolder.ArchivioRicevute %>#<%= (int)MailFolder.ArchivioInviate %>#']]
                }
                else
                {
                    return "[]";
                }


            }

            protected void IbRefresh_Click(object sender, EventArgs e)
            {
                da = 1;
                this.Initialize(true);
            }
        }
}