using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Sistema.Web.Models.Caixa;
using RP.Util;
using System.Data;
using System.Runtime.Serialization;
using RP.Sistema.BLL;

namespace RP.Sistema.Web.Controllers
{
    public class CaixaController : Controller
    {
        private int _idUsuario;

        public CaixaController()
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
        public ActionResult Search(string filter, DateTime? dtInicio, DateTime? dtFim, string situacao, int? page, int? pagesize)
        {
            try
            {
                dtFim = dtFim ?? DateTime.Now;
                dtInicio = dtInicio ?? dtFim.Value.AddDays(-10);
                using (var db = new Context())
                {
                    var _bll = new BLL.CaixaBLL(db, _idUsuario);

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
                    var _bll = new BLL.CaixaBLL(db, _idUsuario);

                    var _caixa = _bll.FindSingle(e => e.idCaixa == id, u => u.ContaPagar.Projeto, u => u.ContaPagar.Fornecedor, u => u.ContaReceber.Cliente, u => u.ContaReceber.Projeto, u => u.MovimentoProfissional.Funcionario);

                    return View(CaixaVM.GetCaixa(_caixa));
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Vale()
        {
            return View();
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Vale(ValeVM model)
        {
            if (string.IsNullOrEmpty(model.Funcionario.nome))
            {
                ModelState.AddModelError("Funcionario.nome", "Informe o funcionario");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {

                            var _caixaBLL = new BLL.CaixaBLL(db, _idUsuario);
                            var _movimento = model.GetMovimento();

                            _movimento.idUsuario = this._idUsuario;

                            var _conta = new ContaPagar
                            {
                                descricao = "Vale entregue para " + model.Funcionario.nome + " " + model.descricao,
                                valorConta = model.valor,
                                valorPago = model.valor,
                                vencimento = DateTime.Now,
                                pagamento = DateTime.Now,
                                parcela = 1,
                                flFormaPagamento = "Dinheiro",
                                situacao = ContaPagar.SITUACAO_PAGO,
                                idUsuario = this._idUsuario
                            };

                            var _caixa = new Caixa
                            {
                                MovimentoProfissional = _movimento,
                                ContaPagar = _conta,
                                situacao = Caixa.CORENTE,
                                valor = (model.valor * -1),
                                descricao = "Vale entregue para " + model.Funcionario.nome + " " + model.descricao,
                                dtLancamento = DateTime.Now

                            };

                            _caixaBLL.Insert(_caixa);
                            _caixaBLL.SaveChanges();

                            trans.Complete();

                            this.AddFlashMessage("Vale adicionado para " + model.Funcionario.nome + " com sucesso!", FlashMessage.SUCCESS);
                            return RedirectToAction("Index", "ContaPagar");
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
                    return new Report.Class.Caixa().GetReport(db, filter, dtInicio, dtFim, situacao, _idUsuario);
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

