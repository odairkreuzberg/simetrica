using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class UnidadeMedidaBLL : RP.DataAccess.Repository<UnidadeMedida>
    {
        public UnidadeMedidaBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
      
        public RP.DataAccess.PaginatedList<UnidadeMedida> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<UnidadeMedida> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.UnidadeMedida>(query.OrderBy(o => o.idUnidadeMedida), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<UnidadeMedida> Search(string filter)
        {
            IQueryable<UnidadeMedida> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<UnidadeMedida> preSearch(string filter)
        {
            IQueryable<UnidadeMedida> query = this.Find(null);

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
    }
}
