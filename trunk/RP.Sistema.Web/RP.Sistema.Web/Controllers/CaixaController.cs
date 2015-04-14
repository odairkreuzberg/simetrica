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

        public class Teste
        {
            public int id { get; set; }
            public string tipo { get; set; }
            public decimal saldo { get; set; }
            public decimal? valorpago { get; set; }
            public string descricao { get; set; }
            public DateTime? dtlancamento { get; set; }
        }
        private int _idUsuario;

        public CaixaController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

        #region ActionResult

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Extrato()
        {
            LogBLL.Insert(new LogDado("Extrato", "Caixa", _idUsuario));
            return View();
        }

        [PersistDataSearch("Search")]
        public ActionResult Index()
        {
            LogBLL.Insert(new LogDado("Index", "Caixa", _idUsuario));
            ViewBag.dtFim = DateTime.Now;
            return View();
        }

        [PersistDataSearch]
        public ActionResult Search(string filter, DateTime? dtInicio, DateTime? dtFim, string situacao, int? page, int? pagesize)
        {
            try
            {
                LogBLL.Insert(new LogDado("Search", "Caixa", _idUsuario));
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
                LogBLL.Insert(new LogDado("Details", "Caixa", _idUsuario));
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
            LogBLL.Insert(new LogDado("Vale", "Caixa", _idUsuario));
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
                    LogBLL.Insert(new LogDado("Vale(ValeVM model)", "Caixa", _idUsuario));
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

        [Auth.Class.Auth("sistema", "padrao", "extrato")]
        public ActionResult Report()
        {
            try
            {
                LogBLL.Insert(new LogDado("Report", "Caixa", _idUsuario));
                using (var db = new Context())
                {
                    return new Report.Class.Caixa().GetReport(db, _idUsuario);
                }

            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }


        [Auth.Class.Auth("sistema", "padrao", "extrato")]
        public ActionResult ReportDetalhado()
        {
            try
            {
                LogBLL.Insert(new LogDado("ReportDetalhado", "Caixa", _idUsuario));
                using (var db = new Context())
                {
                    return new Report.Class.CaixaDetalhado().GetReport(db, _idUsuario);
                }

            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth(true)]
        public JsonResult JsGetExtrato(int dia)
        {
            try
            {
                LogBLL.Insert(new LogDado("JsGetExtrato", "Caixa", _idUsuario));
                using (Context db = new Context())
                {
                    var fim = DateTime.Now;
                    var inicio = fim.AddDays(-dia);

                    string sql = @"create table #temp_caixa
                                    (
                                    id int,
                                    tipo nvarchar(15),
                                    saldo decimal,
                                    valorpago decimal,
                                    descricao nvarchar(255),
                                    dtlancamento datetime
                                    )

                                    insert into #temp_caixa VALUES(0,'', 
                                        (isnull((select SUM(valorpago) from contareceber where situacao = 'Pago' AND pagamento <= '" + inicio.ToString("yyyy-MM-dd") + @"'),0) -
                                         isnull((select SUM(valorpago) from contapagar where situacao = 'Pago' AND pagamento <= '" + inicio.ToString("yyyy-MM-dd") + @"'),0)),null,'', '" + inicio.ToString("yyyy-MM-dd") + @"')
                                    insert into #temp_caixa select 
                                        contapagar.idcontapagar,'Retirada', 0, (contapagar.valorpago* -1), 
                                        contapagar.descricao, contapagar.pagamento  from contapagar 
                                    where situacao = 'Pago' and (pagamento > '" + inicio.ToString("yyyy-MM-dd") + @"' and pagamento <= '"+fim.ToString("yyyy-MM-dd HH:mm")+ @"')
                                    insert into #temp_caixa select 
                                        contareceber.idcontareceber,'Entrada', 0, (contareceber.valorpago), 
                                        contareceber.descricao, contareceber.pagamento  from contareceber 
                                    where situacao = 'Pago' and (pagamento > '" + inicio.ToString("yyyy-MM-dd") + @"' and pagamento <= '" + fim.ToString("yyyy-MM-dd HH:mm") + @"')

                                    select * from #temp_caixa order by dtlancamento";

                    var list = db.Database.SqlQuery<Teste>(sql).ToList();
                        decimal aux = 0;
                        var result = new List<dynamic>();
                        foreach (var item in list.OrderBy(u => u.dtlancamento))
                        {
                            aux += (item.valorpago ?? 0) + item.saldo;
                            result.Add(new 
                            {
                                saldo = aux,
                                item.descricao,
                                dtlancamento = item.dtlancamento != null ? item.dtlancamento.Value.ToString("dd/MM/yyyy"):"",
                                item.id,
                                item.tipo,
                                item.valorpago
                            });
                        }

                    return Json(result, JsonRequestBehavior.AllowGet);
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

