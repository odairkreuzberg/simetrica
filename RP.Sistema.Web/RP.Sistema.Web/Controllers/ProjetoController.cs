using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Sistema.Web.Models.Projeto;
using RP.Util;
using System.Data;
using System.Runtime.Serialization;
using RP.Sistema.BLL;

namespace RP.Sistema.Web.Controllers
{
    public class ProjetoController : Controller
    {
        private int _idUsuario;

        public ProjetoController()
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
        public ActionResult Search(string filter, int? page, int? pagesize)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                    var result = _bll.Search(filter, page, pagesize);

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
        public ActionResult Create(AprovarVM model)
        {
            if (string.IsNullOrEmpty(model.Vendedor.nome))
            {
                ModelState.AddModelError("Vendedor.nome", "Selecione um vendedor");
            }
            if (string.IsNullOrEmpty(model.Cliente.nome))
            {
                ModelState.AddModelError("Cliente.nome", "Selecione um cliente");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _projeto = model.GetProjeto();

                            var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                            _bll.Insert(_projeto);
                            _bll.SaveChanges();
                            trans.Complete();

                            model.idProjeto = _projeto.idProjeto;

                            this.AddFlashMessage(RP.Util.Resource.Message.INSERT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                    return RedirectToAction("Index", "Erro", new { area = string.Empty });
                }
            }
            return View(model);
        }

        [Auth.Class.Auth("sistema", "padrao", "Edit")]
        public ActionResult Duplicar(int idProjeto)
        {
            return this.GetView(idProjeto);
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao", "Edit")]
        public ActionResult Duplicar(AprovarVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _projeto = model.GetProjeto();

                            var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                            _bll.Duplicar(_projeto);
                            _bll.SaveChanges();

                            trans.Complete();

                            this.AddFlashMessage("Projeto duplicado com sucesso!", FlashMessage.SUCCESS);
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
        public ActionResult Edit(int id)
        {
            return this.GetView(id);
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(AprovarVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _projeto = model.GetProjeto();

                            var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                            _bll.Update(_projeto);
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
        public ActionResult AddItem(int id)
        {
            return this.GetView(id);
        }
        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult AddItem(AprovarVM model)
        {
            try
            {
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                        var _projeto = _bll.FindSingle(e => e.idProjeto == model.idProjeto,
                            u => u.Cliente, u => u.Vendedor, u => u.Produtos, u => u.ProjetoCustos,
                            u => u.Produtos.Select(l => l.ProdutoMateriais),
                            u => u.Produtos.Select(k => k.Marceneiro), u => u.Produtos.Select(k => k.Projetista));

                        model = AprovarVM.GetProjeto(_projeto);

                        _projeto.vlProjeto = model.Produtos.Sum(u => u.vlProduto);
                        _projeto.vlVenda = model.Produtos.Sum(u => u.vlVenda);
                        _projeto.vlDesconto = model.Produtos.Sum(u => u.vlDesconto);

                        foreach (var item in _projeto.Produtos)
                        {
                            item.vlVenda = model.Produtos.First(u => u.idProduto == item.idProduto).vlVenda;
                            item.vlProduto = model.Produtos.First(u => u.idProduto == item.idProduto).vlProduto;
                            item.vlDesconto = model.Produtos.First(u => u.idProduto == item.idProduto).vlDesconto;
                        }

                        _bll.Aprovar(_projeto);
                        _bll.SaveChanges();
                        trans.Complete();

                        this.AddFlashMessage("Projeto atualizado com sucesso!", FlashMessage.SUCCESS);
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }


        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult AddCusto(int id)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                    var _projeto = _bll.FindSingle(e => e.idProjeto == id, u => u.Cliente, u => u.ProjetoCustos);

                    return View(CustoProjetoVM.GetProjeto(_projeto));
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Aprovar(int id)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                    var _projeto = _bll.FindSingle(e => e.idProjeto == id,
                        u => u.Cliente, u => u.Vendedor, u => u.Produtos, u => u.ProjetoCustos,
                        u => u.Produtos.Select(l => l.ProdutoMateriais),
                        u => u.Produtos.Select(k => k.Marceneiro), u => u.Produtos.Select(k => k.Projetista));

                    return View(AprovarVM.GetProjeto(_projeto));
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
        public ActionResult Aprovar(AprovarVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _projeto = model.GetProjeto();

                            var _bll = new BLL.ProjetoBLL(db, _idUsuario);
                            if (_projeto.status == Projeto.VENDIDO)
                            {
                                var _movimentoBLL = new MovimentoProfissionalBLL(db, _idUsuario);
                                var _contaBLL = new ContaReceberBLL(db, _idUsuario);
                                int nrParcelas = model.Parcelas.Count;
                                foreach (var item in model.Parcelas)
                                {
                                    //comissao do vendedor
                                    _movimentoBLL.Insert(new MovimentoProfissional
                                    {
                                        tipo = MovimentoProfissional.TIPO_COMISSAO,
                                        idFuncionario = model.Vendedor.idFuncionario ?? 0,
                                        valor = ((model.porcentagemVendedor / 100) * item.vlParcela),
                                        idProjeto = _projeto.idProjeto,
                                        situacao = MovimentoProfissional.SITUACAO_PENDENTE,
                                        descricao = "Comissão referente a " + item.nrParcela + "º parcela do projeto " + _projeto.descricao,
                                        dtVencimento = item.dtVencimento
                                    });

                                    foreach (var produto in _projeto.Produtos)
                                    {
                                        decimal vlParcela = (produto.vlVenda ?? 0) / nrParcelas;

                                        //comissao do projetista
                                        _movimentoBLL.Insert(new MovimentoProfissional
                                        {
                                            tipo = MovimentoProfissional.TIPO_COMISSAO,
                                            idFuncionario = produto.idProjetista ?? 0,
                                            valor = (((produto.porcentagemProjetista ?? 0) / 100) * vlParcela),
                                            idProjeto = _projeto.idProjeto,
                                            situacao = MovimentoProfissional.SITUACAO_PENDENTE,
                                            descricao = "Comissão referente a " + item.nrParcela + "º parcela do projeto " + _projeto.descricao + " [" + produto.nome + "]",
                                            dtVencimento = item.dtVencimento
                                        });

                                        //comissao do marceneiro
                                        _movimentoBLL.Insert(new MovimentoProfissional
                                        {
                                            tipo = MovimentoProfissional.TIPO_COMISSAO,
                                            idFuncionario = produto.idMarceneiro ?? 0,
                                            valor = (((produto.porcentagemMarceneiro ?? 0) / 100) * vlParcela),
                                            idProjeto = _projeto.idProjeto,
                                            situacao = MovimentoProfissional.SITUACAO_PENDENTE,
                                            descricao = "Comissão referente a " + item.nrParcela + "º parcela do projeto " + _projeto.descricao + " [" + produto.nome + "]",
                                            dtVencimento = item.dtVencimento
                                        });

                                    }

                                    string situacao = ContaReceber.SITUACAO_AGUARDANDO_PAGAMENTO;
                                    decimal? vlPago = null;
                                    DateTime? dtPagamento = null;
                                    if (item.dtVencimento <= DateTime.Now.Date)
                                    {
                                        vlPago = item.vlParcela;
                                        situacao = ContaReceber.SITUACAO_PAGO;
                                        dtPagamento = item.dtVencimento;
                                    }

                                    // Lança conta a receber referente a parcela
                                    var _conta = new ContaReceber
                                    {
                                        idCliente = _projeto.idCliente,
                                        parcela = item.nrParcela,
                                        descricao = "Conta a receber referente a " + item.nrParcela + "º parcela do projeto " + _projeto.descricao + ".  " + item.dsObservacao,
                                        vencimento = item.dtVencimento,
                                        pagamento = dtPagamento,
                                        valorConta = item.vlParcela,
                                        valorPago = vlPago,
                                        situacao = situacao,
                                        flFormaPagamento = item.flFormaPagamento,
                                        idProjeto = _projeto.idProjeto
                                    };
                                    _contaBLL.Insert(_conta);

                                    //se a data de vencimento for a atual lança a entrada no caixa
                                    if (item.dtVencimento <= DateTime.Now.Date)
                                    {
                                        var _caixaBLL = new CaixaBLL(db, _idUsuario);
                                        _caixaBLL.Insert(new Caixa
                                        {
                                            ContaReceber = _conta,
                                            situacao = Caixa.CORENTE,
                                            valor = item.vlParcela,
                                            descricao = _conta.descricao + " [" +model.Cliente.nome + "] " + item.dsObservacao,
                                            dtLancamento = dtPagamento.Value
                                        });
                                    }
                                }
                            }

                            _bll.Aprovar(_projeto);
                            _bll.SaveChanges();
                            trans.Complete();

                            this.AddFlashMessage("Projeto atualizado com sucesso!", FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                    return RedirectToAction("Index", "Erro", new { area = string.Empty });
                }
            }
            return View(model);
        }



        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult AddCusto(CustoProjetoVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _projeto = model.GetProjeto();

                            var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                            _bll.AddCustos(_projeto);
                            _bll.SaveChanges();
                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.INSERT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                    return RedirectToAction("Index", "Erro", new { area = string.Empty });
                }
            }
            return View(model);
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Delete(int id)
        {
            try
            {
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                        _bll.Cancelar(id);
                        _bll.SaveChanges();

                        trans.Complete();

                        this.AddFlashMessage("Projeto cancelado com sucesso", FlashMessage.SUCCESS);
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Finalizar(int id)
        {
            try
            {
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                        _bll.Finalizar(id);
                        _bll.SaveChanges();

                        trans.Complete();

                        this.AddFlashMessage("Projeto finalizado com sucesso", FlashMessage.INFO);
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Report(string filter)
        {
            try
            {
                using (var db = new Context())
                {
                    //return new Report.Class.Projeto().GetReport(db, filter, _idUsuario);
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult OrdemCompra(int idProjeto)
        {
            try
            {
                using (var db = new Context())
                {
                    return new Report.Class.OrdemCompra().GetReport(db, idProjeto, _idUsuario);
                }

            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult RaioX(int idProjeto)
        {
            try
            {
                using (var db = new Context())
                {
                    return new Report.Class.RaioX().GetReport(db, idProjeto, _idUsuario);
                }

            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Orcamento(int idProjeto)
        {
            return View(idProjeto);
        }



        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Orcamento(int idProjeto, string dsObservacao, string dsGarantia, string dsPrevisao, string dsIncluso, string dsValidade)
        {
            try
            {
                using (var db = new Context())
                {
                    return new Report.Class.OrcamentoCliente().GetReport(db, idProjeto, dsObservacao, dsGarantia, dsPrevisao, dsIncluso, dsValidade, _idUsuario);
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
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
                    var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                    var _projeto = _bll.FindSingle(e => e.idProjeto == id,
                        u => u.Cliente, u => u.Vendedor, u => u.Produtos, u => u.ProjetoCustos,
                        u => u.Produtos.Select(l => l.ProdutoMateriais),
                        u => u.Produtos.Select(k => k.Marceneiro), u => u.Produtos.Select(k => k.Projetista));

                    return View(AprovarVM.GetProjeto(_projeto));
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
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
                using (var db = new Context())
                {
                    var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                    var result = _bll.Search(filter, page, pagesize, true);

                    var list = result.Select(s => new
                    {
                        s.idProjeto,
                        s.descricao,
                        s.status,
                        s.Cliente.nome
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
        public JsonResult JsCreate(Projeto model)
        {
            try
            {
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.ProjetoBLL(db, _idUsuario);

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

