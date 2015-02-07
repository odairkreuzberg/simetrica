using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class FornecedorBLL : RP.DataAccess.Repository<Fornecedor>
    {
        public FornecedorBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        private ICollection<FornecedorFone> Telefones;

        protected override void BeforeUpdate(Fornecedor bean)
        {
            if (bean.Telefones != null)
                Telefones = bean.Telefones.ToList();
            bean.Telefones = null;
        }

        protected override void AfterUpdate(Fornecedor bean)
        {
            AtualizaTelefone(bean);
        }

        private void AtualizaTelefone(Fornecedor bean)
        {
            var _bll = new FornecedorFoneBLL(db, _idUsuario);
            var _fonesDB = _bll.Find(u => u.idFornecedor == bean.idFornecedor);

            bean.Telefones = _fonesDB.ToList();

            foreach (var item in bean.Telefones.ToList())
            {
                if (!Telefones.Any(u => u.idFornecedor == item.idFornecedor))
                {
                    _bll.Delete(u => u.idFornecedorFone == item.idFornecedorFone);
                }
            }

            if (Telefones != null)
            {
                foreach (var item in Telefones)
                {
                    if (!_fonesDB.Any(u => u.idFornecedorFone == item.idFornecedorFone))
                    {
                        item.Fornecedor = bean;
                        _bll.Insert(item);
                    }
                }
            }
        }
      
        public RP.DataAccess.PaginatedList<Fornecedor> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Fornecedor> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Fornecedor>(query.OrderBy(o => o.idFornecedor), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Fornecedor> Search(string filter)
        {
            IQueryable<Fornecedor> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Fornecedor> preSearch(string filter)
        {
            IQueryable<Fornecedor> query = this.Find();

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
