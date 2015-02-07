using CaptchaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RP.Sistema.Web.Models.Usuario
{
    public class UsuarioVM
    {
        public Sistema.Model.Entities.Usuario VM2E()
        {
            var result = new Sistema.Model.Entities.Usuario
            {
                idUsuario = this.IdUsuario,
                dsLogin = this.Login,
                nmUsuario = this.Nome,
                dsEmail = this.Email,
                flAtivo = this.Ativo ? "Sim" : "Não",
                dtValidade = this.ValidadeSenha
            };

            if (this.Perfis != null)
            {
                foreach (var item in this.Perfis)
                {
                    result.Perfis.Add(new Model.Entities.PerfilUsuario
                    {
                        idPerfil = item.IdPerfil,
                        Perfil = new Model.Entities.Perfil { 
                            idPerfil = item.IdPerfil,
                            nmPerfil = item.Nome
                        }
                    });
                }
            }
            return result;
        }

        public static UsuarioVM E2VM(Sistema.Model.Entities.Usuario model)
        {
            var result = new UsuarioVM
            {
                IdUsuario = model.idUsuario,
                Login = model.dsLogin,
                Nome = model.nmUsuario,
                Email = model.dsEmail,
                Ativo = model.flAtivo.ToLower() == "sim",
                ValidadeSenha = model.dtValidade
            };
            if (model.Perfis != null)
            {
                result.Perfis = new List<PerfilVM>();
                foreach (var item in model.Perfis)
                {
                    result.Perfis.Add(new PerfilVM { IdPerfil = item.idPerfil, Nome = item.Perfil.nmPerfil });
                }
            }

            return result;
        }

        public struct PerfilVM
        {
            public int IdPerfil { get; set; }
            public string Nome { get; set; }
        }

        public List<PerfilVM> Perfis { get; set; }

        [Display(Name = "Id. Usuário")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage="Informe o login do usuário")]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Informe o nome do usuário")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe o e-mail do usuário")]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Display(Name = "Validade da senha")]
        public Nullable<DateTime> ValidadeSenha { get; set; }

        [Display(Name = "Usuário ativo")]
        public bool Ativo { get; set; }

        public string dsFoto { get; set; }
    }

    public class LoginVM
    {
        [Required(ErrorMessage = "Digite o nome de usuário")]
        [Display(Name = "Usuário")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Digite a senha")]
        [Display(Name = "Senha")]
        public string Senha { get; set; }

        [Display(Name = "Lembrar o usuário")]
        public bool LembrarUsuario { get; set; }

        public LoginVM()
        {
            this.LembrarUsuario = false;
        }
    }

    public class LoginCaptchaVM : LoginVM
    {
        [ValidateCaptcha(ErrorMessage = "Código de segurança inválido")]
        public string Codigo { get; set; }    
    }

    public class EditarSenhaVM
    {
        [Display(Name = "Usuário")]
        [Required(ErrorMessage = "Digite o nome de usuário")]
        public string Usuario { get; set; }

        [Display(Name = "Senha atual")]
        [Required(ErrorMessage = "Digite a senha atual")]
        public string Senha { get; set; }

        [Display(Name = "Digite a nova senha")]
        [Required(ErrorMessage = "Digite a nova senha")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d.*)(?=.*\W.*)[a-zA-Z0-9\S]{8,}$", ErrorMessage = "A senha informada não atende os critérios de segurança")]
        public string NovaSenha { get; set; }

        [Display(Name = "Re-digite a nova senha")]
        [Required(ErrorMessage = "Re-digite a nova senha")]
        [Compare("NovaSenha", ErrorMessage = "A confirmação da senha não confere")]
        public string ReNovaSenha { get; set; }
    }

    public class AlterarPerfilVM
    {
        public UsuarioVM Usuario { get; set; }

        [Display(Name = "Senha atual")]
        public string SenhaAtual { get; set; }

        [Display(Name = "Nova senha")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d.*)(?=.*\W.*)[a-zA-Z0-9\S]{8,}$", ErrorMessage = "A senha informada não atende os critérios de segurança")]
        public string NovaSenha { get; set; }

        [Display(Name = "Re-digite a nova senha")]
        [Compare("NovaSenha", ErrorMessage = "A confirmação da senha não confere")]
        public string ConfirmacaoSenha { get; set; }
    }

    public class ResetSenha
    {
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Por favor, informe o email")]
        public string email { get; set; }
    }
}