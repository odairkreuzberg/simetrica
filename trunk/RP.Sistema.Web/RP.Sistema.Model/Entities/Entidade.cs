using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{
    public class Entidade
    {
        public int idEntidade { get; set; }
        public string nmRazaoSocial { get; set; }
        public string nmFantasia { get; set; }
        public string nrCNPJ { get; set; }
        public string dsCidade { get; set; }
        public string dsBairro { get; set; }
        public string dsLogradouro { get; set; }
        public string nrEndereco { get; set; }
        public string nrCEP { get; set; }
        public string nrTelefone { get; set; }
        public string hrInicioManha { get; set; }
        public string hrFimManha { get; set; }
        public string hrInicioTarde { get; set; }
        public string hrFimTarde { get; set; }
        public string dsWebSite { get; set; }
        public byte[] imLogo { get; set; }
        public string dsEmail { get; set; }
    }
}

