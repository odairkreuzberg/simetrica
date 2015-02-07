using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Util;

namespace RP.Sistema.Web.Controllers
{ 
    public class ModuloController : Controller
    {
        private int _idUsuario;
        public ModuloController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

		#region ActionResult
        //
        // GET: /Modulo/
        [PersistDataSearch("Search")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            return View();
        }
		
        //
        // GET: /Modulo/Search?filter=
        [PersistDataSearch]
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Search(string filter, int? page, int? pagesize)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.ModuloBLL moduloBLL = new BLL.ModuloBLL(db, _idUsuario);
                    var result = moduloBLL.Search(filter, page, pagesize);

                    return View("Index", result);
				}
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        [Auth.Class.Auth]
        public ActionResult Query(string nome, string descricao, int? page, int? pagesize, bool searching = false)
        {
            try
            {
                if (searching)
                {
                    using (Context db = new Context())
                    {
                        BLL.ModuloBLL moduloBLL = new BLL.ModuloBLL(db, _idUsuario);
                        var result = moduloBLL.Search(nome, descricao, page, pagesize);

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
        // GET: /Modulo/Details/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Details(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.ModuloBLL moduloBLL = new BLL.ModuloBLL(db, _idUsuario);
                    Modulo modulo = moduloBLL.FindSingle(e => e.idModulo == id);

                    return View(RP.Sistema.Web.Models.Modulo.ModuloVM.E2VM(modulo));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Modulo/Create
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Modulo/Create
        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(RP.Sistema.Web.Models.Modulo.ModuloVM viewData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var modulo = viewData.VM2E();
                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            BLL.ModuloBLL moduloBLL = new BLL.ModuloBLL(db, _idUsuario);
                            moduloBLL.Insert(modulo);
                            moduloBLL.SaveChanges();
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
        // GET: /Modulo/Edit/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.ModuloBLL moduloBLL = new BLL.ModuloBLL(db, _idUsuario);
                    Modulo modulo = moduloBLL.FindSingle(e => e.idModulo == id);

                    return View(RP.Sistema.Web.Models.Modulo.ModuloVM.E2VM(modulo));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Modulo/Edit/5
        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(RP.Sistema.Web.Models.Modulo.ModuloVM viewData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var modulo = viewData.VM2E();
                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
		                    BLL.ModuloBLL moduloBLL = new BLL.ModuloBLL(db, _idUsuario);
                            moduloBLL.Update(modulo);
                            moduloBLL.SaveChanges();
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
        // GET: /Modulo/Delete/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Delete(int id)
        {
            try
            {
                using (Context db = new Context())
                {
	                BLL.ModuloBLL moduloBLL = new BLL.ModuloBLL(db, _idUsuario);
                    Modulo modulo = moduloBLL.FindSingle(e => e.idModulo == id);

                    return View(RP.Sistema.Web.Models.Modulo.ModuloVM.E2VM(modulo));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Modulo/Delete/5
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
	                    BLL.ModuloBLL moduloBLL = new BLL.ModuloBLL(db, _idUsuario);
                        moduloBLL.Delete(e => e.idModulo == id);
                        moduloBLL.SaveChanges();
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
        // GET: /Modulo/Report?filter=
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
            BLL.ModuloBLL moduloBLL;
            
            try
            {
                using (var db = new Context())
                {
                    moduloBLL = new BLL.ModuloBLL(db, idUsuario);
                    var modulos = moduloBLL.Search(filter);

                    table.Columns.Add(new System.Data.DataColumn("idmodulo", System.Type.GetType("System.Int32")));
                    table.Columns.Add(new System.Data.DataColumn("dsmodulo", System.Type.GetType("System.String")));
                    table.Columns.Add(new System.Data.DataColumn("nrordem", System.Type.GetType("System.Int32")));
                    table.Columns.Add(new System.Data.DataColumn("nmmodulo", System.Type.GetType("System.String")));
                    table.Columns.Add(new System.Data.DataColumn("nmurl", System.Type.GetType("System.String")));
                    
                    foreach (Modulo modulo in modulos)
                    {
                        row = table.NewRow();
                        row["idmodulo"] = modulo.idModulo;
                        row["dsmodulo"] = modulo.dsModulo;
                        row["nrordem"] = modulo.nrOrdem;
                        row["nmmodulo"] = modulo.nmModulo;
                        row["nmurl"] = modulo.nmURL;
                        table.Rows.Add(row);
                    }

                    ds.Tables.Add(table);
                    listData.Add("subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db));
                    listData.Add("table", ds);
                }

                titulo = string.Format("<center>Relação de Módulos{0}</center>", !string.IsNullOrEmpty(filter) ? ("</br>Nome contendo: " + filter) : "");
                //return View();
                return RP.Report.Generic.Report(new RP.Report.Generic.GenericData
                {
                    exportTO = RP.Report.Generic.stringTOExportFormatType("PDF"),
                    fileRPT = "relModulo.rpt",
                    listData = listData,
                    parameters = new Dictionary<string, object> { { "titulo", titulo } },

                });
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

        [Auth.Class.Auth("sistema", "padrao", "addusuario")]
        public ActionResult AddUsuario(int id) 
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.ModuloBLL moduloBLL = new BLL.ModuloBLL(db, _idUsuario);
                    Modulo modulo = moduloBLL.FindSingle(e => e.idModulo == id, i => i.Usuarios.Select(s => s.Usuario));

                    return View(Models.Modulo.AdminVM.E2VM(modulo));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }        
        }

        [HttpPost]
        public ActionResult AddUsuario(int id, Models.Modulo.AdminVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Modulo.idModulo = id;
                    var modulo = model.VM2E();

                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            BLL.ModuloBLL moduloBLL = new BLL.ModuloBLL(db, _idUsuario);
                            moduloBLL.UpdateUsuarios(modulo);
                            moduloBLL.SaveChanges();
                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.EDIT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                return View(model);
            }
            catch (RP.Sistema.BLL.RPSistemaException exRP)
            {
                this.AddFlashMessage(exRP.Message, FlashMessage.ALERT);
                return View(model);
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }      
        }

		#endregion

		#region JsonResult
		//
        // GET: /Modulo/JsSearch?filter=
        [Auth.Class.Auth(true)]
        public JsonResult JsSearch(string filter, int? page, int? pagesize)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.ModuloBLL moduloBLL = new BLL.ModuloBLL(db, _idUsuario);
                    var result = moduloBLL.Search(filter, page, pagesize);
                    var list = result.Select(s => new 
                    { 
                        s.idModulo, 
                        s.nmModulo, 
                        s.dsModulo 
                    });

                    return Json(new Util.Class.JsonCollection{ result = list, count = result.TotalCount }, JsonRequestBehavior.AllowGet);
				}
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(RP.Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }

		//
        // GET: /Modulo/JsDetails/5
        [Auth.Class.Auth(true)]
        public JsonResult JsDetails(int idModulo)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.ModuloBLL moduloBLL = new BLL.ModuloBLL(db, _idUsuario);
                    Modulo modulo = moduloBLL.FindSingle(e => e.idModulo == idModulo);

                    if (modulo == null)
                    {
                        return Json(string.Empty, JsonRequestBehavior.AllowGet);
                    }

                    var result = new 
                    { 
                        modulo.idModulo, 
                        modulo.nmModulo, 
                        modulo.dsModulo 
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
		#endregion
    }
}

