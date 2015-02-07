using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RP.Sistema.Report
{
    public class ReportException : Exception
    {
        public ReportException(string message) : base(message) {}
    }
}
