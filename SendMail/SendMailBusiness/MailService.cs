using System;
using System.Collections.Generic;
using SendMail.Model;
using SendMail.Business.Base;
using SendMail.DataContracts.Interfaces;
using SendMail.Model.ComunicazioniMapping;
using SendMail.Data.Contracts.Mail;
using ActiveUp.Net.Mail;


namespace SendMail.Business
{
    public sealed class MailService : BaseSingletonService<MailService>, SendMail.Business.Contracts.IMailService
    {

        public void sendMail(long idRubrica)
        {
            throw new NotImplementedException();
        }

        //public RawMessage LoadRawMessage(string appCode, string uid)
        //{
        //    HTF.CR.Mail.MailServiceMapper service = new HTF.CR.Mail.MailServiceMapper();
        //    RawMessage msg = service.GetStampe(appCode, uid);
        //    return msg;
        //}

        //public Comunicazioni LoadComunicazione(string appCode, string uid)
        //{
        //    HTF.CR.Mail.MailServiceMapper service = new HTF.CR.Mail.MailServiceMapper();
        //    Comunicazioni msg = service.GetStampeComunicazioni(appCode, uid);
        //    return msg;
        //}

        public Mail GetMail(long idMail)
        {
            using (IMailDao dao = this.getDaoContext().DaoImpl.MailDao)
            {
                return dao.GetById(idMail);
            }
        }

        public ICollection<Mail> GetAllMails()
        {
            using (IMailDao dao = this.getDaoContext().DaoImpl.MailDao)
            {
                return dao.GetAll();
            }
        }

        public ICollection<Mail> GetMailsWithAttachment()
        {
            using (IMailDao dao = this.getDaoContext().DaoImpl.MailDao)
            {
                return dao.GetMailsWithAttachment();
            }
        }

        public ICollection<Mail> GetMailsWithoutAttachment()
        {
            using (IMailDao dao = this.getDaoContext().DaoImpl.MailDao)
            {
                return dao.GetMailsWithoutAttachment();
            }
        }

        public ICollection<Mail> GetSentMails()
        {
            using (IMailDao dao = this.getDaoContext().DaoImpl.MailDao)
            {
                return dao.GetSentMails();
            }
        }

        public ICollection<Mail> GetUnsentMails()
        {
            using (IMailDao dao = this.getDaoContext().DaoImpl.MailDao)
            {
                return dao.GetUnsentMails();
            }
        }


        public void Update(Mail mail)
        {
            using (IMailDao dao = this.getDaoContext().DaoImpl.MailDao)
            {
                dao.Update(mail);
            }
        }

        public int Save(Mail mail)
        {
            using (IMailDao dao = this.getDaoContext().DaoImpl.MailDao)
            {
                dao.Insert(mail);
                return (int)mail.IdMail;
            }
        }

        public void SendMail(long idRubrica)
        {
            throw new NotImplementedException();
        }

        public void SaveAllMails(ICollection<Mail> mails)
        {
            if (mails != null)
            {
                foreach (Mail m in mails)
                {
                    Save(m);
                }
            }
        }

        public Mail GetMailByComId(string comId)
        {
            Mail mail = null;
            using (IMailMessageDao dao = this.getDaoContext().DaoImpl.MailMessageDao)
            {
                Message msg = dao.GetOutBoxMessageByComId(comId);

                if (msg != null)
                {
                    mail = new Mail()
                    {
                        IdMail = msg.Id,
                        MailSender = msg.From.Email,
                        Subject = msg.Subject,
                        MailText = msg.BodyHtml.Text,
                        SendDate = msg.Date 
                    };

                    if (msg.To.Count > 0)
                    {
                        mail.Refs = new List<SendMail.Model.MailRefs>();
                        foreach (Address a in msg.To)
                        {
                            SendMail.Model.MailRefs mr = new SendMail.Model.MailRefs()
                            {
                                AddresseeMail = a.Email
                            };
                            mail.Refs.Add(mr);
                        }
                    }

                    if (msg.Attachments.Count > 0)
                    {
                        mail.Attachments = new List<ComAllegato>();
                        foreach (MimePart att in msg.Attachments)
                        {
                            ComAllegato ca = new ComAllegato
                            {
                                AllegatoName = att.Filename
                            };
                            mail.Attachments.Add(ca);
                        }
                    }
                }

            }
            return mail;
        }
    }
}
