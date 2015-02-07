using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.WebPages;
using RP.Saude.Web.Controllers;
using RP.Sistema.Web.Helpers;
using RP.Sistema.Web.Repository;

namespace RP.Sistema.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteTable.Routes.MapHubs();
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DeviceConfig.RegisterDevices(DisplayModeProvider.Instance.Modes);

            ModelBinders.Binders.Add(typeof(DateTime), new RP.Util.DateTimeBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new RP.Util.DateTimeBinder());
        }

        protected void Session_Start()
        {
            Auth.Class.License.Register();
        }

        protected void Application_End()
        {
            //UserRepository.save();
        }

        protected void Application_Disposed()
        {
            //UserRepository.save();        
        }

        protected void Application_EndRequest()
        {
            var context = new HttpContextWrapper(Context);
            if (context.Response.StatusCode == 302 && context.Request.IsAjaxRequest())
            {
                context.Response.Clear();
                context.Response.StatusCode = 401;
            }
        }

        protected void Session_End()
        {
            UserRepository.remove(Session.SessionID);
            Auth.Class.License.SessionEnd();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if (exception is HttpAntiForgeryException)
            {
                Server.ClearError();

                RouteData routeData = new RouteData();
                routeData.Values.Add("controller", "Erro");
                routeData.Values.Add("action", "CookieDisabled");

                IController controller = new ErroController();
                controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            }
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                CustomPrincipalSerialized serializeModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomPrincipalSerialized>(authTicket.UserData);

                //if (FormsAuthentication.SlidingExpiration)
                //{
                //    authCookie.Expires = DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes);
                //    HttpContext.Current.Response.SetCookie(authCookie);
                //}

                CustomPrincipal user = new CustomPrincipal(authTicket.Name);
                user.Id = serializeModel.Id;
                user.Nome = serializeModel.Nome;
                user.Login = serializeModel.Login;
                user.ConnectionId = serializeModel.ConnectionId;

                HttpContext.Current.User = user;
            }
        }
    }
}