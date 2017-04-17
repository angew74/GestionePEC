using GestionePEC.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class DateTime : System.Web.UI.UserControl
    {
        private string _ValidationGroup;

        #region Public Property

        public void setData(string data)
        {
            string[] s = data.Split('/');
            tdDataG.Text = s[0];
            tdDataM.Text = s[1];
            tdDataY.Text = s[2];
        }
        /// <summary>
        /// Espone la label del controllo
        /// </summary>
        public string Label
        {
            set
            {
                if (value != string.Empty)
                {
                    lblTitle.Text = value;
                    lblTitle.Visible = true;
                }
                else
                    lblTitle.Visible = false;
            }
        }

        public string HeaderText
        {
            get { return (lblTitle.Text); }
            set
            {
                if (value != string.Empty)
                {
                    lblTitle.Text = value;
                    lblTitle.Visible = true;
                }
                else
                    lblTitle.Visible = false;
            }
        }


        /// <summary>
        /// Espone la larghezza del controllo
        /// </summary>
        public string Width
        {
            //get
            //{
            //    return panelMain.Width.ToString();
            //}
            set
            {
                //panelMain.Width = Unit.Parse(value);
            }
        }

        /// <summary>
        /// Espone il margine destro del controllo
        /// </summary>
        public string MarginRight
        {
            //get
            //{
            //    return panelMain.Style["margin-right"].ToString();
            //}
            set
            {
                //panelMain.Style["margin-right"] = value;
            }
        }

        /// <summary>
        /// Espone il testo del controllo
        /// </summary>
        public string Giorno
        {
            get { return (Utility.DatePartFormat(true, 1, tdDataG.Text.Trim())); }
            set { tdDataG.Text = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Mese
        {
            get { return (Utility.DatePartFormat(true, 2, tdDataM.Text.Trim())); }
            set { tdDataM.Text = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AnnoString
        {
            get { return tdDataY.Text.Trim(); }
            set { tdDataY.Text = value; }
        }

        /// <summary>
        /// Espone il testo del controllo
        /// </summary>
        public string GiornoString
        {
            get { return tdDataG.Text.Trim(); }
            set { tdDataG.Text = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MeseString
        {
            get { return tdDataM.Text.Trim(); }
            set { tdDataM.Text = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Anno
        {
            get { return (Utility.DatePartFormat(true, 3, tdDataY.Text.Trim())); }
            set { tdDataY.Text = value; }
        }


        /// <summary>
        /// Numeric value of the year.
        /// 
        /// </summary>
        public int iAnno
        {
            get
            {
                String sYear;
                int iYear;

                sYear = tdDataY.Text.Trim();
                if (sYear.Length < 1)
                    sYear = "-1";
                if (!int.TryParse(sYear, out iYear))
                    iYear = -1;
                return (iYear);
            }
            set { tdDataY.Text = value.ToString(); }
        }

        // Partial: missed parts are set to zeroes; the specified values aren't checked at all!
        // i.e.: 45/-2/0000 is a valid returned value
        public string DateStringPartial()
        {
            return (Utility.DateStringGet(true, Giorno, Mese, Anno));
        }

        // This routine returns the raw data ( no checks )
        // i.e.: /44/2011 is a possible returned value
        public string DateStringRaw()
        {
            return (Utility.DateStringGet(false, tdDataG.Text, tdDataM.Text, tdDataY.Text));
        }

        // Either a valid string date either null
        public string Data()
        {
            return (DateString());
        }

        // Either a valid string date either null
        public string DateString()
        {
            string sDate = Utility.DateStringGet(false, tdDataG.Text, tdDataM.Text, tdDataY.Text);

            if (Utility.IsDateValid(sDate))
                return sDate;
            else
                return null;
        }

        // Either a valid DateTime value either null
        public System.DateTime? Date()
        {
            string sDate;
            System.DateTime dt;


            if ((sDate = DateString()) == null)
                return (null);
            else
                //if ( DateTime.TryParse( sDate, out dt ) )
                if (Utility.DateConvert(sDate, out dt))
                return (dt);
            else
                return (null);
        } // DateTime ?Date()


        public string DataToMapper()
        {
            string data = this.tdDataG.Text + "/" + this.tdDataM.Text + "/" + this.tdDataY.Text;
            if (Utility.IsDateValid(data))
                return Utility.FormatDataG_M_AtoMapper(data, "/");
            else
                return null;
        }

        public string ToIsoFormat()
        {
            System.DateTime d = new System.DateTime(int.Parse(tdDataY.Text), int.Parse(tdDataM.Text), int.Parse(tdDataG.Text));
            return d.ToString("yyyyMMdd");
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean EnabledGiorno
        {
            set { this.tdDataG.Enabled = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean EnabledMese
        {
            set { this.tdDataM.Enabled = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean EnabledAnno
        {
            set { this.tdDataY.Enabled = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean EnabledValidator
        {
            get
            {
                return this.vceAnno.Enabled;
            }

            set
            {
                //this.vceRfvAnno.Enabled = value;
                this.rfvAnno.Enabled = value;
                revAnno.Enabled = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ValidationGroup
        {
            get
            {
                return _ValidationGroup;
            }

            set
            {
                _ValidationGroup = value;
                rfvAnno.ValidationGroup = value;
                revAnno.ValidationGroup = value;
            }
        }


        public bool DateTimeVisible
        {
            get
            {
                return Contenitore.Visible;
            }
            set
            {
                Contenitore.Visible = value;
            }
        }


        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            tdDataG.Attributes.Add("onkeyup", "return SetFocusOnItemDate('" + tdDataG.ClientID + "','" + tdDataM.ClientID + "',event)");
            tdDataM.Attributes.Add("onkeyup", "return SetFocusOnItemDate('" + tdDataM.ClientID + "','" + tdDataY.ClientID + "',event)");
            tdDataG.Attributes.Add("onkeypress", "return isNumberKey(event)");
            tdDataM.Attributes.Add("onkeypress", "return isNumberKey(event)");
            tdDataY.Attributes.Add("onkeypress", "return isNumberKey(event)");
        }
    }
}