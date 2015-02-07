using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Util;
using RP.Util.Class;

namespace RP.Sistema.Web.Controllers
{ 
    public class AreaController : Controller
    {
        private int _idUsuario;
        public AreaController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }


		#region ActionResult
        //
        // GET: /Area/
        //[Auth.Class.Auth("sistema", "padrao")]
        [PersistDataSearch("Search")]
        public ActionResult Index()
        {
            return View();
        }
		
        //
        // GET: /Area/Search?filter=
        //[Auth.Class.Auth("sistema", "padrao", "index")]
        [PersistDataSearch]
        public ActionResult Search(string filter, int? page, int? pagesize)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.AreaBLL areaBLL = new BLL.AreaBLL(db, _idUsuario);
                    var result = areaBLL.Search(filter, page, pagesize);

                    return View("Index", result);
				}
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        [Auth.Class.Auth]
        public ActionResult Query(string area, string descricao, string modulo, int? page, int? pagesize, bool searching = false)
        {
            try
            {
                if (searching)
                {
                    using (Context db = new Context())
                    {
                        BLL.AreaBLL areaBLL = new BLL.AreaBLL(db, _idUsuario);
                        var result = areaBLL.Search(area, descricao, modulo, page, pagesize);

                        return View("Query", result);
                    }
                }
                return View("Query");
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Query");
            }
        }

        //
        // GET: /Area/Details/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Details(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.AreaBLL areaBLL = new BLL.AreaBLL(db, _idUsuario);
                    Area area = areaBLL.FindSingle(e => e.idArea == id, i => i.Modulo);

                    return View(RP.Sistema.Web.Models.Area.AreaVM.E2VM(area));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Area/Create
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Area/Create
        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(RP.Sistema.Web.Models.Area.AreaVM viewData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var area = viewData.VM2E();
                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            area.flUsaURL = area.flUsaURL.ToLower() == "true" || area.flUsaURL.ToLower() == "sim" ? "Sim" : "Não";
                            area.idModulo = area.idModulo;

                            BLL.AreaBLL areaBLL = new BLL.AreaBLL(db, _idUsuario);
                            areaBLL.Insert(area);
                            areaBLL.SaveChanges();
                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.INSERT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                    return RedirectToAction("Index");
                }

            }

            return View(viewData);
        }
        
        //
        // GET: /Area/Edit/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.AreaBLL areaBLL = new BLL.AreaBLL(db, _idUsuario);
                    Area area = areaBLL.FindSingle(e => e.idArea == id, i => i.Modulo);

                    return View(RP.Sistema.Web.Models.Area.AreaVM.E2VM(area));
                }

            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Area/Edit/5
        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(RP.Sistema.Web.Models.Area.AreaVM viewData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var area = viewData.VM2E();
                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            area.flUsaURL = area.flUsaURL.ToLower() == "true" || area.flUsaURL.ToLower() == "sim" ? "Sim" : "Não";
                            area.idModulo = area.idModulo;

		                    BLL.AreaBLL areaBLL = new BLL.AreaBLL(db, _idUsuario);
                            areaBLL.Update(area);
                            areaBLL.SaveChanges();
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
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }        
		}

        //
        // GET: /Area/Delete/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Delete(int id)
        {
            try
            {
                using (Context db = new Context())
                {
	                BLL.AreaBLL areaBLL = new BLL.AreaBLL(db, _idUsuario);
                    Area area = areaBLL.FindSingle(e => e.idArea == id, i => i.Modulo);

                    return View(RP.Sistema.Web.Models.Area.AreaVM.E2VM(area));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Area/Delete/5
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
	                    BLL.AreaBLL areaBLL = new BLL.AreaBLL(db, _idUsuario);
                        areaBLL.Delete(e => e.idArea == id);
                        areaBLL.SaveChanges();
                        trans.Complete();

                        this.AddFlashMessage(RP.Util.Resource.Message.DELETE_SUCCESS, FlashMessage.SUCCESS);
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Delete", id);
            }
        }

		//
        // GET: /Area/Report?filter=
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
            BLL.AreaBLL areaBLL;
            
            try
            {
                using (var db = new Context())
                {
                    areaBLL = new BLL.AreaBLL(db, idUsuario);
                    var areas = areaBLL.Search(filter);

                    table.Columns.Add(new System.Data.DataColumn("idarea", System.Type.GetType("System.Int32")));
                    table.Columns.Add(new System.Data.DataColumn("nmarea", System.Type.GetType("System.String")));
                    table.Columns.Add(new System.Data.DataColumn("dsarea", System.Type.GetType("System.String")));
                    table.Columns.Add(new System.Data.DataColumn("flusaurl", System.Type.GetType("System.String")));
                    
                    foreach (Area area in areas)
                    {
                        row = table.NewRow();
                        row["idarea"] = area.idArea;
                        row["nmarea"] = area.nmArea;
                        row["dsarea"] = area.dsArea;
                        row["flusaurl"] = area.flUsaURL;
                        table.Rows.Add(row);
                    }

                    ds.Tables.Add(table);
                    listData.Add("subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db));
                    listData.Add("table", ds);
                }

                titulo = string.Format("<center>Relação de Áreas{0}</center>", !string.IsNullOrEmpty(filter) ? ("</br>Nome contendo: " + filter) : "");

                //return View();
                return RP.Report.Generic.Report(new RP.Report.Generic.GenericData
                {
                    exportTO = RP.Report.Generic.stringTOExportFormatType("PDF"),
                    fileRPT = "relArea.rpt",
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
		#endregion

		#region JsonResult
		//
        // GET: /Area/JsSearch?filter=
        [Auth.Class.Auth(true)]
        public JsonResult JsSearch(string filter, int? page, int? pagesize)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.AreaBLL areaBLL = new BLL.AreaBLL(db, _idUsuario);
                    var result = areaBLL.Search(filter, page, pagesize);
                    var list = result.Select(s => new { 
                        s.idArea,
                        s.nmArea,
                        s.dsArea
                    });

                    return Json(new JsonCollection{ result = list, count = result.TotalCount }, JsonRequestBehavior.AllowGet);
				}
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }

		//
        // GET: /Area/JsDetails/5
        [Auth.Class.Auth(true)]
        public JsonResult JsDetails(int idArea)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.AreaBLL areaBLL = new BLL.AreaBLL(db, _idUsuario);
                    Area area = areaBLL.FindSingle(e => e.idArea == idArea, i => i.Modulo);

                    if (area == null)
                    {
                        return Json(string.Empty, JsonRequestBehavior.AllowGet);
                    }

                    var result = new
                    {
                        area.idArea,
                        area.nmArea,
                        area.dsArea
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

