using System.Collections.Generic;
using System.Linq;
using RP.Sistema.Model.Entities;
using RP.Util;
using RP.DataAccess.Interfaces;

namespace RP.Sistema.BLL
{
    public class AreaBLL : RP.DataAccess.Repository<Area>
    {

        public AreaBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        public RP.DataAccess.PaginatedList<Area> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Area> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Area>(query.OrderBy(o => o.idArea), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public RP.DataAccess.PaginatedList<Area> Search(string area, string descricao, string modulo, int? page, int? pagesize)
        {
            IQueryable<Area> query = preSearch(area, descricao, modulo);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Area>(query.OrderBy(o => o.idArea), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Area> Search(string filter)
        {
            IQueryable<Area> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Area> preSearch(string filter)
        {

            IQueryable<Area> query = this.Find(null, i => i.Modulo);

	        if (!string.IsNullOrEmpty(filter))
            {
                //filter = RP.Util.Class.Fonetiza.Fonetizar(filter);

                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word;
                    query = query.Where(p => p.nmArea.ToLower().Contains(temp.ToLower()) || p.dsArea.ToLower().Contains(temp.ToLower()));
                }
            }

            return query;
        }

        private IQueryable<Area> preSearch(string area, string descricao, string modulo)
        {

            IQueryable<Area> query = this.Find(null, i => i.Modulo);

            query = query.Where(p => p.nmArea.ToLower().Contains(area.ToLower()) && p.dsArea.ToLower().Contains(descricao.ToLower()) && p.Modulo.nmModulo.ToLower().Contains(modulo.ToLower()));

            return query;
        }
	}
}
