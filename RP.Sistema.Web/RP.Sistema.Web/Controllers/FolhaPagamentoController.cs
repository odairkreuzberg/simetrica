using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Sistema.Web.Models.Cidade;
using RP.Util;
using System.Data;
using System.Runtime.Serialization;
using RP.Sistema.Web.Models.FolhaPagamento;
using RP.Sistema.BLL;
using System.Globalization;

namespace RP.Sistema.Web.Controllers
{
    public class FolhaPagamentoController : Controller
    {
        private int _idUsuario;

        public FolhaPagamentoController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

        #region ActionResult

        [PersistDataSearch("Search")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            var _result = new ListVM
            {
                Consulta = new ListVM.ConsultaVM
                {
                    ano = DateTime.Now.Year,
                    mes = DateTime.Now.Month
                },

                Ano = ListVM.GetAnos(),
            };
            return View(_result);
        }

        [PersistDataSearch]
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Search(ListVM.ConsultaVM Consulta, int? page, int? pagesize)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.FuncionarioBLL(db, _idUsuario);

                    var result = _bll.Search(Consulta.nome, Funcionario.ATIVO, page, pagesize, null);
                    var _result = new ListVM
                    {
                        Funcionarios = ListVM.E2VM(result, Consulta.mes, Consulta.ano),
                        Consulta = Consulta,
                        Ano = ListVM.GetAnos(),
                    };

                    return View("Index", _result);
                }
            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(int idFuncionario, int ano, int mes)
        {
            try
            {
                using (var db = new Context())
                {
                    CultureInfo culture = new CultureInfo("pt-BR");
                    DateTimeFormatInfo dtfi = culture.DateTimeFormat;
                    var _feriadoBLL = new BLL.FeriadoBLL(db, _idUsuario);
                    var _bll = new BLL.FuncionarioBLL(db, _idUsuario);
                    var _movimentoBLL = new BLL.MovimentoProfissionalBLL(db, _idUsuario);
                    var empresa = db.Entidades.FirstOrDefault();
                    var data = new DateTime(ano, mes, 1).AddMonths(1).AddDays(-1);
                    var feriados = _feriadoBLL.Find(u => u.nrMes == mes);
                    var _funcionario = _bll.FindSingle(e => e.idFuncionario == idFuncionario);
                    var _movimentos = _movimentoBLL.Find(e => e.idFuncionario == idFuncionario && e.situacao == MovimentoProfissional.SITUACAO_PENDENTE && e.tipo == MovimentoProfissional.TIPO_COMISSAO);
                    var _result = new FolhaVM
                    {
                        Funcionario = Models.Funcionario.Consultar.GetModel(_funcionario),
                        Pontos = FolhaVM.Ponto.GetPontos(ano, mes, idFuncionario, feriados.ToList(), empresa),
                        Proximos = FolhaVM.Comissao.GetComicoes(_movimentos.Where(u => u.dtVencimento > data).ToList()),
                        Comissoes = FolhaVM.Comissao.GetComicoes(_movimentos.Where(u => u.dtVencimento <= data).ToList()),
                        nrAno = ano,
                        nrMes = mes,
                        dsMes = culture.TextInfo.ToTitleCase(dtfi.GetMonthName(mes))
                    };
                    return View(_result);
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(FolhaVM model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var _bll = new CartaoPontoBLL(db, _idUsuario);
                            var _movimentoBLL = new MovimentoProfissionalBLL(db, _idUsuario);
                            var _folhaBLL = new FolhaPagamentoBLL(db, _idUsuario);
                            foreach (var item in model.Pontos)
                            {
                                var cartao = new CartaoPonto
                                {
                                    dsObservacao = item.dsObservacao,
                                    entradaExtra = string.IsNullOrEmpty(item.entradaExtra) ? null : (TimeSpan?)TimeSpan.Parse(item.entradaExtra + ":00"),
                                    entradaManha = string.IsNullOrEmpty(item.entradaManha) ? null : (TimeSpan?)TimeSpan.Parse(item.entradaManha + ":00"),
                                    entraTarde = string.IsNullOrEmpty(item.entraTarde) ? null : (TimeSpan?)TimeSpan.Parse(item.entraTarde + ":00"),
                                    saidaExtra = string.IsNullOrEmpty(item.saidaExtra) ? null : (TimeSpan?)TimeSpan.Parse(item.saidaExtra + ":00"),
                                    saidaManha = string.IsNullOrEmpty(item.saidaManha) ? null : (TimeSpan?)TimeSpan.Parse(item.saidaManha + ":00"),
                                    saidaTarde = string.IsNullOrEmpty(item.saidaTarde) ? null : (TimeSpan?)TimeSpan.Parse(item.saidaTarde + ":00"),
                                    flSituacao = item.flSituacao,
                                    idFuncionario = model.Funcionario.idFuncionario ?? 0,
                                    dtPonto = new DateTime(model.nrAno, item.nrMes, item.nrDia),
                                };
                                _bll.Insert(cartao);
                            }
                            if (model.Comissoes != null)
                            {
                                foreach (var item in model.Comissoes)
                                {
                                    _movimentoBLL.AtualizaMovimento(item.idMovimento, MovimentoProfissional.SITUACAO_AGUARDANDO_PAGAMENTO);
                                }
                            }
                            var folha = new FolhaPagamento 
                            {
                                idFuncionario = model.Funcionario.idFuncionario ?? 0,
                                nrAno = model.nrAno,
                                nrMes = model.nrMes,
                                situacao = FolhaPagamento.AGUARDANDO_PAGAMENTO                                 
                            };
                            _folhaBLL.Insert(folha);

                            _bll.SaveChanges();
                            trans.Complete();

                            this.AddFlashMessage("Folha de pagamento gerada com sucesso!", FlashMessage.SUCCESS);
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
        public ActionResult Pagar(int idFolha, int idFuncionario, int ano, int mes)
        {
            try
            {
                using (var db = new Context())
                {
                    var culture = new CultureInfo("pt-BR");
                    var dtfi = culture.DateTimeFormat;
                    var _bll = new BLL.FuncionarioBLL(db, _idUsuario);
                    var _movimentoBLL = new BLL.MovimentoProfissionalBLL(db, _idUsuario);

                    var _funcionario = _bll.FindSingle(e => e.idFuncionario == idFuncionario);
                    var _movimentos = _movimentoBLL.Find(e => e.idFuncionario == idFuncionario && 
                                                        ((e.situacao == MovimentoProfissional.SITUACAO_AGUARDANDO_PAGAMENTO && e.tipo == MovimentoProfissional.TIPO_COMISSAO )||
                                                        (e.situacao == MovimentoProfissional.SITUACAO_PENDENTE && e.tipo == MovimentoProfissional.TIPO_VALE)));
                    var _result = new PagarVM
                    {
                        Funcionario = Models.Funcionario.Consultar.GetModel(_funcionario),
                        Movimentos = PagarVM.Movimento.GetMovimentos(_movimentos.ToList()),
                        nrAno = ano,
                        nrMes = mes,
                        dsMes = culture.TextInfo.ToTitleCase(dtfi.GetMonthName(mes)),
                        salario = _funcionario.salario,
                        mensalista = _funcionario.flMensalista,
                        idFolha = idFolha
                    };
                    return View(_result);
                }
            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Pagar(PagarVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {

                            var _bll = new BLL.FolhaPagamentoBLL(db, _idUsuario);
                            var _folha = _bll.FindSingle(u => u.idFolhaPagamento == model.idFolha);
                            _folha.situacao = FolhaPagamento.PAGO;
                            _folha.total = model.totalReceber ?? 0;
                            if (model.Movimentos != null)
                            {
                                _folha.comissao = model.Movimentos.Sum(u => u.comissao) ?? 0;
                                _folha.vale = model.Movimentos.Sum(u => u.vale) ?? 0;
                            }
                            _folha.salario = model.salario ?? 0;
                            _folha.bonificacao = model.bonificacao ?? 0;
                            _folha.outrosDescontos = model.outrosDescontos ?? 0;
                            _folha.dsBonificacao = string.IsNullOrEmpty(model.dsBonificacao) ? "Bonificação referente ao mês de " + model.dsMes : model.dsBonificacao;
                            _folha.dsOutrosDescontos = string.IsNullOrEmpty(model.dsOutrosDescontos) ? "Descontos adicionais referente ao mês de " + model.dsMes : model.dsOutrosDescontos;
                            _folha.inss = model.inss ?? 0;
                            _folha.FGTS = model.FGTS ?? 0;
                            _folha.horaExtra = model.horaExtra ?? 0;

                            _bll.Update(_folha);

                            //realizar pagamento e retirada do caixa


                            var _caixaBLL = new BLL.CaixaBLL(db, _idUsuario);

                            var _conta = new ContaPagar
                            {
                                descricao = "Pagamento realizado para " + model.Funcionario.nome + " referente ao mes " + model.dsMes + " de " + model.nrAno,
                                valorConta = _folha.total,
                                valorPago = _folha.total,
                                vencimento = DateTime.Now,
                                pagamento = DateTime.Now,
                                parcela = 1,
                                flFormaPagamento = "Dinheiro",
                                situacao = ContaPagar.SITUACAO_PAGO,
                                idUsuario = this._idUsuario,
                                idFolhaPagamento = model.idFolha
                            };

                            var _caixa = new Caixa
                            {
                                ContaPagar = _conta,
                                situacao = Caixa.CORENTE,
                                valor = (_folha.total * -1),
                                descricao = "Pagamento realizado para " + model.Funcionario.nome + " referente ao mes " + model.dsMes + " de " + model.nrAno,
                                dtLancamento = DateTime.Now

                            };
                            _caixaBLL.Insert(_caixa);

                            // atualiza os movimentos
                            if (model.Movimentos != null)
                            {
                                var _movimentoBLL = new MovimentoProfissionalBLL(db, _idUsuario);
                                foreach (var item in model.Movimentos)
                                {
                                    _movimentoBLL.AtualizaMovimento(item.idMovimento, MovimentoProfissional.SITUACAO_PAGO, _folha);
                                }
                            }

                            _bll.SaveChanges();

                            trans.Complete();

                            this.AddFlashMessage("Pagamento reaizado com sucesso, verifique contas a pagar!", FlashMessage.SUCCESS);
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
        public ActionResult Report(ListVM.ConsultaVM consulta)
        {
            try
            {
                using (var db = new Context())
                {
                    return new Report.Class.HorasExtras().GetReport(db, consulta.ano, consulta.mes, _idUsuario);
                }

            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult ReportMes(ListVM.ConsultaVM consulta)
        {
            try
            {
                using (var db = new Context())
                {
                    return new Report.Class.FolhaPagamento().GetReport(db, consulta.ano, consulta.mes, _idUsuario);
                }

            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult FolhaFrequencia(ListVM.ConsultaVM consulta, int idFuncionario)
        {
            try
            {
                using (var db = new Context())
                {
                    return new Report.Class.FolhaFrequencia().GetReport(db, idFuncionario, consulta.ano, consulta.mes, _idUsuario);
                }

            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Recibo(int idFolha)
        {
            try
            {
                using (var db = new Context())
                {
                    return new Report.Class.Recibo().GetReport(db, idFolha, _idUsuario);
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

