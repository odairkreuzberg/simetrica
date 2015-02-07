using RP.Sistema.Web.Models.Telefone;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using System;

namespace RP.Sistema.Web.Models.Funcionario
{
    public class FuncionarioVM
    {
        public static readonly SelectListItem[] Tipo = new[]
        {
            new SelectListItem { Text = "", Value = "" }, 
            new SelectListItem { Text = "Marceneiro", Value = "Marceneiro" },  
            new SelectListItem { Text = "Vendedor", Value = "Vendedor" }, 
            new SelectListItem { Text = "Projetista", Value = "Projetista" }, 
            new SelectListItem { Text = "Auxiliar", Value = "Auxiliar" }, 
            new SelectListItem { Text = "Secretária", Value = "Secretária" },
            new SelectListItem { Text = "Aux. Marceneiro", Value = "Aux. Marceneiro" },
            new SelectListItem { Text = "Outros", Value = "Outros" }
        };
        public static readonly SelectListItem[] Status = new[]
        {
            new SelectListItem { Text = Sistema.Model.Entities.Funcionario.ATIVO, Value = Sistema.Model.Entities.Funcionario.ATIVO }, 
            new SelectListItem { Text = Sistema.Model.Entities.Funcionario.FERIAS, Value = Sistema.Model.Entities.Funcionario.FERIAS }
        };

        public Sistema.Model.Entities.Funcionario GetFuncionario()
        {
            var _result = new Sistema.Model.Entities.Funcionario
                {
                    nome = this.nome,
                    idFuncionario = this.idFuncionario,
                    tipo = this.tipo,
                    rg = this.rg,
                    nrCargaHoraria = this.nrCargaHoraria,
                    cpf = this.cpf,
                    email = this.email,
                    observacao = this.observacao,
                    numero = this.numero,
                    bairro = this.bairro,
                    cep = this.cep,
                    logradouro = this.logradouro,
                    idCidade = this.Cidade.idCidade ?? 0,
                    fone = this.fone,
                    celular = this.celular,
                    dtNascimento = this.dtNascimento,
                    salario = this.salario,
                    dtEntrada = this.dtEntrada,
                    comissao = this.comissao,
                    motivoSaida = this.motivoSaida,
                    status = this.status,
                    ctps = this.ctps,
                    flMensalista = this.flMensalista,
                    idUsuario = this.Usuario.idUsuario,
                };
            return _result;
        }

        public static FuncionarioVM GetFuncionario(Sistema.Model.Entities.Funcionario model)
        {
            var _result = new FuncionarioVM
            {
                nome = model.nome,
                idFuncionario = model.idFuncionario,
                tipo = model.tipo,
                rg = model.rg,
                nrCargaHoraria = model.nrCargaHoraria,
                cpf = model.cpf,
                email = model.email,
                observacao = model.observacao,
                numero = model.numero,
                bairro = model.bairro,
                cep = model.cep,
                logradouro = model.logradouro,
                fone = model.fone,
                celular = model.celular,
                dtNascimento = model.dtNascimento,
                salario = model.salario,
                dtEntrada = model.dtEntrada,
                comissao = model.comissao,
                status = model.status,
                ctps = model.ctps,
                motivoSaida = model.motivoSaida,
                Cidade = Models.Cidade.Consultar.GetModel(model.Cidade),
                Usuario = Models.Usuario.Consultar.GetModel(model.Usuario),
                flMensalista = model.flMensalista
            };

            return _result;
        }

        public int idFuncionario { get; set; }

        [Display(Name = "Mensalista")]
        public string flMensalista { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Informe o nome")]
        public string nome { get; set; }

        [Display(Name = "Atividade")]
        [Required(ErrorMessage = "Informe o atividade")]
        public string tipo { get; set; }

        [Display(Name = "CTPS")]
        [Required(ErrorMessage = "Informe a CTPS")]
        public string ctps { get; set; }

        [Display(Name = "Carga horaria (mensal)")]
        [Required(ErrorMessage = "Informe a carga horaria")]
        public int? nrCargaHoraria { get; set; }

        [Display(Name = "CPF")]
        public string cpf { get; set; }

        [Display(Name = "E-mail")]
        public string email { get; set; }

        [Display(Name = "RG")]
        public string rg { get; set; }

        [Display(Name = "Observação")]
        public string observacao { get; set; }

        [Display(Name = "Nº. residencial")]
        [Required(ErrorMessage = "Informe o número")]
        public string numero { get; set; }

        [Display(Name = "CEP")]
        public string cep { get; set; }

        [Display(Name = "Logradouro")]
        [Required(ErrorMessage = "Informe o logradouro")]
        public string logradouro { get; set; }

        [Display(Name = "Bairro")]
        [Required(ErrorMessage = "Informe o bairro")]
        public string bairro { get; set; }

        [Display(Name = "Telefone")]
        public string fone { get; set; }

        [Display(Name = "Celular")]
        public string celular { get; set; }

        [Display(Name = "Dt. nascimento")]
        [Required(ErrorMessage = "Informe a data de nascimento")]
        public DateTime? dtNascimento { get; set; }

        [Display(Name = "Dt. admissão")]
        [Required(ErrorMessage = "Informe a data de admissão")]
        public DateTime? dtEntrada { get; set; }

        [Display(Name = "Salário (R$)")]
        [Required(ErrorMessage = "Informe o salário")]
        public decimal? salario { get; set; }

        [Display(Name = "Comissão (%)")]
        public decimal? comissao { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }

        [Display(Name = "Motivo")]
        public string motivoSaida { get; set; }

        public Models.Cidade.Consultar Cidade { get; set; }
        public Models.Usuario.Consultar Usuario { get; set; }
    }
}