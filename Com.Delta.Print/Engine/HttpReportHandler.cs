using System;
using System.Collections;
using System.Web;

namespace Com.Delta.Print.Engine
{
	/// <summary>
	/// HTTP Report handler class for use in ASP.NET environment. By using Process() method of this class one
	/// can output Stampa report in HTML format using HttpContext object of the ASP.NET web application.
	/// </summary>
	public class HttpReportHandler
	{

		/// <summary>
		/// Creates new instance of HttpReportHandler class.
		/// </summary>
		public HttpReportHandler()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void Process(HttpContext context, ReportDocument report, string reportName)
		{
			if (context.Request.Params["stampaImage"] != null)
			{
				Hashtable serializers = context.Session["StampaSerializers"] as Hashtable;
				string serializerKey = context.Session.SessionID + "::" + reportName;
				HtmlSerializer serializer = serializers[serializerKey] as HtmlSerializer;

				string imageName = context.Request.Params["stampaImage"];

				int imageNumber = -2;
				try
				{
					imageNumber = Convert.ToInt32(imageName.Substring(5));
				}
				catch(Exception){}

				byte[] content = serializer.Images[imageName] as byte[];

				

				context.Response.ContentType = "img/jpeg";
				context.Response.OutputStream.Write(content, 0, content.Length);
				if (Environment.Version.Major==1)
					context.Response.End();

				if (imageNumber == serializer.Images.Count)
				{
					serializers[serializerKey] = null;
					context.Session["StampaSerializers"] = serializers;
				}
			}
			else
			{

				HtmlSerializer serializer = new HtmlSerializer();
				string query = "";
				foreach (string key in context.Request.QueryString.Keys)
				{
					query += "&" + key + "=" + context.Request.QueryString[key];
				}
				serializer.SetExternalQuery(query);

				string content = serializer.Serialize(report, false); 

				Hashtable serializers;
				if (context.Session["Serializers"] == null)
				{
					serializers = new Hashtable();
				}
				else
				{
					serializers = context.Session["Serializers"] as Hashtable;
				}
				string serializerKey = context.Session.SessionID + "::" + reportName; 
				serializers[serializerKey] = serializer;
				context.Session["StampaSerializers"] = serializers;

				context.Response.ContentType = "text/html";
				context.Response.Output.Write(content);

				if (Environment.Version.Major==1)
					context.Response.End();

            
			}	
		}
	}
}
