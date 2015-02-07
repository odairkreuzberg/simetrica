using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using System;
using System.Web;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

namespace RP.Sistema.Report
{
    public class RPT
    {
        private string _file;
        private CrystalDecisions.Shared.ExportFormatType _type;
        private Dictionary<string, DataSet> _listData;
        private Dictionary<string, object> _parameters;

        private CrystalDecisions.Shared.ExportFormatType stringTOExportFormatType(string exportTO)
        {
            if (exportTO.Trim().ToLower() == "doc")
            {
                return CrystalDecisions.Shared.ExportFormatType.WordForWindows;
            }
            if (exportTO.Trim().ToLower() == "xls")
            {
                return CrystalDecisions.Shared.ExportFormatType.Excel;
            }
            if (exportTO.Trim().ToLower() == "html")
            {
                return CrystalDecisions.Shared.ExportFormatType.HTML40;
            }
            if (exportTO.Trim().ToLower() == "txt")
            {
                return CrystalDecisions.Shared.ExportFormatType.RichText;
            }
            return CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
        }

        public static string GetFullPath(string fileName)
        {
            var _pathRPT = System.Configuration.ConfigurationManager.AppSettings["PathRPT"] ?? "";
            return _pathRPT.Trim().Substring(_pathRPT.Length - 1) == @"\" ? _pathRPT + fileName : _pathRPT.Trim() + @"\" + fileName;

        }

        private CrystalDecisions.Shared.ExportFormatType stringTOExportFormatType(ExportFormatType exportTO)
        {
            if (exportTO == ExportFormatType.DOC)
            {
                return CrystalDecisions.Shared.ExportFormatType.WordForWindows;
            }
            if (exportTO == ExportFormatType.XLS)
            {
                return CrystalDecisions.Shared.ExportFormatType.Excel;
            }
            if (exportTO == ExportFormatType.HTML)
            {
                return CrystalDecisions.Shared.ExportFormatType.HTML40;
            }
            if (exportTO == ExportFormatType.TXT)
            {
                return CrystalDecisions.Shared.ExportFormatType.RichText;
            }
            return CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
        }

        public enum ExportFormatType
        {
            PDF,
            DOC,
            XLS,
            TXT,
            HTML
        }

        public RPT(
            string file,
            ExportFormatType type,
            Dictionary<string, DataSet> listData,
            Dictionary<string, object> parameters)
        {
            _file = file;
            _type = stringTOExportFormatType(type);
            _parameters = parameters;
            _listData = listData;
        }

        public RPT(
            string file,
            string type,
            Dictionary<string, DataSet> listData,
            Dictionary<string, object> parameters)
        {
            _file = file;
            _type = stringTOExportFormatType(type);
            _parameters = parameters;
            _listData = listData;
        }

        public byte[] readRPT()
        {
            ReportClass rptH = new ReportClass();
            try
            {
                ParameterDiscreteValue par;
                rptH.FileName = _file;
                rptH.Load();

                if (_listData != null)
                    if (_listData.ContainsKey("table"))
                        rptH.SetDataSource(_listData["table"]);

                foreach (ReportDocument subreport in rptH.Subreports)
                {
                    if (_listData.ContainsKey(subreport.Name.Trim().ToLower()))
                    {
                        subreport.SetDataSource(_listData[subreport.Name.Trim().ToLower()]);
                    }
                }

                _parameters = insencitiveParameter();
                foreach (ParameterField field in rptH.ParameterFields)
                {
                    if (_parameters != null)
                    {
                        if (_parameters.ContainsKey(field.Name.Trim().ToLower()))
                        {
                            par = new ParameterDiscreteValue();
                            par.Value = _parameters[field.Name.Trim().ToLower()];
                            field.CurrentValues.Add(par);
                        }
                    }
                }

                System.IO.Stream stream = rptH.ExportToStream(_type);
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
            if (_parameters != null)
            {
                foreach (var item in _parameters)
                {
                    p.Add(item.Key.Trim().ToLower(), item.Value);
                }
                return p;
            }
            return null;
        }


    }
}
