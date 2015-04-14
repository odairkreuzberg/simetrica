using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class LogDadoBLL : RP.DataAccess.Repository<LogDado>
    {
        public LogDadoBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        public RP.DataAccess.PaginatedList<LogDado> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<LogDado> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.LogDado>(query.OrderBy(o => o.idLog), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<LogDado> Search(string filter)
        {
            IQueryable<LogDado> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<LogDado> preSearch(string filter)
        {
            IQueryable<LogDado> query = this.Find(null, u => u.Usuario);

            if (!string.IsNullOrEmpty(filter))
            {
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word.ToLower();
                    query = query.Where(p => p.nmControle.ToLower().Contains(temp) || p.nmAcao.ToLower().Contains(temp) || p.Usuario.nmUsuario.ToLower().Contains(temp));
                }
            }
            return query;
        }
    }
}
