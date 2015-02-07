using System.ComponentModel.DataAnnotations;

namespace RP.Sistema.Web.Models.Agenda
{
    public class AgendaVM
    {

        public int idAgenda { get; set; }

        [Display(Name = "Dt. Agenda")]
        public System.DateTime? dtAgenda { get; set; }

        [Display(Name = "Situação")]
        public string flSituacao { get; set; }

        [Display(Name = "Dt. Agendamento")]
        public System.DateTime? dtAgendado { get; set; }

        [Display(Name = "Dt. Execução")]
        public System.DateTime? dtExecucao { get; set; }

        [Display(Name = "Descrição")]
        public string dsAgenda { get; set; }
    }
}