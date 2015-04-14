using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Sistema.Web.Models.ContaReceber;
using RP.Util;
using System.Data;
using System.Runtime.Serialization;
using RP.Sistema.BLL;

namespace RP.Sistema.Web.Controllers
{
    public class ContaReceberController : Controller
    {
        private int _idUsuario;

        public ContaReceberController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

        #region ActionResult

        [PersistDataSearch("Search")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            LogBLL.Insert(new LogDado("Index", "ContaReceber", _idUsuario));
            ViewBag.dtFim = DateTime.Now;
            return View();
        }

        [PersistDataSearch]
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Search(string filter, DateTime? dtInicio, DateTime? dtFim, string situacao, int? page, int? pagesize)
        {
            try
            {
                LogBLL.Insert(new LogDado("Search", "ContaReceber", _idUsuario));
                using (var db = new Context())
                {
                    var _bll = new BLL.ContaReceberBLL(db, _idUsuario);

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
                LogBLL.Insert(new LogDado("Details", "ContaReceber", _idUsuario));
                using (var db = new Context())
                {
                    var _bll = new BLL.ContaReceberBLL(db, _idUsuario);

                    var _contaReceber = _bll.FindSingle(e => e.idContaReceber == id, u => u.Cliente, u => u.Projeto.Cliente);

                    return View(ContaReceberVM.GetContaReceber(_contaReceber));
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
        public ActionResult Create(ContaReceberVM model)
        {
            if (string.IsNullOrEmpty(model.Cliente.nome))
            {
                ModelState.AddModelError("Cliente.nome", "Informe o cliente");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    LogBLL.Insert(new LogDado("Create", "ContaReceber", _idUsuario));
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _contaReceber = model.GetContaReceber();

                            var _bll = new BLL.ContaReceberBLL(db, _idUsuario);
                            //bool receber = _contaReceber.vencimento.Date <= DateTime.Now.Date;
                            //_contaReceber.situacao = receber ? ContaReceber.SITUACAO_PAGO : ContaReceber.SITUACAO_AGUARDANDO_PAGAMENTO;
                            //_contaReceber.pagamento = receber ? (DateTime?)_contaReceber.vencimento : null;
                            //_contaReceber.valorPago = receber ? (decimal?)_contaReceber.valorConta : null;
                            _contaReceber.situacao = ContaReceber.SITUACAO_AGUARDANDO_PAGAMENTO;
                            _bll.Insert(_contaReceber);
                            //if (receber)
                            //{
                            //    var _caixaBLL = new BLL.CaixaBLL(db, _idUsuario);

                            //    _caixaBLL.Insert(new Caixa
                            //    {
                            //        ContaReceber = _contaReceber,
                            //        situacao = Caixa.CORENTE,
                            //        valor = (model.valorConta.Value),
                            //        descricao = "Conta recebida de " + model.Cliente.nome + " " + model.descricao,
                            //        dtLancamento = DateTime.Now
                            //    });
                            //}
                            _bll.SaveChanges();

                            trans.Complete();

                            this.AddFlashMessage("Conta a receber adicionada com sucesso!", FlashMessage.SUCCESS);
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
                LogBLL.Insert(new LogDado("Cancelar", "ContaReceber", _idUsuario));
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.ContaReceberBLL(db, _idUsuario);

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

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Receber(int id)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.ContaReceberBLL(db, _idUsuario);

                    var _contaReceber = _bll.FindSingle(e => e.idContaReceber == id, u => u.Cliente, u => u.Projeto.Cliente);
                    var _result = ContaReceberVM.GetContaReceber(_contaReceber);
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
        public ActionResult Receber(ContaReceberVM model)
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
                    LogBLL.Insert(new LogDado("Receber", "ContaReceber", _idUsuario));
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _contaReceber = model.GetContaReceber();

                            var _bll = new BLL.ContaReceberBLL(db, _idUsuario);
                            _bll.Receber(_contaReceber);
                            if (model.flDiferenca == "Sim")
                            {
                                // Lança conta a receber referente a diferença
                                var _diferenca = new ContaReceber
                                {
                                    idProjeto = model.Projeto != null ? model.Projeto.idProjeto : null,
                                    idCliente = _contaReceber.idCliente,
                                    parcela = _contaReceber.parcela,
                                    descricao = "Conta gerada da diferença. Valor original: " + model.valorConta + ". Valor pago: " + model.valorPago,
                                    vencimento = _contaReceber.vencimento,
                                    pagamento = _contaReceber.pagamento,
                                    valorConta = model.vlDiferenca ?? 0,
                                    situacao = ContaReceber.SITUACAO_AGUARDANDO_PAGAMENTO,
                                    flFormaPagamento = _contaReceber.flFormaPagamento,
                                    idCompra = _contaReceber.idCompra,
                                    idOrigem = _contaReceber.idOrigem == null ? _contaReceber.idContaReceber :_contaReceber.idOrigem
                                };
                                _bll.Insert(_diferenca);
                            }
                            var _caixaBLL = new BLL.CaixaBLL(db, _idUsuario);

                            _caixaBLL.Insert(new Caixa
                            {
                                ContaReceber = _contaReceber,
                                situacao = Caixa.CORENTE,
                                valor = (_contaReceber.valorPago.Value),
                                descricao = _contaReceber.descricao + " [" + model.Cliente.nome + "] " + (model.Projeto != null ? model.Projeto.descricao : ""),
                                dtLancamento = _contaReceber.pagamento.Value
                            });
                            _bll.SaveChanges();

                            trans.Complete();

                            this.AddFlashMessage("Conta a receber adicionada com sucesso!", FlashMessage.SUCCESS);
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
                    return new Report.Class.ContaReceber().GetReport(db, filter, dtInicio, dtFim, situacao, _idUsuario);
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

