using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;

namespace RP.Sistema.Model.Entities
{
    [Serializable]
    public class Usuario
	{
        public class Preferencias
        {
            public struct Atalho
            {
                public string Nome { get; set; }
                public string Icone { get; set; }
                public string Href { get; set; }
                public int Posicao { get; set; }
            }

            public Preferencias()
            {
                Atalhos = new List<Atalho>();
            }

            public List<Atalho> Atalhos { get; set; }
        }

	    public Usuario()
		{
            this.Tabelas = new List<Tabela>();
            this.Perfis = new List<PerfilUsuario>();
            this.Modulos = new List<ModuloUsuario>();
            this.funcionarios = new List<Funcionario>();
		}

        public int idUsuario { get; set; }
        public string dsLogin { get; set; }
        public string dsSenha { get; set; }
        public string flAtivo { get; set; }
        public DateTime? dtValidade { get; set; }
        public string nmUsuario { get; set; }
        public string dsEmail { get; set; }
        public string fzUsuario { get; set; }
        public string dsPreferencia { get; set; }
        public int? nrFalhalogin { get; set; }

        public Preferencias Preferencia { get; set; }
        public ICollection<Tabela> Tabelas { get; set; }
        public ICollection<PerfilUsuario> Perfis { get; set; }
        public ICollection<ModuloUsuario> Modulos { get; set; }
        public ICollection<Projeto> projetoes { get; set; }
        public ICollection<Compra> compras { get; set; }
        public ICollection<FolhaPagamento> folhapagamentoes { get; set; }
        public ICollection<Funcionario> funcionarios { get; set; }
        public ICollection<MovimentoProfissional> movimentoprofissionals { get; set; }
    }
}

