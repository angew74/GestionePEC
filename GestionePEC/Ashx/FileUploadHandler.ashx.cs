using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Web.Session;
using GestionePEC.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.SessionState;

namespace GestionePEC.Ashx
{
    /// <summary>
    /// Descrizione di riepilogo per FileUploadHandler
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class FileUploadHandler : IHttpHandler, IRequiresSessionState
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(FileUploadHandler));
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            DTOFileUploadResult result = new DTOFileUploadResult();
            string extension = string.Empty;
            Dictionary<string, DTOFileUploadResult> dict = new Dictionary<string, DTOFileUploadResult>();
            SessionManager<Dictionary<string, DTOFileUploadResult>>.del(SessionKeys.DTO_FILE);
            if (SessionManager<Dictionary<string, DTOFileUploadResult>>.exist(SessionKeys.DTO_FILE))
            {
                dict = SessionManager<Dictionary<string, DTOFileUploadResult>>.get(SessionKeys.DTO_FILE);
            }
            try
            {

                HttpPostedFile file = ParseFile(context);
                if (file != null) { extension = System.IO.Path.GetExtension(file.FileName); }
                if (file != null)
                {
                    if (!(dict.ContainsKey(file.FileName)))
                    {
                        // switch tipo upload                       
                        result = ParseMail(file);
                        dict.Add(file.FileName, result);
                        if (result.errormessage != "Upload non riuscito.")
                        {
                            // salvo in sessione     
                            SessionManager<Dictionary<string, DTOFileUploadResult>>.set(SessionKeys.DTO_FILE, dict);
                            result.FileName = file.FileName;
                            result.success = true;
                            result.CustomData = null;                    
                        }
                    }
                }
            }
            catch (ManagedException mex)
            {
                _log.Error(mex);
                result.success = false;
                result.errormessage = mex.Message;
            }
            catch (System.Web.HttpException wex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = wex.Message;
                error.logCode = "ERR771";
                error.loggingAppCode = "PEC";
                error.loggingTime = System.DateTime.Now;
                error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                _log.Error(error);
                result.success = false;
                result.errormessage = wex.Message;
            }
            finally
            {
                context.Response.Write(JsonResult(result));
                context.ApplicationInstance.CompleteRequest();
            }
        }

        /// <summary>
        /// Serializza in json il risultato dell'operazione.
        /// </summary>
        /// <param name="result">l'oggetto FileUploadResult da serializzare</param>
        /// <returns></returns>
        private string JsonResult(DTOFileUploadResult result)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StringBuilder sbJsonResults = new StringBuilder();
            serializer.Serialize(result, sbJsonResults);
            return sbJsonResults.ToString();
        }

        /// <summary>
        /// Verifica la validità del file caricato.
        /// </summary>
        /// <param name="context"></param>
        /// <remarks>
        /// il controllo delle estensioni valide è demandato al service layer
        /// </remarks>
        /// <returns></returns>
        private HttpPostedFile ParseFile(HttpContext context)
        {

            if (context.Request.Files.Count > 0)
            {
                HttpPostedFile file = context.Request.Files[0];
                if (file.ContentLength > 0)
                {
                    return file;
                }
            }

            return null;
        }
        private DTOFileUploadResult ParseMail(HttpPostedFile file)
        {
            byte[] btotal = ReadFully(file.InputStream);
            DTOFileUploadResult dto = new DTOFileUploadResult();
            string Extension = System.IO.Path.GetExtension(file.FileName);
            string[] args = Extension.Split('.');
            dto.Extension = args[1];
            dto.FileName = file.FileName;
            if (btotal.Length > 0)
            {

                dto.CustomData = btotal;
                dto.success = true;
                dto.errormessage = string.Empty;

            }
            return dto;
        }

        /// <summary>
        /// Legge i bytes del file caricato.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static byte[] ReadFully(Stream input)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                using (Stream requestStream = input)
                {
                    int bufferSize = 1024;
                    byte[] buffer = new byte[bufferSize];
                    int byteCount = 0;
                    while ((byteCount = requestStream.Read(buffer, 0, bufferSize)) > 0)
                    {
                        memStream.Write(buffer, 0, byteCount);
                    }
                    return memStream.ToArray();
                }
            }
        }
    }
}