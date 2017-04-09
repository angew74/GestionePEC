using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Repository
{
   public class MailSQLDb :IMailDao
    {
        #region IMail Membri di

        public ICollection<Mail> GetMailsWithAttachment()
        {
            List<Mail> mailList = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = cmdSelectMailWithAttachment;
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            mailList = new List<Mail>();
                            while (r.Read())
                            {
                                mailList.Add((Mail)DataMapper.FillObject(r, typeof(Mail)));
                            }
                        }
                    }
                }
            }
            catch
            {
                mailList = null;
            }
            return mailList;
        }

        public ICollection<Mail> GetMailsWithoutAttachment()
        {
            List<Mail> mailList = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = cmdSelectMailWithoutAttachment;
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            mailList = new List<Mail>();
                            while (r.Read())
                            {
                                mailList.Add((Mail)DataMapper.FillObject(r, typeof(Mail)));
                            }
                        }
                    }
                }
            }
            catch
            {
                mailList = null;
            }
            return mailList;
        }

        public ICollection<Mail> GetSentMails()
        {
            List<Mail> mailList = null;
             using (var dbcontext = new FAXPECContext())
                {

                    try { 
                       dbcontext.COMUNICAZIONI.Where(x=>x.)
                        using (OracleDataReader r = oCmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                mailList = new List<Mail>();
                                while (r.Read())
                                {
                                    mailList.Add((Mail)DataMapper.FillObject(r, typeof(Mail)));
                                }
                            }
                        
                    }
                    catch
                    {
                        mailList = null;
                    }

                }
            }
            return mailList;
        }

        public ICollection<Mail> GetUnsentMails()
        {
            List<Mail> mailList = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = cmdSelectUnsentMail;
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            mailList = new List<Mail>();
                            while (r.Read())
                            {
                                mailList.Add((Mail)DataMapper.FillObject(r, typeof(Mail)));
                            }
                        }
                    }
                }

                foreach (Mail mail in mailList)
                {
                    mail.Refs = this.Context.DaoImpl.MailRefsDao.GetMailRefsOfAMail(mail.IdMail);
                }
            }
            catch
            {
                mailList = null;
            }
            return mailList;
        }

        #endregion

        #region IDao<Mail,long> Membri di

        public ICollection<Mail> GetAll()
        {
            List<Mail> mailList = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = cmdSelectAll;
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            mailList = new List<Mail>();
                            while (r.Read())
                            {
                                mailList.Add((Mail)DataMapper.FillObject(r, typeof(Mail)));
                            }
                        }
                    }
                }

                foreach (Mail m in mailList)
                {
                    m.Refs = this.Context.DaoImpl.MailRefsDao.GetMailRefsOfAMail(m.IdMail);
                }
            }
            catch
            {
                mailList = null;
            }
            return mailList;
        }

        public Mail GetById(long id)
        {
            Mail mail = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = cmdSelectMailById;
                    oCmd.Parameters.Add("IdMail", id);

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            mail = (Mail)DataMapper.FillObject(r, typeof(Mail));
                        }
                    }
                }
            }
            catch
            { }
            return mail;
        }

        public void Insert(Mail entity)
        {
            //int insertValue = 0;
            //try
            //{
            //    this.Context.StartTransaction(typeof(MailOracleDb));
            //    using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            //    {
            //        oCmd.CommandText = cmdInsertMail;
            //        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            //        OracleParameter codapp = new OracleParameter("V_CODICEAPPLICAZIONE", OracleDbType.Varchar2, 15, entity.CodiceApplicazione, ParameterDirection.Input);
            //        OracleParameter utente = new OracleParameter("V_UTENTE", OracleDbType.Varchar2, 11, entity.UtenteInserimento, ParameterDirection.Input);
            //        OracleParameter testo = new OracleParameter("V_TESTO", OracleDbType.Varchar2, 500, entity.MailText, ParameterDirection.Input);
            //        OracleParameter notify = new OracleParameter("V_TONOTIFY", OracleDbType.Varchar2, 1, "0", ParameterDirection.Input);
            //        if (entity.IsToNotify == true)
            //        {
            //            notify.Value = "1";
            //        }
            //        OracleParameter oggetto = new OracleParameter("V_OGGETTO", OracleDbType.Varchar2, 500, entity.Subject, ParameterDirection.Input);
            //        OracleParameter mailmittente = new OracleParameter("V_MAILMITTENTE", OracleDbType.Varchar2, 100, entity.MailMittente.ToUpper().Trim(), ParameterDirection.Input);
            //        OracleParameter allegati = new OracleParameter("V_ALLEGATI", OracleDbType.Varchar2, 1, "0", ParameterDirection.Input);
            //        if (entity.HasAttachment == true)
            //        {
            //            allegati.Value = "1";
            //        }
            //        OracleParameter idmail = new OracleParameter("V_ID_MAIL", OracleDbType.Decimal, 1, null, ParameterDirection.Output);
            //        oCmd.Parameters.Add(codapp);
            //        oCmd.Parameters.Add(utente);
            //        oCmd.Parameters.Add(testo);
            //        oCmd.Parameters.Add(notify);
            //        oCmd.Parameters.Add(oggetto);
            //        oCmd.Parameters.Add(mailmittente);
            //        oCmd.Parameters.Add(allegati);
            //        oCmd.Parameters.Add(idmail);
            //        OracleParameter mess = new OracleParameter("V_MESS", OracleDbType.NVarchar2, 200, null, ParameterDirection.Output);
            //        oCmd.Parameters.Add(mess);
            //        oCmd.ExecuteNonQuery();
            //        insertValue = int.Parse(oCmd.Parameters["V_ID_MAIL"].Value.ToString());                   
            //        if (insertValue < 1)
            //        {
            //            throw new Exception("Errore aggiornamento righe");
            //        }
            //    }


            //}
            //catch (Exception ex)
            //{
            //    this.Context.RollBackTransaction(typeof(MailOracleDb));
            //}
            //entity.IdMail = insertValue;
        }

        public int Save(Mail entity)
        {
            //int insertValue = 0;
            //try
            //{
            //    this.Context.StartTransaction(typeof(MailOracleDb));               
            //    using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            //    {
            //        oCmd.CommandText = cmdInsertMail;
            //        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            //        OracleParameter codapp = new OracleParameter("V_CODICEAPPLICAZIONE", OracleDbType.Varchar2,15, entity.CodiceApplicazione,ParameterDirection.Input);
            //        OracleParameter utente = new OracleParameter("V_UTENTE", OracleDbType.Varchar2,11, entity.UtenteInserimento,ParameterDirection.Input);
            //        OracleParameter testo = new OracleParameter("V_TESTO", OracleDbType.Varchar2, 500, entity.MailText, ParameterDirection.Input);
            //        OracleParameter notify = new OracleParameter("V_TONOTIFY", OracleDbType.Varchar2, 1, "0", ParameterDirection.Input);
            //        if(entity.IsToNotify == true)
            //        {
            //            notify.Value = "1";
            //        }
            //        OracleParameter oggetto = new OracleParameter("V_OGGETTO", OracleDbType.Varchar2, 500, entity.Subject, ParameterDirection.Input);
            //        OracleParameter mailmittente = new OracleParameter("V_MAILMITTENTE", OracleDbType.Varchar2, 100, entity.MailMittente.ToUpper().Trim(), ParameterDirection.Input);
            //        OracleParameter allegati = new OracleParameter("V_ALLEGATI", OracleDbType.Varchar2, 1, "0", ParameterDirection.Input);
            //        if (entity.HasAttachment == true)
            //        {
            //            allegati.Value = "1";
            //        }
            //        OracleParameter idmail = new OracleParameter("V_ID_MAIL", OracleDbType.Decimal, 1, null, ParameterDirection.Output);
            //        oCmd.Parameters.Add(codapp);
            //        oCmd.Parameters.Add(utente);
            //        oCmd.Parameters.Add(testo);
            //        oCmd.Parameters.Add(notify);
            //        oCmd.Parameters.Add(oggetto);
            //        oCmd.Parameters.Add(mailmittente);
            //        oCmd.Parameters.Add(allegati);
            //        oCmd.Parameters.Add(idmail);
            //        OracleParameter mess = new OracleParameter("V_MESS", OracleDbType.NVarchar2, 200, null, ParameterDirection.Output);
            //        oCmd.Parameters.Add(mess);
            //        oCmd.ExecuteNonQuery();
            //        insertValue = int.Parse(oCmd.Parameters["V_ID_MAIL"].Value.ToString());
            //        if (insertValue < 1)
            //        {
            //            throw new Exception("Errore aggiornamento righe");
            //        } 
            //    }


            //}
            //catch(Exception ex)
            //{
            //    this.Context.RollBackTransaction(typeof(MailOracleDb));
            //}
            //return insertValue;
            return 0;
        }

        public void Update(Mail entity)
        {
            //try
            //{
            //    this.Context.StartTransaction(typeof(MailOracleDb));

            //    using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            //    {
            //        oCmd.CommandText = cmdUpdateMail;

            //        OracleParameter pFlgAnnullamento = new OracleParameter("FlgAnnullamento", OracleDbType.Int16);
            //        oCmd.Parameters.Add(pFlgAnnullamento);
            //        if (entity.IsCanceled)
            //            pFlgAnnullamento.Value = 1;
            //        else pFlgAnnullamento.Value = 0;

            //        oCmd.Parameters.Add("StatusInvio", (Int16)entity.SendStatus);
            //        oCmd.Parameters.Add("NumInvio", entity.SendNumber);

            //        OracleParameter pSendDate = new OracleParameter("Datainvio", OracleDbType.Date);
            //        oCmd.Parameters.Add(pSendDate);
            //        if (entity.SendDate.HasValue)
            //            pSendDate.Value = entity.SendDate.Value;
            //        else pSendDate.Value = DBNull.Value;

            //        OracleParameter pFlgNotifica = new OracleParameter("FlgNotifica", OracleDbType.Int16);
            //        oCmd.Parameters.Add(pFlgNotifica);
            //        if (entity.IsToNotify)
            //            pFlgNotifica.Value = 1;
            //        else pFlgNotifica.Value = 0;

            //        oCmd.Parameters.Add("IdMail", entity.IdMail);

            //        int updateRows = oCmd.ExecuteNonQuery();
            //        if (updateRows == 0)
            //        {
            //            throw new Exception("Errore aggiornamento righe");
            //        }

            //        this.Context.DaoImpl.Allegato.UpdateAttachments(entity.Attachments, false);
            //    }

            //    this.Context.EndTransaction(typeof(MailOracleDb));
            //}
            //catch
            //{
            //    this.Context.RollBackTransaction(typeof(MailOracleDb));
            //}
        }

        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
