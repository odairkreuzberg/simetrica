using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class RequisicaoBLL : RP.DataAccess.Repository<Requisicao>
    {
        public RequisicaoBLL(IContext db, int idUsuario) : base(db, idUsuario) { }


        protected override void BeforeInsert(Requisicao bean)
        {
            var _materialBLL = new MaterialBLL(db, _idUsuario);
            bean.dtRequisicao = DateTime.Now;
            bean.idUsuario = this._idUsuario;
            foreach (var item in bean.RequisicaoItens.ToList())
            {
                item.Requisicao = bean;
                decimal valor = _materialBLL.AtualizarSaldo(item.nrQuantidade, item.idMaterial);
                item.vlPreco = valor;
            }
        }

        public RP.DataAccess.PaginatedList<Requisicao> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Requisicao> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Requisicao>(query.OrderBy(o => o.idRequisicao), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Requisicao> Search(string filter)
        {
            IQueryable<Requisicao> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Requisicao> preSearch(string filter)
        {
            IQueryable<Requisicao> query = this.Find(null, u => u.Funcionario, u => u.Projeto);

            if (!string.IsNullOrEmpty(filter))
            {
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word.ToLower();
                    query = query.Where(p => p.dsObservacao.ToLower().Contains(temp) || p.Projeto.descricao.ToLower().Contains(temp) || p.Funcionario.nome.ToLower().Contains(temp));
                }
            }
            return query;
        }

        public void Remover(int id)
        {
            var requisicao = this.FindSingle(u => u.idRequisicao == id, u => u.RequisicaoItens);
            var _materialBLL = new MaterialBLL(db, _idUsuario);
            var _itemBLL = new RequisicaoItemBLL(db, _idUsuario);

            foreach (var item in requisicao.RequisicaoItens.ToList())
            {
                _materialBLL.AtualizarSaldo((item.nrQuantidade *(-1)), item.idMaterial);
                _itemBLL.Delete(item);
            }
            this.Delete(requisicao);
        }
    }

}
