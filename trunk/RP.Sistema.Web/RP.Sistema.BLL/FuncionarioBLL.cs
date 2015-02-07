using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class FuncionarioBLL : RP.DataAccess.Repository<Funcionario>
    {
        public FuncionarioBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        protected override void BeforeInsert(Funcionario bean)
        {
            bean.status = Funcionario.ATIVO;
        }

        public RP.DataAccess.PaginatedList<Funcionario> Search(string filter, string status, int? page, int? pagesize, string mensalista, string tipo = "")
        {
            IQueryable<Funcionario> query = preSearch(filter);

            if (status != "Todos" && !string.IsNullOrEmpty(status))
            {
                query = query.Where(u => u.status == status);
            }
            if (!string.IsNullOrEmpty(tipo))
            {
                query = query.Where(u => u.tipo == tipo);
            }
            if (!string.IsNullOrEmpty(mensalista))
            {
                mensalista = mensalista == "nao" ? "Não" : "Sim";
                query = query.Where(u => u.flMensalista == mensalista);
            }

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Funcionario>(query.OrderBy(o => o.idFuncionario), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Funcionario> Search(string filter)
        {
            IQueryable<Funcionario> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Funcionario> preSearch(string filter)
        {
            IQueryable<Funcionario> query = this.Find(null, u => u.FolhaPagamentos);

	        if (!string.IsNullOrEmpty(filter))
            {
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word.ToLower();
                    query = query.Where(p => p.nome.ToLower().Contains(temp));
                }
            }
            return query;
        }

        public void Desativar(Funcionario bean)
        {
            bean.dtSaida = DateTime.Now;
            bean.status = Funcionario.INATIVO;
            this.Update(bean);
        }
    }
}
