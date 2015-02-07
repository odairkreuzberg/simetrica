using System;
using System.Linq;
using System.Web.Mvc;
using RP.Util;

namespace RP.Saude.Web.Controllers
{
    public class ErroController : Controller
    {
        //
        // GET: /Erro/
        public ActionResult Index()
        {
            this.AddFlashMessage("<strong>Erro:</strong> Uma exceção foi disparada pelo sistema, verifique os detalhes do erro.", FlashMessage.ERROR);
            return View(RP.Util.Entity.ErroLog.GetLast(Session.SessionID));
        }

        public ActionResult JsDisabled(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        public ActionResult CookieDisabled()
        {
            return View();
        }

        public JsonResult List()
        {
            try
            {
                var result = RP.Util.Entity.ErroLog.List();

                var list = result.Select(s => new
                {
                    dtErro = s.dtErro.ToString("dd/MM/yyyy hh:mm:ss"),
                    s.idUsuario,
                    s.dsMessage,
                    s.dsInner,
                    s.dsTrace
                });

                return Json(new Util.Class.JsonCollection { result = list, count = result.Count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(RP.Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }        
        }

        

    }
}
