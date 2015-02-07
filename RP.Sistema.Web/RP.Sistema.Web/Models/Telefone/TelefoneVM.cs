using RP.Sistema.Model.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RP.Sistema.Web.Models.Telefone
{
    public class TelefoneVM
    {
        public static readonly SelectListItem[] Tipo = new[]
        {
            new SelectListItem { Text = "", Value = "" }, 
            new SelectListItem { Text = "Fixo", Value = "Fixo" }, 
            new SelectListItem { Text = "Celular", Value = "Celular" }, 
            new SelectListItem { Text = "Fax", Value = "Fax" }
        };

        public string tipo { get; set; }
        public string numero { get; set; }

        public static List<TelefoneVM> GetTelefonesCliente(List<ClienteFone> list)
        {
            var _result = new List<TelefoneVM>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    var fone = new TelefoneVM
                    {
                        tipo = item.tipo,
                        numero = item.numero,
                    };
                    _result.Add(fone);
                }
            }
            return _result;
        }

        public static List<ClienteFone> GetTelefonesCliente(List<TelefoneVM> list)
        {
            var _result = new List<ClienteFone>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    var fone = new ClienteFone
                    {
                        tipo = item.tipo,
                        numero = item.numero
                    };
                    _result.Add(fone);
                }
            }
            return _result;
        }

        internal static List<TelefoneVM> GetTelefonesFornecedor(List<FornecedorFone> list)
        {
            var _result = new List<TelefoneVM>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    var fone = new TelefoneVM
                    {
                        tipo = item.tipo,
                        numero = item.numero
                    };
                    _result.Add(fone);
                }
            }
            return _result;
        }

        internal static List<FornecedorFone> GetTelefonesFornecedor(List<TelefoneVM> list)
        {
            var _result = new List<FornecedorFone>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    var fone = new FornecedorFone
                    {
                        tipo = item.tipo,
                        numero = item.numero,
                    };
                    _result.Add(fone);
                }
            }
            return _result;
        }
    }
}