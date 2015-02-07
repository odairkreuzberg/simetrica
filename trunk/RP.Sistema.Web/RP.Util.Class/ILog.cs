using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RP.Util.Entity
{
    public interface ILog
    {
        string logData { get; }
        string tableName { get; }
        string keyName { get; }
    }
}