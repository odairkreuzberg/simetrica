using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RP.Auth.Class
{

    public class AuthAttribute : ActionFilterAttribute
    {
        private string _user;
        private string _moduloName;
        private string _areaName;
        private string[] _actionNames;
        private string _actionName;
        private bool _flAuthenticateOnly;
        private bool _flJson;


        public AuthAttribute()
        {
            _flAuthenticateOnly = true;
            _moduloName = string.Empty;
            _areaName = string.Empty;
            _actionName = string.Empty;
            _flJson = false;
        }

        public AuthAttribute(string modulo, string area)
        {
            _moduloName = modulo;
            _areaName = area;
            _actionName = string.Empty;
            _flAuthenticateOnly = false;
            _flJson = false;
        }

        public AuthAttribute(string modulo, string area, string actionName)
        {
            _moduloName = modulo;
            _areaName = area;
            _actionName = actionName;
            _flAuthenticateOnly = false;
            _flJson = false;
        }

        public AuthAttribute(string modulo, string area, string[] actionNames)
        {
            _moduloName = modulo;
            _areaName = area;
            _actionNames = actionNames;
            _flAuthenticateOnly = false;
            _flJson = false;
        }

        public AuthAttribute(string modulo, string area, string actionName, bool json)
        {
            _moduloName = modulo;
            _areaName = area;
            _actionName = actionName;
            _flAuthenticateOnly = false;
            _flJson = json;
        }

        public AuthAttribute(bool flJson)
        {
            _flJson = true;
            _flAuthenticateOnly = true;
            _moduloName = string.Empty;
            _areaName = string.Empty;
            _actionName = string.Empty;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _user = System.Web.HttpContext.Current.User.Identity.Name.ToString();

            if (filterContext.RequestContext.HttpContext.Request["autorizado"] != "true")
            {

                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    // Removido para não salvar redirecionamento após o login
                    //string redirectOnSuccess = filterContext.HttpContext.Request.Url.AbsolutePath;
                    //string redirectUrl = string.Format("?returnUrl={0}", redirectOnSuccess);
                    string loginUrl = FormsAuthentication.LoginUrl/* + redirectUrl*/;

                    if (_flJson)
                    {
                        filterContext.HttpContext.Response.StatusCode = 401;
                    }
                    else
                    {
                        filterContext.HttpContext.Response.Redirect(loginUrl, true);
                    }
                }
                else
                {
                    Auth.Class.License.UseLicense(_user);

                    if (!this._flAuthenticateOnly)
                    {
                        string _key = string.Empty;

                        HttpCookie cookie = filterContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];

                        if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                        {
                            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(ticket.UserData);
                            _key = obj.ConnectionId;
                        }

                        bool isAuthorized = Authorize(_user, _key, filterContext);

                        if (!isAuthorized)
                        {

                            filterContext.HttpContext.Response.Redirect("/Alert/NaoAutorizado" +
                                "?action=" + filterContext.RouteData.Values["Action"] +
                                "&controller=" + filterContext.RouteData.Values["Controller"] +
                                "&area=" + _areaName);
                        }
                        else
                        {
                            if (!Auth.Class.License.HaveLicense())
                            {
                                filterContext.HttpContext.Response.Redirect("/Alert/SemLicenca");
                            }
                        }
                    }
                }
            }
        }

        private bool Authorize(string user, string key, ActionExecutingContext filterContext)
        {
            if(_actionNames == null)
                return AuthModel.check(user, key, new AuthModel.permissao { modulo = this._moduloName, area = this._areaName, controle = filterContext.RouteData.Values["Controller"].ToString(), acao = string.IsNullOrEmpty(_actionName) ? filterContext.RouteData.Values["Action"].ToString() : _actionName });
            foreach (var action in _actionNames)
            {
                
                _actionName = action;
                var result = AuthModel.check(user, key, new AuthModel.permissao { modulo = this._moduloName, area = this._areaName, controle = filterContext.RouteData.Values["Controller"].ToString(), acao = string.IsNullOrEmpty(_actionName) ? filterContext.RouteData.Values["Action"].ToString() : _actionName });
                if (result)
                    return true;
            }
            return false;
        }
    }
}