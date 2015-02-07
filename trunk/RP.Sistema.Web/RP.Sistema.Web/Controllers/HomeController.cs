using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RP.Sistema.Web.Models;
using RP.Sistema.Model;
using System.Data.Entity.Infrastructure;
using System.Web;
using System.Web.Security;
using System;

namespace RP.Sistema.Web.Controllers
{
    public class HomeController : Controller
    {
        [Auth.Class.Auth()]
        public ActionResult Index()
        {
            try
            {
                BLL.UsuarioBLL usuarioBLL = null;

                RP.Sistema.Model.Entities.Usuario usuario = null;
                Model.Entities.Usuario.Preferencias preferencias = null;
                //}

                using (Context db = new Context())
                {
                    usuarioBLL = new BLL.UsuarioBLL(db, Helpers.Helper.UserId);
                    usuario = usuarioBLL.FindSingle(u => u.dsLogin.ToLower().Trim().Equals(Helpers.Helper.UserLogin));
                    if (usuario != null)
                    {
                        preferencias = usuarioBLL.GetPreferencias(usuario.idUsuario);
                    }
                }

                // seta dados para view
                return View(new SistemaVM
                {
                    Menu = LoadMenus(),
                    Painel = new PainelVM
                    {
                        Preferencias = preferencias
                    }
                });
            }
            catch(Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, Helpers.Helper.UserId);
                return RedirectToAction("Index", "Erro");
            }
        }

        [Auth.Class.Auth()]
        public ActionResult SQL()
        {
            string sqlscript;
            using (var db = new RP.Sistema.Model.Context())
            {
                sqlscript = (db as IObjectContextAdapter).ObjectContext.CreateDatabaseScript();
            }
            ViewBag.SQL = sqlscript;

            return View();
        }

        #region Private
        private List<RP.Sistema.Web.Models.MenuVM> LoadMenus()
        {
            string user = Helpers.Helper.UserLogin.ToLower();

            List<RP.Sistema.Web.Models.MenuVM> menus;

            using (Context db = new Context())
            {
                menus = (from modulo in db.Modulos
                         join area in db.Areas on modulo.idModulo equals area.idModulo
                         join controle in db.Controles on area.idArea equals controle.idArea
                         join acao in db.Acoes on controle.idControle equals acao.idControle
                         join menu in db.Menus on acao.idMenu equals menu.idMenu
                         from acaoperfil in acao.Perfis
                         join perfil in db.Perfis on acaoperfil.idPerfil equals perfil.idPerfil
                         from perfilusuario in perfil.Usuarios
                         join usuario in db.Usuarios on perfilusuario.idUsuario equals usuario.idUsuario
                         where (acao.flMenu.Equals("Sim") && usuario.dsLogin.ToLower().Equals(user))
                         orderby modulo.nrOrdem, menu.nrOrdem, acao.nrOrdem, menu.nmMenu, acao.nmMenu
                         select new RP.Sistema.Web.Models.MenuVM
                         {
                             Modulo = modulo.nmModulo,
                             Grupo = menu.nmMenu,
                             Nome = acao.nmMenu,
                             Descricao = acao.dsAcao,
                             Url = "/" + controle.nmControle + "/" + acao.nmAcao,
                             Icone = acao.dsIcone
                         }).ToList();
            }

            return menus;
        }
        #endregion
    }
}
