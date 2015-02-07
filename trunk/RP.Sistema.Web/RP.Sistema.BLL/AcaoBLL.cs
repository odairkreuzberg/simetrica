using System.Collections.Generic;
using System.Linq;
using RP.Sistema.Model.Entities;
using RP.Util;
using RP.DataAccess.Interfaces;

namespace RP.Sistema.BLL
{
    public class AcaoBLL : RP.DataAccess.Repository<Acao>
    {

        public AcaoBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
      
        public RP.DataAccess.PaginatedList<Acao> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Acao> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Acao>(query.OrderBy(o => o.idAcao), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Acao> Search(string filter)
        {
            IQueryable<Acao> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Acao> preSearch(string filter)
        {
            IQueryable<Acao> query = this.Find(null, i => i.Controle, i => i.Controle.Area, i => i.Controle.Area.Modulo, i => i.Menu);

	        if (!string.IsNullOrEmpty(filter))
            {
                //filter = RP.Util.Class.Fonetiza.Fonetizar(filter);
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word.ToLower();
                    query = query.Where(p => p.dsAcao.ToLower().Contains(temp) || p.nmMenu.ToLower().Contains(temp) || p.nmAcao.ToLower().Contains(temp) || p.Controle.nmControle.ToLower().Contains(temp));
                }
            }
            return query;
        }
    }
}
