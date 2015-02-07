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
    public class MenuController : Controller
    {
        private int _idUsuario;
        public MenuController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

		#region ActionResult
        //
        // GET: /Menu/
        [PersistDataSearch("Search")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            return View();
        }
		
        //
        // GET: /Menu/Search?filter=
        [PersistDataSearch]
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Search(string filter, int? page, int? pagesize)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.MenuBLL menuBLL = new BLL.MenuBLL(db, _idUsuario);
                    var result = menuBLL.Search(filter, page, pagesize);

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
        public ActionResult Query(string nome, int? page, int? pagesize, bool searching = false)
        {
            try
            {
                if (searching)
                {
                    using (Context db = new Context())
                    {
                        BLL.MenuBLL menuBLL = new BLL.MenuBLL(db, _idUsuario);
                        var result = menuBLL.Search(nome, page, pagesize);

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
        // GET: /Menu/Details/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Details(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.MenuBLL menuBLL = new BLL.MenuBLL(db, _idUsuario);
                    Menu menu = menuBLL.FindSingle(e => e.idMenu == id);

                    return View(RP.Sistema.Web.Models.Menu.MenuVM.E2VM(menu));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Menu/Create
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Menu/Create
        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(RP.Sistema.Web.Models.Menu.MenuVM viewData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Menu menu = viewData.VM2E();
                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            BLL.MenuBLL menuBLL = new BLL.MenuBLL(db, _idUsuario);
                            menuBLL.Insert(menu);
                            menuBLL.SaveChanges();
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
        // GET: /Menu/Edit/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.MenuBLL menuBLL = new BLL.MenuBLL(db, _idUsuario);
                    Menu menu = menuBLL.FindSingle(e => e.idMenu == id);

                    return View(RP.Sistema.Web.Models.Menu.MenuVM.E2VM(menu));
                }

            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Menu/Edit/5
        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(RP.Sistema.Web.Models.Menu.MenuVM viewData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Menu menu = viewData.VM2E();
                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
		                    BLL.MenuBLL menuBLL = new BLL.MenuBLL(db, _idUsuario);
                            menuBLL.Update(menu);
                            menuBLL.SaveChanges();
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
        // GET: /Menu/Delete/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Delete(int id)
        {
            try
            {
                using (Context db = new Context())
                {
	                BLL.MenuBLL menuBLL = new BLL.MenuBLL(db, _idUsuario);
                    Menu menu = menuBLL.FindSingle(e => e.idMenu == id);

                    return View(RP.Sistema.Web.Models.Menu.MenuVM.E2VM(menu));
                }
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR); RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Menu/Delete/5
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
	                    BLL.MenuBLL menuBLL = new BLL.MenuBLL(db, _idUsuario);
                        menuBLL.Delete(e => e.idMenu == id);
                        menuBLL.SaveChanges();
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
        // GET: /Menu/Report?filter=
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
            BLL.MenuBLL menuBLL;
            BLL.AcaoBLL acaoBLL;

            try
            {
                using (var db = new Context())
                {
                    menuBLL = new BLL.MenuBLL(db, idUsuario);
                    acaoBLL = new BLL.AcaoBLL(db, idUsuario);
                    var menus = menuBLL.Search(filter);

                    table.Columns.Add(new System.Data.DataColumn("idmenu", System.Type.GetType("System.Int32")));
                    table.Columns.Add(new System.Data.DataColumn("nmmenu", System.Type.GetType("System.String")));
                    table.Columns.Add(new System.Data.DataColumn("nmacao", System.Type.GetType("System.String")));
                    table.Columns.Add(new System.Data.DataColumn("dsacao", System.Type.GetType("System.String")));
                    table.Columns.Add(new System.Data.DataColumn("acaomenu", System.Type.GetType("System.String")));

                    foreach (Menu menu in menus)
                    {
                        var acoes = acaoBLL.Find(e => e.idMenu == menu.idMenu).ToList();

                        if (acoes.Count() > 0)
                        {

                            foreach (Acao acao in acoes)
                            {
                                row = table.NewRow();
                                row["idmenu"] = menu.idMenu;
                                row["nmmenu"] = menu.nmMenu;

                                row["nmacao"] = acao.nmAcao;
                                row["dsacao"] = acao.dsAcao;
                                row["acaomenu"] = acao.nmMenu;
                                table.Rows.Add(row);
                            }
                        }
                        else
                        {
                            row = table.NewRow();
                            row["idmenu"] = menu.idMenu;
                            row["nmmenu"] = menu.nmMenu;
                            table.Rows.Add(row);
                        }
                    }

                    ds.Tables.Add(table);
                    listData.Add("subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db));
                    listData.Add("table", ds);
                }

                titulo = string.Format("<center>Relação de Menus{0}</center>", !string.IsNullOrEmpty(filter) ? ("</br>Nome contendo: " + filter) : "");

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
		#endregion

		#region JsonResult
		//
        // GET: /Menu/JsSearch?filter=
        [Auth.Class.Auth(true)]
        public JsonResult JsSearch(string filter, int? page, int? pagesize)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.MenuBLL menuBLL = new BLL.MenuBLL(db, _idUsuario);
                    var result = menuBLL.Search(filter, page, pagesize);
                    var list = result.Select(s => new
                    {
                        s.idMenu,
                        s.nmMenu
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
        // GET: /Menu/JsDetails/5
        [Auth.Class.Auth(true)]
        public JsonResult JsDetails(int idMenu)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.MenuBLL menuBLL = new BLL.MenuBLL(db, _idUsuario);
                    Menu menu = menuBLL.FindSingle(e => e.idMenu == idMenu);

                    if (menu == null)
                    {
                        return Json(string.Empty, JsonRequestBehavior.AllowGet);
                    }

                    var result = new
                    {
                        menu.idMenu,
                        menu.nmMenu,
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

