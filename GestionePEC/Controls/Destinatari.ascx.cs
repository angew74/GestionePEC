using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class Destinatari : System.Web.UI.UserControl
    {
        //public CambiResidenzaWebApp.controls.UCGeneralSearchWs[] UCMailOrgs { get; set; }
        public GeneralSearcher[] UCMailOrgs
        {
            get
            {
                var ctrl = from r in gridDett.Rows.Cast<GridViewRow>()
                           from c in r.Cells.Cast<TableCell>()
                           select c.Controls.OfType<GeneralSearcher>();
                return ctrl.SelectMany(x => x).ToArray();
            }
        }
        public CheckBox[] ChkDefault
        {
            get
            {
                var chk = from r in gridDett.Rows.Cast<GridViewRow>()
                          from c in r.Cells.Cast<TableCell>()
                          select c.Controls.OfType<CheckBox>();
                return chk.SelectMany(x => x).ToArray();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPaginaRichiesta"></param>
        protected void OnPagerIndexChanged(string s, int pag)
        {


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void Grid_DataBound(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Grid_RowEditing(object sender, GridViewEditEventArgs e)
        {
        }

        public void DataBind(object DataSource)
        {
            gridDett.DataSource = DataSource;
            gridDett.DataBind();
        }

        private enum col
        {
            Destinatario,
            Email
        }

        //public string[] EmailDestinatario
        //{
        //    get
        //    {
        //        //IEnumerable<string> mDest = UCMailOrgs.Select(mo => mo.Text);
        //        IEnumerable<string> mDest = UCMailOrgs.Select(mo => mo.DefaultSearch);
        //        if (mDest.Count() == 0) return null;
        //        return mDest.ToArray();
        //    }
        //}

        public String[] MailDestinatario
        {
            get
            {
                //IEnumerable<string> idDest = UCMailOrgs.Select(mo => mo.Value);
                //  IEnumerable<string> idDest = UCMailOrgs.Select(mo => mo.HiddenFieldValueID);
                IEnumerable<string> idDest = from m in UCMailOrgs
                                             where m.HiddenFieldValueID != "0"
                                             select m.DefaultSearch;
                IEnumerable<string> idDestChio = from m in UCMailOrgs
                                                 where m.DefaultSearch.Contains('@')
                                                 & m.HiddenFieldValueID == "0"
                                                 select m.DefaultSearch.Trim();
                IEnumerable<string> result = idDest.Union(idDestChio);
                if (result.Count() == 0) return null;
                return result.ToArray();
            }
        }

        public String[] IDDestinatario
        {
            get
            {
                //IEnumerable<string> idDest = UCMailOrgs.Select(mo => mo.Value);
                //  IEnumerable<string> idDest = UCMailOrgs.Select(mo => mo.HiddenFieldValueID);
                IEnumerable<string> idDest = from m in UCMailOrgs
                                             where m.HiddenFieldValueID != "0"
                                             select m.HiddenFieldValueID;
                IEnumerable<string> idDestChio = from m in UCMailOrgs
                                                 where m.DefaultSearch.Contains('@')
                                                 & m.HiddenFieldValueID == "0"
                                                 select m.DefaultSearch.Trim();
                IEnumerable<string> result = idDest.Union(idDestChio);
                if (result.Count() == 0) return null;
                return result.ToArray();
            }
        }

        private string getCellValue(int riga, int colonna)
        {

            string s = this.gridDett.Rows[riga].Cells[colonna].Text;
            if (s != "&nbsp;") return s;
            else return "";
        }
    }
}