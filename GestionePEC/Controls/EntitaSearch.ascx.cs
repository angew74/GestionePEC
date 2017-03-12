using SendMail.Model.WebserviceMappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class EntitaSearch : System.Web.UI.UserControl
    {

        public Item EnteSelected
        {
            get
            {
                if (String.IsNullOrEmpty(hfSubType.Value))
                    return null;
                else
                {
                    Item item = Newtonsoft.Json.JsonConvert.DeserializeObject<Item>(hfSubType.Value);
                    return item;
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}