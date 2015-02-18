using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Sistema.Web.Models.ContaPagar;
using RP.Util;
using System.Data;
using System.Runtime.Serialization;
using RP.Sistema.BLL;

namespace RP.Sistema.Web.Controllers
{
    public class ContaPagarController : Controller
    {
        private int _idUsuario;

        public ContaPagarController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

        #region ActionResult

        [PersistDataSearch("Search")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            ViewBag.dtFim = DateTime.Now;
            return View();
        }

        [PersistDataSearch]
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Search(string filter, DateTime? dtInicio, DateTime? dtFim, string situacao, int? page, int? pagesize)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.ContaPagarBLL(db, _idUsuario);

                    var result = _bll.Search(filter, dtInicio, dtFim, situacao, page, pagesize);
                    ViewBag.dtInicio = dtInicio;
                    ViewBag.dtFim = dtFim;
                    ViewBag.situacao = situacao;

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
                    var _bll = new BLL.ContaPagarBLL(db, _idUsuario);

                    var _contaPagar = _bll.FindSingle(e => e.idContaPagar == id, u => u.Fornecedor, u => u.Projeto.Cliente);

                    return View(ContaPagarVM.GetContaPagar(_contaPagar));
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(ContaPagarVM model)
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
                            var _contaPagar = model.GetContaPagar();

                            var _bll = new BLL.ContaPagarBLL(db, _idUsuario);
                            _contaPagar.situacao =ContaPagar.SITUACAO_AGUARDANDO_PAGAMENTO;
                            _bll.Insert(_contaPagar);
                            //if (pagar)
                            //{
                            //    var _caixaBLL = new BLL.CaixaBLL(db, _idUsuario);

                            //    _caixaBLL.Insert(new Caixa
                            //    {
                            //        ContaPagar = _contaPagar,
                            //        situacao = Caixa.CORENTE,
                            //        valor = (model.valorConta.Value * -1),
                            //        descricao = "Conta paga para " + model.Fornecedor.nome + " " + model.descricao,
                            //        dtLancamento = DateTime.Now
                            //    });
                            //}
                            _bll.SaveChanges();

                            trans.Complete();

                            this.AddFlashMessage("Conta a pagar adicionada com sucesso!", FlashMessage.SUCCESS);
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
        public ActionResult Cancelar(int id)
        {
            try
            {
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.ContaPagarBLL(db, _idUsuario);

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
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Pagar(int id)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.ContaPagarBLL(db, _idUsuario);

                    var _contaPagar = _bll.FindSingle(e => e.idContaPagar == id, u => u.Fornecedor, u => u.Projeto.Cliente);
                    var _result = ContaPagarVM.GetContaPagar(_contaPagar);
                    _result.pagamento = DateTime.Now;
                    _result.valorPago = _result.valorConta;
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
        public ActionResult Pagar(ContaPagarVM model)
        {
            if (model.valorPago == null || model.valorPago <= 0)
            {
                ModelState.AddModelError("valorPago", "Informe o valor do pagamento");
            }
            if (model.pagamento == null)
            {
                ModelState.AddModelError("pagamento", "Informe o data do pagamento");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _contaPagar = model.GetContaPagar();

                            var _bll = new BLL.ContaPagarBLL(db, _idUsuario);
                            _bll.Pagar(_contaPagar);
                            if (model.flDiferenca == "Sim")
                            {
                                // Lança conta a pagar referente a diferença
                                var _diferenca = new ContaPagar
                                {
                                    idProjeto = model.Projeto != null ? model.Projeto.idProjeto : null,
                                    idFornecedor = _contaPagar.idFornecedor,
                                    parcela = _contaPagar.parcela,
                                    descricao = "Conta gerada da diferença. Valor original: " + model.valorConta + ". Valor pago: " + model.valorPago,
                                    vencimento = _contaPagar.vencimento,
                                    pagamento = _contaPagar.pagamento,
                                    valorConta = model.vlDiferenca ?? 0,
                                    situacao = ContaPagar.SITUACAO_AGUARDANDO_PAGAMENTO,
                                    flFormaPagamento = _contaPagar.flFormaPagamento,
                                    idCompra = _contaPagar.idCompra,
                                    idOrigem = _contaPagar.idOrigem == null ? _contaPagar.idContaPagar :_contaPagar.idOrigem
                                };
                                _bll.Insert(_diferenca);
                            }
                            var _caixaBLL = new BLL.CaixaBLL(db, _idUsuario);

                            _caixaBLL.Insert(new Caixa
                            {
                                ContaPagar = _contaPagar,
                                situacao = Caixa.CORENTE,
                                valor = (_contaPagar.valorPago.Value * -1),
                                descricao =  _contaPagar.descricao + " [" + model.Fornecedor.nome + "] " + (model.Projeto != null ? model.Projeto.descricao : ""),
                                dtLancamento = _contaPagar.pagamento.Value
                            });
                            _bll.SaveChanges();

                            trans.Complete();

                            this.AddFlashMessage("Conta a pagar adicionada com sucesso!", FlashMessage.SUCCESS);
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

        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Report(string filter, DateTime? dtInicio, DateTime? dtFim, string situacao)
        {
            try
            {
                using (var db = new Context())
                {
                    return new Report.Class.ContaPagar().GetReport(db, filter, dtInicio, dtFim, situacao, _idUsuario);
                }

            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        #endregion
    }
}

