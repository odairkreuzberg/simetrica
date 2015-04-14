using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.Sistema.Model.Entities
{
    public class LogDado
    {
        public LogDado(string nmAcao, string nmControle, int idUsuario)
        {
            // TODO: Complete member initialization
            this.nmAcao = nmAcao;
            this.nmControle = nmControle;
            this.idUsuario = idUsuario;
            this.dtLog = DateTime.Now;
        }
        public LogDado()
        {
        }
        public int idLog{ get; set; }
        public string nmAcao { get; set; }
        public string nmControle { get; set; }
        public int idUsuario { get; set; }
        public DateTime dtLog { get; set; }
        public Usuario Usuario { get; set; }

        public static List<LogDado> GetData(Context context, System.Data.Entity.Infrastructure.DbEntityEntry entity, int _idUsuario)
        {
            return null;
        }
    }
}
