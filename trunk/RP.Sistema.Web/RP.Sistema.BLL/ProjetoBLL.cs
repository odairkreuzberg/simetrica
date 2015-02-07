using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class ProjetoBLL : RP.DataAccess.Repository<Projeto>
    {
        public ProjetoBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        private ICollection<Produto> Produtos;

        protected override void BeforeInsert(Projeto bean)
        {
            bean.idUsuario = this._idUsuario;
            bean.status = Projeto.ORCAMENTO;
            bean.flConcluido = "Não";
        }

        public override void Update(Projeto bean)
        {
            ValidUpdate(bean);
            //BeforeUpdate(bean);
            var bll = new ProdutoBLL(db, _idUsuario);

            var produtos = bean.Produtos;
            bean.Produtos = null;
            var produtosDB = bll.Find(u => u.idProjeto == bean.idProjeto).Select(u => u.idProduto).ToList();

            ((Model.Context)db).Projetos.Attach(bean);
            ((Model.Context)db).Entry(bean).Property(e => e.dtFim).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.descricao).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.porcentagemVendedor).IsModified = true;

            foreach (var item in produtosDB.Where(u => produtos.All(k => k.idProduto != u)))
            {
                bll.Delete(u => u.idProduto == item); 
            }
            foreach (var item in produtos.Where(u => u.idProduto == 0))
            {
                item.idProjeto = bean.idProjeto;
                bll.Insert(item); 
            }
            foreach (var item in produtos.Where(u => produtosDB.Any(k => u.idProduto == k)))
            {
                item.idProjeto = bean.idProjeto;
                ((Model.Context)db).Produtos.Attach(item);
                ((Model.Context)db).Entry(item).Property(e => e.idProjeto).IsModified = false;
                ((Model.Context)db).Entry(item).Property(e => e.porcentagemMarceneiro).IsModified = true;
                ((Model.Context)db).Entry(item).Property(e => e.porcentagemProjetista).IsModified = true;
                ((Model.Context)db).Entry(item).Property(e => e.margemGanho).IsModified = true;
                ((Model.Context)db).Entry(item).Property(e => e.nome).IsModified = true;
                ((Model.Context)db).Entry(item).Property(e => e.descricao).IsModified = true;
                ((Model.Context)db).Entry(item).Property(e => e.idProjetista).IsModified = true;
                ((Model.Context)db).Entry(item).Property(e => e.idMarceneiro).IsModified = true;
            }

               //AfterUpdate(bean);
        }

        public void Aprovar(Projeto bean)
        {
            ValidUpdate(bean);
            BeforeUpdate(bean);

            ((Model.Context)db).Projetos.Attach(bean);
            ((Model.Context)db).Entry(bean).Property(e => e.status).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.porcentagemVendedor).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.vlVenda).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.vlDesconto).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.vlProjeto).IsModified = true;

            AfterUpdate(bean);
        }

        protected override void BeforeUpdate(Projeto bean)
        {
            if (bean.Produtos != null)
                Produtos = bean.Produtos.ToList();
            bean.Produtos = null;
        }

        protected override void AfterUpdate(Projeto bean)
        {
            AtualizaProduto(bean);
        }

        private void AtualizaProduto(Projeto bean)
        {
            var _bll = new ProdutoBLL(db, _idUsuario);
            var _materialBLL = new ProdutoMaterialBLL(db, _idUsuario);
            var _produtosDB = _bll.Find(u => u.idProjeto == bean.idProjeto, u => u.ProdutoMateriais);


            bean.Produtos = _produtosDB.ToList();

            foreach (var item in bean.Produtos.ToList())
            {
                if (!Produtos.Any(u => u.idProduto == item.idProduto))
                {
                    foreach (var material in item.ProdutoMateriais.ToList())
                    {
                        _materialBLL.Delete(material);
                    }
                    _bll.Delete(u => u.idProduto == item.idProduto);
                }
            }

            if (Produtos != null)
            {
                foreach (var item in Produtos.ToList())
                {
                    if (!_produtosDB.Any(u => u.idProduto == item.idProduto))
                    {
                        item.projeto = bean;
                        _bll.Insert(item);
                    }
                    else
                    {
                        item.idProjeto = bean.idProjeto;
                        var _produto = _produtosDB.FirstOrDefault(u => u.idProduto == item.idProduto);
                        ((Model.Context)db).Entry(_produto).CurrentValues.SetValues(item);
                        _bll.Update(_produto);
                    }
                }
            }
        }

        public RP.DataAccess.PaginatedList<Projeto> Search(string filter, int? page, int? pagesize, bool concluido = false)
        {
            IQueryable<Projeto> query = preSearch(filter);
            if (concluido)
                query = query.Where(u => u.flConcluido == "Não" && u.status != Projeto.CANCELADO);
            query = query.Where(u => u.status != Projeto.CANCELADO);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Projeto>(query.OrderBy(o => o.idProjeto), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Projeto> Search(string filter)
        {
            IQueryable<Projeto> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Projeto> preSearch(string filter)
        {
            IQueryable<Projeto> query = this.Find(null, u => u.Cliente);

            if (!string.IsNullOrEmpty(filter))
            {
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word.ToLower();
                    query = query.Where(p => p.descricao.ToLower().Contains(temp));
                }
            }
            return query;
        }

        public void AddCustos(Projeto bean)
        {
            var _bll = new ProjetoCustoBLL(db, _idUsuario);
            var _custosDB = _bll.Find(u => u.idProjeto == bean.idProjeto && u.situacao == ProjetoCusto.NORMAL).ToList();
            var _custosVW = bean.ProjetoCustos.ToList();

            foreach (var item in _custosDB)
            {
                if (!_custosVW.Any(u => u.idProjetoCusto == item.idProjetoCusto))
                {
                    _bll.Cancelar(item);
                }
            }
            foreach (var item in _custosVW)
            {
                if (!_custosDB.Any(u => u.idProjetoCusto == item.idProjetoCusto))
                {
                    item.idProjeto = bean.idProjeto;
                    _bll.Insert(item);
                }
            }
        }

        public void Cancelar(int id)
        {
            if (this.Exist(u => u.idProjeto == id && u.MovimentoProfissionais.Any(k => k.situacao == MovimentoProfissional.SITUACAO_PAGO)))
            {
                throw new Exception("Este projeto nao pode mais ser cancelado");
            }

            var _projeto = this.FindSingle(u => u.idProjeto == id, u => u.MovimentoProfissionais, u => u.ContasReceber, u => u.ContasReceber.Select(k => k.Caixas));
            foreach (var item in _projeto.MovimentoProfissionais.ToList())
            {
                item.situacao = MovimentoProfissional.SITUACAO_CANCELADO;
                ((Model.Context)db).MovimentoProfissionais.Attach(item);
                ((Model.Context)db).Entry(item).Property(e => e.situacao).IsModified = true;
            }

            foreach (var item in _projeto.ContasReceber.ToList())
            {
                item.situacao = ContaReceber.SITUACAO_CANCELADO;
                ((Model.Context)db).ContasReceber.Attach(item);
                ((Model.Context)db).Entry(item).Property(e => e.situacao).IsModified = true;
                var _caixaBLL = new CaixaBLL(db, _idUsuario);
                foreach (var caixa in item.Caixas.ToList())
                {
                    if (caixa.situacao == Caixa.CORENTE)
                    {
                        caixa.situacao = ContaReceber.SITUACAO_CANCELADO;
                        ((Model.Context)db).Caixas.Attach(caixa);
                        ((Model.Context)db).Entry(caixa).Property(e => e.situacao).IsModified = true;
                        _caixaBLL.Insert(new Caixa
                        {
                            descricao = "Cancelamento de caixa referente o projeto " + _projeto.descricao,
                            dtLancamento = DateTime.Now,
                            idCaixaExtorno = caixa.idCaixa,
                            situacao = Caixa.EXTORNO,
                            valor = (caixa.valor * -1)
                        });
                    }
                }

            }
            _projeto.status = Projeto.CANCELADO;
            ((Model.Context)db).Projetos.Attach(_projeto);
            ((Model.Context)db).Entry(_projeto).Property(e => e.status).IsModified = true;
        }

        public void Finalizar(int id)
        {
            var _projeto = this.FindSingle(u => u.idProjeto == id);
            _projeto.flConcluido = "Sim";
            ((Model.Context)db).Projetos.Attach(_projeto);
            ((Model.Context)db).Entry(_projeto).Property(e => e.flConcluido).IsModified = true;
        }
    }
}
