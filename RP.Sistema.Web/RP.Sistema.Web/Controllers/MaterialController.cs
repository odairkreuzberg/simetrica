using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Sistema.Web.Models.Material;
using RP.Util;
using System.Data;
using System.Runtime.Serialization;
using RP.Sistema.BLL;

namespace RP.Sistema.Web.Controllers
{ 
    public class MaterialController : Controller
    {
        private int _idUsuario;

        public MaterialController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

		#region ActionResult

        [PersistDataSearch("Search")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            LogBLL.Insert(new LogDado("Index", "Material", _idUsuario));
            return View();
        }

        [PersistDataSearch]
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Search(string filter, string saldo, int? page, int? pagesize)
        {
            try
            {
                LogBLL.Insert(new LogDado("Search", "Material", _idUsuario));
                using (var db = new Context())
                {
                    var _bll = new BLL.MaterialBLL(db, _idUsuario);
                    ViewBag.saldo = saldo;

                    var result = _bll.Search(filter, saldo, page, pagesize);

                    return View("Index", result);
				}
            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }		

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Details(int id)
        {
            LogBLL.Insert(new LogDado("Details", "Material", _idUsuario));
            return this.GetView(id);
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create()
        {
            return View();
        } 

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(MaterialVM model)
        {
            if (string.IsNullOrEmpty(model.UnidadeMedida.nome))
            {
                ModelState.AddModelError("UnidadeMedida.nome", "Selecione uma unid. de med.");
            }
            //if (string.IsNullOrEmpty(model.Fabricante.nome))
            //{
            //    ModelState.AddModelError("Fabricante.nome", "Selecione um fabricante");
            //}
            if (ModelState.IsValid)
            {
                try
                {
                    LogBLL.Insert(new LogDado("Create", "Material", _idUsuario));
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _material = model.GetMaterial();

                            var _bll = new BLL.MaterialBLL(db, _idUsuario);

                            _bll.Insert(_material);
                            _bll.SaveChanges();

                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.INSERT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                    return View(model);   
                }
            }
            return View(model);
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(int id)
        {
            return this.GetView(id);
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(MaterialVM model)
        {
            if (string.IsNullOrEmpty(model.UnidadeMedida.nome))
            {
                ModelState.AddModelError("UnidadeMedida.nome", "Selecione uma unid. de med.");
            }
            //if (string.IsNullOrEmpty(model.Fabricante.nome))
            //{
            //    ModelState.AddModelError("Fabricante.nome", "Selecione um fabricante");
            //}
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _material = model.GetMaterial();

                            var _bll = new BLL.MaterialBLL(db, _idUsuario);

                            _bll.Update(_material);
                            _bll.SaveChanges();

                            LogBLL.Insert(new LogDado("Edit", "Material", _idUsuario));
                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.EDIT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                    return View(model);   
                }
            }
            return View(model);   
		}

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Delete(int id)
        {
            return this.GetView(id);
        }

        [HttpPost, ActionName("Delete")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult DeleteConfirmed(int id)
        {            
            try
            {
                LogBLL.Insert(new LogDado("DeleteConfirmed", "Material", _idUsuario));
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.MaterialBLL(db, _idUsuario);

                        _bll.Delete(e => e.idMaterial == id);
                        _bll.SaveChanges();

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
        public ActionResult Report(string filter, string saldo)
        {
            try
            {
                LogBLL.Insert(new LogDado("Report", "Material", _idUsuario));
                using (var db = new Context())
                {
                    return new Report.Class.Material().GetReport(db, filter, saldo, _idUsuario);
                }

            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

		#endregion

        #region methodPrivatre

        private ActionResult GetView(int id)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.MaterialBLL(db, _idUsuario);

                    var _material = _bll.FindSingle(e => e.idMaterial == id, u => u.UnidadeMedida, u => u.Fabricante);

                    return View(MaterialVM.GetMaterial(_material));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region JsonResult

        [Auth.Class.Auth(true)]
        public JsonResult JsSearch(string filter, int? page, int? pagesize)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.MaterialBLL(db, _idUsuario);

                    var result = _bll.Search(filter, "todos", page, pagesize);

                    var list = result.Select(s => new 
                    {
                        s.idMaterial,
                        s.nome,
                        s.margemGanho,
                        s.preco
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

        [Auth.Class.Auth(true)]
        public JsonResult JsCreate(Material model)
        {
            try
            {
                LogBLL.Insert(new LogDado("JsCreate", "Material", _idUsuario));
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.MaterialBLL(db, _idUsuario);

                        _bll.Insert(model);
                        _bll.SaveChanges();

                        trans.Complete();

                        return Json(new { model = model }, JsonRequestBehavior.AllowGet);
                    }
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

