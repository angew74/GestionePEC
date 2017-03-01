


using System;
using System.Web;
using log4net;
using System.Web.Routing;
using System.Xml.Linq;
using GestionePEC.Master;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Messaging.WebMessages;
using GestionePEC.Routing;
using Com.Delta.Security;

namespace GestionePEC.Extensions
{
    public delegate void ControlEventHandler(object sender, object EventArgs);

        public partial class BasePage : System.Web.UI.Page, IRoutablePage
        {

            private static readonly ILog log = LogManager.GetLogger("BasePage");

            #region IRoutablePage Membri di

            private RequestContext requestContext;
            public RequestContext RequestContext
            {
                get { return requestContext; }
                set { requestContext = value; }
            }

            #endregion

            #region Routing
            public virtual string VirtualPath(string path, RouteValueDictionary routeDictionary)
            {
                var contextWrapper = new HttpContextWrapper(HttpContext.Current);
                var virtualPath = RouteTable.Routes.GetVirtualPath(
                    requestContext ?? new RequestContext(
                        contextWrapper,
                        RouteTable.Routes.GetRouteData(contextWrapper) ?? new RouteData()),
                        path, routeDictionary);
                return virtualPath.VirtualPath;
            }

            protected virtual void Redirect(string path, RouteValueDictionary routeDictionary)
            {
                var virtualPath = VirtualPath(path, routeDictionary);
                Response.Redirect(virtualPath, true);
            }
            #endregion


            //public Info info = new Info(Com.Delta.Web.Cache.CacheKeys.LISTA_DECODIFICA_MESSAGGI);

            private const string MSG_SESSION_KEY = "EUIDFXJKDNDUD78404KRN7FYUCFWIDK";
            private Info _info;

            /// <summary>
            /// 
            /// </summary>
            public Info info
            {
                get
                {
                    return _info;
                }
                set
                {
                    _info = value;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnLoad(EventArgs e)
            {
                if (Session[MSG_SESSION_KEY] != null) this._info = (Info)Session[MSG_SESSION_KEY];
                else
                {

                    _info = new Info(Com.Delta.Web.Cache.CacheKeys.LISTA_DECODIFICA_MESSAGGI);
                    Session.Add(MSG_SESSION_KEY, _info);
                }

                if (HttpContext.Current.Request["__EVENTARGUMENT"] == "XButton")
                {
                    ClearAll();
                }



                base.OnLoad(e);
            }

            protected System.Web.UI.WebControls.Panel InfoBox;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected virtual void Page_PreRender(object sender, EventArgs e)
            {
                //Why this in prerender?

                IBaseMaster myMaster = (IBaseMaster)this.Master;


                if (info != null && info.messageCount() > 0)
                {
                    string msg = info.renderMessage(this.Page);
                    myMaster.ShowMessageList(msg, true);
                } //else aggiunto da Apisa 14/02/2011
                else if (info != null && info.messageCount() == 0)
                {
                    string msg = info.renderMessage(this.Page);
                    myMaster.ShowMessageList(msg, false);
                }
                Session.Remove(MSG_SESSION_KEY);

            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected void Page_Error(object sender, EventArgs e)
            {
                Exception e0 = Context.Error;

                if (e0.GetType() == typeof(ManagedException))
                {
                    //se è di tipo ManagedException sicuramente è già stata loggata

                    if (info != null)
                    {
                        info.AddMessage("#ERR_P01: La pagina non può essere visualizzata ", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                        info.AddMessage("DETTAGLI: " + e0.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.DETAILS);
                    }
                }
                else
                {
                    //TASK: Allineamento log - Ciro
                    string msg = e0.Message;
                    msg += (e0.InnerException != null) ? " >> InnerException: " + e0.InnerException.Message : " >> InnerException: vuoto. ";
                    msg += (e0.StackTrace != null) ? " >> StackTrace: " + e0.StackTrace : " >> StackTrace: vuoto. ";
                    ManagedException tempEx = new ManagedException(msg,
                        "ERR005",
                        string.Empty,
                        string.Empty,
                        e0.InnerException);
                    tempEx.addEnanchedInfosTag("DETAILS", new XElement("info",
                        new XElement("user_msg", "La pagina non può essere visualizzata. Dettaglio: " + e0.Message),
                        new XElement("exception",
                            new XElement("message", e0.Message),
                            new XElement("source", e0.Source),
                            new XElement("stack", e0.StackTrace),
                            new XElement("innerException", e0.InnerException))).ToString(SaveOptions.DisableFormatting));
                    ErrorLogInfo err = new ErrorLogInfo(tempEx);
                    log.Error(err);
                    if (info != null)
                    {
                        info.AddMessage("#ERR_P02: La pagina non può essere visualizzata ", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                        info.AddMessage("DETTAGLI: " + e0.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.DETAILS);
                    }
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <param name="propertyName"></param>
            /// <param name="nullValue"></param>
            /// <returns></returns>
            protected V GetPropertyValue<V>(string propertyName, V nullValue)
            {
                if (this.ViewState[propertyName] == null)
                {
                    return nullValue;
                }
                return (V)this.ViewState[propertyName];
            }


            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <param name="propertyName"></param>
            /// <param name="value"></param>
            protected void SetPropertyValue<V>(string propertyName, V value)
            {
                this.ViewState[propertyName] = value;
            }



            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnPreInit(EventArgs e)
            {
                base.OnPreInit(e);               
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="role"></param>
            /// <param name="procedura"></param>
            /// <returns></returns>
            protected bool IsPageEditable(string role, string procedura)
            {
                bool fResult = false;

                //test sul diritto di lavorare su questa procedura
                if (!MySecurityProvider.CheckAccessRight(procedura))
                {
                    System.Web.Security.FormsAuthentication.SignOut();
                    Session.Abandon();
                    //TODO:aggiungere il log
                    //Response.Redirect("~/Login.aspx", true);
                    Redirect("LoginPage", null);
                }
                else
                {
                    fResult = !string.IsNullOrEmpty(role) && MySecurityProvider.CheckAccessRight(role);
                }

                return fResult;
            }

            protected void ClearAll()
            {
                if (MySecurityProvider.CurrentPrincipal == null)
                    return;
                string user = MySecurityProvider.CurrentPrincipal.Identity.Name;
                string dip = MySecurityProvider.CurrentPrincipal.MyIdentity.dipartimento;
                //BUSPraticaCRI pratica = new BUSPraticaCRI();
                //pratica.UnLock<MessaggiResponse>("", user, dip);
                //Response.Redirect("~/logoff.aspx");
                Cache.Remove(Context.User.Identity.Name);
                System.Threading.Thread.CurrentPrincipal = null;
                Context.User = null;
                System.Web.Security.FormsAuthentication.SignOut();
                Session.Abandon();

            }
        }
    }

