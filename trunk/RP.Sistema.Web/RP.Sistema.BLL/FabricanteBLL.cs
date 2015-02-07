using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class FabricanteBLL : RP.DataAccess.Repository<Fabricante>
    {
        public FabricanteBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
      
        public RP.DataAccess.PaginatedList<Fabricante> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Fabricante> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Fabricante>(query.OrderBy(o => o.idFabricante), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Fabricante> Search(string filter)
        {
            IQueryable<Fabricante> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Fabricante> preSearch(string filter)
        {
            IQueryable<Fabricante> query = this.Find(null);

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
