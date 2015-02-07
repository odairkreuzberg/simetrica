using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using System.Linq;

namespace RP.Report
{
    class Result : ActionResult
    {
        public string file { get; set; }
        public CrystalDecisions.Shared.ExportFormatType type { get; set; }
        public Dictionary<string, DataSet> listData { get; set; }
        public Dictionary<string, object> parameters { get; set; }
        public string saveName { get; set; }
        //public Model reportModel { get; set; }

        public override void ExecuteResult(System.Web.Mvc.ControllerContext context)
        {
        }

        public byte[] readRPT()
        {
            ReportClass rptH = new ReportClass();
            try
            {
                ParameterDiscreteValue par;
                rptH.FileName = file;
                rptH.Load();

                rptH.SetDataSource(listData["table"]);

                foreach (ReportDocument subreport in rptH.Subreports)
                {
                    if (listData.ContainsKey(subreport.Name.Trim().ToLower()))
                    {
                        subreport.SetDataSource(listData[subreport.Name.Trim().ToLower()]);
                    }
                }

                parameters = insencitiveParameter();
                foreach (ParameterField field in rptH.ParameterFields)
                {
                    if (parameters != null)
                    {
                        if (parameters.ContainsKey(field.Name.Trim().ToLower()))
                        {
                            par = new ParameterDiscreteValue();
                            par.Value = parameters[field.Name.Trim().ToLower()];
                            field.CurrentValues.Add(par);
                        }
                    }
                }

                System.IO.Stream stream = rptH.ExportToStream(this.type);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);

                return buffer;
            }
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }
        }

        private Dictionary<string, object> insencitiveParameter()
        {
            Dictionary<string, object> p = new Dictionary<string, object>();
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    p.Add(item.Key.Trim().ToLower(), item.Value);
                }
                return p;
            }
            return null;
        }


        private string getContentType(CrystalDecisions.Shared.ExportFormatType type)
        {
            if (type == ExportFormatType.PortableDocFormat) { return "application/pdf"; }
            else if (type == ExportFormatType.Excel) { return "application/vnd.ms-excel"; }
            else if (type == ExportFormatType.RichText) { return "application/rtf"; }
            else if (type == ExportFormatType.WordForWindows) { return "application/vnd.ms-word"; }
            return "text/html";
        }
    }
}
