using System.Collections.Generic;
using System.Linq;
using RP.Sistema.Model.Entities;
using RP.DataAccess;
using RP.Util;
using RP.DataAccess.Interfaces;

namespace RP.Sistema.BLL
{
    public class ControleBLL : RP.DataAccess.Repository<Controle>
    {

        public ControleBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        public List<Controle> Search(string filter)
        {
            var filters = new Paginate<Controle>.Filter();

            foreach (string word in filter.NSplit(' '))
            {
                string temp = word;
                filters.predicates.Add(u => u.nmControle.Contains(temp));
            }
            return base.Search(this.FindIncluding(u => u.Area), filters).ToList();
        }

        public RP.DataAccess.PaginatedList<Controle> Search(string filter, int? page, int? pagesize)
        {
            var filters = new Paginate<Controle>.Filter();

            foreach (string word in filter.NSplit(' '))
            {
                string temp = word;
                filters.predicates.Add(u => u.nmControle.ToLower().Contains(temp.ToLower()));
            }
            return base.Search(this.FindIncluding(u => u.Area), filters, page, pagesize, q => q.idControle);
        }

        public RP.DataAccess.PaginatedList<Controle> Search(string nome, string descricao, string area, int? page, int? pagesize)
        {
            var filters = new Paginate<Controle>.Filter();

            foreach (string word in nome.NSplit(' '))
            {
                string temp = word;
                filters.predicates.Add(u => u.nmControle.ToLower().Contains(temp.ToLower()));
            }

            foreach (string word in descricao.NSplit(' '))
            {
                string temp = word;
                filters.predicates.Add(u => u.dsControle.ToLower().Contains(temp.ToLower()));
            }


            foreach (string word in area.NSplit(' '))
            {
                string temp = word;
                filters.predicates.Add(u => u.Area.dsArea.ToLower().Contains(temp.ToLower()));
            }

            return base.Search(this.FindIncluding(u => u.Area), filters, page, pagesize, q => q.idControle);
        }
	}
}
