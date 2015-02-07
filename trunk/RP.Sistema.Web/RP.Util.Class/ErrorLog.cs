using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

namespace RP.Util.Entity
{
    public static class ErroLog
    {
        public class Erro
        {
            public string idErro { get; set; }
            public DateTime dtErro { get; set; }
            public string dsInner { get; set; }
            public string dsMessage { get; set; }
            public string dsTrace { get; set; }
            public int idUsuario { get; set; }
            public bool flInner { get; set; }
            public string dsURL { get; set; }
        }

        private static List<Erro> _Erros = new List<Erro>();

        public static void Add(string ex, string id, int idUsuario)
        {
            Erro e = new Erro { idErro = id, dsInner = string.Empty, flInner = false, dsMessage = ex, dsTrace = string.Empty, dtErro = DateTime.Now, idUsuario = idUsuario, dsURL = string.Empty };
            Clear();
            _Erros.Add(e);
        }

        public static void Add(System.Exception ex, string id, int idUsuario)
        {
            Add(ex, id, idUsuario, string.Empty);
        }

        public static void Add(System.Exception ex, string id)
        {
            Add(ex, id, 0, string.Empty);
        }

        public static void Add(System.Exception ex, string id, int idUsuario, string URL)
        {
            Erro e = new Erro { idErro = id, dsInner = RP.Util.Exception.Message.Get(ex), flInner = (ex.InnerException != null || ex is DbEntityValidationException), dsMessage = ex.Message, dsTrace = ex.StackTrace, dtErro = DateTime.Now, idUsuario = idUsuario, dsURL = URL };
            Clear();
            _Erros.Add(e);
        }

        public static Erro GetLast(string id)
        {
            return _Erros.LastOrDefault(u => u.idErro == id);
        }

        private static void Clear()
        {
            if (_Erros.Count >= 100)
            {
                _Erros.Remove(_Erros.FirstOrDefault());
            }
        }

        public static List<Erro> List()
        {
            return _Erros;
        }
    }
}
