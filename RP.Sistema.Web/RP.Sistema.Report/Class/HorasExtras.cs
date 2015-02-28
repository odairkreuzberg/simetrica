using RP.Sistema.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class HorasExtras
    {
        public class CalculoHora
        {
            public string nmFuncionario { get; set; }
            public decimal? salario { get; set; }
            public decimal porcentagem { get; set; }
            public int caragaHoraria { get; set; }
            public string flTipo { get; set; }
            public TimeSpan hora { get; set; }
            public decimal valor { get; set; }
        }

        private List<CalculoHora> Horas { get; set; }

        public System.Web.Mvc.ActionResult GetReport(Model.Context db, int ano, int mes, int _idUsuario)
        {
            var culture = new CultureInfo("pt-BR");
            var dtfi = culture.DateTimeFormat;
            string dsMes = culture.TextInfo.ToTitleCase(dtfi.GetMonthName(mes));
            Horas = new List<CalculoHora>();
            string titulo = "<center>RELAÇÃO DE HORAS EXTRAS - " + dsMes.ToUpper() + "  -  " + ano + "<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "HorasExtras.rpt",
                    listData = this.GetReportData(db, ano, mes, _idUsuario),
                    parameters = new Dictionary<string, object> { { "titulo", titulo } }
                }
            );

        }

        public Dictionary<string, DataSet> GetReportData(Model.Context db, int ano, int mes, int _idUsuario)
        {
            var listData = new Dictionary<string, DataSet>
            { 
                {"table", GetDataSet(db, ano, mes, _idUsuario)},
                {"subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db)}, 
                {"calculohoras", this.GetSub()}, 
                {"calculototal", this.GetSubTotal()}, 
            };

            return listData;
        }

        private DataSet GetSubTotal()
        {
            var list = this.Horas;
            var dataSet = new DataSet();
            var table = new DataTable("table");
            System.Data.DataRow row;

            table.Columns.Add("nmFuncionario", Type.GetType("System.String"));
            table.Columns.Add("salario", Type.GetType("System.Decimal"));
            table.Columns.Add("porcentagem", Type.GetType("System.Decimal"));
            table.Columns.Add("flTipo", Type.GetType("System.String"));
            table.Columns.Add("hora", Type.GetType("System.TimeSpan"));
            table.Columns.Add("valor", Type.GetType("System.Decimal"));
            decimal? total = 0;
            foreach (var item in list.OrderByDescending(u => u.nmFuncionario))
            {
                row = table.NewRow();
                row["nmFuncionario"] = item.nmFuncionario;
                row["flTipo"] = item.flTipo;
                row["valor"] = item.valor;
                table.Rows.Add(row);
                row = table.NewRow();
                row["nmFuncionario"] = item.nmFuncionario;
                row["flTipo"] = "Valor do Descanso Semanal";
                row["valor"] = item.valor/26 * 5;
                table.Rows.Add(row);

            }

            dataSet.Tables.Add(table);
            return dataSet;
        }

        private DataSet GetSub()
        {
            var list = this.Horas;
            Horas = new List<CalculoHora>();
            var dataSet = new DataSet();
            var table = new DataTable("table");
            System.Data.DataRow row;

            table.Columns.Add("nmFuncionario", Type.GetType("System.String"));
            table.Columns.Add("salario", Type.GetType("System.Decimal"));
            table.Columns.Add("porcentagem", Type.GetType("System.Decimal"));
            table.Columns.Add("flTipo", Type.GetType("System.String"));
            table.Columns.Add("hora", Type.GetType("System.TimeSpan"));
            table.Columns.Add("valor", Type.GetType("System.Decimal"));
            decimal? total = 0;
            foreach (var item in list.OrderByDescending(u => u.nmFuncionario))
            {
                decimal? valor = (((item.salario / item.caragaHoraria) * ((decimal)item.hora.TotalHours) * item.porcentagem) / 100);
                row = table.NewRow();
                row["nmFuncionario"] = item.nmFuncionario;
                row["flTipo"] = item.flTipo;
                row["valor"] = valor;
                table.Rows.Add(row);

            }

            foreach (var grupo in list.GroupBy(u => u.flTipo))
            {
                decimal? valor = 0;
                foreach (var item in grupo)
                {
                    valor += (((item.salario / 220) * ((decimal)item.hora.TotalHours) * item.porcentagem) / 100);
                }
                var fun = grupo.First();

                row = table.NewRow();
                row["nmFuncionario"] = "Total";
                row["flTipo"] = fun.flTipo;
                row["valor"] = valor;
                table.Rows.Add(row);
                total += valor;
            }

            foreach (var grupo in list.GroupBy(u => u.nmFuncionario))
            {
                decimal? valor = 0;
                foreach (var item in grupo)
                {
                    valor += (((item.salario / 220) * ((decimal)item.hora.TotalHours) * item.porcentagem) / 100);
                }
                var fun = grupo.First();

                row = table.NewRow();
                row["nmFuncionario"] = fun.nmFuncionario;
                row["flTipo"] = "Total";
                row["valor"] = valor;
                table.Rows.Add(row);
                this.Horas.Add(new CalculoHora 
                {
                    nmFuncionario = fun.nmFuncionario,
                    caragaHoraria = fun.caragaHoraria,
                    porcentagem = fun.porcentagem, 
                    valor  = valor.Value, 
                    flTipo = "Valor das Horas Extras" });
            }

            row = table.NewRow();
            row["nmFuncionario"] = "Total";
            row["flTipo"] = "Total";
            row["valor"] = total;
            table.Rows.Add(row);

            dataSet.Tables.Add(table);
            return dataSet;
        }

        private DataSet GetDataSet(Model.Context db, int ano, int mes, int _idUsuario)
        {
            var inicio = new DateTime(ano, mes, 1);
            var fim = inicio.AddMonths(1);
            var _bll = new FuncionarioBLL(db, _idUsuario);
            var _feriadoBLL = new FeriadoBLL(db, _idUsuario);
            var _horaBLL = new HoraExtraBLL(db, _idUsuario);

            var _funcionarios = _bll.Find(u => u.status == RP.Sistema.Model.Entities.Funcionario.ATIVO && u.CartaoPontos.Any(h => h.dtPonto >= inicio && h.dtPonto < fim), u => u.CartaoPontos).ToList();
            var _feriados = _feriadoBLL.Find(u => u.nrMes == mes).ToList();
            var _horas = _horaBLL.Find().ToList();

            var dataTable = GetDataTable(_funcionarios, _feriados, _horas, inicio, fim);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private DataTable GetDataTable(List<RP.Sistema.Model.Entities.Funcionario> funcionarios, List<RP.Sistema.Model.Entities.Feriado> feriados, List<RP.Sistema.Model.Entities.HoraExtra> horas, DateTime inicio, DateTime fim)
        {
            var _result = new DataTable("table");

            _result.Columns.Add("idfuncionario", Type.GetType("System.Int32"));
            _result.Columns.Add("fltipo", Type.GetType("System.String"));
            _result.Columns.Add("hrextra", Type.GetType("System.String"));
            _result.Columns.Add("nome", Type.GetType("System.String"));

            var geral = new TimeSpan();
            foreach (var item in funcionarios)
            {
                var dia = new TimeSpan();
                var totalFuncionario = new TimeSpan();
                //sabados domingos e feriados
                foreach (var hora in horas)
                {
                    // (u.entradaExtra <= hora.inicioHora && hora.fimHora == null) || 
                    foreach (var h in item.CartaoPontos.Where(u => (u.flSituacao.ToLower().Contains("domingo") || u.flSituacao.ToLower().Contains("sábado") || u.flSituacao.ToLower().Contains("feriado")) && hora.flTipo == "Sab / Dom e Feriados"))
                    {
                        if (h.entradaExtra != null && h.saidaExtra != null)
                        {
                            dia += h.saidaExtra.Value - h.entradaExtra.Value;
                        }
                        if (h.entraTarde != null && h.saidaTarde != null)
                        {
                            dia += h.saidaTarde.Value - h.entraTarde.Value;
                        }
                        if (h.entradaManha != null && h.saidaManha != null)
                        {
                            dia += h.saidaManha.Value - h.entradaManha.Value;
                        }
                    }
                    var total = new TimeSpan();
                    foreach (var f in funcionarios)
                    {
                        foreach (var h in f.CartaoPontos.Where(u => (u.flSituacao.ToLower().Contains("domingo") || u.flSituacao.ToLower().Contains("sábado") || u.flSituacao.ToLower().Contains("feriado")) && hora.flTipo == "Sab / Dom e Feriados"))
                        {
                            if (h.entradaExtra != null && h.saidaExtra != null)
                            {
                                total += h.saidaExtra.Value - h.entradaExtra.Value;
                            }
                            if (h.entraTarde != null && h.saidaTarde != null)
                            {
                                total += h.saidaTarde.Value - h.entraTarde.Value;
                            }
                            if (h.entradaManha != null && h.saidaManha != null)
                            {
                                total += h.saidaManha.Value - h.entradaManha.Value;
                            }
                        }

                    }
                    bool entrou = false;
                    if (dia.TotalMinutes > 1)
                    {
                        var row = _result.NewRow();
                        row["fltipo"] = hora.flTipo;
                        row["hrextra"] = string.Format("{0}:{1}", ((int)dia.TotalHours).ToString("00"), dia.Minutes.ToString("00"));
                        row["idfuncionario"] = item.idFuncionario;
                        row["nome"] = item.nome;
                        _result.Rows.Add(row);
                        entrou = true;
                        totalFuncionario += dia;
                        this.Horas.Add(new CalculoHora 
                        { 
                            flTipo = hora.flTipo, 
                            hora = dia,
                            nmFuncionario = item.nome,
                            porcentagem = hora.porcentagem,
                            salario = item.salario,
                            caragaHoraria = item.nrCargaHoraria ?? 0
                        });
                    }

                    if (entrou)
                    {
                        var row = _result.NewRow();
                        row["fltipo"] = hora.flTipo;
                        row["hrextra"] = string.Format("{0}:{1}", ((int)total.TotalHours).ToString("00"), total.Minutes.ToString("00"));
                        row["nome"] = "Total das Horas Extras";
                        _result.Rows.Add(row);
                    }
                    dia = new TimeSpan();
                }
                //horas extras
                foreach (var hora in horas.Where(u => u.flTipo != "Sab / Dom e Feriados"))
                {
                    // (u.entradaExtra <= hora.inicioHora && hora.fimHora == null) || 
                    foreach (var h in item.CartaoPontos.Where(u => u.flSituacao.ToLower().Contains("dia útil") && u.saidaExtra >= hora.inicioHora))
                    {
                        if (hora.fimHora != null && hora.fimHora <= h.saidaExtra)
                        {
                            if (hora.inicioHora != null && hora.inicioHora > h.entradaExtra)
                            {
                                dia += hora.fimHora.Value - hora.inicioHora.Value;
                            }
                            else
                            {
                                dia += hora.fimHora.Value - h.entradaExtra.Value;

                            }
                        }
                        else if (hora.fimHora != null && hora.fimHora > h.saidaExtra)
                        {
                            dia += h.saidaExtra.Value - hora.inicioHora.Value;
                        }
                        else if (hora.fimHora == null)
                        {
                            if (h.entradaExtra < hora.inicioHora)
                            {
                                dia += h.saidaExtra.Value - hora.inicioHora.Value;
                            }
                            if (h.entradaExtra >= hora.inicioHora)
                            {
                                dia += h.saidaExtra.Value - h.entradaExtra.Value;
                            }
                        }
                    }
                    var total = new TimeSpan();
                    foreach (var f in funcionarios)
                    {
                        foreach (var h in f.CartaoPontos.Where(u => u.flSituacao.ToLower().Contains("dia útil") && u.saidaExtra >= hora.inicioHora))
                        {
                            if (hora.fimHora != null && hora.fimHora <= h.saidaExtra)
                            {
                                if (hora.inicioHora != null && hora.inicioHora > h.entradaExtra)
                                {
                                    total += hora.fimHora.Value - hora.inicioHora.Value;
                                }
                                else
                                {
                                    total += hora.fimHora.Value - h.entradaExtra.Value;

                                }
                            }
                            else if (hora.fimHora != null && hora.fimHora > h.saidaExtra)
                            {
                                total += h.saidaExtra.Value - hora.inicioHora.Value;
                            }
                            else if (hora.fimHora == null)
                            {
                                total += h.saidaExtra.Value - hora.inicioHora.Value;
                            }
                        }

                    }
                    bool entrou = false;
                    if (dia.TotalMinutes > 1)
                    {
                        var row = _result.NewRow();
                        row["fltipo"] = hora.flTipo;
                        row["hrextra"] = string.Format("{0}:{1}", ((int)dia.TotalHours).ToString("00"), dia.Minutes.ToString("00"));
                        row["idfuncionario"] = item.idFuncionario;
                        row["nome"] = item.nome;
                        _result.Rows.Add(row);
                        entrou = true;
                        totalFuncionario += dia;
                        this.Horas.Add(new CalculoHora 
                        { 
                            flTipo = hora.flTipo, 
                            hora = dia, 
                            nmFuncionario = item.nome,
                            porcentagem = hora.porcentagem,
                            salario = item.salario,
                            caragaHoraria = item.nrCargaHoraria ?? 0
                        });
                    }

                    if (entrou)
                    {
                        var row = _result.NewRow();
                        row["fltipo"] = hora.flTipo;
                        row["hrextra"] = string.Format("{0}:{1}", ((int)total.TotalHours).ToString("00"), total.Minutes.ToString("00"));
                        row["nome"] = "Total das Horas Extras";
                        _result.Rows.Add(row);
                    }
                    dia = new TimeSpan();
                }
                if (totalFuncionario.TotalMinutes > 0)
                {
                    var rw = _result.NewRow();
                    rw["fltipo"] = "Total";
                    rw["hrextra"] = string.Format("{0}:{1}", ((int)totalFuncionario.TotalHours).ToString("00"), totalFuncionario.Minutes.ToString("00"));
                    rw["nome"] = item.nome;
                    _result.Rows.Add(rw);
                    geral += totalFuncionario;
                }
            }
            var r = _result.NewRow();
            r["fltipo"] = "Total";
            r["hrextra"] = string.Format("{0}:{1}", ((int)geral.TotalHours).ToString("00"), geral.Minutes.ToString("00"));
            r["nome"] = "Total das Horas Extras";
            _result.Rows.Add(r);
            return _result;
        }
    }
}
