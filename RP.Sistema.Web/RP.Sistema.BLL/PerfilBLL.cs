using System.Collections.Generic;
using System.Linq;
using RP.Sistema.Model.Entities;
using RP.Util;
using RP.DataAccess.Interfaces;

namespace RP.Sistema.BLL
{
    public class PerfilBLL : DataAccess.Repository<Perfil> 
    {
        public PerfilBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        public void UpdateAcoes(Perfil bean)
        {
            // instancia bll
            PerfilAcaoBLL perfilAcaoBLL = new PerfilAcaoBLL(db, _idUsuario);

            // busca todos acoes do perfil (banco)
            List<PerfilAcao> acoesDoPerfil = perfilAcaoBLL.Find(u => u.idPerfil == bean.idPerfil, i => i.Acao).ToList();

            // obtem todas acoes do bean (view)
            List<PerfilAcao> acoesDaView = bean.Acoes.ToList();

            // percorre as acoes do banco
            foreach (PerfilAcao item in acoesDoPerfil)
            {
                // se a acao percorrida nao estiver na view
                if (!acoesDaView.Any(u => u.idAcao == item.idAcao))
                {
                    // remove a acao do perfil
                    perfilAcaoBLL.Delete(u => u.idAcao == item.idAcao && u.idPerfil == bean.idPerfil);
                }
            }

            // percorre as acoes da view
            foreach (PerfilAcao item in acoesDaView)
            {
                // se a acao percorrida nao estiver no banco
                if (!acoesDoPerfil.Any(u => u.idAcao == item.idAcao))
                {
                    // insere acao no perfil
                    perfilAcaoBLL.Insert(new PerfilAcao { idPerfil = bean.idPerfil, idAcao = item.idAcao });
                }
            } 
        }

        public RP.DataAccess.PaginatedList<Perfil> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Perfil> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Perfil>(query.OrderBy(o => o.idPerfil), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Perfil> Search(string filter)
        {
            return preSearch(filter).ToList();
        }

        private IQueryable<Perfil> preSearch(string filter)
        {
            IQueryable<Perfil> query = this.Find();

	        if (!string.IsNullOrEmpty(filter))
            {
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word;
                    query = query.Where(p => p.nmPerfil.ToLower().Contains(temp.ToLower()));
                }
            }
            return query;
        }

	}
}
