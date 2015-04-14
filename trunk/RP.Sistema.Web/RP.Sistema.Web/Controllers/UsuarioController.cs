using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Sistema.Web.Models.Usuario;
using RP.Util;
using RP.Sistema.BLL;

namespace RP.Sistema.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private int _idUsuario;
        public UsuarioController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

        #region ActionResult
        //
        // GET: /Usuario/
        [PersistDataSearch("Search")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            LogBLL.Insert(new LogDado("Index", "Usuario", _idUsuario));
            return View();
        }

        //
        // GET: /Usuario/Search?filter=
        [PersistDataSearch]
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Search(string filter, int? page, int? pagesize)
        {
            try
            {
                LogBLL.Insert(new LogDado("Search", "Usuario", _idUsuario));
                using (Context db = new Context())
                {
                    BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                    var result = usuarioBLL.Search(filter, page, pagesize);

                    return View("Index", result);
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario, Url.Action("Index", "Usuario"));
                return RedirectToAction("Index", "Erro");
            }
        }

        [Auth.Class.Auth]
        public ActionResult Query(string nome, string email, string login, int? page, int? pagesize, bool searching = false)
        {
            try
            {
                if (searching)
                {
                    using (Context db = new Context())
                    {
                        BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                        var result = usuarioBLL.Search(nome, email, login, page, pagesize);

                        return View("Query", result);
                    }
                }
                return View("Query");
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Query");
            }
        }

        //
        // GET: /Usuario/Details/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Details(int id)
        {
            try
            {
                LogBLL.Insert(new LogDado("Details", "Usuario", _idUsuario));
                using (Context db = new Context())
                {
                    BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                    Usuario usuario = usuarioBLL.FindSingle(u => u.idUsuario == id);

                    if (usuario == null)
                    {
                        throw new Exception(string.Format(RP.Util.Resource.Message.RECORD_NOT_FOUND, id));
                    }

                    return View(Models.Usuario.UsuarioVM.E2VM(usuario));
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario, Url.Action("Index", "Usuario"));
                return RedirectToAction("Index", "Erro");
            }
        }

        //
        // GET: /Usuario/Create
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Usuario/Create
        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(UsuarioVM model, HttpPostedFileBase fuFoto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    LogBLL.Insert(new LogDado("Create", "Usuario", _idUsuario));
                    Usuario usuario = model.VM2E();
                    usuario.dtValidade = DateTime.Now.Date.AddDays(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["UsuarioValidadeSenha"]));

                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                            usuarioBLL.Insert(usuario);
                            usuarioBLL.SaveChanges();
                            trans.Complete();

                            if (fuFoto != null)
                            {
                                string path = System.Configuration.ConfigurationManager.AppSettings["PathFile"] + @"Fotos\Usuarios\";
                                usuarioBLL.SavePhoto(path, usuario.idUsuario, fuFoto);
                            }

                            this.AddFlashMessage(RP.Util.Resource.Message.INSERT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario, Url.Action("Index", "Usuario"));
                    return RedirectToAction("Index", "Erro");
                }

            }

            return View(model);
        }

        //
        // GET: /Usuario/Edit/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                    Usuario usuario = usuarioBLL.FindSingle(u => u.idUsuario == id);

                    if (usuario == null)
                    {
                        throw new Exception(string.Format(RP.Util.Resource.Message.RECORD_NOT_FOUND, id));
                    }

                    var model = Models.Usuario.UsuarioVM.E2VM(usuario);
                    string path = System.Configuration.ConfigurationManager.AppSettings["PathFile"] + @"Fotos\Usuarios\";
                    model.dsFoto = usuarioBLL.PathPhoto(path, model.IdUsuario, false);

                    return View(model);
                }

            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario, Url.Action("Index", "Usuario"));
                return RedirectToAction("Index", "Erro");
            }
        }

        //
        // POST: /Usuario/Edit/5
        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(UsuarioVM model, int id, HttpPostedFileBase fuFoto)
        {
            try
            {
                LogBLL.Insert(new LogDado("Edit", "Usuario", _idUsuario));
                if (ModelState.IsValid)
                {
                    var usuarioView = model.VM2E();
                    usuarioView.idUsuario = id;

                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                            var _usuarioDB = usuarioBLL.FindSingle(u => u.idUsuario == id);

                            _usuarioDB.nmUsuario = usuarioView.nmUsuario;
                            _usuarioDB.dsEmail = usuarioView.dsEmail;

                            usuarioBLL.Update(_usuarioDB);
                            usuarioBLL.SaveChanges();
                            trans.Complete();

                            string path = System.Configuration.ConfigurationManager.AppSettings["PathFile"] + @"Fotos\Usuarios\";
                            if (fuFoto != null)
                            {
                                usuarioBLL.SavePhoto(path, usuarioView.idUsuario, fuFoto);
                            }
                            else if (this.HttpContext.Request.Params.AllKeys.Contains("fuFoto"))
                            {
                                if (string.IsNullOrEmpty(this.HttpContext.Request.Params["fuFoto"]))
                                {
                                    usuarioBLL.RemovePhoto(path, usuarioView.idUsuario);
                                }
                            }

                            this.AddFlashMessage(RP.Util.Resource.Message.EDIT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario, Url.Action("Index", "Usuario"));
                return RedirectToAction("Index", "Erro");
            }
        }

        #region AlterPassword


        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Perfil()
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                    Usuario usuario = usuarioBLL.FindSingle(u => u.idUsuario == _idUsuario);

                    if (usuario == null)
                    {
                        throw new Exception(string.Format(RP.Util.Resource.Message.RECORD_NOT_FOUND, _idUsuario));
                    }

                    string path = System.Configuration.ConfigurationManager.AppSettings["PathFile"] + @"Fotos\Usuarios\";

                    var model = new Models.Usuario.AlterarPerfilVM();
                    model.Usuario = Models.Usuario.UsuarioVM.E2VM(usuario);
                    model.Usuario.dsFoto = usuarioBLL.PathPhoto(path, model.Usuario.IdUsuario, false);

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro");
            }
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Perfil(AlterarPerfilVM model, HttpPostedFileBase fuFoto)
        {
            bool AlterarSenha = (!string.IsNullOrEmpty(model.SenhaAtual) && !string.IsNullOrEmpty(model.NovaSenha));

            try
            {
                LogBLL.Insert(new LogDado("Perfil", "Usuario", _idUsuario));
                using (Context db = new Context())
                {
                    Usuario usuarioLogado;

                    BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);

                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        usuarioLogado = usuarioBLL.FindSingle(u => u.idUsuario == _idUsuario);

                        if (usuarioLogado == null)
                        {
                            throw new Exception(string.Format(RP.Util.Resource.Message.RECORD_NOT_FOUND, _idUsuario));
                        }

                        if (AlterarSenha && (usuarioLogado.dsSenha != RP.Util.Class.Util.getHash(model.SenhaAtual)))
                        {
                            ModelState.AddModelError("SenhaAtual", "A senha atual está incorreta.");
                        }

                        if (ModelState.IsValid)
                        {
                            if (AlterarSenha)
                            {
                                usuarioLogado.dsSenha = RP.Util.Class.Util.getHash(model.NovaSenha);
                                usuarioLogado.dtValidade = DateTime.Now.Date.AddDays(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["UsuarioValidadeSenha"]));

                                usuarioBLL.Update(usuarioLogado);
                                usuarioBLL.SaveChanges();
                                trans.Complete();
                            }

                            string path = System.Configuration.ConfigurationManager.AppSettings["PathFile"] + @"Fotos\Usuarios\";
                            if (fuFoto != null)
                            {
                                usuarioBLL.SavePhoto(path, usuarioLogado.idUsuario, fuFoto);
                            }
                            else if (this.HttpContext.Request.Params.AllKeys.Contains("fuFoto"))
                            {
                                if (string.IsNullOrEmpty(this.HttpContext.Request.Params["fuFoto"]))
                                {
                                    usuarioBLL.RemovePhoto(path, usuarioLogado.idUsuario);
                                }
                            }

                            this.AddFlashMessage("Perfil atualizado com sucesso!", FlashMessage.SUCCESS);
                            return RedirectToAction("Perfil");
                        }

                        return View(model);
                    }
                }

            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario, Url.Action("Index", "Usuario"));
                return RedirectToAction("Index", "Erro");
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult AlterPasswordMaster(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                    Usuario usuario = usuarioBLL.FindSingle(u => u.idUsuario == id);

                    if (usuario == null)
                    {
                        throw new Exception(string.Format(RP.Util.Resource.Message.RECORD_NOT_FOUND, id));
                    }

                    return View(UsuarioVM.E2VM(usuario));
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario, Url.Action("Index", "Usuario"));
                return RedirectToAction("Index", "Erro");
            }
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult AlterPasswordMaster(int id, UsuarioVM model)
        {
            try
            {
                LogBLL.Insert(new LogDado("AlterPasswordMaster", "Usuario", _idUsuario));
                if (ModelState.IsValid)
                {
                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                            Usuario usuario = usuarioBLL.FindSingle(u => u.idUsuario == id);

                            if (usuario == null)
                            {
                                throw new Exception(string.Format(RP.Util.Resource.Message.RECORD_NOT_FOUND, id));
                            }

                            usuarioBLL.ResetarSenha(usuario);
                            usuarioBLL.SaveChanges();
                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.EDIT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario, Url.Action("Index", "Usuario"));
                return RedirectToAction("Index", "Erro");
            }
        }
        #endregion


        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Bloquear(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                    Usuario usuario = usuarioBLL.FindSingle(u => u.idUsuario == id);

                    if (usuario == null)
                    {
                        throw new Exception(string.Format(RP.Util.Resource.Message.RECORD_NOT_FOUND, id));
                    }

                    if (id == _idUsuario)
                    {
                        ViewBag.ocultarComandos = true;

                        this.AddFlashMessage(new FlashMessage.Message
                        {
                            closeable = false,
                            textMessage = "Você não pode bloquear seu próprio usuário!",
                            type = FlashMessage.ERROR
                        });
                    }

                    return View(UsuarioVM.E2VM(usuario));
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario, Url.Action("Index", "Usuario"));
                return RedirectToAction("Index", "Erro");
            }
        }

        [HttpPost, ActionName("Bloquear")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult BloquearConfirmed(int id)
        {
            try
            {
                LogBLL.Insert(new LogDado("BloquearConfirmed", "Usuario", _idUsuario));
                if (id == _idUsuario)
                {
                    throw new Exception("Você não pode bloquear seu próprio usuário!");
                }

                using (Context db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                        Usuario usuario = usuarioBLL.FindSingle(u => u.idUsuario == id);

                        if (usuario == null)
                        {
                            throw new Exception(string.Format(RP.Util.Resource.Message.RECORD_NOT_FOUND, id));
                        }
                        else
                        {
                            usuario.flAtivo = "Não";
                            Auth.Class.AuthModel.Remove(usuario.dsLogin);

                            usuarioBLL.Update(usuario);

                            usuarioBLL.SaveChanges();
                            trans.Complete();
                        }

                        this.AddFlashMessage("Usuário bloqueado", FlashMessage.SUCCESS);
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Bloquear", new { id });
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Desbloquear(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                    Usuario usuario = usuarioBLL.FindSingle(u => u.idUsuario == id);

                    if (usuario == null)
                    {
                        throw new Exception(string.Format(RP.Util.Resource.Message.RECORD_NOT_FOUND, id));
                    }

                    if (usuario.flAtivo == "Sim")
                    {
                        this.AddFlashMessage("Usuário já esta desbloqueado", FlashMessage.ALERT);
                        return RedirectToAction("Index", "Usuario");
                    }

                    return View(UsuarioVM.E2VM(usuario));
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario, Url.Action("Index", "Usuario"));
                return RedirectToAction("Index", "Erro");
            }
        }

        [HttpPost, ActionName("Desbloquear")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult DesbloquearConfirmed(int id)
        {
            try
            {
                LogBLL.Insert(new LogDado("DesbloquearConfirmed", "Usuario", _idUsuario));
                using (Context db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                        Usuario usuario = usuarioBLL.FindSingle(u => u.idUsuario == id);

                        if (usuario == null)
                        {
                            throw new Exception(string.Format(RP.Util.Resource.Message.RECORD_NOT_FOUND, id));
                        }
                        else
                        {
                            usuario.flAtivo = "Sim";
                            usuario.nrFalhalogin = 0;
                            //usuario.dtValidade = DateTime.Now.AddDays(5);

                            usuarioBLL.Update(usuario);

                            usuarioBLL.SaveChanges();
                            trans.Complete();
                        }

                        this.AddFlashMessage("Usuário desbloqueado", FlashMessage.SUCCESS);
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Desbloquear", new { id });
            }
        }

        //
        // GET: /Usuario/Delete/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Delete(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                    Usuario usuario = usuarioBLL.FindSingle(u => u.idUsuario == id);

                    if (usuario == null)
                    {
                        throw new Exception(string.Format(RP.Util.Resource.Message.RECORD_NOT_FOUND, id));
                    }

                    return View(Models.Usuario.UsuarioVM.E2VM(usuario));
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario, Url.Action("Index", "Usuario"));
                return RedirectToAction("Index", "Erro");
            }
        }

        //
        // POST: /Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                LogBLL.Insert(new LogDado("DeleteConfirmed", "Usuario", _idUsuario));
                using (Context db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                        usuarioBLL.Delete(id);
                        usuarioBLL.SaveChanges();
                        trans.Complete();

                        this.AddFlashMessage(RP.Util.Resource.Message.DELETE_SUCCESS, FlashMessage.SUCCESS);
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Delete", new { id });
            }
        }

        //
        // GET: /Usuario/Report?filter=
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Report(string filter)
        {
            //if (report.Acao == RP.Report.TipoAcao.Agendar)
            //{
            //    return RP.Report.Generic.Json(report);
            //}

            int idUsuario = _idUsuario > 0 ? _idUsuario : Convert.ToInt32(Request["idUsuario"]);
            Dictionary<string, System.Data.DataSet> listData = new Dictionary<string, System.Data.DataSet>();
            System.Data.DataSet ds = new System.Data.DataSet();
            System.Data.DataTable table = new System.Data.DataTable("table");
            System.Data.DataRow row;
            string titulo;
            BLL.UsuarioBLL usuarioBLL;

            try
            {
                using (var db = new Context())
                {
                    usuarioBLL = new BLL.UsuarioBLL(db, idUsuario);
                    var usuarios = usuarioBLL.Search(filter);

                    table.Columns.Add(new System.Data.DataColumn("idusuario", System.Type.GetType("System.Int32")));
                    table.Columns.Add(new System.Data.DataColumn("dsemail", System.Type.GetType("System.String")));
                    table.Columns.Add(new System.Data.DataColumn("dtvalidade", System.Type.GetType("System.String")));
                    table.Columns.Add(new System.Data.DataColumn("flativo", System.Type.GetType("System.String")));
                    table.Columns.Add(new System.Data.DataColumn("dslogin", System.Type.GetType("System.String")));
                    table.Columns.Add(new System.Data.DataColumn("nmusuario", System.Type.GetType("System.String")));

                    foreach (Usuario usuario in usuarios)
                    {
                        row = table.NewRow();
                        row["idusuario"] = usuario.idUsuario;
                        row["dsemail"] = usuario.dsEmail;
                        row["dtvalidade"] = usuario.dtValidade;
                        row["flativo"] = usuario.flAtivo;
                        row["dslogin"] = usuario.dsLogin;
                        row["nmusuario"] = usuario.nmUsuario;
                        table.Rows.Add(row);
                    }

                    ds.Tables.Add(table);
                    listData.Add("subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db));
                    listData.Add("table", ds);
                }

                titulo = string.Format("<center>Relação de Usuários{0}</center>", !string.IsNullOrEmpty(filter) ? ("</br>Nome contendo: " + filter) : "");

                return View();
                //return RP.Report.Generic.Report(new RP.Report.Generic.GenericData
                //{
                //    exportTO = RP.Report.Generic.stringTOExportFormatType("PDF"),
                //    fileRPT = "relModulo.rpt",
                //    listData = listData,
                //    parameters = new Dictionary<string, object> { { "titulo", titulo } },

                //});
            }
            catch (RP.Report.Exception rex)
            {
                RP.Util.Entity.ErroLog.Add(rex, Session.SessionID, idUsuario);
                return RedirectToAction("Index", "Erro");

            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, idUsuario);
                return RedirectToAction("Index", "Erro");
            }
            finally
            {
                ds.Dispose();
            }
        }

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult AddPerfil(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                    Usuario usuario = usuarioBLL.FindSingle(u => u.idUsuario == id, i => i.Perfis.Select(s => s.Perfil));

                    if (usuario == null)
                    {
                        throw new Exception(string.Format(RP.Util.Resource.Message.RECORD_NOT_FOUND, id.ToString()));
                    }

                    return View(Models.Usuario.UsuarioVM.E2VM(usuario));
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario, Url.Action("Index", "Usuario"));
                return RedirectToAction("Index", "Erro");
            }
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult AddPerfil(int id, UsuarioVM model)
        {
            try
            {
                LogBLL.Insert(new LogDado("AddPerfil", "Usuario", _idUsuario));
                if (ModelState.IsValid)
                {
                    Usuario usuario = model.VM2E();
                    usuario.idUsuario = id;

                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                            usuarioBLL.UpdatePerfis(usuario);
                            usuarioBLL.SaveChanges();
                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.EDIT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario, Url.Action("Index", "Usuario"));
                return RedirectToAction("Index", "Erro");
            }
        }
        #endregion

        #region JsonResult
        //
        // GET: /Usuario/JsSearch?filter=
        [Auth.Class.Auth(true)]
        public JsonResult JsSearch(string filter, int? page, int? pagesize)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                    var result = usuarioBLL.Search(filter, page, pagesize);
                    var list = result.Select(s => new
                    {
                        s.idUsuario,
                        s.nmUsuario,
                        s.dsLogin
                    });

                    return Json(new Util.Class.JsonCollection { result = list, count = result.TotalCount }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(RP.Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /Usuario/JsDetails/5
        [Auth.Class.Auth(true)]
        public JsonResult JsDetails(int idUsuario)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                    Usuario usuario = usuarioBLL.FindSingle(u => u.idUsuario == idUsuario);

                    if (usuario == null)
                    {
                        return Json(string.Empty, JsonRequestBehavior.AllowGet);
                    }

                    var result = new
                    {
                        usuario.idUsuario,
                        usuario.nmUsuario,
                        usuario.dsLogin
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(RP.Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }

        [Auth.Class.Auth(true)]
        public JsonResult JsAdicionarAtalho(string nome, string icone, string acao)
        {
            Usuario.Preferencias preferencias = null;

            try
            {
                LogBLL.Insert(new LogDado("JsAdicionarAtalho", "Usuario", _idUsuario));
                using (Context db = new Context())
                {
                    using (var transaction = new RP.DataAccess.RPTransactionScope(db))
                    {

                        BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                        preferencias = usuarioBLL.GetPreferencias(_idUsuario);

                        preferencias.Atalhos.Add(new Usuario.Preferencias.Atalho
                        {
                            Nome = nome,
                            Icone = icone,
                            Href = acao
                        });

                        usuarioBLL.SetPreferencias(_idUsuario, preferencias);

                        usuarioBLL.SaveChanges();
                        transaction.Complete();

                    }

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(RP.Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }

        [Auth.Class.Auth(true)]
        public JsonResult JsRemoverAtalho(string nome, string icone, string acao)
        {
            Usuario.Preferencias preferencias = null;

            try
            {
                using (Context db = new Context())
                {
                    using (var transaction = new RP.DataAccess.RPTransactionScope(db))
                    {

                        BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL(db, _idUsuario);
                        preferencias = usuarioBLL.GetPreferencias(_idUsuario);

                        preferencias.Atalhos.Remove(preferencias.Atalhos.Find(e => e.Nome == nome && e.Icone == icone && e.Href == acao));

                        usuarioBLL.SetPreferencias(_idUsuario, preferencias);

                        usuarioBLL.SaveChanges();
                        transaction.Complete();

                    }

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(RP.Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }

        [Auth.Class.Auth(true)]
        public JsonResult JsCreate(Usuario model)
        {
            try
            {
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.UsuarioBLL(db, _idUsuario);
                        model.flAtivo = "Sim";
                        _bll.Insert(model);
                        _bll.SaveChanges();

                        trans.Complete();

                        return Json(new { idUsuario = model.idUsuario, nmUsuario = model.nmUsuario }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(RP.Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}

