using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class CaixaBLL : RP.DataAccess.Repository<Caixa>
    {
        public CaixaBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        protected override void BeforeInsert(Caixa bean)
        {
            bean.idUsuario = this._idUsuario;
            bean.situacao = Caixa.CORENTE;
            bean.saldoAnterior = this.GetSaldoAtual();
            bean.saldoAtual = this.GetSaldoAtual() + bean.valor;

           // bean.dtLancamento = bean.dtLancamento.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
        }

        private decimal GetSaldoAtual()
        {
            string sql = @"Select Top 1 Max(saldoatual) From caixa Group By idcaixa Order By idcaixa Desc";

            decimal? saldoAtual = ((Model.Context)db).Database.SqlQuery<decimal?>(sql).FirstOrDefault();

            return (saldoAtual ?? 0);
        }


        public RP.DataAccess.PaginatedList<Caixa> Search(string filter, DateTime? dtInicio, DateTime? dtFim, string situacao, int? page, int? pagesize)
        {
            IQueryable<Caixa> query = preSearch(filter);
            if (dtFim != null)
            {
                dtFim = dtFim.Value.AddDays(1);
                query = query.Where(u => u.dtLancamento < dtFim);
            }
            if (dtInicio != null)
            {
                query = query.Where(u => u.dtLancamento >= dtInicio);
            }

            if (situacao != "Todos")
                query = query.Where(u => u.situacao == situacao);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Caixa>(query.OrderByDescending(o => new { o.dtLancamento, o.idCaixa }), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        private IQueryable<Caixa> preSearch(string filter)
        {
            IQueryable<Caixa> query = this.Find();

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
    }
}
 