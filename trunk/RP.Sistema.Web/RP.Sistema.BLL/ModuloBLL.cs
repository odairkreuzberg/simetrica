using System.Collections.Generic;
using System.Linq;
using RP.Sistema.Model.Entities;
using RP.Util;
using RP.DataAccess.Interfaces;

namespace RP.Sistema.BLL
{
    public class ModuloBLL : DataAccess.Repository<Modulo>
    {

        public ModuloBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        public RP.DataAccess.PaginatedList<Modulo> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Modulo> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Modulo>(query.OrderBy(o => o.idModulo), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public RP.DataAccess.PaginatedList<Modulo> Search(string nome, string descricao, int? page, int? pagesize)
        {
            IQueryable<Modulo> query = preSearch(nome, descricao);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Modulo>(query.OrderBy(o => o.idModulo), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Modulo> Search(string filter)
        {
            return preSearch(filter).ToList();
        }

        private IQueryable<Modulo> preSearch(string filter)
        {

            IQueryable<Modulo> query = this.Find();

	        if (!string.IsNullOrEmpty(filter))
            {
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word;
                    query = query.Where(p => p.nmModulo.ToLower().Contains(temp.ToLower()));
                }
            }
            return query;
        }

        private IQueryable<Modulo> preSearch(string nome, string descricao)
        {
            return this.Find().Where(p => p.nmModulo.ToLower().Contains(nome.ToLower()) && p.dsModulo.ToLower().Contains(descricao.ToLower()));
        }

        public void UpdateUsuarios(Modulo bean)
        {
            // instancia bll
            ModuloUsuarioBLL moduloUsuarioBLL = new ModuloUsuarioBLL(db, _idUsuario);
            
            // busca todos usuarios do Modulo (banco)
            List<ModuloUsuario> usuariosDoModulo = moduloUsuarioBLL.Find(u => u.idModulo == bean.idModulo, i => i.Usuario).ToList();

            // obtem todos usuario do bean (view)
            List<ModuloUsuario> usuariosView = bean.Usuarios.ToList();

            // percorre os usuarios do Modulo
            foreach (ModuloUsuario item in usuariosDoModulo)
            {
                // se o usuario percorrido nao estiver na view
                if (!usuariosView.Any(u => u.idUsuario == item.idUsuario))
                {
                    // remove o usuario do Modulo
                    moduloUsuarioBLL.Delete(u => u.idUsuario == item.idUsuario && u.idModulo == bean.idModulo);
                }
            }

            // percorre os usuarios da view
            foreach (ModuloUsuario item in usuariosView)
            {
                // se o usuario percorrido nao estiver nos usuarios do Modulo
                if (!usuariosDoModulo.Any(u => u.idUsuario == item.idUsuario))
                {
                    // insere usuario no Modulo
                    moduloUsuarioBLL.Insert(new ModuloUsuario { idModulo = bean.idModulo, idUsuario = item.idUsuario });
                }
            }
        }
	}
}
