using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RP.Sistema.Web.Repository;
using RP.Sistema.Model;
using RP.Util;
using System.Transactions;
using RP.Sistema.Web.Helpers;
using RP.Sistema.Web.Models.Usuario;
using CaptchaLib;
using RP.Sistema.BLL;
using RP.Sistema.Model.Entities;
using System.Configuration;

namespace RP.Sistema.Web.Controllers
{
    public class AuthController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult GetCaptcha()
        {
            return this.Captcha(new CaptchaImage()
            {
                EllipseCount = 20,
                NoiseCount = 500
            }, 80, 205, 50);
        }

        #region Login
        public ActionResult Login()
        {
            // se estiver logado
            if (Helpers.Helper.isAuthenticated)
            {
                // redireciona para tela inicial
                return RedirectToAction("Index", "Home");
            }

            // define model para view
            LoginVM model = null;

            // busca pelo cookie que contem os dados do usuario
            HttpCookie cookie = Request.Cookies["RP_StoreDataLogin"];

            // se existir o cookie e se houver nome de usuario
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                // instancia model com dados disponiveis
                model = new LoginVM { Usuario = cookie.Value, LembrarUsuario = true };
            }

            // retorna view com model
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            // se ModelState for valido
            if (ModelState.IsValid)
            {
                try
                {
                    // Cria numero para o acesso (ID do acesso)
                    string nrAcesso = Guid.NewGuid().ToString();

                    // executa metodo de consulta e validacao de usuario
                    LoginData login = DoLogin(model, nrAcesso);

                    if (login.ShowCaptcha)
                    {
                        return View("LoginCaptcha", new LoginCaptchaVM
                            {
                                Usuario = model.Usuario,
                                Senha = model.Senha
                            });
                    }


                    // se o login for Sucesso
                    if (login.LoginMessage == LoginMessageId.Sucesso)
                    {
                        // faz login do usuario no sistema
                        CriarCookieUsuario(login.Usuario, nrAcesso, model.LembrarUsuario);

                        // registra o acesso como sucesso
                        AccessRegister(login.Usuario.dsLogin, true);

                        // remove usuario da sessao
                        Session.Remove("login.UsuarioId");

                        // remove Id de acesso na sessao
                        Session.Remove("login.nrAcesso");

                        // redireciona para tela inicial
                        return RedirectToAction("Index");
                    }
                    // se existir algum erro no login
                    else
                    {
                        // se o usuario for invalido
                        if (login.LoginMessage == LoginMessageId.UsuarioInvalido)
                        {
                            // adiciona erro no modelstate
                            ModelState.AddModelError("Senha", "Usuário ou senha incorretos");
                        }
                        // se o usuario estiver inativo
                        else if (login.LoginMessage == LoginMessageId.UsuarioInativo)
                        {
                            ModelState.AddModelError("Usuario", "Usuário não está ativo no sistema");
                        }
                        // se a senha for invalida
                        else if (login.LoginMessage == LoginMessageId.SenhaInvalida)
                        {
                            // adiciona erro no modelstate
                            ModelState.AddModelError("Senha", "Usuário ou senha incorretos");

                            // se as tentivas forem maiores que o definido
                            if (login.TentativasFalhas >= Convert.ToInt32(ConfigurationManager.AppSettings["Seguranca:tentativasParaExibirCaptcha"]))
                            {
                                // retorna para a view que contem o captcha
                                return View("LoginCaptcha", new LoginCaptchaVM
                                {
                                    Usuario = model.Usuario,
                                    Senha = model.Senha
                                });
                            }
                        }
                        // se o usuario estiver com a senha expirada
                        else if (login.LoginMessage == LoginMessageId.SenhaExpirada)
                        {
                            // adiciona mensagem de alerta
                            this.AddFlashMessage("Senha expirada! Altere para continuar acessando.", FlashMessage.ALERT);

                            // adiciona o usuario em sessao
                            Session.Add("login.UsuarioId", login.Usuario.idUsuario);

                            // Adiciona Id de acesso na sessao
                            Session.Add("login.nrAcesso", nrAcesso);

                            // retorna para a view que contem o formulario de alteração da senha
                            return View("ChangePassword", new EditarSenhaVM
                            {
                                Usuario = model.Usuario,
                                Senha = model.Senha
                            });
                        }
                        // se o usuario estiver sem licenca disponivel
                        else if (login.LoginMessage == LoginMessageId.SemLicenca)
                        {
                            ModelState.AddModelError("Usuario", "Não existem licenças disponíveis, tente novamente mais tarde.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // adiciona mensagem de erro
                    this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); 

                    // redireciona para login
                    return RedirectToAction("Login");
                }
            }

            return View(model);
            //}
        }
        #endregion

        #region LoginCaptcha
        public ActionResult LoginCaptcha()
        {
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginCaptcha(LoginCaptchaVM model)
        {
            // se ModelState for valido
            if (ModelState.IsValid)
            {
                try
                {
                    // Cria numero para o acesso (ID do acesso)
                    string nrAcesso = Guid.NewGuid().ToString();

                    // executa metodo de consulta e validacao de usuario
                    LoginData login = DoLogin(model, nrAcesso);

                    // se o login for Sucesso
                    if (login.LoginMessage == LoginMessageId.Sucesso)
                    {
                        // faz login do usuario no sistema
                        CriarCookieUsuario(login.Usuario, nrAcesso, model.LembrarUsuario);

                        // registra o acesso como sucesso
                        AccessRegister(login.Usuario.dsLogin, true);

                        // remove usuario da sessao
                        Session.Remove("login.UsuarioId");

                        // remove Id de acesso na sessao
                        Session.Remove("login.nrAcesso");

                        // redireciona para tela inicial
                        return RedirectToAction("Index");
                    }
                    // se existir algum erro no login
                    else
                    {
                        // se o usuario ou a senha forem invalidos
                        if (login.LoginMessage == LoginMessageId.UsuarioInvalido || login.LoginMessage == LoginMessageId.SenhaInvalida)
                        {
                            // adiciona erro no modelstate
                            ModelState.AddModelError("Senha", "Usuário ou senha incorretos");
                        }
                        // se o usuario estiver inativo
                        else if (login.LoginMessage == LoginMessageId.UsuarioInativo)
                        {
                            ModelState.AddModelError("Usuario", "Usuário não está ativo no sistema");
                        }
                        // se o usuario estiver com a senha expirada
                        else if (login.LoginMessage == LoginMessageId.SenhaExpirada)
                        {
                            // adiciona mensagem de alerta
                            this.AddFlashMessage("Senha expirada! Altere para continuar acessando.", FlashMessage.ALERT);

                            // adiciona o usuario em sessao
                            Session.Add("login.UsuarioId", login.Usuario.idUsuario);

                            // Adiciona Id de acesso na sessao
                            Session.Add("login.nrAcesso", nrAcesso);

                            // retorna para a view que contem o formulario de alteração da senha
                            return View("ChangePassword", new EditarSenhaVM
                            {
                                Usuario = model.Usuario,
                                Senha = model.Senha
                            });
                        }
                        // se o usuario estiver sem licenca disponivel
                        else if (login.LoginMessage == LoginMessageId.SemLicenca)
                        {
                            ModelState.AddModelError("Usuario", "Não existem licenças disponíveis, tente novamente mais tarde.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // adiciona mensagem de erro
                    this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); 

                    // redireciona para login
                    return RedirectToAction("Login");
                }
            }

            // limpa campo codigo
            model.Codigo = string.Empty;

            // retorna view
            return View(model);
        }
        #endregion

        #region ChangePassword
        public ActionResult ChangePassword()
        {
            return RedirectToAction("Login");
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ResetSenha model)
        {
            try
            {
                using (Context db = new Context())
                {
                    // instancia bll do usuario
                    UsuarioBLL usuarioBLL = new UsuarioBLL(db, 0);

                    // consulta usuario pelo email
                    Usuario usuario = usuarioBLL.FindSingle(u => u.dsEmail == model.email);

                    // se o usuario do formulario for diferente do usuario da sessao
                    if (usuario == null)
                    {
                        ModelState.AddModelError("email", "Não foi possível determinar o email informado, informe um email valido");
                    }

                    // se modelstate for valido
                    if (ModelState.IsValid)
                    {

                        // reseta a senha
                        using (var transaction = new RP.DataAccess.RPTransactionScope(db))
                        {
                            usuarioBLL.ResetarSenha(usuario);
                            usuarioBLL.SaveChanges();
                            transaction.Complete();
                        }

                        // redireciona para index
                        this.AddFlashMessage("Verifique seu email!", FlashMessage.ALERT);
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                // adiciona mensagem de erro
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR);

                // redireciona para login
                return RedirectToAction("ForgotPassword");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(EditarSenhaVM model)
        {
            try
            {
                // obtem o id do usuario da sessao
                int idUsuario = Convert.ToInt32(Session["login.UsuarioId"]);

                using (Context db = new Context())
                {
                    // instancia bll do usuario
                    UsuarioBLL usuarioBLL = new UsuarioBLL(db, 0);

                    // consulta usuario pelo id
                    Usuario usuario = usuarioBLL.FindSingle(u => u.idUsuario == idUsuario);

                    // se o usuario do formulario for diferente do usuario da sessao
                    if (usuario.dsLogin != model.Usuario)
                    {
                        throw new Exception("O usuário não confere com o login! Certifique-se de estar alterando o seu usuário.");
                    }
                    //certifica-se de que o usuario informado tem a senha com data expirada
                    // garante que somente um usuário com senha expirada, poderá ser alterado
                    else if (usuario.dtValidade >= DateTime.Now.Date)
                    {
                        throw new Exception("O usuário informado não possui a senha expirada! Certifique-se de estar alterando o seu usuário.");
                    }
                    // se a senha do usuario for diferente da senha informada no formulario
                    else if (!(usuario.dsSenha == RP.Util.Class.Util.getHash(model.Senha)))
                    {
                        // adiciona mensagem no modelstate
                        ModelState.AddModelError("Senha", "Usuário ou senha incorretos");
                    }
                    // se a nova senha for igual a senha atual do usuario
                    else if (usuario.dsSenha == RP.Util.Class.Util.getHash(model.NovaSenha))
                    {
                        // adiciona mensagem no modelstate
                        ModelState.AddModelError("NovaSenha", "A nova senha deve ser diferente da senha atual");
                    }

                    // se modelstate for valido
                    if (ModelState.IsValid)
                    {
                        // seta nova senha criptografada para o usuario
                        usuario.dsSenha = RP.Util.Class.Util.getHash(model.NovaSenha);
                        usuario.dtValidade = DateTime.Now.Date.AddDays(Convert.ToInt32(ConfigurationManager.AppSettings["UsuarioValidadeSenha"]));
                        usuario.flAtivo = "Sim";

                        // altera o usuario
                        using (var transaction = new RP.DataAccess.RPTransactionScope(db))
                        {
                            BLL.UsuarioBLL bll = new BLL.UsuarioBLL(db, Helpers.Helper.UserId);
                            bll.Update(usuario);
                            bll.SaveChanges();
                            transaction.Complete();
                        }

                        // faz login do usuario no sistema
                        CriarCookieUsuario(usuario, Convert.ToString(Session["login.nrAcesso"]), false);

                        // registra o acesso como sucesso
                        AccessRegister(usuario.dsLogin, true);

                        // remove usuario da sessao
                        Session.Remove("login.UsuarioId");

                        // remove Id de acesso na sessao
                        Session.Remove("login.nrAcesso");

                        // adiciona mensagem de sucesso
                        this.AddFlashMessage(RP.Util.Resource.Message.PASSWORD_UPDATE, FlashMessage.SUCCESS);

                        // redireciona para index
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                // adiciona mensagem de erro
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR);

                // redireciona para login
                return RedirectToAction("Login");
            }

            return View(model);
        }
        #endregion

        public ActionResult Logout()
        {
            UserRepository.remove(Session.SessionID);
            FormsAuthentication.SignOut();
            Auth.Class.License.Logout();

            this.AddFlashMessage("Sua sessão foi finalizada!", FlashMessage.ALERT);
            return RedirectToAction("Index");
        }

        public ActionResult LoggedUsers()
        {
            return View(UserRepository.get);
        }

        #region Private
        private enum LoginMessageId
        {
            Sucesso,
            UsuarioInvalido,
            UsuarioInativo,
            SenhaExpirada,
            SenhaInvalida,
            SemLicenca
        }

        private class LoginData
        {
            public LoginMessageId LoginMessage { get; set; }
            public int TentativasFalhas { get; set; }
            public Usuario Usuario { get; set; }
            public bool ShowCaptcha { get; set; }
        }

        private LoginData DoLogin(LoginVM model, string nrAcesso)
        {
            LoginData data = new LoginData
            {
                LoginMessage = LoginMessageId.Sucesso,
                ShowCaptcha = false
            };

            using (Context db = new Context())
            {
                using (var transaction = new RP.DataAccess.RPTransactionScope(db))
                {

                    // instancia bll do usuario
                    UsuarioBLL usuarioBLL = new UsuarioBLL(db, 0);

                    // consulta usuario pelo login
                    Usuario usuario = usuarioBLL.FindSingle(u => u.dsLogin.ToLower().Equals(model.Usuario.ToLower()));

                    //if (data.ShowCaptcha)
                    //    return data;

                    // se o usuario nao existir
                    if (usuario == null)
                    {
                        data.LoginMessage = LoginMessageId.UsuarioInvalido;
                    }
                    // se o usuario existir
                    else
                    {
                        data.ShowCaptcha = usuario.nrFalhalogin >= Convert.ToInt32(ConfigurationManager.AppSettings["Seguranca:tentativasParaExibirCaptcha"]);
                        // se a senha informada estiver incorreta
                        if (!(usuario.dsSenha == RP.Util.Class.Util.getHash(model.Senha)))
                        {
                            // registra a tentiva falha de acesso
                            AccessRegister(model.Usuario, false);

                            // seta status do login
                            data.LoginMessage = LoginMessageId.SenhaInvalida;

                            // instancia bll de Log
                            //RP.Log.Model.BLL LogBLL = new Log.Model.BLL();

                            // altera a quantidade de falhas
                            usuario.nrFalhalogin = (usuario.nrFalhalogin ?? 0) + 1;
                            data.ShowCaptcha = usuario.nrFalhalogin >= Convert.ToInt32(ConfigurationManager.AppSettings["Seguranca:tentativasParaExibirCaptcha"]);

                            // armazena tentativas falhas de acesso
                            data.TentativasFalhas = usuario.nrFalhalogin ?? 0;

                            // armazena as tentativas falhas de acesso
                            if (data.TentativasFalhas >= Convert.ToInt32(ConfigurationManager.AppSettings["Seguranca:tentativasParaBloquearUsuario"]))
                            {
                                // bloqueia o usuario no banco
                                usuario.flAtivo = "Não";
                                usuarioBLL.Update(usuario);

                                // seta status do login
                                data.LoginMessage = LoginMessageId.UsuarioInativo;
                            }
                            else
                            {
                                usuarioBLL.UpdateLoginCount(usuario);
                            }

                        }
                        // se a senha estiver correta
                        else
                        {
                            // se usuario não estiver ativo
                            if (!(usuario.flAtivo.ToLower().Equals("sim")))
                            {
                                data.LoginMessage = LoginMessageId.UsuarioInativo;
                            }
                            // se usuario nao tiver licencas disponiveis
                            else if (!Auth.Class.License.UseLicense(usuario.dsLogin, nrAcesso))
                            {
                                data.LoginMessage = LoginMessageId.SemLicenca;
                            }
                            // se a senha do usuario estiver expirada
                            else if ((usuario.dtValidade ?? DateTime.Now.Date.AddDays(-1)) < DateTime.Now.Date)
                            {
                                data.LoginMessage = LoginMessageId.SenhaExpirada;
                            }
                            else
                            {
                                usuario.nrFalhalogin = 0;
                                usuarioBLL.UpdateLoginCount(usuario);
                            }

                            // armazena usuario
                            data.Usuario = usuario;
                        }

                        usuarioBLL.SaveChanges();
                        transaction.Complete();
                    }
                }
            }

            return data;
        }

        private void AccessRegister(string dsLogin, bool sucess)
        {
            //using (var db = new Log.Model.Context())
            //{
            //    using (var trans = new TransactionScope(TransactionScopeOption.Suppress))
            //    {
            //        var _access = new Log.Model.Entities.LogAcesso
            //        {
            //            dtAcesso = DateTime.Now,
            //            flSituacao = sucess ? "Sucesso" : "Falha",
            //            dsLogin = dsLogin,
            //            nrIP = Request.ServerVariables["REMOTE_ADDR"],
            //            nmBrowser = Request.Browser.Browser,
            //            dsBrowser = GetBrowserInfo(Request)
            //        };

            //        db.LogAcesso.Add(_access);
            //        db.SaveChanges();

            //        trans.Complete();
            //    }
            //}        
        }

        private string GetBrowserInfo(HttpRequestBase request)
        {
            var browser = request.Browser;
            string result = string.Empty;

            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Type", browser.Type);
            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Version", browser.Version);
            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Major Version", browser.MajorVersion);
            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Minor Version", browser.MinorVersion);
            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Platform", browser.Platform);
            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Is Beta", browser.Beta);
            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Is Crawler", browser.Crawler);
            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Supports Frames", browser.Frames);
            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Supports Table", browser.Tables);
            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Supports Cookies", browser.Cookies);
            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Supports VBScript", browser.VBScript);
            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Supports JavaScript", browser.EcmaScriptVersion.ToString());
            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Supports Java Applets", browser.JavaApplets);
            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Supports ActiveX Controls", browser.ActiveXControls);
            result += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Supports JavaScript Version", browser["JavaScriptVersion"]);


            return string.Format("<table>{0}</table>", result);
        }

        private void CriarCookieUsuario(Model.Entities.Usuario usuario, string nrAcesso, bool lembrar)
        {
            // cria objeto com os dados do usuario
            CustomPrincipalSerialized model = new CustomPrincipalSerialized();
            model.Id = usuario.idUsuario;
            model.Nome = usuario.nmUsuario;
            model.Login = usuario.dsLogin;
            model.ConnectionId = nrAcesso;

            // serializa objeto
            string userData = Newtonsoft.Json.JsonConvert.SerializeObject(model);

            // cria ticket
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, usuario.dsLogin, DateTime.Now, DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false, userData);

            // cria e adiciona cookie com valores criptografados
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket))
            {
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath,
                Domain = FormsAuthentication.CookieDomain
                //,Expires = authTicket.Expiration
            };
            Response.AppendCookie(cookie);


            // adiciona usuario no repositorio
            UserRepository.add(new UserInfo
            {
                dsLogin = usuario.dsLogin,
                nrSessionID = Session.SessionID,
                nmUsuario = usuario.nmUsuario,
                idUsuario = usuario.idUsuario
            });

            // se setado para lembrar usuario
            if (lembrar)
            {
                // cria cookie com o login do usuario
                HttpCookie infoCookie = new HttpCookie("RP_StoreDataLogin", usuario.dsLogin);
                infoCookie.Expires = DateTime.Now.AddMonths(1);
                Response.Cookies.Add(infoCookie);
            }
            // se nao, remove cookie
            else
            {
                // verifica se o cookie existe para remove-lo
                if (Request.Cookies["RP_StoreDataLogin"] != null)
                {
                    HttpCookie cookieLogin = new HttpCookie("RP_StoreDataLogin");
                    cookieLogin.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(cookieLogin);
                }
            }
        }
        #endregion
    }
}
