using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Sistema.Web.Models.Controle;
using RP.Util;

namespace RP.Sistema.Web.Controllers
{
    public class ControleController : Controller
    {
        private int _idUsuario;

        public ControleController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

        #region ActionResult

        [PersistDataSearch("Search")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            return View();
        }

        [PersistDataSearch]
        [Auth.Class.Auth("sistema", "padrao","Index")]
        public ActionResult Search(string filter, int? page, int? pagesize)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.ControleBLL controleBLL = new BLL.ControleBLL(db, _idUsuario);
                    var result = controleBLL.Search(filter, page, pagesize);

                    return View("Index", result);
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", new {@clear = true });
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Details(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.ControleBLL controleBLL = new BLL.ControleBLL(db, _idUsuario);
                    //Controle controle = controleBLL.Select(id);
                    Controle controle = controleBLL.FindSingle(u => u.idControle == id, q => q.Area);

                    return View(ControleVM.E2VM(controle));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(ControleVM viewData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Controle controle = viewData.VM2E();

                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            BLL.ControleBLL controleBLL = new BLL.ControleBLL(db, _idUsuario);
                            controleBLL.Insert(controle);
                            controleBLL.SaveChanges();
                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.INSERT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                    return RedirectToAction("Index");
                }

            }

            return View(viewData);
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.ControleBLL controleBLL = new BLL.ControleBLL(db, _idUsuario);
                    //Controle controle = controleBLL.Select(id);
                    Controle controle = controleBLL.FindSingle(u => u.idControle == id, q => q.Area);

                    return View(ControleVM.E2VM(controle));
                }

            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(ControleVM viewData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Controle controle = viewData.VM2E();

                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            BLL.ControleBLL controleBLL = new BLL.ControleBLL(db, _idUsuario);
                            controleBLL.Update(controle);
                            controleBLL.SaveChanges();
                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.EDIT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                return View(viewData);
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Delete(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.ControleBLL controleBLL = new BLL.ControleBLL(db, _idUsuario);
                    //Controle controle = controleBLL.Select(id);
                    Controle controle = controleBLL.FindSingle(u => u.idControle == id);

                    return View(ControleVM.E2VM(controle));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ActionName("Delete")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        BLL.ControleBLL controleBLL = new BLL.ControleBLL(db, _idUsuario);
                        controleBLL.Delete(u => u.idControle == id);
                        controleBLL.SaveChanges();
                        trans.Complete();

                        this.AddFlashMessage(RP.Util.Resource.Message.DELETE_SUCCESS, FlashMessage.SUCCESS);
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Delete", id);
            }
        }

        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Report(string filter)
        {
            try
            {
                using (var db = new Context())
                {
                    return new Report.Class.Controle().GetReport(db, filter, _idUsuario);
                }

            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }
        #endregion

        #region JsonResult

        [Auth.Class.Auth(true)]
        public JsonResult JsSearch(string filter, int? page, int? pagesize)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.ControleBLL controleBLL = new BLL.ControleBLL(db, _idUsuario);
                    var result = controleBLL.Search(filter, page, pagesize);
                    var list = result.Select(s => new
                    {
                        s.idControle,
                        s.nmControle,
                        s.dsControle,
                        s.Area.nmArea
                    });

                    return Json(new Util.Class.JsonCollection { result = list, count = result.TotalCount }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(RP.Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}

