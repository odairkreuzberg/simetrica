using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using RP.Sistema.BLL;
using RP.Sistema.Model;
using RP.Sistema.Model.Entities;

namespace RP.Sistema.Web.Services
{
    public class EntidadeService : IEntidadeService
    {
        public Entidade BuscarEntidadeJson(string idEntidade)
        {
            return BuscarEntidade(idEntidade);
        }

        public Entidade BuscarEntidadeXml(string idEntidade)
        {
            return BuscarEntidade(idEntidade);
        }

        private Entidade BuscarEntidade(string idEntidade)
        {
            using (Context db = new Context())
            {
                EntidadeBLL bll = new EntidadeBLL(db, -999);
                int id = Convert.ToInt32(idEntidade);
                return bll.FindSingle(e => e.idEntidade == id);
            }
        }

        public List<Entidade> ListarEntidadesJson()
        {
            return ListarEntidades();
        }

        public List<Entidade> ListarEntidadesXml()
        {
            return ListarEntidades();
        }

        private List<Entidade> ListarEntidades()
        {
            using (Context db = new Context())
            {
                EntidadeBLL bll = new EntidadeBLL(db, -999);
                return bll.Find().ToList();
            }
        }
    }
}
