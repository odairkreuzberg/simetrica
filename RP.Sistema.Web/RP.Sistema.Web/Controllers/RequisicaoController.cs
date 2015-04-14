using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Sistema.Web.Models.Requisicao;
using RP.Util;
using System.Data;
using System.Runtime.Serialization;
using RP.Sistema.BLL;

namespace RP.Sistema.Web.Controllers
{
    public class RequisicaoController : Controller
    {
        private int _idUsuario;

        public RequisicaoController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

        #region ActionResult

        [PersistDataSearch("Search")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            LogBLL.Insert(new LogDado("Index", "Requisicao", _idUsuario));
            return View();
        }

        [PersistDataSearch]
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Search(string filter, int? page, int? pagesize)
        {
            try
            {
                LogBLL.Insert(new LogDado("Search", "Requisicao", _idUsuario));
                using (var db = new Context())
                {
                    var _bll = new BLL.RequisicaoBLL(db, _idUsuario);

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

        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Projeto()
        {
            try
            {
                LogBLL.Insert(new LogDado("Projeto", "Requisicao", _idUsuario));
                using (var db = new Context())
                {
                    var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                    var _projetos = _bll.Find(e => e.status != RP.Sistema.Model.Entities.Projeto.CANCELADO && e.flConcluido != "Sim", u => u.Cliente).ToList();

                    return View(_projetos);
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Details(int id)
        {
            LogBLL.Insert(new LogDado("Details", "Requisicao", _idUsuario));
            return this.GetView(id);
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(int idProjeto)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                    var _projeto = _bll.FindSingle(e => e.idProjeto == idProjeto, u => u.Produtos.Select(k => k.ProdutoMateriais.Select(j => j.Material)));
                    var _result = new RequisicaoVM
                    {
                        Projeto = RP.Sistema.Web.Models.Projeto.Consultar.GetModel(_projeto),
                        Itens = RequisicaoVM.RequisicaoItemVM.GetItens(_projeto.Produtos.ToList()),
                    };

                    return View(_result);
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
        public ActionResult Create(RequisicaoVM model)
        {
            if (string.IsNullOrEmpty(model.Funcionario.nome))
            {
                ModelState.AddModelError("Funcionario.nome", "Informe o funcionário");
            }
            if (model.Itens == null || !model.Itens.Any())
            {
                ModelState.AddModelError("Itens", "Informe pelo menos 1 item");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    LogBLL.Insert(new LogDado("Create", "Requisicao", _idUsuario));
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _requisicao = model.GetRequisicao();

                            var _bll = new BLL.RequisicaoBLL(db, _idUsuario);

                            _bll.Insert(_requisicao);
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
        public ActionResult Edit(RequisicaoVM model)
        {
            if (string.IsNullOrEmpty(model.Funcionario.nome))
            {
                ModelState.AddModelError("Funcionario.nome", "Informe o estado");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    LogBLL.Insert(new LogDado("Edit", "Requisicao", _idUsuario));
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _requisicao = model.GetRequisicao();

                            var _bll = new BLL.RequisicaoBLL(db, _idUsuario);

                            _bll.Update(_requisicao);
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
                LogBLL.Insert(new LogDado("DeleteConfirmed", "Requisicao", _idUsuario));
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.RequisicaoBLL(db, _idUsuario);

                        _bll.Remover(id);
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
        public ActionResult Report(int idRequisicao)
        {
            try
            {
                LogBLL.Insert(new LogDado("Report", "Requisicao", _idUsuario));
                using (var db = new Context())
                {
                    return new Report.Class.Requisicao().GetReport(db, idRequisicao, _idUsuario);
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
                    var _bll = new BLL.RequisicaoBLL(db, _idUsuario);

                    var _requisicao = _bll.FindSingle(e => e.idRequisicao == id, u => u.Funcionario, u => u.Projeto, u => u.RequisicaoItens.Select(k => k.Material));

                    return View(RequisicaoVM.GetRequisicao(_requisicao));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        #endregion


    }
}

