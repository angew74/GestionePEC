using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionePEC.Models
{

    public class TreeNode
    {
       public string text { get; set; }
       public string itemId { get; set; }
       public string cls { get; set; }
    }
    public class TreeModel
    {
        public string message { get; set; }
        public string totale { get; set; }

        public List<TreeNode> Items { get; set; }
        
    }
}