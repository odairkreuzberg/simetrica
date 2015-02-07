using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Area
{
    public class Consultar
    {
        [Display(Name="Área")]
        public int? idArea { get; set; }
        public string nmArea { get; set; }
    }
}