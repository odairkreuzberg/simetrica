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
    public class OrcamentoController : Controller
    {
        private int _idUsuario;

        public OrcamentoController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

        #region ActionResult

        [PersistDataSearch("Search")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            LogBLL.Insert(new LogDado("Index", "Orcamento", _idUsuario));
            return View();
        }

        [PersistDataSearch]
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Search(string filter, int? page, int? pagesize)
        {
            try
            {
                LogBLL.Insert(new LogDado("Search", "Orcamento", _idUsuario));
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
        public ActionResult Projeto(int idProjeto)
        {
            try
            {
                LogBLL.Insert(new LogDado("Projeto", "Orcamento", _idUsuario));
                using (var db = new Context())
                {
                    var _bll = new BLL.ProjetoBLL(db, _idUsuario);
                    var _produtoBLL = new BLL.ProdutoBLL(db, _idUsuario);

                    var _projeto = _bll.Find(e => e.idProjeto == idProjeto)
                        .Select(u => new Models.Orcamento.ProjetoVM
                        {
                            idProjeto = idProjeto,
                            descricao = u.descricao,                            
                        }).FirstOrDefault();
                    _projeto.Produtos = _produtoBLL.Find(u => u.idProjeto == idProjeto)
                        .Select(k => new
                             Models.Orcamento.ProjetoVM.ProdutoVM
                             {
                                 idProduto = k.idProduto,
                                 nome = k.nome,
                                 descricao = k.descricao
                             }).ToList();
                    return View(_projeto);
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }


        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Produto(int idProduto)
        {
            try
            {
                using (var db = new Context())
                {
                    var _produtoBLL = new BLL.ProdutoBLL(db, _idUsuario);
                    var _materialBLL = new BLL.ProdutoMaterialBLL(db, _idUsuario);

                    var _produto = _produtoBLL.Find(e => e.idProduto == idProduto)
                        .Select(u => new Models.Orcamento.ProdutoVM
                        {
                            idProjeto = u.idProjeto,
                            nome = u.nome,
                            idProduto = u.idProduto,
                            descricao = u.descricao,
                        }).FirstOrDefault();
                    _produto.Itens = _materialBLL.Find(u => u.idProduto == idProduto)
                        .Select(k => new
                             Models.Orcamento.ProdutoVM.MaterialVM
                        {
                            idMaterial = k.idMaterial,
                            nome = k.Material.nome,
                            quantidade = k.quantidade
                        }).ToList();
                    return View(_produto);
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
        public ActionResult Produto(Models.Orcamento.ProdutoVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    LogBLL.Insert(new LogDado("Produto", "Orcamento", _idUsuario));
                    using (var db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            var materiais = new List<Material>();
                            if (model.Itens != null)
                            {
                                foreach (var item in model.Itens)
                                {
                                    materiais.Add(new Material { idMaterial = item.idMaterial.Value, nrQuantidade = item.quantidade.Value});
                                }
                            }
                            var _produtoMaterial = new ProdutoMaterialBLL(db, _idUsuario);
                            _produtoMaterial.Orcamento(model.idProduto, model.idProjeto, materiais);
                            _produtoMaterial.SaveChanges();
                            trans.Complete();

                            this.AddFlashMessage("Orçamento realizado com sucesso", FlashMessage.SUCCESS);
                            return RedirectToAction("Projeto", new { model.idProjeto});
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


        //[Auth.Class.Auth("sistema", "padrao")]
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[Auth.Class.Auth("sistema", "padrao")]
        //public ActionResult Create(AprovarVM model)
        //{
        //    if (string.IsNullOrEmpty(model.Vendedor.nome))
        //    {
        //        ModelState.AddModelError("Vendedor.nome", "Selecione um vendedor");
        //    }
        //    if (string.IsNullOrEmpty(model.Cliente.nome))
        //    {
        //        ModelState.AddModelError("Cliente.nome", "Selecione um cliente");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            using (var db = new Context())
        //            {
        //                using (var trans = new RP.DataAccess.RPTransactionScope(db))
        //                {
        //                    var _projeto = model.GetProjeto();

        //                    var _bll = new BLL.ProjetoBLL(db, _idUsuario);

        //                    _bll.Insert(_projeto);
        //                    _bll.SaveChanges();
        //                    trans.Complete();

        //                    model.idProjeto = _projeto.idProjeto;

        //                    this.AddFlashMessage(RP.Util.Resource.Message.INSERT_SUCCESS, FlashMessage.SUCCESS);
        //                    return RedirectToAction("Index");
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //            return RedirectToAction("Index", "Erro", new { area = string.Empty });
        //        }
        //    }
        //    return View(model);
        //}

        //[Auth.Class.Auth("sistema", "padrao")]
        //public ActionResult Edit(int id)
        //{
        //    return this.GetView(id);
        //}

        //[HttpPost]
        //[Auth.Class.Auth("sistema", "padrao")]
        //public ActionResult Edit(AprovarVM model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            using (var db = new Context())
        //            {
        //                using (var trans = new RP.DataAccess.RPTransactionScope(db))
        //                {
        //                    var _projeto = model.GetProjeto();

        //                    var _bll = new BLL.ProjetoBLL(db, _idUsuario);

        //                    _bll.Update(_projeto);
        //                    _bll.SaveChanges();

        //                    //_projeto = _bll.FindSingle(e => e.idProjeto == model.idProjeto,
        //                    //    u => u.Cliente, u => u.Vendedor, u => u.Produtos, u => u.ProjetoCustos,
        //                    //    u => u.Produtos.Select(l => l.ProdutoMateriais),
        //                    //    u => u.Produtos.Select(k => k.Marceneiro), u => u.Produtos.Select(k => k.Projetista));

        //                    //model = AprovarVM.GetProjeto(_projeto);

        //                    //_projeto.vlProjeto = model.Produtos.Sum(u => u.vlProduto);
        //                    //_projeto.vlVenda = model.Produtos.Sum(u => u.vlVenda);
        //                    //_projeto.vlDesconto = model.Produtos.Sum(u => u.vlDesconto);

        //                    //foreach (var item in _projeto.Produtos)
        //                    //{
        //                    //    item.vlVenda = model.Produtos.First(u => u.idProduto == item.idProduto).vlVenda;
        //                    //    item.vlProduto = model.Produtos.First(u => u.idProduto == item.idProduto).vlProduto;
        //                    //    item.vlDesconto = model.Produtos.First(u => u.idProduto == item.idProduto).vlDesconto;
        //                    //}

        //                    //_bll.Aprovar(_projeto);
        //                    _bll.SaveChanges();

        //                    trans.Complete();

        //                    this.AddFlashMessage(RP.Util.Resource.Message.EDIT_SUCCESS, FlashMessage.SUCCESS);
        //                    return RedirectToAction("Index");
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //            return RedirectToAction("Index", "Erro", new { area = string.Empty });
        //        }
        //    }
        //    return View(model);
        //}

        //[Auth.Class.Auth("sistema", "padrao")]
        //public ActionResult AddItem(int id)
        //{
        //    return this.GetView(id);
        //}
        //[HttpPost]
        //[Auth.Class.Auth("sistema", "padrao")]
        //public ActionResult AddItem(AprovarVM model)
        //{
        //    try
        //    {
        //        using (var db = new Context())
        //        {
        //            using (var trans = new RP.DataAccess.RPTransactionScope(db))
        //            {
        //                var _bll = new BLL.ProjetoBLL(db, _idUsuario);

        //                var _projeto = _bll.FindSingle(e => e.idProjeto == model.idProjeto,
        //                    u => u.Cliente, u => u.Vendedor, u => u.Produtos, u => u.ProjetoCustos,
        //                    u => u.Produtos.Select(l => l.ProdutoMateriais),
        //                    u => u.Produtos.Select(k => k.Marceneiro), u => u.Produtos.Select(k => k.Projetista));

        //                model = AprovarVM.GetProjeto(_projeto);

        //                _projeto.vlProjeto = model.Produtos.Sum(u => u.vlProduto);
        //                _projeto.vlVenda = model.Produtos.Sum(u => u.vlVenda);
        //                _projeto.vlDesconto = model.Produtos.Sum(u => u.vlDesconto);

        //                foreach (var item in _projeto.Produtos)
        //                {
        //                    item.vlVenda = model.Produtos.First(u => u.idProduto == item.idProduto).vlVenda;
        //                    item.vlProduto = model.Produtos.First(u => u.idProduto == item.idProduto).vlProduto;
        //                    item.vlDesconto = model.Produtos.First(u => u.idProduto == item.idProduto).vlDesconto;
        //                }

        //                _bll.Aprovar(_projeto);
        //                _bll.SaveChanges();
        //                trans.Complete();

        //                this.AddFlashMessage("Projeto atualizado com sucesso!", FlashMessage.SUCCESS);
        //                return RedirectToAction("Index");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //        return RedirectToAction("Index", "Erro", new { area = string.Empty });
        //    }
        //}


        //[Auth.Class.Auth("sistema", "padrao")]
        //public ActionResult AddCusto(int id)
        //{
        //    try
        //    {
        //        using (var db = new Context())
        //        {
        //            var _bll = new BLL.ProjetoBLL(db, _idUsuario);

        //            var _projeto = _bll.FindSingle(e => e.idProjeto == id, u => u.Cliente, u => u.ProjetoCustos);

        //            return View(CustoProjetoVM.GetProjeto(_projeto));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //        return RedirectToAction("Index", "Erro", new { area = string.Empty });
        //    }
        //}

        //[Auth.Class.Auth("sistema", "padrao")]
        //public ActionResult Aprovar(int id)
        //{
        //    try
        //    {
        //        using (var db = new Context())
        //        {
        //            var _bll = new BLL.ProjetoBLL(db, _idUsuario);

        //            var _projeto = _bll.FindSingle(e => e.idProjeto == id,
        //                u => u.Cliente, u => u.Vendedor, u => u.Produtos, u => u.ProjetoCustos,
        //                u => u.Produtos.Select(l => l.ProdutoMateriais),
        //                u => u.Produtos.Select(k => k.Marceneiro), u => u.Produtos.Select(k => k.Projetista));

        //            return View(AprovarVM.GetProjeto(_projeto));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //        return RedirectToAction("Index", "Erro", new { area = string.Empty });
        //    }
        //}



        //[HttpPost]
        //[Auth.Class.Auth("sistema", "padrao")]
        //public ActionResult Aprovar(AprovarVM model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            using (var db = new Context())
        //            {
        //                using (var trans = new RP.DataAccess.RPTransactionScope(db))
        //                {
        //                    var _projeto = model.GetProjeto();

        //                    var _bll = new BLL.ProjetoBLL(db, _idUsuario);
        //                    if (_projeto.status == Projeto.VENDIDO)
        //                    {
        //                        var _movimentoBLL = new MovimentoProfissionalBLL(db, _idUsuario);
        //                        var _contaBLL = new ContaReceberBLL(db, _idUsuario);
        //                        int nrParcelas = model.Parcelas.Count;
        //                        foreach (var item in model.Parcelas)
        //                        {
        //                            //comissao do vendedor
        //                            _movimentoBLL.Insert(new MovimentoProfissional
        //                            {
        //                                tipo = MovimentoProfissional.TIPO_COMISSAO,
        //                                idFuncionario = model.Vendedor.idFuncionario ?? 0,
        //                                valor = ((model.porcentagemVendedor / 100) * item.vlParcela),
        //                                idProjeto = _projeto.idProjeto,
        //                                situacao = MovimentoProfissional.SITUACAO_PENDENTE,
        //                                descricao = "Comissão referente a " + item.nrParcela + "º parcela do projeto " + _projeto.descricao,
        //                                dtVencimento = item.dtVencimento
        //                            });

        //                            foreach (var produto in _projeto.Produtos)
        //                            {
        //                                decimal vlParcela = (produto.vlVenda ?? 0) / nrParcelas;

        //                                //comissao do projetista
        //                                _movimentoBLL.Insert(new MovimentoProfissional
        //                                {
        //                                    tipo = MovimentoProfissional.TIPO_COMISSAO,
        //                                    idFuncionario = produto.idProjetista ?? 0,
        //                                    valor = (((produto.porcentagemProjetista ?? 0) / 100) * vlParcela),
        //                                    idProjeto = _projeto.idProjeto,
        //                                    situacao = MovimentoProfissional.SITUACAO_PENDENTE,
        //                                    descricao = "Comissão referente a " + item.nrParcela + "º parcela do projeto " + _projeto.descricao + " [" + produto.nome + "]",
        //                                    dtVencimento = item.dtVencimento
        //                                });

        //                                //comissao do marceneiro
        //                                _movimentoBLL.Insert(new MovimentoProfissional
        //                                {
        //                                    tipo = MovimentoProfissional.TIPO_COMISSAO,
        //                                    idFuncionario = produto.idMarceneiro ?? 0,
        //                                    valor = (((produto.porcentagemMarceneiro ?? 0) / 100) * vlParcela),
        //                                    idProjeto = _projeto.idProjeto,
        //                                    situacao = MovimentoProfissional.SITUACAO_PENDENTE,
        //                                    descricao = "Comissão referente a " + item.nrParcela + "º parcela do projeto " + _projeto.descricao + " [" + produto.nome + "]",
        //                                    dtVencimento = item.dtVencimento
        //                                });

        //                            }

        //                            string situacao = ContaReceber.SITUACAO_AGUARDANDO_PAGAMENTO;
        //                            decimal? vlPago = null;
        //                            DateTime? dtPagamento = null;
        //                            if (item.dtVencimento <= DateTime.Now.Date)
        //                            {
        //                                vlPago = item.vlParcela;
        //                                situacao = ContaReceber.SITUACAO_PAGO;
        //                                dtPagamento = item.dtVencimento;
        //                            }

        //                            // Lança conta a receber referente a parcela
        //                            var _conta = new ContaReceber
        //                            {
        //                                idCliente = _projeto.idCliente,
        //                                parcela = item.nrParcela,
        //                                descricao = "Conta a receber referente a " + item.nrParcela + "º parcela do projeto " + _projeto.descricao + ".  " + item.dsObservacao,
        //                                vencimento = item.dtVencimento,
        //                                pagamento = dtPagamento,
        //                                valorConta = item.vlParcela,
        //                                valorPago = vlPago,
        //                                situacao = situacao,
        //                                flFormaPagamento = item.flFormaPagamento,
        //                                idProjeto = _projeto.idProjeto
        //                            };
        //                            _contaBLL.Insert(_conta);

        //                            //se a data de vencimento for a atual lança a entrada no caixa
        //                            if (item.dtVencimento <= DateTime.Now.Date)
        //                            {
        //                                var _caixaBLL = new CaixaBLL(db, _idUsuario);
        //                                _caixaBLL.Insert(new Caixa
        //                                {
        //                                    ContaReceber = _conta,
        //                                    situacao = Caixa.CORENTE,
        //                                    valor = item.vlParcela,
        //                                    descricao = "Conta a recebida de " + model.Cliente.nome + " " + item.dsObservacao,
        //                                    dtLancamento = item.dtVencimento
        //                                });
        //                            }
        //                        }
        //                    }

        //                    _bll.Aprovar(_projeto);
        //                    _bll.SaveChanges();
        //                    trans.Complete();

        //                    this.AddFlashMessage("Projeto atualizado com sucesso!", FlashMessage.SUCCESS);
        //                    return RedirectToAction("Index");
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //            return RedirectToAction("Index", "Erro", new { area = string.Empty });
        //        }
        //    }
        //    return View(model);
        //}



        //[HttpPost]
        //[Auth.Class.Auth("sistema", "padrao")]
        //public ActionResult AddCusto(CustoProjetoVM model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            using (var db = new Context())
        //            {
        //                using (var trans = new RP.DataAccess.RPTransactionScope(db))
        //                {
        //                    var _projeto = model.GetProjeto();

        //                    var _bll = new BLL.ProjetoBLL(db, _idUsuario);

        //                    _bll.AddCustos(_projeto);
        //                    _bll.SaveChanges();
        //                    trans.Complete();

        //                    this.AddFlashMessage(RP.Util.Resource.Message.INSERT_SUCCESS, FlashMessage.SUCCESS);
        //                    return RedirectToAction("Index");
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //            return RedirectToAction("Index", "Erro", new { area = string.Empty });
        //        }
        //    }
        //    return View(model);
        //}

        //[Auth.Class.Auth("sistema", "padrao")]
        //public ActionResult Delete(int id)
        //{
        //    try
        //    {
        //        using (var db = new Context())
        //        {
        //            using (var trans = new RP.DataAccess.RPTransactionScope(db))
        //            {
        //                var _bll = new BLL.ProjetoBLL(db, _idUsuario);

        //                _bll.Cancelar(id);
        //                _bll.SaveChanges();

        //                trans.Complete();

        //                this.AddFlashMessage("Projeto cancelado com sucesso", FlashMessage.SUCCESS);
        //                return RedirectToAction("Index");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //        RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //        return RedirectToAction("Index");
        //    }
        //}

        //[Auth.Class.Auth("sistema", "padrao")]
        //public ActionResult Finalizar(int id)
        //{
        //    try
        //    {
        //        using (var db = new Context())
        //        {
        //            using (var trans = new RP.DataAccess.RPTransactionScope(db))
        //            {
        //                var _bll = new BLL.ProjetoBLL(db, _idUsuario);

        //                _bll.Finalizar(id);
        //                _bll.SaveChanges();

        //                trans.Complete();

        //                this.AddFlashMessage("Projeto finalizado com sucesso", FlashMessage.INFO);
        //                return RedirectToAction("Index");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //        RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //        return RedirectToAction("Index");
        //    }
        //}

        //[Auth.Class.Auth("sistema", "padrao", "index")]
        //public ActionResult Report(string filter)
        //{
        //    try
        //    {
        //        using (var db = new Context())
        //        {
        //            //return new Report.Class.Projeto().GetReport(db, filter, _idUsuario);
        //            return RedirectToAction("Index");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //        return RedirectToAction("Index", "Erro", new { area = string.Empty });
        //    }
        //}

        //[Auth.Class.Auth("sistema", "padrao", "index")]
        //public ActionResult OrdemCompra(int idProjeto)
        //{
        //    try
        //    {
        //        using (var db = new Context())
        //        {
        //            return new Report.Class.OrdemCompra().GetReport(db, idProjeto, _idUsuario);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //        return RedirectToAction("Index", "Erro", new { area = string.Empty });
        //    }
        //}

        //[Auth.Class.Auth("sistema", "padrao", "index")]
        //public ActionResult RaioX(int idProjeto)
        //{
        //    try
        //    {
        //        using (var db = new Context())
        //        {
        //            return new Report.Class.RaioX().GetReport(db, idProjeto, _idUsuario);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //        return RedirectToAction("Index", "Erro", new { area = string.Empty });
        //    }
        //}

        //[Auth.Class.Auth("sistema", "padrao", "index")]
        //public ActionResult OrcamentoCliente(int idProjeto)
        //{
        //    try
        //    {
        //        using (var db = new Context())
        //        {
        //            return new Report.Class.OrcamentoCliente().GetReport(db, idProjeto, _idUsuario);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
        //        return RedirectToAction("Index", "Erro", new { area = string.Empty });
        //    }
        //}

        #endregion
    }
}

