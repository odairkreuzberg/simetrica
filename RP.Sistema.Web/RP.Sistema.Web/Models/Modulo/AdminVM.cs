using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Sistema.Web.Models.Modulo
{
    public class AdminVM
    {
        public Sistema.Model.Entities.Modulo VM2E()
        {
            var result = new Sistema.Model.Entities.Modulo
            {
                idModulo = this.Modulo.idModulo ?? 0,
                nmModulo = this.Modulo.nmModulo
            };

            if (this.Usuarios != null)
            {
                result.Usuarios = new List<Model.Entities.ModuloUsuario>();
                foreach (var item in this.Usuarios)
                {
                    result.Usuarios.Add(new Model.Entities.ModuloUsuario { 
                        idUsuario = item.idUsuario ?? 0,
                        idModulo = result.idModulo,
                        Usuario = new Model.Entities.Usuario 
                        {
                            idUsuario = item.idUsuario ?? 0,
                            nmUsuario = item.nmUsuario 
                        } 
                    });
                }
            }

            return result;
        }

        public static AdminVM E2VM(Sistema.Model.Entities.Modulo model)
        {
            var result = new Web.Models.Modulo.AdminVM
            {
                Modulo = new Models.Modulo.Consultar
                {
                    idModulo = model.idModulo,
                    nmModulo = model.nmModulo
                },
                Usuarios = new List<Models.Usuario.Consultar>()
            };

            foreach (var item in model.Usuarios)
            {
                result.Usuarios.Add(new Models.Usuario.Consultar
                {
                    idUsuario = item.idUsuario,
                    nmUsuario = item.Usuario.nmUsuario
                });
            };

            return result;
        }

        public Consultar Modulo { get; set; }
        public List<Usuario.Consultar> Usuarios { get; set; }
    }
}