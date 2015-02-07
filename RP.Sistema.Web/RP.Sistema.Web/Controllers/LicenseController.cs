using System.Collections.Generic;
using System.Web.Mvc;

namespace RP.Sistema.Web.Controllers
{
    public class LicenseController : Controller
    {
        //
        // GET: /Licence/

        public ActionResult Index()
        {
            List<Auth.Class.License.Info> _list = Auth.Class.License.Get();

            return View(_list);
        }

        public JsonResult KeepAlive()
        {
            Auth.Class.License.SetSignal();
            return Json(string.Empty, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Kill(string id)
        {
            Auth.Class.License.kill(id);

            return RedirectToAction("Index");
        }

    }
}
