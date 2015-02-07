using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RP.Sistema.Web.Models.Ticket
{
    public class TicketVM 
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }

    public enum TipoTicketVM
    { 
        Ideia = 1,
        Problema = 2,
        Pergunta = 3,
        Agradecimento = 4
    }

    public class InserirVM
    {
        public TicketVM Ideia { get; set; }
        public TicketVM Problema { get; set; }
        public TicketVM Pergunta { get; set; }
        public TicketVM Agradecimento { get; set; }
        public TipoTicketVM Tipo { get; set; }
        public string Email { get; set; }
    }
}