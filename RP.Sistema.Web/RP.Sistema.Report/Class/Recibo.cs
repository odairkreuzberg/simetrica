using RP.Sistema.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace RP.Sistema.Report.Class
{
    public class Recibo
    {
        public System.Web.Mvc.ActionResult GetReport(Model.Context db, int idFolha, int _idUsuario)
        {
            String titulo = "<center>Recibo Nº. " + idFolha.ToString().PadLeft(8, '0') + "<center>";
            return Report.genericReport(
                new Report.genericReportData
                {
                    exportTO = Report.stringTOExportFormatType("PDF"),
                    fileRPT = "Recibo.rpt",
                    listData = this.GetReportData(db, idFolha, _idUsuario),
                    parameters = new Dictionary<string, object> { { "titulo", titulo } }
                }
            );
        }

        public Dictionary<string, DataSet> GetReportData(Model.Context db, int idFolha, int _idUsuario)
        {
            var listData = new Dictionary<string, DataSet>
            { 
                {"table", GetDataSet(db, idFolha, _idUsuario)},
                {"subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db)}, 
            };

            return listData;
        }

        private DataSet GetDataSet(Model.Context db, int idFolha, int _idUsuario)
        {
            var _bll = new RP.Sistema.BLL.FolhaPagamentoBLL(db, _idUsuario);
            var _folha = _bll.FindSingle(u => u.idFolhaPagamento == idFolha, u => u.Funcionario, u => u.MovimentoProfissionais);

            var dataTable = Relatorio.GetDataTable(_folha);

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);

            return dataset;
        }

        private class Relatorio
        {

            internal static DataTable GetDataTable(RP.Sistema.Model.Entities.FolhaPagamento _folha)
            {
                var _result = new DataTable("table");
                System.Data.DataRow row;

                _result.Columns.Add("idfuncionario", Type.GetType("System.Int32"));
                _result.Columns.Add("nome", Type.GetType("System.String"));
                _result.Columns.Add("tipo", Type.GetType("System.String"));
                _result.Columns.Add("rg", Type.GetType("System.String"));
                _result.Columns.Add("cpf", Type.GetType("System.String"));
                _result.Columns.Add("email", Type.GetType("System.String"));
                _result.Columns.Add("observacao", Type.GetType("System.String"));
                _result.Columns.Add("numero", Type.GetType("System.String"));
                _result.Columns.Add("cep", Type.GetType("System.String"));
                _result.Columns.Add("logradouro", Type.GetType("System.String"));
                _result.Columns.Add("bairro", Type.GetType("System.String"));
                _result.Columns.Add("fone", Type.GetType("System.String"));
                _result.Columns.Add("celular", Type.GetType("System.String"));
                _result.Columns.Add("dtnascimento", Type.GetType("System.DateTime"));
                _result.Columns.Add("dtentrada", Type.GetType("System.DateTime"));
                _result.Columns.Add("salario", Type.GetType("System.Decimal"));
                _result.Columns.Add("comissao", Type.GetType("System.Decimal"));
                _result.Columns.Add("dtsaida", Type.GetType("System.DateTime"));
                _result.Columns.Add("motivosaida", Type.GetType("System.String"));
                _result.Columns.Add("status", Type.GetType("System.String"));
                _result.Columns.Add("ctps", Type.GetType("System.String"));


                _result.Columns.Add("total", Type.GetType("System.Decimal"));
                _result.Columns.Add("bonificacao", Type.GetType("System.String"));
                _result.Columns.Add("outrosDescontos", Type.GetType("System.Decimal"));
                _result.Columns.Add("inss", Type.GetType("System.Decimal"));
                _result.Columns.Add("vale", Type.GetType("System.Decimal"));
                _result.Columns.Add("dtPagamento", Type.GetType("System.DateTime"));
                _result.Columns.Add("nrAno", Type.GetType("System.Int32"));
                _result.Columns.Add("nrMes", Type.GetType("System.Int32"));
                _result.Columns.Add("situacao", Type.GetType("System.String"));
                _result.Columns.Add("tipoMovimento", Type.GetType("System.String"));
                _result.Columns.Add("descricaoMovimento", Type.GetType("System.String"));
                _result.Columns.Add("valorMovimento", Type.GetType("System.Decimal"));
                _result.Columns.Add("dataExtenco", Type.GetType("System.String"));
                _result.Columns.Add("valorExtenco", Type.GetType("System.String"));

                var culture = new CultureInfo("pt-BR");
                var dtfi = culture.DateTimeFormat;
                string dsMes = culture.TextInfo.ToTitleCase(dtfi.GetMonthName(_folha.nrMes));

                var funcionario = _folha.Funcionario;

                row = _result.NewRow();
                row["idfuncionario"] = funcionario.idFuncionario;
                row["nome"] = funcionario.nome;
                row["tipo"] = funcionario.tipo;
                row["rg"] = funcionario.rg;
                row["cpf"] = funcionario.cpf;
                row["email"] = funcionario.email;
                row["observacao"] = funcionario.observacao;
                row["numero"] = funcionario.numero;
                row["cep"] = funcionario.cep;
                row["logradouro"] = funcionario.logradouro;
                row["bairro"] = funcionario.bairro;
                row["fone"] = funcionario.fone;
                row["celular"] = funcionario.celular;
                row["dtnascimento"] = funcionario.dtNascimento;
                row["dtentrada"] = funcionario.dtEntrada;
                row["ctps"] = funcionario.ctps;
                row["valorExtenco"] = RP.Util.Class.Converter.toExtenso(_folha.total);
                row["dataExtenco"] = dsMes + " de " + _folha.nrAno;
                row["total"] = _folha.total;

                if (_folha.salario > 0)
                {
                    row = _result.NewRow();
                    row["idfuncionario"] = funcionario.idFuncionario;
                    row["nome"] = funcionario.nome;
                    row["tipo"] = funcionario.tipo;
                    row["rg"] = funcionario.rg;
                    row["cpf"] = funcionario.cpf;
                    row["email"] = funcionario.email;
                    row["observacao"] = funcionario.observacao;
                    row["numero"] = funcionario.numero;
                    row["cep"] = funcionario.cep;
                    row["logradouro"] = funcionario.logradouro;
                    row["bairro"] = funcionario.bairro;
                    row["fone"] = funcionario.fone;
                    row["celular"] = funcionario.celular;
                    row["dtnascimento"] = funcionario.dtNascimento;
                    row["dtentrada"] = funcionario.dtEntrada;
                    row["ctps"] = funcionario.ctps;
                    row["valorExtenco"] = RP.Util.Class.Converter.toExtenso(_folha.total);
                    row["dataExtenco"] = dsMes + " de " + _folha.nrAno;
                    row["nrAno"] = _folha.nrAno;
                    row["nrMes"] = _folha.nrMes;


                    row["valorMovimento"] = _folha.salario;
                    row["descricaoMovimento"] = "Salário normal";
                    row["tipoMovimento"] = "Comissão";
                    row["total"] = _folha.total;
                    _result.Rows.Add(row);
                }

                if (_folha.horaExtra > 0)
                {
                    row = _result.NewRow();
                    row["idfuncionario"] = funcionario.idFuncionario;
                    row["nome"] = funcionario.nome;
                    row["tipo"] = funcionario.tipo;
                    row["rg"] = funcionario.rg;
                    row["cpf"] = funcionario.cpf;
                    row["email"] = funcionario.email;
                    row["observacao"] = funcionario.observacao;
                    row["numero"] = funcionario.numero;
                    row["cep"] = funcionario.cep;
                    row["logradouro"] = funcionario.logradouro;
                    row["bairro"] = funcionario.bairro;
                    row["fone"] = funcionario.fone;
                    row["celular"] = funcionario.celular;
                    row["dtnascimento"] = funcionario.dtNascimento;
                    row["dtentrada"] = funcionario.dtEntrada;
                    row["ctps"] = funcionario.ctps;
                    row["valorExtenco"] = RP.Util.Class.Converter.toExtenso(_folha.total);
                    row["dataExtenco"] = dsMes + " de " + _folha.nrAno;
                    row["nrAno"] = _folha.nrAno;
                    row["nrMes"] = _folha.nrMes;


                    row["valorMovimento"] = _folha.horaExtra;
                    row["descricaoMovimento"] = "Total de horas extras referente";
                    row["tipoMovimento"] = "Comissão";
                    row["total"] = _folha.total;
                    _result.Rows.Add(row);
                }

                if (_folha.outrosDescontos > 0)
                {
                    row = _result.NewRow();
                    row["idfuncionario"] = funcionario.idFuncionario;
                    row["nome"] = funcionario.nome;
                    row["tipo"] = funcionario.tipo;
                    row["rg"] = funcionario.rg;
                    row["cpf"] = funcionario.cpf;
                    row["email"] = funcionario.email;
                    row["observacao"] = funcionario.observacao;
                    row["numero"] = funcionario.numero;
                    row["cep"] = funcionario.cep;
                    row["logradouro"] = funcionario.logradouro;
                    row["bairro"] = funcionario.bairro;
                    row["fone"] = funcionario.fone;
                    row["celular"] = funcionario.celular;
                    row["dtnascimento"] = funcionario.dtNascimento;
                    row["dtentrada"] = funcionario.dtEntrada;
                    row["ctps"] = funcionario.ctps;
                    row["valorExtenco"] = RP.Util.Class.Converter.toExtenso(_folha.total);
                    row["dataExtenco"] = dsMes + " de " + _folha.nrAno;
                    row["nrAno"] = _folha.nrAno;
                    row["nrMes"] = _folha.nrMes;


                    row["vale"] = _folha.outrosDescontos;
                    row["descricaoMovimento"] = "Descontos adicionais";
                    row["tipoMovimento"] = "Vale";
                    row["total"] = _folha.total;
                    _result.Rows.Add(row);
                }

                if (_folha.inss > 0)
                {
                    row = _result.NewRow();
                    row["idfuncionario"] = funcionario.idFuncionario;
                    row["nome"] = funcionario.nome;
                    row["tipo"] = funcionario.tipo;
                    row["rg"] = funcionario.rg;
                    row["cpf"] = funcionario.cpf;
                    row["email"] = funcionario.email;
                    row["observacao"] = funcionario.observacao;
                    row["numero"] = funcionario.numero;
                    row["cep"] = funcionario.cep;
                    row["logradouro"] = funcionario.logradouro;
                    row["bairro"] = funcionario.bairro;
                    row["fone"] = funcionario.fone;
                    row["celular"] = funcionario.celular;
                    row["dtnascimento"] = funcionario.dtNascimento;
                    row["dtentrada"] = funcionario.dtEntrada;
                    row["ctps"] = funcionario.ctps;
                    row["valorExtenco"] = RP.Util.Class.Converter.toExtenso(_folha.total);
                    row["dataExtenco"] = dsMes + " de " + _folha.nrAno;
                    row["nrAno"] = _folha.nrAno;
                    row["nrMes"] = _folha.nrMes;
                    row["total"] = _folha.total;


                    row["vale"] = _folha.inss;
                    row["descricaoMovimento"] = "INSS";
                    row["tipoMovimento"] = "Vale";
                    _result.Rows.Add(row);
                }

                if (_folha.bonificacao > 0)
                {
                    row = _result.NewRow();
                    row["idfuncionario"] = funcionario.idFuncionario;
                    row["nome"] = funcionario.nome;
                    row["tipo"] = funcionario.tipo;
                    row["rg"] = funcionario.rg;
                    row["cpf"] = funcionario.cpf;
                    row["email"] = funcionario.email;
                    row["observacao"] = funcionario.observacao;
                    row["numero"] = funcionario.numero;
                    row["cep"] = funcionario.cep;
                    row["logradouro"] = funcionario.logradouro;
                    row["bairro"] = funcionario.bairro;
                    row["fone"] = funcionario.fone;
                    row["celular"] = funcionario.celular;
                    row["dtnascimento"] = funcionario.dtNascimento;
                    row["dtentrada"] = funcionario.dtEntrada;
                    row["ctps"] = funcionario.ctps;
                    row["valorExtenco"] = RP.Util.Class.Converter.toExtenso(_folha.total);
                    row["dataExtenco"] = dsMes + " de " + _folha.nrAno;
                    row["nrAno"] = _folha.nrAno;
                    row["nrMes"] = _folha.nrMes;
                    row["total"] = _folha.total;


                    row["valorMovimento"] = _folha.bonificacao;
                    row["descricaoMovimento"] = "Bonificações extras";
                    row["tipoMovimento"] = "Comissão";
                    _result.Rows.Add(row);
                }

                if (_folha.MovimentoProfissionais.Any())
                {
                    foreach (var movimento in _folha.MovimentoProfissionais.ToList())
                    {
                        row = _result.NewRow();
                        row["idfuncionario"] = funcionario.idFuncionario;
                        row["nome"] = funcionario.nome;
                        row["tipo"] = funcionario.tipo;
                        row["rg"] = funcionario.rg;
                        row["cpf"] = funcionario.cpf;
                        row["email"] = funcionario.email;
                        row["observacao"] = funcionario.observacao;
                        row["numero"] = funcionario.numero;
                        row["cep"] = funcionario.cep;
                        row["logradouro"] = funcionario.logradouro;
                        row["bairro"] = funcionario.bairro;
                        row["fone"] = funcionario.fone;
                        row["celular"] = funcionario.celular;
                        row["dtnascimento"] = funcionario.dtNascimento;
                        row["dtentrada"] = funcionario.dtEntrada;
                        row["ctps"] = funcionario.ctps;
                        row["valorExtenco"] = RP.Util.Class.Converter.toExtenso(_folha.total);
                        row["dataExtenco"] = dsMes + " de " + _folha.nrAno;
                        row["nrAno"] = _folha.nrAno;
                        row["nrMes"] = _folha.nrMes;
                        row["total"] = _folha.total;

                        if (movimento.tipo == MovimentoProfissional.TIPO_VALE)
                        {
                            row["vale"] = movimento.valor;
                            row["descricaoMovimento"] = movimento.descricao + " - " + movimento.dtLancamento.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            row["descricaoMovimento"] = movimento.descricao;
                            row["valorMovimento"] = movimento.valor;
                        }
                        row["tipoMovimento"] = movimento.tipo;

                        _result.Rows.Add(row);
                    }
                }
                return _result;
            }
        }
    }
}
