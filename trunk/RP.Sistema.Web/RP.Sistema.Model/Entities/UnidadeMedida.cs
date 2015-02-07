using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class UnidadeMedida
    {
        public UnidadeMedida()
        {
            this.Materiais = new List<Material>();
        }

        public int idUnidadeMedida { get; set; }
        public string nome { get; set; }
        public string abreviatura { get; set; }
        public ICollection<Material> Materiais { get; set; }
    }
}
