using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Mapping
{
    public class DaoSQLServerDBHelper
    {


        internal static MailServer MapToMailServer(IDataRecord dr)
        {
            MailServer r = new MailServer();
            r.Id = dr.GetDecimal("ID_SVR");
            r.DisplayName = dr.GetString("NOME");
            r.IncomingServer = dr.GetString("INDIRIZZO_IN");
            r.OutgoingServer = dr.GetString("INDIRIZZO_OUT");
            r.PortIncomingServer = int.Parse(dr.GetString("PORTA_IN"));
            r.PortIncomingChecked = !r.PortIncomingServer.Equals(110);
            r.PortOutgoingServer = int.Parse(dr.GetString("PORTA_OUT"));
            r.PortOutgoingChecked = !r.PortOutgoingServer.Equals(25);
            r.IsIncomeSecureConnection = bool.Parse(dr.GetString("SSL_IN"));
            r.IsOutgoingSecureConnection = bool.Parse(dr.GetString("SSL_OUT"));
            r.IsOutgoingWithAuthentication = bool.Parse(dr.GetString("AUTH_OUT"));
            r.IncomingProtocol = dr.GetString("PROTOCOLLO_IN");
            r.Dominus = dr.GetString("DOMINUS");
            r.IsPec = Convert.ToBoolean(int.Parse(dr.GetString("FLG_ISPEC")));

            return r;
        }

        internal static ActiveUp.Net.Common.DeltaExt.Action MapToAction(IDataRecord dr)
        {
            ActiveUp.Net.Common.DeltaExt.Action a = new ActiveUp.Net.Common.DeltaExt.Action();
            a.Id = dr.GetDecimal("ID");
            a.NomeAzione = dr.GetString("NOME_AZIONE");
            a.IdDestinazione = dr.GetDecimal("ID_NOME_DESTINAZIONE");
            a.NuovoStatus = dr.GetString("NUOVO_STATUS");
            a.TipoAzione = dr.GetString("TIPO_AZIONE");
            a.TipoDestinazione = dr.GetString("TIPO_DESTINAZIONE");
            if (dr.FieldCount > 7)
            { a.IdComp = dr.GetDecimal("IDFOLDER").ToString(); }
            a.IdFolderDestinazione = int.Parse(dr.GetDecimal("ID_FOLDER_DESTINAZIONE").ToString());
            return a;
        }

        internal static Folder MapToFolder(IDataRecord dr, List<ActiveUp.Net.Common.DeltaExt.Action> l)
        {
            Folder f = new Folder();
            f.Id = dr.GetDecimal("ID");
            f.Nome = dr.GetString("NOME");
            f.Abilitata = dr.GetString("SYSTEM");
            f.TipoFolder = dr.GetString("TIPO");
            f.IdNome = dr.GetDecimal("IDNOME").ToString();
            if (l != null)
            { f.Azioni = l; }
            return f;
        }
        internal static SimpleTreeItem MapToSimpleTreeItem(IDataRecord dr)
        {
            SimpleTreeItem t = new SimpleTreeItem();
            t.Value = dr["VALUE"].ToString();
            t.Text = dr["TEXT"].ToString();
            t.SubType = dr["SUBTYPE"].ToString();
            t.Source = dr["SOURCE"].ToString();
            t.Padre = dr["PADRE"].ToString();
            return t;
        }


        internal static MailHeaderExtended MapToMailHeaderExtended(IDataRecord dr)
        {
            MailHeaderExtended me = new MailHeaderExtended();
            me.CC = dr.GetString("MAIL_CC");
            me.CCn = dr.GetString("MAIL_CCN");
            me.Date = dr.GetDateTime("DATA_INVIO");
            me.From = dr.GetString("MAIL_FROM");
            me.MailPartialText = dr.GetString("MAIL_TEXT");
            me.MailStatus = (MailStatus)Enum.Parse(typeof(MailStatus), dr.GetString("STATUS_MAIL"));
            me.ReceiveDate = dr.GetDateTime("DATA_RICEZIONE");
            if (me.Date.ToString("dd/MM/yyyy") == "01/01/0001")
            { me.Date = me.ReceiveDate; }
            me.ServerStatus = (MailStatusServer)Enum.Parse(typeof(MailStatusServer), dr.GetString("STATUS_SERVER"));
            me.Subject = dr.GetString("MAIL_SUBJECT");
            me.To = dr.GetString("MAIL_TO");
            me.UniqueId = dr.GetString("MAIL_ID");
            me.MailRating = dr.GetInt32("FLG_RATING");
            me.HasAttachments = Convert.ToBoolean(int.Parse(dr.GetString("FLG_ATTACHMENTS")));
            me.Utente = dr.GetString("UTENTE");
            me.FolderId = dr.GetDecimal("FOLDERID");
            me.FolderTipo = dr.GetString("FOLDERTIPO");
            me.Dimensione = (int)dr.GetDecimal("msg_length");
            me.NomeFolder = dr.GetString("NOME");
            return me;
        }
    }

}