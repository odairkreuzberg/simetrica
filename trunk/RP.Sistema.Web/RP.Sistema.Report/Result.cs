using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using RP.Report;

namespace RP.Sistema.Report
{
    class Result : ActionResult
    {
        public string file { get; set; }
        public RPT.ExportFormatType type { get; set; }
        public Dictionary<string, DataSet> listData { get; set; }
        public Dictionary<string, object> parameters { get; set; }
        public string saveName { get; set; }

        public override void ExecuteResult(System.Web.Mvc.ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = getContentType(type);
            context.HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=" + saveName);

            var report = new RPT(file, type, listData, parameters);
            var buffer = report.readRPT();

            context.HttpContext.Response.BinaryWrite(buffer);
        }

        private string getContentType(RPT.ExportFormatType type)
        {
            if (type == RPT.ExportFormatType.PDF) { return "application/pdf"; }
            else if (type == RPT.ExportFormatType.XLS) { return "application/vnd.ms-excel"; }
            else if (type == RPT.ExportFormatType.TXT) { return "application/rtf"; }
            else if (type == RPT.ExportFormatType.DOC) { return "application/vnd.ms-word"; }
            return "text/html";
        }
    }

}
