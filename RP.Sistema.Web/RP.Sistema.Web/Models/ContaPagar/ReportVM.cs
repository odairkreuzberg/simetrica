using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.ContaPagar
{
    public class ReportVM
    {

        [Display(Name = "Filtros")]
        public DateTime? dtInicio { get; set; }
        public DateTime? dtFim { get; set; }
        public string tipo { get; set; }
        public List<Fornecedor.Consultar>Fornecedores { get; set; }
    }
}