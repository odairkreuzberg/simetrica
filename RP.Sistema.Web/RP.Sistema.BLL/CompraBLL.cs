using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class CompraBLL : RP.DataAccess.Repository<Compra>
    {
        public CompraBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        protected override void BeforeInsert(Compra bean)
        {
            var _materialBLL = new MaterialBLL(db, _idUsuario);
            bean.dtLancamento = DateTime.Now;
            bean.idUsuario = this._idUsuario;
            bean.flCancelado = "Não";
            foreach (var item in bean.ItensCompra.ToList())
            {
                item.Compra = bean;
                _materialBLL.AtualizarSaldo(item.quantidade, item.valor, item.idMaterial);
            }
        }

        public RP.DataAccess.PaginatedList<Compra> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Compra> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Compra>(query.OrderBy(o => o.idCompra), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Compra> Search(string filter)
        {
            IQueryable<Compra> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Compra> preSearch(string filter)
        {
            IQueryable<Compra> query = this.Find(u => u.flCancelado == "Não", u => u.Fornecedor, u => u.Projeto);

            if (!string.IsNullOrEmpty(filter))
            {
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word.ToLower();
                    query = query.Where(p => p.Fornecedor.nome.ToLower().Contains(temp) || p.Projeto.descricao.ToLower().Contains(temp) || p.descricao.ToLower().Contains(temp));
                }
            }
            return query;
        }

        public void Cancelar(int id)
        {
            var _compra = this.FindSingle(u => u.idCompra == id, u => u.ContasPagar.Select(k => k.Caixas));

            foreach (var item in _compra.ContasPagar.ToList())
            {
                item.situacao = ContaPagar.SITUACAO_CANCELADO;
                ((Model.Context)db).ContasPagar.Attach(item);
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
                            descricao = "Extorno de caixa referente ao cancelamento da compra de " + _compra.descricao,
                            dtLancamento = DateTime.Now,
                            situacao = Caixa.EXTORNO,
                            valor = (caixa.valor * -1),
                            idCaixaExtorno = caixa.idCaixa
                        });
                    }
                    db.SaveChanges(_idUsuario);
                }

            }
            _compra.flCancelado = "Sim";
            ((Model.Context)db).Compras.Attach(_compra);
            ((Model.Context)db).Entry(_compra).Property(e => e.flCancelado).IsModified = true;
        }
    }
}
