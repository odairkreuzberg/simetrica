using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class EstadoBLL : RP.DataAccess.Repository<Estado>
    {
        public EstadoBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
      
        public RP.DataAccess.PaginatedList<Estado> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Estado> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Estado>(query.OrderBy(o => o.idEstado), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Estado> Search(string filter)
        {
            IQueryable<Estado> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Estado> preSearch(string filter)
        {
            IQueryable<Estado> query = this.Find(null, u => u.Pais);

	        if (!string.IsNullOrEmpty(filter))
            {
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word.ToLower();
                    query = query.Where(p => p.nome.ToLower().Contains(temp) || p.sigla.ToLower().Contains(temp));
                }
            }
            return query;
        }
    }
}
