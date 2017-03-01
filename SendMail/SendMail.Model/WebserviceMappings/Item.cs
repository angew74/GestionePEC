using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SendMail.Model.WebserviceMappings
{
    [DataContract(Namespace = "http://Delta.cdr.mailservice/generalsearcher")]
    public class Item
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "text")]
        public string Text { get; set; }
        [DataMember(Name = "cls")]
        public string Subtype { get; set; }
    }
}
