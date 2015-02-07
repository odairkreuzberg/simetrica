using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{
    public class Tabela
    {
        public Tabela()
        {
            this.Usuarios = new List<Usuario>();
        }

        public int idTabela { get; set; }
        public string nmTabela { get; set; }

        public ICollection<Usuario> Usuarios { get; set; }
    }
}
