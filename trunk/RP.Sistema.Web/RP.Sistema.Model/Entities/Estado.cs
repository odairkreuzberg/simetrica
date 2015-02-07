using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class Estado
    {
        public Estado()
        {
            this.Cidades = new List<Cidade>();
        }

        public int idEstado { get; set; }
        public string nome { get; set; }
        public string sigla { get; set; }
        public int idPais { get; set; }
        public ICollection<Cidade> Cidades { get; set; }
        public Pais Pais { get; set; }
    }
}
