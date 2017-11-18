using ActiveUp.Net.Mail.DeltaExt;
using AspNet.Identity.SQLServerProvider;
using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static GestionePEC.Models.MailModel;

namespace GestionePEC.Models
{
    public class UsersMailModel
    {
        public string message;
        public string success;
        public UserMail[] UsersList;

        public List<MailUser> MailUsers;
        public List<OwnProfile> OwnProfiles;

        public string Totale { get; internal set; }
    }

    public class UserMail
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    
    public class OwnProfile
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public virtual string Cognome { get; set; }
        public virtual string Nome { get; set; }
        public virtual string CodiceFiscale { get; set; }
        public virtual Int64 Department { get; set; }
        public virtual string Domain { get; set; }
        public virtual Int32 UserRole { get; set; }
        public virtual Int32 RoleMail { get; set; }
        public virtual string RoleMailDesription { get; set; }
        public virtual string Password { get; set; }
        public virtual List<IdentityRole> Roles {get;set;}
        public virtual List<CasellaMail> MappedMails { get; set; }
    }

   
}