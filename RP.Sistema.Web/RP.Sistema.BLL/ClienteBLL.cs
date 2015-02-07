using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class ClienteBLL : RP.DataAccess.Repository<Cliente>
    {
        public ClienteBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        private ICollection<ClienteFone> Telefones;

        protected override void BeforeUpdate(Cliente bean)
        {
            if (bean.Telefones != null)
                Telefones = bean.Telefones.ToList();
            bean.Telefones = null;
        }

        protected override void AfterUpdate(Cliente bean)
        {
            AtualizaTelefone(bean);
        }

        private void AtualizaTelefone(Cliente bean)
        {
            var _bll = new ClienteFoneBLL(db, _idUsuario);
            var _fonesDB = _bll.Find(u => u.idCliente == bean.idCliente);

            bean.Telefones = _fonesDB.ToList();

            foreach (var item in bean.Telefones.ToList())
            {
                if (!Telefones.Any(u => u.idCliente == item.idCliente))
                {
                    _bll.Delete(u => u.idClienteFone == item.idClienteFone);
                }
            }

            if (Telefones != null)
            {
                foreach (var item in Telefones)
                {
                    if (!_fonesDB.Any(u => u.idClienteFone == item.idClienteFone))
                    {
                        item.Cliente = bean;
                        _bll.Insert(item);
                    }
                }
            }
        }
      
        public RP.DataAccess.PaginatedList<Cliente> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Cliente> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Cliente>(query.OrderBy(o => o.idCliente), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Cliente> Search(string filter)
        {
            IQueryable<Cliente> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Cliente> preSearch(string filter)
        {
            IQueryable<Cliente> query = this.Find();

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
