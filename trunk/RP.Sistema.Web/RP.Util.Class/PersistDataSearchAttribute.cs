using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace RP.Util
{

    public class PersistDataSearchAttribute : ActionFilterAttribute
    {
        private bool _load;
        private string _actionName;
        private string _actionRedirect;

        /// <summary>
        /// </summary>
        public PersistDataSearchAttribute()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="ActionName">Nome do método que contém os valores de atributos. Se existirem valores, será redirecionado via GET para este mesmo método.</param>
        public PersistDataSearchAttribute(string ActionName)
        {
            _load = !string.IsNullOrEmpty(ActionName);
            _actionName = ActionName;
            _actionRedirect = ActionName;
        }

        /// <summary>
        /// </summary>
        /// <param name="ActionName">Nome do método que contém os valores de atributos.</param>
        /// <param name="ActionRedirect">Método que a requisição será redirecionada.</param>
        public PersistDataSearchAttribute(string ActionName, string ActionRedirect)
        {
            _load = !string.IsNullOrEmpty(ActionName) && !string.IsNullOrEmpty(ActionRedirect);
            _actionName = ActionName;
            _actionRedirect = ActionRedirect;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string SessionName;

            // se estiver definido para carregar os atributos
            if (_load)
            {
                // define o nome da sessao com o action name informado no construtor
                SessionName = string.Format("{0}.{1}", filterContext.Controller, _actionName);
                // mantem o valor que define se é para apagar o valor da sessao
                bool Clear = false;

                // verifica nas chaves da query string se existe uma com o nome "clear"
                if (filterContext.HttpContext.Request.QueryString.AllKeys.Contains("clear"))
                {
                    // verifica se o valor da chave é "true"
                    //if (filterContext.HttpContext.Request.QueryString["clear"].ToLower() == "true")
                    //{ 
                    //}

                    // limpa sessao
                    filterContext.HttpContext.Session[SessionName] = null;
                    // define valor que foi apagado os valores da sessao
                    Clear = true;
                }

                // se não for setado para limpar
                if (!Clear)
                {
                    // armazena o valor da sessao com o nome definido
                    string url = (string)filterContext.HttpContext.Session[SessionName];

                    if(url != null && !string.IsNullOrEmpty(url))
                    {
                        filterContext.Result = new RedirectResult(url);
                    }
                }
            }
            // senao
            else
            {
                // define o nome da sessao
                SessionName = string.Format("{0}.{1}", filterContext.Controller, (filterContext.ActionDescriptor).ActionName);

                // armazena na sessao os parametros do action
                if (filterContext.RequestContext.HttpContext.Request.UrlReferrer != null && !filterContext.RequestContext.HttpContext.Request.UrlReferrer.IsDefaultPort)
                {
                    var port = filterContext.RequestContext.HttpContext.Request.UrlReferrer.Port;
                    var url = filterContext.RequestContext.HttpContext.Request.Url.ToString();
                    var local = filterContext.RequestContext.HttpContext.Request.Url.LocalPath;
                    filterContext.HttpContext.Session[SessionName] = url.Insert(url.IndexOf(local), ":" + port.ToString());
                }
                else 
                {
                    filterContext.HttpContext.Session[SessionName] = filterContext.RequestContext.HttpContext.Request.Url.ToString();
                }
            }
        }
    }

}
