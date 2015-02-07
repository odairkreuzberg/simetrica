using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class PaisBLL : RP.DataAccess.Repository<Pais>
    {
        public PaisBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
      
        public RP.DataAccess.PaginatedList<Pais> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Pais> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Pais>(query.OrderBy(o => o.idPais), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Pais> Search(string filter)
        {
            IQueryable<Pais> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Pais> preSearch(string filter)
        {
            IQueryable<Pais> query = this.Find();

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
