using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Util;
using System.Data;
using System.Collections;



namespace RP.Sistema.Report
{
    public class Perfil
    {
        public class _dado
        {
            public int idperfil { get; set; }
            public string nmperfil { get; set; }
            public int idacao { get; set; }
            public string nmacao { get; set; }
            public string dsacao { get; set; }
            public int idcontrole { get; set; }
            public string flmenu { get; set; }
            public string nmmenu { get; set; }
            public string nmcontrole { get; set; }
            public string dscontrole { get; set; }
            public string nmarea { get; set; }
        
        }

        //public System.Web.Mvc.ActionResult getReport( Context db, int? idPerfil)
        //{

        //    return RP.Report.Generic.Report(new RP.Report.Generic.GenericData
        //    {
        //        exportTO = RP.Report.Generic.stringTOExportFormatType("PDF"),
        //        fileRPT = "relPerfilAcesso.rpt",
        //        listData = this.getReportData(db, idPerfil),
        //        parameters = new Dictionary<string, object> { { "titulo", "<center>Relátorio de Perfis</center>" } },
                
        //    });
        //}

        private Dictionary<string, DataSet> getReportData(Model.Context db, int? idPerfil)
        {
            var listData = new Dictionary<string, DataSet>
                {
                    {"table", GetDataSet( db, idPerfil)},
                    {"subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db)},
                };
            return listData;
        }

        private DataSet GetDataSet(Model.Context db, int? idPerfil)
        {
        

             var sql = @"SELECT tbperfil.idperfil, 
                                    tbperfil.nmperfil, 
                                    tbperfilacao.idacao, 
                                    tbacao.nmacao, 
                                    tbacao.dsacao, 
                                    tbcontrole.idcontrole, 
                                    tbacao.flmenu, 
                                    tbacao.nmmenu, 
                                    tbcontrole.nmcontrole, 
                                    tbcontrole.dscontrole, 
                                    tbarea.nmarea
                                    FROM         tbarea 
                                    INNER JOIN tbcontrole ON tbarea.idarea = tbcontrole.idarea 
                                    INNER JOIN tbperfil INNER JOIN tbperfilacao ON tbperfil.idperfil = tbperfilacao.idperfil 
                                    INNER JOIN tbacao ON tbperfilacao.idacao = tbacao.idacao ON tbcontrole.idcontrole = tbacao.idcontrole";



             if (idPerfil != null)
             {
                 sql += " where tbperfil.idperfil = " + idPerfil + "";
             }

            sql += "order by tbarea.nmarea, tbcontrole.nmcontrole, tbacao.nmacao";


            var item =  db.Database.SqlQuery<_dado>(sql);

            var ds = new DataSet();

            ds.Tables.Add(I2D(item.ToList()));

            return ds;
        }
        private DataTable I2D(List<_dado> items)
        {
            var _result = new DataTable("table");

            _result.Columns.Add("idperfil", System.Type.GetType("System.Int32"));
            _result.Columns.Add("nmperfil", System.Type.GetType("System.String"));
            _result.Columns.Add("idacao", System.Type.GetType("System.Int32"));
            _result.Columns.Add("nmacao", System.Type.GetType("System.String"));
            _result.Columns.Add("dsacao", System.Type.GetType("System.String"));
            _result.Columns.Add("idcontrole", System.Type.GetType("System.Int32"));
            _result.Columns.Add("flmenu", System.Type.GetType("System.String"));
            _result.Columns.Add("nmmenu", System.Type.GetType("System.String"));
            _result.Columns.Add("nmcontrole", System.Type.GetType("System.String"));
            _result.Columns.Add("dscontrole", System.Type.GetType("System.String"));
            _result.Columns.Add("nmarea", System.Type.GetType("System.String"));
        

            foreach (var item in items)
            {
                var row = _result.NewRow();

                row["idperfil"] = item.idperfil;
                row["nmperfil"] = item.nmperfil;
                row["idacao"] = item.idacao;
                row["nmacao"] = item.nmacao;
                row["dsacao"] = item.dsacao;
                row["idcontrole"] = item.idcontrole;
                row["flmenu"] = item.flmenu;
                row["nmmenu"] = item.nmmenu;
                row["nmcontrole"] = item.nmcontrole;
                row["dscontrole"] = item.dscontrole;
                row["nmarea"] = item.nmarea;

                _result.Rows.Add(row);
            }

            return _result;
        }
    }
}
