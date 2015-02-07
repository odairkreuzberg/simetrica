using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using System.Data.Common;
using RP.Report;

namespace RP.Sistema.Report
{
    public class Report
    {

        public static RP.Sistema.Report.RPT.ExportFormatType stringTOExportFormatType(string exportTO)
        {
            if (exportTO.Trim().ToLower() == "doc")
            {
                return RP.Sistema.Report.RPT.ExportFormatType.DOC;
            }
            if (exportTO.Trim().ToLower() == "xls")
            {
                return RP.Sistema.Report.RPT.ExportFormatType.XLS;
            }
            if (exportTO.Trim().ToLower() == "html")
            {
                return RP.Sistema.Report.RPT.ExportFormatType.HTML;
            }
            if (exportTO.Trim().ToLower() == "txt")
            {
                return RP.Sistema.Report.RPT.ExportFormatType.TXT;
            }
            return RP.Sistema.Report.RPT.ExportFormatType.PDF;
        }

        public struct genericReportData
        {
            public Dictionary<string, DataSet> listData { get; set; }
            public Dictionary<string, object> parameters { get; set; }
            public string PathRPT { get { return System.Configuration.ConfigurationManager.AppSettings["PathRPT"] ?? ""; } }
            public string area { get; set; }
            public string fileRPT { get; set; }
            public RPT.ExportFormatType exportTO { get; set; }
        }

        public static byte[] byteResult(genericReportData data)
        {
            string _file = string.Empty;
            string _ext = ".pdf";
            try
            {
                _file = data.PathRPT.Trim().Substring(data.PathRPT.Length - 1) == @"\" ? data.PathRPT + (string.IsNullOrEmpty(data.area) ? string.Empty : data.area + @"\") + data.fileRPT : data.PathRPT.Trim() + @"\" + (string.IsNullOrEmpty(data.area) ? string.Empty : data.area + @"\") + data.fileRPT;
                if (System.IO.File.Exists(_file))
                {
                    //CrystalDecisions.Shared.ExportFormatType _exportType;

                    //if (data.exportTO == RP.Saude.Report.RPT.ExportFormatType.DOC)
                    //{
                    //    _exportType = CrystalDecisions.Shared.ExportFormatType.WordForWindows;
                    //    _ext = ".doc";

                    //}
                    //else if (data.exportTO == RP.Saude.Report.RPT.ExportFormatType.HTML)
                    //{
                    //    _exportType = CrystalDecisions.Shared.ExportFormatType.HTML40;
                    //    _ext = ".html";
                    //}
                    //else if (data.exportTO == RP.Saude.Report.RPT.ExportFormatType.TXT)
                    //{
                    //    _exportType = CrystalDecisions.Shared.ExportFormatType.RichText;
                    //    _ext = ".txt";
                    //}
                    //else if (data.exportTO == RP.Saude.Report.RPT.ExportFormatType.XLS)
                    //{
                    //    _exportType = CrystalDecisions.Shared.ExportFormatType.Excel;
                    //    _ext = ".xls";
                    //}
                    //else
                    //{
                    //    _exportType = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
                    //}



                    var rep = new RPT(
                        _file,
                        data.exportTO,
                        data.listData,
                        data.parameters);

                    return rep.readRPT();

                }
                return null;
            }
            finally
            {
                /* dispose method */
            }
        }

        public static ActionResult genericReport(genericReportData data)
        {
            string _file = string.Empty;
            string _ext = ".pdf";
            try
            {
                _file = data.PathRPT.Trim().Substring(data.PathRPT.Length - 1) == @"\" ? data.PathRPT + (string.IsNullOrEmpty(data.area) ? string.Empty : data.area + @"\") + data.fileRPT : data.PathRPT.Trim() + @"\" + (string.IsNullOrEmpty(data.area) ? string.Empty : data.area + @"\") + data.fileRPT;
                if (System.IO.File.Exists(_file))
                {
                    //CrystalDecisions.Shared.ExportFormatType _exportType;

                    //if (data.exportTO == ExportFormatType.DOC)
                    //{
                    //    _exportType = CrystalDecisions.Shared.ExportFormatType.WordForWindows;
                    //    _ext = ".doc";

                    //}
                    //else if (data.exportTO == ExportFormatType.HTML)
                    //{
                    //    _exportType = CrystalDecisions.Shared.ExportFormatType.HTML40;
                    //    _ext = ".html";
                    //}
                    //else if (data.exportTO == ExportFormatType.TXT)
                    //{
                    //    _exportType = CrystalDecisions.Shared.ExportFormatType.RichText;
                    //    _ext = ".txt";
                    //}
                    //else if (data.exportTO == ExportFormatType.XLS)
                    //{
                    //    _exportType = CrystalDecisions.Shared.ExportFormatType.Excel;
                    //    _ext = ".xls";
                    //}
                    //else
                    //{
                    //    _exportType = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
                    //}

                    return new Result
                    {
                        file = _file,
                        listData = data.listData,
                        type = data.exportTO,
                        parameters = data.parameters,
                        saveName = data.fileRPT.ToLower().Trim().Replace(".rpt", _ext)
                    };
                }
                else
                {
                    throw new Exception("Arquivo não encontrado [" + _file + "]");
                }
            }
            finally
            {
                /* dispose method */
            }
        }


    }
}

