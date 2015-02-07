using RP.Sistema.Model;
using RP.Sistema.Web.Models.Parametro;
using RP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace RP.Sistema.Web.Controllers
{
    public class ParametroController : Controller
    {
        private readonly int _idUsuario;

        public ParametroController()
        {
            _idUsuario = Helpers.Helper.UserId;
        }

        #region ActionResult

        [PersistDataSearch("Search")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            return View();
        }

        [PersistDataSearch]
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Search(string filter, int? page, int? pagesize)
        {
            try
            {
                using (var db = new Context())
                {
                    var parametroBLL = new BLL.ParametroBLL(db, _idUsuario);
                    var result = parametroBLL.Search(filter, page, pagesize);

                    return View("Index", result);
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Details(string parametros)
        {
            try
            {
                using (var db = new Context())
                {
                    var parametroBLL = new BLL.ParametroBLL(db, _idUsuario);
                    var parametro = parametroBLL.FindSingle(u => u.nmParametro == parametros);
                    var model = ParametroVM.E2VM(parametro);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(string parametros)
        {
            try
            {
                using (var db = new Context())
                {
                    var parametroBLL = new BLL.ParametroBLL(db, _idUsuario);
                    var parametro = parametroBLL.FindSingle(u => u.nmParametro == parametros);

                    var model = ParametroVM.E2VM(parametro);
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(ParametroVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var parametro = model.VM2E();

                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var parametroBLL = new BLL.ParametroBLL(db, _idUsuario);

                            parametroBLL.Update(parametro);
                            parametroBLL.SaveChanges();
                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.EDIT_SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        #endregion

    }
}
