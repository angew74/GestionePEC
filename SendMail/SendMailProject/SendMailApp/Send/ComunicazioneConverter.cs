using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model.ComunicazioniMapping;
using Com.Unisys.MetaBus.Base;
using Com.Unisys.MetaBus.Schemas.Smtp;
using Com.Unisys.MetaBus.Schemas.Envelope;
using ActiveUp.Net.Common.UnisysExt;

namespace SendMailApp
{
    public static class ComunicazioniExtensionMethods
    {
        public static object ConvertTo(this Comunicazioni comunicazione, Type destinationType)
        {
            if (destinationType == typeof(Request))
                return ConvertToRequest(comunicazione);

            if (destinationType == typeof(email))
                return ConvertToEmail(comunicazione);
            if (destinationType == typeof(attachments))
                return ConvertToAttachments(comunicazione);
            return null;
        }

        private static object ConvertToRequest(Comunicazioni comunicazione)
        {
            Request ric = new Request();
            ric.RoutingInfo = MapRoutingInfo(comunicazione);
            //ric.SecurityContext = MapSecurityContext(comunicazione);
            ric.SecurityContext = null;
            ric.Originator = MapRequestOriginator(comunicazione);
            ric.Body = new object();
            return ric;
        }

        private static routingInfo MapRoutingInfo(Comunicazioni comunicazione)
        {
            routingInfo r = new routingInfo();
            r.Protocol = "SMTP";
            if (comunicazione.MailComunicazione == null)
            {
                throw new InvalidOperationException("Errore nella mail da inviare");
            }
            //MODIFICATO DA ALBERTO COLETTI PER RENDERE UNIVOCO SE LA STESSA MAIL VIENE REINVIATA
            r.UniqueId = comunicazione.MailComunicazione.IdMail.Value.ToString();
            string sender = comunicazione.MailComunicazione.MailSender;
            r.RuleId = sender.Substring(sender.IndexOf('@') + 1);
            return r;
        }

        private static securityContext MapSecurityContext(Comunicazioni comunicazione)
        {
            securityContext s = new securityContext();
            s.pName = comunicazione.UtenteInserimento;
            s.pUserName = comunicazione.UtenteInserimento;
            s.pUserExt1 = s.pUserExt2 = null;
            return s;
        }

        private static originator MapRequestOriginator(Comunicazioni comunicazione)
        {
            originator o = new originator();
            o.UserName = comunicazione.UtenteInserimento;
            o.Dipartimento = null;
            o.AppCode = comunicazione.AppCode;
            return o;
        }

        private static object ConvertToEmail(Comunicazioni comunicazione)
        {
            email mail = new email();
            if (comunicazione.MailComunicazione == null)
            {
                throw new InvalidOperationException("Errore nella mail da inviare");
            }
            mail.from = comunicazione.MailComunicazione.MailSender;

            var to = from refs in comunicazione.MailComunicazione.MailRefs
                     where refs.TipoRef == AddresseeType.TO
                     select refs.MailDestinatario;
            if (to.Count() == 0)
            {
                throw new InvalidOperationException("Nella mail mancano i destinatari");
            }
            mail.to = String.Join(",", to.ToArray());

            var cc = from refs in comunicazione.MailComunicazione.MailRefs
                     where refs.TipoRef == AddresseeType.CC
                     select refs.MailDestinatario;
            if (cc.Count() > 0)
            {
                mail.cc = String.Join(",", cc.ToArray());
            }

            var ccn = from refs in comunicazione.MailComunicazione.MailRefs
                      where refs.TipoRef == AddresseeType.CCN
                      select refs.MailDestinatario;
            if (ccn.Count() > 0)
            {
                mail.bcc = String.Join(",", ccn.ToArray());
            }

            if (!String.IsNullOrEmpty(comunicazione.MailComunicazione.MailSubject))
            {
                mail.subject = comunicazione.MailComunicazione.MailSubject;
            }

            mail.body = comunicazione.MailComunicazione.MailText;
            mail.body_encoding = emailBody_encoding.UTF8;
            mail.body_format = emailBody_format.HTML;           
            return mail;
        }

        private static object ConvertToAttachments(Comunicazioni comunicazione)
        {
            attachments attaches = null;
            if (comunicazione.ComAllegati != null && comunicazione.ComAllegati.Count > 0)
            {
                attaches = new attachments();
                attaches.attachment = (from all in comunicazione.ComAllegati
                                       select new attachment
                                       {
                                           tipo = all.AllegatoExt,
                                           nome = all.AllegatoName,
                                           file = all.AllegatoFile
                                       }).ToList();
            }
            return attaches;
        }
    }
}
