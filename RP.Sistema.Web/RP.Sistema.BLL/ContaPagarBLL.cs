using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class ContaPagarBLL : RP.DataAccess.Repository<ContaPagar>
    {
        public ContaPagarBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        protected override void BeforeInsert(ContaPagar bean)
        {
            bean.idUsuario = this._idUsuario;
        }


        public RP.DataAccess.PaginatedList<ContaPagar> Search(string filter, DateTime? dtInicio, DateTime? dtFim, string situacao, int? page, int? pagesize)
        {
            dtFim = dtFim.Value.AddDays(1);
            IQueryable<ContaPagar> query = preSearch(filter);
            if (situacao != "Todos")
                query = query.Where(u => u.situacao == situacao);
            query = query.Where(u => u.vencimento >= dtInicio && u.vencimento < dtFim);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.ContaPagar>(query.OrderBy(o => new { o.vencimento, o.idContaPagar }), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<ContaPagar> Search(string filter)
        {
            IQueryable<ContaPagar> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<ContaPagar> preSearch(string filter)
        {
            IQueryable<ContaPagar> query = this.Find(u => u.situacao != ContaPagar.SITUACAO_CANCELADO).Include(u => u.Fornecedor);

            if (!string.IsNullOrEmpty(filter))
            {
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word.ToLower();
                    query = query.Where(p => p.Fornecedor.nome.ToLower().Contains(temp));
                }
            }
            return query;
        }

        public void Cancelar(int id)
        {
            var _caixaBLL = new CaixaBLL(db, _idUsuario);
            var _folhaBLL = new FolhaPagamentoBLL(db, _idUsuario);
            var _conta = this.FindSingle(u => u.idContaPagar == id, u => u.Caixas.Select(k => k.MovimentoProfissional));
            var _folha = _folhaBLL.FindSingle(u => u.idFolhaPagamento == _conta.idFolhaPagamento);
            var contas = this.Find(u => u.idOrigem == id && u.situacao != ContaPagar.SITUACAO_CANCELADO, u => u.Caixas, u => u.Fornecedor, u => u.Caixas.Select(k => k.MovimentoProfissional));
            var _ultimaConta = this.FindSingle(u => u.idOrigem == _conta.idOrigem && u.situacao == ContaPagar.SITUACAO_AGUARDANDO_PAGAMENTO);
            if (_folha != null)
            {
                _folha.situacao = FolhaPagamento.AGUARDANDO_PAGAMENTO;
                _folha.total = 0;
                _folha.inss = 0;
                _folha.horaExtra = 0;
                _folha.comissao = 0;
                _folha.salario = 0;
                _folha.bonificacao = 0;
                _folha.outrosDescontos = 0;
                _folhaBLL.Update(_folha);
            }
            foreach (var item in contas.ToList())
            {
                item.situacao = ContaPagar.SITUACAO_CANCELADO;
                ((Model.Context)db).ContasPagar.Attach(item);
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
                            descricao = "Extorno de caixa referente ao cancelamento da conta. [ " + item.Fornecedor.nome + " ]",
                            dtLancamento = DateTime.Now,
                            situacao = Caixa.EXTORNO,
                            valor = (caixa.valor * -1),
                            idCaixaExtorno = caixa.idCaixa
                        });
                        db.SaveChanges(_idUsuario);
                    }
                }

            }
            _conta.situacao = ContaPagar.SITUACAO_CANCELADO;
            ((Model.Context)db).ContasPagar.Attach(_conta);
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
                        descricao = "Extorno de caixa referente ao cancelamento da conta. [ " + (_conta.Fornecedor != null ? _conta.Fornecedor.nome : caixa.descricao) + " ]",
                        dtLancamento = DateTime.Now,
                        situacao = Caixa.EXTORNO,
                        valor = (caixa.valor * -1),
                        idCaixaExtorno = caixa.idCaixa
                    });
                }
            }
        }

        public void Pagar(ContaPagar bean)
        {
            bean.situacao = ContaPagar.SITUACAO_PAGO;
            ((Model.Context)db).ContasPagar.Attach(bean);
            ((Model.Context)db).Entry(bean).Property(e => e.situacao).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.pagamento).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.valorPago).IsModified = true;
        }
    }
}
