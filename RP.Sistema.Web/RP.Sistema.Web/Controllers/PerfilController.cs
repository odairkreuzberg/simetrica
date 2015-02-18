using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Sistema.Web.Models.Perfil;
using RP.Util;
using System.Data;
using System.Collections;

namespace RP.Sistema.Web.Controllers
{
    public class PerfilController : Controller
    {
        private int _idUsuario;
        public PerfilController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

        #region ActionResult
        //
        // GET: /Perfil/
        [PersistDataSearch("Search")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Perfil/Search?filter=
        [PersistDataSearch]
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Search(string filter, int? page, int? pagesize)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.PerfilBLL perfilBLL = new BLL.PerfilBLL(db, _idUsuario);
                    var result = perfilBLL.Search(filter, page, pagesize);

                    return View("Index", result);
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Perfil/Details/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Details(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.PerfilBLL perfilBLL = new BLL.PerfilBLL(db, _idUsuario);
                    Perfil perfil = perfilBLL.FindSingle(u => u.idPerfil == id);

                    return View(PerfilVM.E2VM(perfil));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Perfil/Create
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Perfil/Create
        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(PerfilVM viewData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Perfil perfil = viewData.VM2E();
                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            BLL.PerfilBLL perfilBLL = new BLL.PerfilBLL(db, _idUsuario);
                            perfilBLL.Insert(perfil);
                            perfilBLL.SaveChanges();
                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.INSERT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                    return RedirectToAction("Index");
                }

            }

            return View(viewData);
        }

        //
        // GET: /Perfil/Edit/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.PerfilBLL perfilBLL = new BLL.PerfilBLL(db, _idUsuario);
                    Perfil perfil = perfilBLL.FindSingle(u => u.idPerfil == id);

                    return View(PerfilVM.E2VM(perfil));
                }

            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Perfil/Edit/5
        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(PerfilVM viewData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Perfil perfil = viewData.VM2E();
                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            BLL.PerfilBLL perfilBLL = new BLL.PerfilBLL(db, _idUsuario);
                            perfilBLL.Update(perfil);
                            perfilBLL.SaveChanges();
                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.EDIT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                return View(viewData);
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Perfil/Delete/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Delete(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.PerfilBLL perfilBLL = new BLL.PerfilBLL(db, _idUsuario);
                    Perfil perfil = perfilBLL.FindSingle(u => u.idPerfil == id);

                    return View(PerfilVM.E2VM(perfil));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Perfil/Delete/5
        [HttpPost, ActionName("Delete")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        BLL.PerfilBLL perfilBLL = new BLL.PerfilBLL(db, _idUsuario);
                        perfilBLL.Delete(u => u.idPerfil == id);
                        perfilBLL.SaveChanges();
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
                return RedirectToAction("Delete", id);
            }
        }

        //
        // GET: /Perfil/Report?filter=
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

            RP.Sistema.BLL.PerfilBLL perfilBLL;
            string titulo;

            try
            {
                using (var db = new Context())
                {
                    perfilBLL = new BLL.PerfilBLL(db, idUsuario);

                    var perfis = perfilBLL.Search(filter);

                    table.Columns.Add(new System.Data.DataColumn("idperfil", System.Type.GetType("System.Int32")));
                    table.Columns.Add(new System.Data.DataColumn("nmperfil", System.Type.GetType("System.String")));

                    foreach (Perfil perfil in perfis)
                    {
                        row = table.NewRow();
                        row["idperfil"] = perfil.idPerfil;
                        row["nmperfil"] = perfil.nmPerfil;
                        table.Rows.Add(row);
                    }

                    ds.Tables.Add(table);
                    listData.Add("subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db));
                    listData.Add("table", ds);
                }

                titulo = string.Format("<center>Relação de Perfis{0}</center>", !string.IsNullOrEmpty(filter) ? ("</br>Nome contendo: " + filter) : "");

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
        public ActionResult AddAcao(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.PerfilBLL perfilBLL = new BLL.PerfilBLL(db, _idUsuario);

                    Perfil perfil = perfilBLL.FindSingle(u => u.idPerfil == id,
                        i => i.Acoes,
                        i => i.Acoes.Select(s => s.Acao),
                        i => i.Acoes.Select(s => s.Acao.Controle)
                    );

                    return View(PerfilVM.E2VM(perfil));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult AddAcao(int id, RP.Sistema.Web.Models.Perfil.PerfilVM viewData)
        {
            try
            {
                viewData.idPerfil = id;
                var perfil = viewData.VM2E();

                using (Context db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        BLL.PerfilBLL perfilBLL = new BLL.PerfilBLL(db, _idUsuario);
                        perfilBLL.UpdateAcoes(perfil);
                        perfilBLL.SaveChanges();
                        trans.Complete();

                        this.AddFlashMessage(RP.Util.Resource.Message.EDIT_SUCCESS, FlashMessage.SUCCESS);
                        return RedirectToAction("Index");
                    }
                }

            }
            catch (RP.Sistema.BLL.RPSistemaException exRP)
            {
                this.AddFlashMessage(exRP.Message, FlashMessage.ALERT);
                return View(viewData);
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(Util.Exception.Message.Get(ex), FlashMessage.ERROR);
                return RedirectToAction("Index");
            }
        }

        [Auth.Class.Auth]
        public ActionResult Query(string nmPerfil, int? page, int? pagesize, bool searching = false)
        {
            try
            {
                if (searching)
                {
                    using (Context db = new Context())
                    {
                        var _bll = new BLL.PerfilBLL(db, _idUsuario);
                        var result = _bll.Search(nmPerfil, page, pagesize);

                        return View("Query", result);
                    }
                }
                return View("Query");
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(Util.Exception.Message.Get(ex), FlashMessage.ERROR);
                return RedirectToAction("Query");
            }
        }

        //   [Auth.Class.Auth("saude", "psf")]
        public ActionResult ReportPerfil()
        {
            return View(new ReportPerfilVM());
        }

        [HttpPost]
        //   [Auth.Class.Auth("saude", "vacina")]
        public ActionResult ReportPerfil(ReportPerfilVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new Context())
                    {
                        var reportPerfil = new Report.Perfil();
                        return View();
                    }
                }

                return View(model);
            }

            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }
        #endregion

        #region JsonResult
        //
        // GET: /Perfil/JsSearch?filter=
        [Auth.Class.Auth(true)]
        public JsonResult JsSearch(string filter, int? page, int? pagesize)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.PerfilBLL perfilBLL = new BLL.PerfilBLL(db, _idUsuario);
                    var result = perfilBLL.Search(filter, page, pagesize);
                    var list = result.Select(s => new
                    {
                        s.idPerfil,
                        s.nmPerfil
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

        [Auth.Class.Auth(true)]
        public JsonResult JsSearchPerfis(int idUsuario)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.PerfilUsuarioBLL bll = new BLL.PerfilUsuarioBLL(db, _idUsuario);
                    var result = bll.Find(u => u.idUsuario == idUsuario, i => i.Perfil).ToList();

                    var list = result.Select(s => new
                    {
                        s.idPerfil,
                        s.Perfil.nmPerfil
                    });

                    return Json(new Util.Class.JsonCollection { result = list, count = result.Count }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(RP.Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /Perfil/JsDetails/5
        [Auth.Class.Auth(true)]
        public JsonResult JsDetails(int idPerfil)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.PerfilBLL perfilBLL = new BLL.PerfilBLL(db, _idUsuario);
                    Perfil perfil = perfilBLL.FindSingle(u => u.idPerfil == idPerfil);

                    if (perfil == null)
                    {
                        return Json(string.Empty, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { perfil.idPerfil, perfil.nmPerfil }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(RP.Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /Perfil/JsDetails/5
        [Auth.Class.Auth(true)]
        public JsonResult JsListarAcoes(int idControle)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.AcaoBLL bll = new BLL.AcaoBLL(db, _idUsuario);

                    var result =
                        bll.Find(e =>
                            e.idControle == idControle
                        ).OrderByDescending(o => o.idAcao);

                    var list = result.Select(s => new
                    {
                        idAcao = s.idAcao,
                        dsAcao = s.dsAcao,
                        nmAcao = s.nmAcao,
                        s.Controle.nmControle,
                        nmMenu = s.nmMenu
                    }).ToList();

                    return Json(new Util.Class.JsonCollection { result = list, count = result.Count() }, JsonRequestBehavior.AllowGet);
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

