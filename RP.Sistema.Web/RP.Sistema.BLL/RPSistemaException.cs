using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.Sistema.BLL
{
    [SerializableAttribute]
    public class RPSistemaException : Exception
    {
        public RPSistemaException(string message)
            : base(message)
        {
        }

    }
}
