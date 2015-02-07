using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{
    public class Parametro
	{
        public string nmParametro { get; set; }
        public string dsParametro { get; set; }
        public string dsValor { get; set; }        
    }
}
