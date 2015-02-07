using System;
using System.Collections.Generic;
using System.Linq;
using RP.Sistema.Model.Entities;
using RP.Util;
using System.Data.Entity;
using RP.DataAccess.Interfaces;

namespace RP.Sistema.BLL
{
    public class ParametroBLL : DataAccess.Repository<Parametro>
    {
        public ParametroBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        public RP.DataAccess.PaginatedList<Parametro> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Parametro> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<Parametro>(query.OrderBy(o => o.dsParametro), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Parametro> Search(string filter)
        {
            return preSearch(filter).ToList();
        }

        public RP.DataAccess.PaginatedList<Parametro> Search(string dsparametro, string nmparametro, int? page, int? pagesize)
        {
            IQueryable<Parametro> query = preSearch(dsparametro, nmparametro);

            var result = new RP.DataAccess.PaginatedList<Parametro>(query.OrderBy(o => o.dsParametro), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        private IQueryable<Parametro> preSearch(string filter)
        {
            IQueryable<Parametro> query = this.Find();

            if (!string.IsNullOrEmpty(filter))
            {
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word;
                    query = query.Where(p => p.dsParametro.ToLower().Contains(temp.ToLower()));
                }
            }

            return query.AsNoTracking();
        }

        private IQueryable<Parametro> preSearch(string nmparametro, string dsparametro)
        {
            IQueryable<Parametro> query = this.Find(u => u.dsParametro.ToLower().Contains(dsparametro.ToLower()));

            if (!string.IsNullOrEmpty(dsparametro))
            {
                foreach (string word in dsparametro.NSplit(' '))
                {
                    string temp = word;
                    query = query.Where(p => p.dsParametro.ToLower().Contains(temp.ToLower()));
                }
            }

            return query.AsNoTracking();
        }

        public string GetParametro(string nmParametro)
        {
            var parametro = this.FindSingle(u => u.nmParametro.ToLower() == nmParametro.ToLower());

            if (parametro == null)
            {
                throw new Exception(string.Format("O parametro '{0}' não está cadastrado no sistema.", nmParametro));
            }

            return parametro.dsValor;
        }
    }
}
