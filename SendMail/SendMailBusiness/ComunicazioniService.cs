using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model.ComunicazioniMapping;
using SendMail.Business.Base;
using SendMail.DataContracts.Interfaces;
using SendMail.Model.RubricaMapping;
using System.Xml;

using System.Xml.Linq;
using System.Collections;
using Com.Delta.Web.Cache;
using System.Data;
using System.Collections.Specialized;
using SendMail.Model.WebserviceMappings;
using Com.Delta.Print;
using Com.Delta.Print.Engine;
using Com.Delta.Web.Session;

namespace SendMail.Business
{
    public class ComunicazioniService : BaseSingletonService<ComunicazioniService>, SendMail.Business.Contracts.IComunicazioniService
    {

        #region "Reports"

        public ICollection<StatoComunicazioneItem> GetComunicazioneStatus(string originalUid)
        {
            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                return dao.GetStatoComunicazione(originalUid);
            }
        }

        #endregion

        #region "Comunicazioni Mapper"

        //public SendMail.Model.RawMessage LoadRawMessage(string appCode, string uid)
        //{
        //    HTF.CR.Mail.MailServiceMapper service = new HTF.CR.Mail.MailServiceMapper();
        //    SendMail.Model.RawMessage msg = service.GetStampe(appCode, uid);
        //    return msg;
        //}

        //public SendMail.Model.ComunicazioniMapping.Comunicazioni LoadComunicazione(string appCode, string uid)
        //{

        //    HTF.CR.Mail.MailServiceMapper service = new HTF.CR.Mail.MailServiceMapper();
        //    Comunicazioni msg = service.GetStampeComunicazioni(appCode, uid);
        //    return msg;
        //}

        #endregion

        #region "Pdf production service"

        public byte[] GetPdfTpuStampeBUS(string tpu, byte[] pru, string pathFolder)
        {
            XmlDocument tpuDoc = null;
            XmlDocument pruDoc = null;
            string tpuPath = null;
            byte[] resp = null;
            if (string.IsNullOrEmpty(pathFolder))
            {
                string k = System.IO.Path.GetFileNameWithoutExtension(tpu);
                tpuDoc = Com.Delta.Web.Cache.CacheManager<Com.Delta.Web.Cache.Types.TpuFile>.get(
                    (CacheKeys)Enum.Parse(typeof(CacheKeys), k, true), VincoloType.FILESYSTEM);
            }
            else
            {
                tpuPath = tpu;
                if (!String.IsNullOrEmpty(pathFolder))
                {
                    tpuPath = System.IO.Path.Combine(pathFolder, tpu);
                }

                if (System.IO.File.Exists(tpuPath))
                {
                    tpuDoc = new XmlDocument();
                    tpuDoc.Load(tpuPath);
                }
            }

            using (System.IO.Stream str = new System.IO.MemoryStream(pru))
            {
                str.Position = 0;
                pruDoc = new XmlDocument();
                pruDoc.Load(str);
            }

            if (pruDoc != null)
                //   resp = GetPdfTpuStampeTableBUS(tpuDoc, pruDoc);
                resp = GetPdfTpuStampeBUS(tpuDoc, pruDoc);

            return resp;
        }

        public byte[] GetPdfTpuStampeBUS(string appCode, string stringa_id, int progAllegato, string pathFolder)
        {
            Comunicazioni com = SessionManager<Comunicazioni>.get(stringa_id);           
            byte[] resp = null;
            if (com.ComAllegati != null && com.ComAllegati.Count != 0)
            {
                ComAllegato all = com.ComAllegati[progAllegato];

                switch (all.AllegatoExt.ToUpper())
                {
                    case "PDF":
                        resp = all.AllegatoFile;
                        break;

                    case "PRU":
                        resp = GetPdfTpuStampeBUS(all.AllegatoTpu, all.AllegatoFile, pathFolder);
                        break;

                    default:
                        break;
                }
            }

            return resp;
        }

        public byte[] GetPdfTpuStampeBUS(XmlDocument tpu, XmlDocument pru)
        {
            
            Com.Delta.Print.PDFBuilder b = new Com.Delta.Print.PDFBuilder(new PRUBuilder(pru.InnerXml, null));         
            b.setTPU(tpu);          
            System.IO.Stream str = null;
            byte[] ser = null;
            try
            {
                XDocument xTpu = XDocument.Parse(tpu.InnerXml);
                //elementi picture
                var picturesTag = from nd in xTpu.Root.DescendantNodesAndSelf().OfType<XElement>()
                                  where nd.Name.LocalName.Equals("pictureBox")
                                  select nd;
                //estrae gli attributi delle picture
                var pictures = from nd in picturesTag
                               select new
                               {
                                   Name = nd.Attribute("name").Value,
                                   FileName = nd.Element("file").Attribute("value").Value,
                                   Width = int.Parse(nd.Attribute("width").Value),
                                   Heigth = int.Parse(nd.Attribute("height").Value)
                               };
                //aggiunge le picture alla stampa
                foreach (var pic in pictures)
                {
                    String keyImg = System.IO.Path.GetFileNameWithoutExtension(pic.FileName);
                    
                    Com.Delta.Web.Cache.Types.TpuBinaryResource im =
                        CacheManager<Com.Delta.Web.Cache.Types.TpuBinaryResource>.get(
                        (CacheKeys)Enum.Parse(typeof(CacheKeys), keyImg, true),
                        VincoloType.FILESYSTEM);

                    System.IO.MemoryStream ms = new System.IO.MemoryStream(im.File);
                    System.Drawing.Bitmap bb = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(ms);
                    b.AddImage(pic.Name, bb, true);
                }
                ser = b.CreatePDF();
            }
            catch
            {
            }
            return ser;

        }

        public byte[] GetPdfTpuStampeTableBUS(XmlDocument tpu, XmlDocument pru)
        {
            ReportDocument document = new ReportDocument();
            document.setXML(tpu, tpu.DocumentElement.Name);
            System.IO.Stream str = null;
            Hashtable ht = new Hashtable();

            XmlDocument XMLData = new XmlDocument();
            XMLData.LoadXml(pru.OuterXml);

            // Estrae i valori dei parametri
            System.Xml.XmlNodeList nl = XMLData.SelectNodes("params/p");
            foreach (System.Xml.XmlNode xn in nl)
            {
                ht.Add(xn.Attributes["name"].Value, xn.InnerText);
            }

            document.SetParameters(ht);

            // Estrae le tabelle
            DataTable dt = new DataTable();
            StringCollection sCollection = new StringCollection();
            System.Xml.XmlNodeList nlt = XMLData.SelectNodes("params/t");
            foreach (System.Xml.XmlNode xn in nlt)
            {
                sCollection.Add(xn.InnerXml);
            }
            DataSet ds = new DataSet();
            System.IO.MemoryStream memStream;
            System.IO.StreamWriter sWriter;

            foreach (string tabella in sCollection)
            {
                ds.Tables.Clear();
                memStream = new System.IO.MemoryStream();
                sWriter = new System.IO.StreamWriter(memStream);
                sWriter.Write(tabella);

                sWriter.Flush();
                memStream.Position = 0;

                ds.ReadXml(memStream);

                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                }

                sWriter.Dispose();
                memStream.Dispose();
            }
            document.AddData(dt);
            byte[] ser = null;
            try
            {
                XDocument xTpu = XDocument.Parse(tpu.InnerXml);
                //elementi picture
                var picturesTag = from nd in xTpu.Root.DescendantNodesAndSelf().OfType<XElement>()
                                  where nd.Name.LocalName.Equals("pictureBox")
                                  select nd;
                //estrae gli attributi delle picture
                var pictures = from nd in picturesTag
                               select new
                               {
                                   Name = nd.Attribute("name").Value,
                                   FileName = nd.Element("file").Attribute("value").Value,
                                   Width = int.Parse(nd.Attribute("width").Value),
                                   Heigth = int.Parse(nd.Attribute("height").Value)
                               };
                //aggiunge le picture alla stampa
                foreach (var pic in pictures)
                {
                    String keyImg = System.IO.Path.GetFileNameWithoutExtension(pic.FileName);

                    Com.Delta.Web.Cache.Types.TpuBinaryResource im =
                        CacheManager<Com.Delta.Web.Cache.Types.TpuBinaryResource>.get(
                        (CacheKeys)Enum.Parse(typeof(CacheKeys), keyImg, true),
                        VincoloType.FILESYSTEM);

                    System.IO.MemoryStream ms = new System.IO.MemoryStream(im.File);
                    System.Drawing.Bitmap bb = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(ms);
                    document.AddPicture(pic.Name, bb, true);
                }

                ser = document.SerializeToPdfStream();
            }               
            catch
            {
            }
            return ser;
            
        }

        #endregion

        #region IComunicazioniService Membri di

        public Int32 LoadCountComunicazioniInviate(SendMail.Model.TipoCanale tipoCanale, string utente)
        {
            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                return dao.GetCountComunicazioniInviate(tipoCanale, utente);
            }
        }

        public ICollection<StatoComunicazioneItem> GetComunicazioniByProtocollo(ComunicazioniProtocollo prot)
        {

            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                return dao.GetComunicazioniByProtocollo(prot);
            }
        }

        public Int32 LoadCountComunicazioniNonInviate(SendMail.Model.TipoCanale tipoCanale, string utente)
        {
            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                return dao.GetCountComunicazioniNonInviate(tipoCanale, utente);
            }
        }

        public ICollection<Comunicazioni> GetAll()
        {
            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                return dao.GetAll();
            }
        }

        public ICollection<Comunicazioni> LoadComunicazioniInviate(SendMail.Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente)
        {
            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                return dao.GetComunicazioniInviate(tipoCanale, minRec, maxRec, utente);
            }
        }

        public ICollection<Comunicazioni> LoadComunicazioniNonInviate(SendMail.Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente)
        {
            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                return dao.GetComunicazioniNonInviate(tipoCanale, minRec, maxRec, utente);
            }
        }

        public ICollection<Comunicazioni> LoadComunicazioniDaInviare(SendMail.Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente)
        {
            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                return dao.GetComunicazioniDaInviare(tipoCanale, minRec, maxRec, utente);
            }
        }

        public ICollection<Comunicazioni> LoadComunicazioniConAllegati()
        {
            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                return dao.GetComunicazioniConAllegati();
            }
        }

        public ICollection<Comunicazioni> LoadComunicazioniSenzaAllegati()
        {
            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                return dao.GetComunicazioniSenzaAllegati();
            }
        }

        public ComAllegato LoadAllegatoComunicazioneById(long idAllegato)
        {
            using (IComAllegatoDao dao = getDaoContext().DaoImpl.ComAllegatoDao)
            {
                return dao.GetById(idAllegato);
            }
        }

        public Comunicazioni LoadComunicazioneByIdMail(Int64 idMail)
        {
            using (IComunicazioniDao dao = getDaoContext().DaoImpl.ComunicazioniDao)
            {
                return dao.GetComunicazioneByIdMail(idMail);
            }
        }

        public void InsertComunicazione(long idSottotitolo, long idCanale, bool isToNotify, string mailNotifica, string utenteInserimento, IList<ComAllegato> allegati, string mailSender, string oggetto, string testo, IList<RubricaContatti> refs)
        {
            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                dao.Insert(idSottotitolo, idCanale, isToNotify, mailNotifica, utenteInserimento, allegati, mailSender, oggetto, testo, refs);
            }
        }

        public void InsertComunicazione(Comunicazioni entity)
        {
            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                dao.Insert(entity);
            }
        }

        public void UpdateFlussoComunicazione(SendMail.Model.TipoCanale tipoCanale, Comunicazioni comunicazione)
        {
            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                dao.UpdateFlussoComunicazione(tipoCanale, comunicazione);
            }
        }

        public void UpdateAllegati(SendMail.Model.TipoCanale tipoCanale, Comunicazioni comunicazione)
        {
            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                dao.UpdateAllegati(tipoCanale, comunicazione);
            }
        }

        public void UpdateMailBody(long idMail, string mailBody)
        {
            using (IComunicazioniDao dao = this.getDaoContext().DaoImpl.ComunicazioniDao)
            {
                dao.UpdateMailBody(idMail, mailBody);
            }
        }

        #endregion
    }
}
