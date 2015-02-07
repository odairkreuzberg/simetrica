using System.Web.Mvc;

namespace RP.Sistema.Web.Controllers
{
    public class AlertController : Controller
    {
        //
        // GET: /Alert/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult NaoAutorizado()
        {
            return View();
        }

        public ActionResult SemLicenca()
        {
            return View();
        }

    }
}
