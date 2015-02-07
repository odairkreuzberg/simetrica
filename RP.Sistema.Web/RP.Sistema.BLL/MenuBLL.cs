using System.Collections.Generic;
using System.Linq;
using RP.Sistema.Model.Entities;
using RP.Util;
using RP.DataAccess.Interfaces;

namespace RP.Sistema.BLL
{
    public class MenuBLL : RP.DataAccess.Repository<Menu>
    {
        public MenuBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        public RP.DataAccess.PaginatedList<Menu> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Menu> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Menu>(query.OrderBy(o => o.idMenu), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Menu> Search(string filter)
        {
            IQueryable<Menu> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Menu> preSearch(string filter)
        {

            IQueryable<Menu> query = this.Find();

	        if (!string.IsNullOrEmpty(filter))
            {
                //filter = RP.Util.Class.Fonetiza.Fonetizar(filter);
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word;
                    query = query.Where(p => p.nmMenu.ToLower().Contains(temp.ToLower()));
                }
            }
            return query;
        }
	

	}
}