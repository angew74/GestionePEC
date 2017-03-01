using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Com.Unisys.MetaBus.Base;
using SendMail.Model;
using Com.Unisys.MetaBus.Schemas.Envelope;
using Com.Unisys.MetaBus.Schemas.Smtp;
using ActiveUp.Net.Common.UnisysExt;

namespace SendMailApp
{
    public static class MailExtensionsMethods
    {
        public static object ConvertTo(this Mail mail, Type destinationType)
        {
            if (destinationType == typeof(Request))
                return ConvertToRequest(mail);

            if (destinationType == typeof(email))
                return ConvertToEmail(mail);

            return null;
        }

        private static Request ConvertToRequest(this Mail mail)
        {
            Request ric = new Request();
            MapRoutingInfo(mail, ric);
            MapSecurityContext(mail, ric);
            MapRequestOriginator(mail, ric);
            ric.Body = new object();
            return ric;
        }

        private static email ConvertToEmail(this Mail mail)
        {
            email email = new email();
            email.from = mail.MailSender;

            var to = from refs in mail.Refs
                     where refs.AddresseeClass == AddresseeType.TO
                     select refs.AddresseeMail;
            if (to.Count() == 0)
            {
                return null;
            }
            email.to = String.Join(",", to.ToArray());

            var cc = from refs in mail.Refs
                     where refs.AddresseeClass == AddresseeType.CC
                     select refs.AddresseeMail;
            if (cc.Count() > 0)
            {
                email.cc = String.Join(",", cc.ToArray());
            }

            var ccn = from refs in mail.Refs
                      where refs.AddresseeClass == AddresseeType.CCN
                      select refs.AddresseeMail;
            if (ccn.Count() > 0)
            {
                email.bcc = String.Join(",", ccn.ToArray());
            }

            if (!String.IsNullOrEmpty(mail.Subject))
                email.subject = mail.Subject;
            email.body = mail.MailText;
            email.body_encoding = emailBody_encoding.UTF8;
            email.body_format = emailBody_format.HTML;
            //if (mail.HasAttachment)
            //{
            //    email.attachments = (from att in mail.Attachments
            //                         select new Attachement
            //                         {
            //                             tipo = att.AttachmentExtension,
            //                             nome = Guid.NewGuid().ToString(),
            //                             file = att.AttachmentFile
            //                         }).ToArray();
            //}

            return email;
        }

        private static void MapRequestOriginator(Mail mail, Request ric)
        {
            originator originator = new originator();
            originator.UserName = mail.UtenteInvio;
            originator.Dipartimento = string.Empty;
            originator.AppCode = null;// mail.CodiceApplicazione;
            ric.Originator = originator;
        }

        private static void MapSecurityContext(Mail mail, Request ric)
        {
            securityContext securityCtx = new securityContext();
            securityCtx.pName = mail.UtenteInvio;
            securityCtx.pUserName = mail.UtenteInvio;
            //securityCtx.pUserPassword = mail.Sender.Password;
            securityCtx.pUserExt1 = string.Empty;
            securityCtx.pUserExt2 = string.Empty;
            ric.SecurityContext = securityCtx;
        }

        private static void MapRoutingInfo(Mail mail, Request ric)
        {
            routingInfo routingInfo = new routingInfo();
            routingInfo.Protocol = "SMTP";
            routingInfo.UniqueId = mail.IdMail.ToString();
            //da config
            routingInfo.RuleId = mail.MailSender.Substring(mail.MailSender.IndexOf('@') + 1);

            ric.RoutingInfo = routingInfo;
        }
    }
}
