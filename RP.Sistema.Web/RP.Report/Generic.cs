using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using System;
using System.Web;

namespace RP.Report
{
    public class Generic
    {
        public static ExportFormatType stringTOExportFormatType(string exportTO)
        {
            if (exportTO.Trim().ToLower() == "doc")
            {
                return ExportFormatType.DOC;
            }
            else if (exportTO.Trim().ToLower() == "xls")
            {
                return ExportFormatType.XLS;
            }
            else if (exportTO.Trim().ToLower() == "html")
            {
                return ExportFormatType.HTML;
            }
            else if (exportTO.Trim().ToLower() == "txt")
            {
                return ExportFormatType.TXT;
            }
            return ExportFormatType.PDF;
        }

        public struct GenericData
        {
            public Dictionary<string, DataSet> listData { get; set; }
            public Dictionary<string, object> parameters { get; set; }
            public string PathRPT { get { return System.Configuration.ConfigurationManager.AppSettings["PathRPT"] ?? ""; } }
            public string area { get; set; }
            public string fileRPT { get; set; }
            public ExportFormatType exportTO { get; set; }
        }

        public struct JsonData
        {
            public long Id { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        public enum ExportFormatType
        {
            PDF,
            DOC,
            XLS,
            TXT,
            HTML
        }

        public static byte[] byteResult(GenericData data)
        {
            string _file = string.Empty;
            string _ext = ".pdf";
            try
            {
                _file = data.PathRPT.Trim().Substring(data.PathRPT.Length - 1) == @"\" ? data.PathRPT + (string.IsNullOrEmpty(data.area) ? string.Empty : data.area + @"\") + data.fileRPT : data.PathRPT.Trim() + @"\" + data.area + @"\" + data.fileRPT;
                if (System.IO.File.Exists(_file))
                {
                    CrystalDecisions.Shared.ExportFormatType _exportType;

                    if (data.exportTO == ExportFormatType.DOC)
                    {
                        _exportType = CrystalDecisions.Shared.ExportFormatType.WordForWindows;
                        _ext = ".doc";

                    }
                    else if (data.exportTO == ExportFormatType.HTML)
                    {
                        _exportType = CrystalDecisions.Shared.ExportFormatType.HTML40;
                        _ext = ".html";
                    }
                    else if (data.exportTO == ExportFormatType.TXT)
                    {
                        _exportType = CrystalDecisions.Shared.ExportFormatType.RichText;
                        _ext = ".txt";
                    }
                    else if (data.exportTO == ExportFormatType.XLS)
                    {
                        _exportType = CrystalDecisions.Shared.ExportFormatType.Excel;
                        _ext = ".xls";
                    }
                    else
                    {
                        _exportType = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
                    }

                    var rep = new Result
                    {
                        file = _file,
                        listData = data.listData,
                        type = _exportType,
                        parameters = data.parameters,
                        saveName = data.fileRPT.ToLower().Trim().Replace(".rpt", _ext)
                    };

                    return rep.readRPT();

                }
                return null;
            }
            finally
            {
                /* dispose method */
            }
        }

        public static ActionResult Report(GenericData data)
        {
            string _file = string.Empty;
            string _ext = ".pdf";
            try
            {
                _file = data.PathRPT.Trim().Substring(data.PathRPT.Length - 1) == @"\" ? data.PathRPT + (string.IsNullOrEmpty(data.area) ? string.Empty : data.area + @"\") + data.fileRPT : data.PathRPT.Trim() + @"\" + data.area + @"\" + data.fileRPT;
                if (System.IO.File.Exists(_file))
                {
                    CrystalDecisions.Shared.ExportFormatType _exportType;

                    if (data.exportTO == ExportFormatType.DOC)
                    {
                        _exportType = CrystalDecisions.Shared.ExportFormatType.WordForWindows;
                        _ext = ".doc";

                    }
                    else if (data.exportTO == ExportFormatType.HTML)
                    {
                        _exportType = CrystalDecisions.Shared.ExportFormatType.HTML40;
                        _ext = ".html";
                    }
                    else if (data.exportTO == ExportFormatType.TXT)
                    {
                        _exportType = CrystalDecisions.Shared.ExportFormatType.RichText;
                        _ext = ".txt";
                    }
                    else if (data.exportTO == ExportFormatType.XLS)
                    {
                        _exportType = CrystalDecisions.Shared.ExportFormatType.Excel;
                        _ext = ".xls";
                    }
                    else
                    {
                        _exportType = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
                    }

                    return new Result
                    {
                        file = _file,
                        listData = data.listData,
                        type = _exportType,
                        parameters = data.parameters,
                        saveName = data.fileRPT.ToLower().Trim().Replace(".rpt", _ext),
                    };

                }
                else
                {
                    throw new RP.Report.Exception("Arquivo não encontrado [" + _file + "]");
                }
            }
            finally
            {
                /* dispose method */
            }
        }
    }
}
