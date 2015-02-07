using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Sistema.Web.Models.Compra;
using RP.Util;
using System.Data;
using System.Runtime.Serialization;
using RP.Sistema.BLL;

namespace RP.Sistema.Web.Controllers
{
    public class CompraController : Controller
    {
        private int _idUsuario;

        public CompraController()
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
                    var _bll = new BLL.CompraBLL(db, _idUsuario);

                    var result = _bll.Search(filter, page, pagesize);

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
        public ActionResult Details(int id)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.CompraBLL(db, _idUsuario);

                    var _compra = _bll.FindSingle(e => e.idCompra == id, u => u.Fornecedor, u => u.Projeto.Cliente, u => u.ContasPagar);

                    return View(CompraVM.GetCompra(_compra));
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(int idProjeto)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.ProjetoBLL(db, _idUsuario);

                    var _projeto = _bll.FindSingle(e => e.idProjeto == idProjeto, u => u.Cliente,
                        u => u.Produtos.Select(k => k.ProdutoMateriais.Select(j => j.Material)),
                        u => u.Compras.Select(k => k.Fornecedor),
                        u => u.Compras.Select(k => k.ItensCompra.Select(j => j.Material)));
                    var _result = new CompraVM
                    {
                        Projeto = RP.Sistema.Web.Models.Projeto.Consultar.GetModel(_projeto),
                        Itens = CompraVM.ItemCompraVM.GetItens(_projeto.Produtos.ToList()),
                        Materiais = CompraVM.ItemCompraVM.GetItens(_projeto.Compras.ToList())
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

        [Auth.Class.Auth("sistema", "padrao", "Create")]
        public ActionResult Projeto()
        {
            try
            {
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

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(CompraVM model)
        {
            if (string.IsNullOrEmpty(model.Fornecedor.nome))
            {
                ModelState.AddModelError("Fornecedor.nome", "Informe o fornecedor");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _compra = model.GetCompra();

                            var _bll = new BLL.CompraBLL(db, _idUsuario);

                            _bll.Insert(_compra);
                            var _contaBLL = new ContaPagarBLL(db, _idUsuario);
                            int nrParcelas = model.Parcelas.Count;
                            foreach (var item in model.Parcelas)
                            {

                                string situacao = ContaPagar.SITUACAO_AGUARDANDO_PAGAMENTO;
                                decimal? vlPago = null;
                                DateTime? dtPagamento = null;
                                if (item.dtVencimento <= DateTime.Now.Date)
                                {
                                    vlPago = item.vlParcela;
                                    situacao = ContaPagar.SITUACAO_PAGO;
                                    dtPagamento = item.dtVencimento;
                                }

                                // Lança conta a receber referente a parcela
                                var _conta = new ContaPagar
                                {
                                    idFornecedor = _compra.idFornecedor,
                                    parcela = item.nrParcela,
                                    descricao = "Conta a pagar referente a " + item.nrParcela + "º parcela do projeto " + model.Projeto.descricao + ".  " + item.dsObservacao,
                                    vencimento = item.dtVencimento,
                                    pagamento = dtPagamento,
                                    valorConta = item.vlParcela,
                                    valorPago = vlPago,
                                    situacao = situacao,
                                    flFormaPagamento = item.flFormaPagamento,
                                    idProjeto = _compra.idProjeto,
                                    Compra = _compra
                                };
                                _contaBLL.Insert(_conta);

                                //se a data de vencimento for a atual lança a entrada no caixa
                                if (item.dtVencimento <= DateTime.Now.Date)
                                {
                                    var _caixaBLL = new CaixaBLL(db, _idUsuario);
                                    _caixaBLL.Insert(new Caixa
                                    {
                                        ContaPagar = _conta,
                                        situacao = Caixa.CORENTE,
                                        valor = (item.vlParcela * -1),
                                        descricao = "Conta paga para " + model.Fornecedor.nome + " " + item.dsObservacao,
                                        dtLancamento = item.dtVencimento
                                    });
                                }
                            }
                            _bll.SaveChanges();

                            trans.Complete();

                            this.AddFlashMessage("Compra realizada com sucesso!", FlashMessage.SUCCESS);
                            return RedirectToAction("Index", "Compra");
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
        public ActionResult Vulso()
        {
            return View();
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Vulso(CompraVM model)
        {
            if (string.IsNullOrEmpty(model.Fornecedor.nome))
            {
                ModelState.AddModelError("Fornecedor.nome", "Informe o fornecedor");
            }
            if (model.total <= 0)
            {
                ModelState.AddModelError("total", "Informe o total");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _compra = model.GetCompra();

                            var _bll = new BLL.CompraBLL(db, _idUsuario);

                            _bll.Insert(_compra);
                            var _contaBLL = new ContaPagarBLL(db, _idUsuario);
                            var _caixaBLL = new CaixaBLL(db, _idUsuario);
                            if (model.Parcelas != null && model.Parcelas.Any())
                            {
                                int nrParcelas = model.Parcelas.Count;
                                foreach (var item in model.Parcelas)
                                {
                                    string situacao = ContaPagar.SITUACAO_AGUARDANDO_PAGAMENTO;
                                    decimal? vlPago = null;
                                    DateTime? dtPagamento = null;
                                    if (item.dtVencimento <= DateTime.Now.Date)
                                    {
                                        vlPago = item.vlParcela;
                                        situacao = ContaPagar.SITUACAO_PAGO;
                                        dtPagamento = item.dtVencimento;
                                    }

                                    // Lança conta a pagar referente a parcela
                                    var _conta = new ContaPagar
                                    {
                                        idFornecedor = _compra.idFornecedor,
                                        parcela = item.nrParcela,
                                        descricao = "Conta a pagar referente a " + item.nrParcela + "º parcela.  " + item.dsObservacao,
                                        vencimento = item.dtVencimento,
                                        pagamento = dtPagamento,
                                        valorConta = item.vlParcela,
                                        valorPago = vlPago,
                                        situacao = situacao,
                                        flFormaPagamento = item.flFormaPagamento,
                                        Compra = _compra
                                    };
                                    _contaBLL.Insert(_conta);

                                    //se a data de vencimento for a atual lança a entrada no caixa
                                    if (item.dtVencimento <= DateTime.Now.Date)
                                    {
                                        _caixaBLL.Insert(new Caixa
                                        {
                                            ContaPagar = _conta,
                                            situacao = Caixa.CORENTE,
                                            valor = (item.vlParcela * -1),
                                            descricao = "Conta paga para " + model.Fornecedor.nome + " " + item.dsObservacao,
                                            dtLancamento = item.dtVencimento
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var _conta = new ContaPagar
                                {
                                    idFornecedor = _compra.idFornecedor,
                                    parcela = 1,
                                    descricao = "Conta a pagar referente a " + model.descricao + " [Fornecedor:" + model.Fornecedor.nome + "] ",
                                    vencimento = DateTime.Now,
                                    pagamento = DateTime.Now,
                                    valorConta = model.total,
                                    valorPago = model.total,
                                    situacao = ContaPagar.SITUACAO_PAGO,
                                    flFormaPagamento = string.Empty,
                                    Compra = _compra
                                };
                                _contaBLL.Insert(_conta);
                                _caixaBLL.Insert(new Caixa
                                {
                                    ContaPagar = _conta,
                                    situacao = Caixa.CORENTE,
                                    valor = (model.total * -1),
                                    descricao = "Conta paga para " + model.Fornecedor.nome + " " + model.descricao,
                                    dtLancamento = DateTime.Now
                                });
                            }
                            _bll.SaveChanges();

                            trans.Complete();

                            this.AddFlashMessage("Compra realizada com sucesso!", FlashMessage.SUCCESS);
                            return RedirectToAction("Index", "Compra");
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
        public ActionResult Cancelar(int id)
        {
            try
            {
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.CompraBLL(db, _idUsuario);

                        _bll.Cancelar(id);
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
        public ActionResult Report(int id)
        {
            try
            {
                using (var db = new Context())
                {
                    return this.Index();//new Report.Class.Compra().GetReport(db, filter, _idUsuario);
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

        #endregion

        #region JsonResult

        [Auth.Class.Auth(true)]
        public JsonResult JsSearch(string filter, int? page, int? pagesize)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.CompraBLL(db, _idUsuario);

                    var result = _bll.Search(filter, page, pagesize);

                    var list = result.Select(s => new
                    {
                        s.idCompra,
                        //s.nome,
                        //estado = s.Estado.nome,
                        //pais = s.Estado.Pais.nome 
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
        public JsonResult JsCreate(Compra model)
        {
            try
            {
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.CompraBLL(db, _idUsuario);

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

