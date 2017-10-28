using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model.ComunicazioniMapping;
using ActiveUp.Net.Common.DeltaExt;
using System.Net.Mail;
using System.IO;

namespace SendMailApp
{
    public static class ComunicazioniExtensionMethods
    {
       

        public static MailMessage ConvertToEmail(Comunicazioni comunicazione)
        {
            MailMessage mail = new MailMessage();
            if (comunicazione.MailComunicazione == null)
            {
                throw new InvalidOperationException("Errore nella mail da inviare");
            }
            mail.From = new MailAddress(comunicazione.MailComunicazione.MailSender);

            var to = from refs in comunicazione.MailComunicazione.MailRefs
                     where refs.TipoRef == AddresseeType.TO
                     select refs.MailDestinatario;
            if (to.Count() == 0)
            {
                throw new InvalidOperationException("Nella mail mancano i destinatari");
            }

            MailAddressCollection collection = new MailAddressCollection();
            foreach(string t in to)
            {mail.To.Add(new MailAddress(t));}
            var cc = from refs in comunicazione.MailComunicazione.MailRefs
                     where refs.TipoRef == AddresseeType.CC
                     select refs.MailDestinatario;
            if (cc.Count() > 0)
            {
                foreach (string c in cc)
                { mail.CC.Add(new MailAddress(c)); }
            }

            var ccn = from refs in comunicazione.MailComunicazione.MailRefs
                      where refs.TipoRef == AddresseeType.CCN
                      select refs.MailDestinatario;
            if (ccn.Count() > 0)
            {
                foreach (string bcc in ccn)
                { mail.Bcc.Add(bcc); }
            }
            if (!String.IsNullOrEmpty(comunicazione.MailComunicazione.MailSubject))
            {
                mail.Subject = comunicazione.MailComunicazione.MailSubject;
            }

            mail.Body = comunicazione.MailComunicazione.MailText;
            mail.BodyEncoding = Encoding.UTF8; 
            mail.IsBodyHtml = true;          
            return mail;
        }

        public static Attachment ConvertToAttachment(ComAllegato allegato)
        {
            Stream stream = new MemoryStream(allegato.AllegatoFile);
            Attachment attache = new Attachment(stream, allegato.AllegatoName);
            return attache;
        }
    }
}
