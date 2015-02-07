using System;
using System.Collections.Generic;

namespace RP.Sistema.Model.Entities
{
    public partial class Fabricante
    {
        public Fabricante()
        {
            this.Materiais = new List<Material>();
        }

        public int idFabricante { get; set; }
        public string nome { get; set; }
        public ICollection<Material> Materiais { get; set; }
    }
}
