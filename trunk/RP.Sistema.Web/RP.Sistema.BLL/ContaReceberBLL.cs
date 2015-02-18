using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class ContaReceberBLL : RP.DataAccess.Repository<ContaReceber>
    {
        public ContaReceberBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        protected override void BeforeInsert(ContaReceber bean)
        {
            bean.idUsuario = this._idUsuario;
        }


        public RP.DataAccess.PaginatedList<ContaReceber> Search(string filter, DateTime? dtInicio, DateTime? dtFim, string situacao, int? page, int? pagesize)
        {
            IQueryable<ContaReceber> query = preSearch(filter);
            if (dtFim != null)
            {
                dtFim = dtFim.Value.AddDays(1);
                query = query.Where(u => u.vencimento < dtFim);
            }
            if (dtInicio != null)
            {
                query = query.Where(u => u.vencimento >= dtInicio);
            }
            if (situacao != "Todos")
                query = query.Where(u => u.situacao == situacao);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.ContaReceber>(query.OrderBy(o => new { o.vencimento, o.idContaReceber }), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        private IQueryable<ContaReceber> preSearch(string filter)
        {
            IQueryable<ContaReceber> query = this.Find(u => u.situacao != ContaReceber.SITUACAO_CANCELADO, u => u.Cliente, u => u.Projeto);

            if (!string.IsNullOrEmpty(filter))
            {
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word.ToLower();
                    query = query.Where(p => p.Cliente.nome.ToLower().Contains(temp) || p.Projeto.descricao.ToLower().Contains(temp) || p.descricao.ToLower().Contains(temp));
                }
            }
            return query;
        }

        public void Cancelar(int id)
        {

            var _caixaBLL = new CaixaBLL(db, _idUsuario);
            var _conta = this.FindSingle(u => u.idContaReceber == id, u => u.Caixas, u => u.Cliente);
            var contas = this.Find(u => u.idOrigem == id && u.situacao != ContaReceber.SITUACAO_CANCELADO, u => u.Caixas, u => u.Cliente);
            foreach (var item in contas.ToList())
            {
                item.situacao = ContaReceber.SITUACAO_CANCELADO;
                ((Model.Context)db).ContasReceber.Attach(item);
                ((Model.Context)db).Entry(item).Property(e => e.situacao).IsModified = true;

                foreach (var caixa in item.Caixas.ToList())
                {
                    if (caixa.situacao == Caixa.CORENTE)
                    {
                        caixa.situacao = ContaReceber.SITUACAO_CANCELADO;
                        ((Model.Context)db).Caixas.Attach(caixa);
                        ((Model.Context)db).Entry(caixa).Property(e => e.situacao).IsModified = true;
                        _caixaBLL.Insert(new Caixa
                        {
                            descricao = "Extorno de caixa referente ao cancelamento da conta. [ " + item.Cliente.nome + " ]",
                            dtLancamento = DateTime.Now,
                            situacao = Caixa.EXTORNO,
                            valor = (caixa.valor * -1),
                            idCaixaExtorno = caixa.idCaixa
                        });
                        db.SaveChanges(_idUsuario);
                    }
                }

            }
            _conta.situacao = ContaReceber.SITUACAO_CANCELADO;
            ((Model.Context)db).ContasReceber.Attach(_conta);
            ((Model.Context)db).Entry(_conta).Property(e => e.situacao).IsModified = true;

            foreach (var caixa in _conta.Caixas.ToList())
            {
                if (caixa.situacao == Caixa.CORENTE)
                {
                    caixa.situacao = ContaReceber.SITUACAO_CANCELADO;
                    ((Model.Context)db).Caixas.Attach(caixa);
                    ((Model.Context)db).Entry(caixa).Property(e => e.situacao).IsModified = true;
                    _caixaBLL.Insert(new Caixa
                    {
                        descricao = "Extorno de caixa referente ao cancelamento da conta. [ " + _conta.Cliente.nome + " ]",
                        dtLancamento = DateTime.Now,
                        situacao = Caixa.EXTORNO,
                        valor = (caixa.valor * -1),
                        idCaixaExtorno = caixa.idCaixa
                    });
                }
            }
        }


        public void Receber(ContaReceber bean)
        {
            bean.situacao = ContaReceber.SITUACAO_PAGO;
            ((Model.Context)db).ContasReceber.Attach(bean);
            ((Model.Context)db).Entry(bean).Property(e => e.situacao).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.pagamento).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.valorPago).IsModified = true;
        }
    }
}
