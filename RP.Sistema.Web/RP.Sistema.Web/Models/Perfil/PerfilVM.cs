using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RP.Sistema.Web.Models.Usuario;

namespace RP.Sistema.Web.Models.Perfil
{
    public class PerfilVM
    {
        public class AcaoVM
        {
            public int idAcao { get; set; }
            public string nmAcao { get; set; }
            public string dsAcao { get; set; }
            public string nmControle { get; set; }
            public string nmMenu { get; set; }

            public Sistema.Model.Entities.Acao VM2E() 
            {
                var result = new Sistema.Model.Entities.Acao
                {
                    dsAcao = this.dsAcao,
                    idAcao = this.idAcao,
                    nmAcao = this.nmAcao,
                    nmMenu = this.nmMenu
                };
                return result;
            }
        }

        public Sistema.Model.Entities.Perfil VM2E()
        {
            var result = new Sistema.Model.Entities.Perfil 
            { 
                idPerfil = this.idPerfil,
                nmPerfil = this.nmPerfil
            };

            if (this.AcoesPerfil != null)
            { 
                foreach (var item in this.AcoesPerfil)
	            {
                    result.Acoes.Add(new Model.Entities.PerfilAcao { 
                        idPerfil = this.idPerfil,
                        idAcao = item.idAcao,
                        Acao = item.VM2E()
                    });
	            }
            }

            if (this.Usuarios != null)
            {
                foreach (var item in this.Usuarios)
                {
                    result.Usuarios.Add(new Model.Entities.PerfilUsuario
                    {
                        Usuario = new Model.Entities.Usuario 
                        {
                            idUsuario = item.IdUsuario,
                            dsEmail = item.Email,
                            dsLogin = item.Login,
                            dtValidade = item.ValidadeSenha,
                            flAtivo = item.Ativo ? "Sim" : "Não",
                            nmUsuario = item.Nome
                        }
                    });
                }
            }

            return result;
        }

        public static PerfilVM E2VM(Sistema.Model.Entities.Perfil model)
        {
            var result = new PerfilVM
            {
                idPerfil = model.idPerfil,
                nmPerfil = model.nmPerfil,
            };

            if (model.Acoes != null)
            {
                result.AcoesPerfil = new List<AcaoVM>();

                foreach (var item in model.Acoes)
                {
                    result.AcoesPerfil.Add(new AcaoVM
                    {
                        idAcao = item.Acao.idAcao,
                        dsAcao = item.Acao.dsAcao,
                        nmAcao = item.Acao.nmAcao,
                        nmControle = item.Acao.Controle.nmControle,
                        nmMenu = item.Acao.nmMenu
                    });
                }
            }

            if (model.Usuarios != null)
            {
                result.Usuarios = new List<UsuarioVM>();
                foreach (var item in model.Usuarios)
                {
                    result.Usuarios.Add(new UsuarioVM
                    {
                        Email = item.Usuario.dsEmail,
                        Nome = item.Usuario.nmUsuario,
                        Ativo = item.Usuario.flAtivo.ToLower().Equals("sim"),
                        ValidadeSenha = item.Usuario.dtValidade,
                        Login = item.Usuario.dsLogin,
                        IdUsuario = item.idUsuario
                    });
                }
            }

            return result;
        }

        public static List<PerfilVM.AcaoVM> E2VM(List<Sistema.Model.Entities.Acao> acoes)
        {
            List<PerfilVM.AcaoVM> result = new List<PerfilVM.AcaoVM>();
            AcaoVM acao;
            foreach (Sistema.Model.Entities.Acao item in acoes)
            {
                acao = new AcaoVM
                {
                    idAcao = item.idAcao,
                    dsAcao = item.dsAcao,
                    nmAcao = item.nmAcao,
                    nmControle = item.Controle.nmControle,
                    nmMenu = item.nmMenu
                };
               
                result.Add(acao);
            }

            return result;
        }

        [Display(Name = "Código Perfil")]
        public int idPerfil { get; set; }

        [Display(Name = "Nome")]
        public string nmPerfil { get; set; }

        public virtual List<PerfilVM.AcaoVM> Acoes { get; set; }
        public virtual List<PerfilVM.AcaoVM> AcoesPerfil { get; set; }
        public virtual List<UsuarioVM> Usuarios { get; set; }
    }
}