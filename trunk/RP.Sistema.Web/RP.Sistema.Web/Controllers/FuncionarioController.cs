using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Sistema.Web.Models.Funcionario;
using RP.Util;
using System.Data;
using System.Runtime.Serialization;

namespace RP.Sistema.Web.Controllers
{ 
    public class FuncionarioController : Controller
    {
        private int _idUsuario;

        public FuncionarioController()
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
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Search(string filter, string status, int? page, int? pagesize)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.FuncionarioBLL(db, _idUsuario);

                    var result = _bll.Search(filter, status, page, pagesize, null);

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
            return this.GetView(id);
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create()
        {
            return View();
        } 

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(FuncionarioVM model)
        {
            if (string.IsNullOrEmpty(model.Cidade.nome))
            {
                ModelState.AddModelError("Cidade.nome", "Selecione uma cidade");
            }
            if (model.flMensalista == "Não" && (model.comissao == null || model.comissao == 0))
            {
                ModelState.AddModelError("comissao", "Informe a comissão");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _funcionario = model.GetFuncionario();

                            var _bll = new BLL.FuncionarioBLL(db, _idUsuario);

                            _bll.Insert(_funcionario);
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
        public ActionResult Edit(FuncionarioVM model)
        {
            if (string.IsNullOrEmpty(model.Cidade.nome))
            {
                ModelState.AddModelError("Cidade.nome", "Selecione uma cidade");
            }
            if (model.flMensalista == "Não" && (model.comissao == null || model.comissao == 0))
            {
                ModelState.AddModelError("salario", "Informe a comissão");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _funcionario = model.GetFuncionario();

                            var _bll = new BLL.FuncionarioBLL(db, _idUsuario);

                            _bll.Update(_funcionario);
                            _bll.SaveChanges();

                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.EDIT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                    return RedirectToAction("Index", "Erro", new { area = string.Empty });
                }
            }
            return View(model);   
		}

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Delete(int id)
        {
            return this.GetView(id);
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Delete(FuncionarioVM model)
        {
            if (string.IsNullOrEmpty(model.motivoSaida))
            {
                ModelState.AddModelError("motivoSaida", "Informe o motivo");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _funcionario = model.GetFuncionario();

                            var _bll = new BLL.FuncionarioBLL(db, _idUsuario);

                            _bll.Desativar(_funcionario);
                            _bll.SaveChanges();

                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.EDIT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                    return RedirectToAction("Index", "Erro", new { area = string.Empty });
                }
            }
            return View(model);
        }

        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Report(string filter)
        {
            try
            {
                using (var db = new Context())
                {
                    return new Report.Class.Funcionario().GetReport(db, filter, _idUsuario);
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
                    var _bll = new BLL.FuncionarioBLL(db, _idUsuario);

                    var _funcionario = _bll.FindSingle(e => e.idFuncionario == id, u => u.Usuario, u => u.Cidade);

                    return View(FuncionarioVM.GetFuncionario(_funcionario));
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
        public JsonResult JsSearch(string filter, string tipo, string mensalista, int? page, int? pagesize)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.FuncionarioBLL(db, _idUsuario);

                    var result = _bll.Search(filter,Funcionario.ATIVO, page, pagesize, mensalista, tipo);

                    var list = result.Select(s => new 
                    {
                        s.idFuncionario,
                        s.nome,
                        s.tipo,
                        comissao = s.comissao ?? 0
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
        public JsonResult JsCreate(Funcionario model)
        {
            try
            {
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.FuncionarioBLL(db, _idUsuario);

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

