using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class CidadeBLL : RP.DataAccess.Repository<Cidade>
    {
        public CidadeBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
      
        public RP.DataAccess.PaginatedList<Cidade> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Cidade> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Cidade>(query.OrderBy(o => o.idCidade), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Cidade> Search(string filter)
        {
            IQueryable<Cidade> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Cidade> preSearch(string filter)
        {
            IQueryable<Cidade> query = this.Find(null, u => u.Estado.Pais);

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
