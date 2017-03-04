using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class Paging : System.Web.UI.UserControl
    {
        /// <summary>
        /// Vedi un esempio di utilizzo in UCRicercaIndividuoPaging.ascx
        /// 
        /// 1 - mettere il controllo nel PagerTemplate della GridView da paginare:
        /// 
        ///     -PagerTemplate-
        ///         -uc2:UCPaging ID="ucPaging" runat="server" OnPagerIndexChanged="OnPagerIndexChanged" /-
        ///     -/PagerTemplate-
        /// 
        /// 2 - DOPO IL BINDING DEI DATI aggiungere:
        /// 
        ///     FormViewUtils.setFormViewPaging(gridDett, "ucPaging", er.totale, er.da);
        /// 
        ///     dove er.totale, er.da sono i valori ricevuti da MAPPER
        /// 
        /// 3 - AGGIUNGERE L'EVENTO:
        /// 
        ///     protected void OnPagerIndexChanged(string sPaginaRichiesta)
        /// 
        ///     dove sPaginaRichiesta e' la pagina selezionata dall'utente
        /// </summary>
        /// <summary>
            /// 
            /// </summary>
            /// <param name="pagina"></param>
            public delegate void PagerIndexChangedHandler(string sPaginaRichiesta, int Pag);
            /// <summary>
            /// 
            /// </summary>
            public event PagerIndexChangedHandler PagerIndexChanged;

            int _pagingPagineTotali = 0;
            int _pagingPaginaCorrente = 0;
            /// <summary>
            /// 
            /// </summary>
            public int PagineTotali
            {
                set { _pagingPagineTotali = value; }
            }
            /// <summary>
            /// 
            /// </summary>
            public int PaginaCorrente
            {
                set
                {
                    _pagingPaginaCorrente = value;
                    rinfresca();
                }
                get { return _pagingPaginaCorrente; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string PagingValue
            {
                set
                {
                    hfPagingValue.Value = value;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected void Page_Load(object sender, EventArgs e)
            {
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected void OnPagerIndexChanged(object sender, EventArgs e)
            {

                PagerIndexChanged(string.Empty, ddlPagerPages.SelectedIndex);
            }

            void rinfresca()
            {
                for (int i = 1; i <= _pagingPagineTotali; i++)
                {
                    ListItem item = new ListItem(i.ToString());
                    ddlPagerPages.Items.Add(item);
                }
                if (_pagingPagineTotali > 0) ddlPagerPages.SelectedIndex = _pagingPaginaCorrente - 1;

                labPagerPages.Text = string.Format("Pagina {0} di {1}",
                    _pagingPaginaCorrente, _pagingPagineTotali);
            }

            public void configureControl(string da, string per, string totale)
            {
                this.PagineTotali = (int)Math.Ceiling(Double.Parse(totale) / Double.Parse(per));
                //  this.PaginaCorrente = (int)Math.Ceiling(Double.Parse(da) / Double.Parse(per));
                this.PaginaCorrente = int.Parse(da);
                this.PagingValue = per;
            }

        }
    }
