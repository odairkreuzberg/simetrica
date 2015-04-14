using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Sistema.Web.Models.Cliente;
using RP.Util;
using System.Data;
using System.Runtime.Serialization;
using RP.Sistema.BLL;

namespace RP.Sistema.Web.Controllers
{ 
    public class ClienteController : Controller
    {
        private int _idUsuario;

        public ClienteController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

		#region ActionResult

        [PersistDataSearch("Search")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            LogBLL.Insert(new LogDado("Index", "Cliente", _idUsuario));
            return View();
        }

        [PersistDataSearch]
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Search(string filter, int? page, int? pagesize)
        {
            try
            {
                LogBLL.Insert(new LogDado("Search", "Cliente", _idUsuario));
                using (var db = new Context())
                {
                    var _bll = new BLL.ClienteBLL(db, _idUsuario);

                    var result = _bll.Search(filter, page, pagesize);

                    return View("Index", result);
				}
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }		

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Details(int id)
        {
            LogBLL.Insert(new LogDado("Details", "Cliente", _idUsuario));
            return this.GetView(id);
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create()
        {
            return View();
        } 

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(ClienteVM model)
        {
            if (string.IsNullOrEmpty(model.Cidade.nome))
            {
                ModelState.AddModelError("Cidade.nome", "Informe a cidade");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    LogBLL.Insert(new LogDado("Create", "Cliente", _idUsuario));
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _cliente = model.GetCliente();

                            var _bll = new BLL.ClienteBLL(db, _idUsuario);

                            _bll.Insert(_cliente);
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
                    return RedirectToAction("Index");
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
        public ActionResult Edit(ClienteVM model)
        {
            if (string.IsNullOrEmpty(model.Cidade.nome))
            {
                ModelState.AddModelError("Cidade.nome", "Informe a cidade");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    LogBLL.Insert(new LogDado("Edit", "Cliente", _idUsuario));
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _cliente = model.GetCliente();

                            var _bll = new BLL.ClienteBLL(db, _idUsuario);

                            _bll.Update(_cliente);
                            _bll.SaveChanges();

                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.EDIT_SUCCESS, FlashMessage.SUCCESS);
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
                LogBLL.Insert(new LogDado("DeleteConfirmed", "Cliente", _idUsuario));
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.ClienteBLL(db, _idUsuario);

                        _bll.Delete(e => e.idCliente == id);
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
        public ActionResult Report(string filter)
        {
            try
            {
                LogBLL.Insert(new LogDado("Report", "Cliente", _idUsuario));
                using (var db = new Context())
                {
                    return new Report.Class.Cliente().GetReport(db, filter, _idUsuario);
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
                    var _bll = new BLL.ClienteBLL(db, _idUsuario);

                    var _cliente = _bll.FindSingle(e => e.idCliente == id, u => u.Telefones, u => u.Cidade);

                    return View(ClienteVM.GetCliente(_cliente));
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
                    var _bll = new BLL.ClienteBLL(db, _idUsuario);

                    var result = _bll.Search(filter, page, pagesize);

                    var list = result.Select(s => new 
                    {
                        s.idCliente,
                        s.nome,
                        s.tipo
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
        public JsonResult JsCreate(Cliente model)
        {
            try
            {
                LogBLL.Insert(new LogDado("JsCreate", "Cliente", _idUsuario));
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.ClienteBLL(db, _idUsuario);

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

