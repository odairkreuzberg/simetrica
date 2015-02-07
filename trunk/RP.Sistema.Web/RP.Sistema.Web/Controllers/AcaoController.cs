using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Sistema.Web.Models.Acao;
using RP.Util;

namespace RP.Sistema.Web.Controllers
{ 
    public class AcaoController : Controller
    {
        private int _idUsuario;
        public AcaoController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }


		#region ActionResult
        //
        // GET: /Acao/
        [PersistDataSearch("Search")]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            return View();
        }
		
        //
        // GET: /Acao/Search?filter=
        [PersistDataSearch]
        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult Search(string filter, int? page, int? pagesize)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.AcaoBLL acaoBLL = new BLL.AcaoBLL(db, _idUsuario);
                    var result = acaoBLL.Search(filter, page, pagesize);

                    return View("Index", result);
				}
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }		

        //
        // GET: /Acao/Details/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Details(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.AcaoBLL acaoBLL = new BLL.AcaoBLL(db, _idUsuario);
                    Acao acao = acaoBLL.FindSingle(e => e.idAcao == id, i => i.Controle, i => i.Menu);

                    return View(AcaoVM.E2VM(acao));
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Acao/Create
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create()
        {
            List<string> icones = new List<string>();
            System.IO.FileInfo finfo;

            foreach (string file in System.IO.Directory.GetFiles(Server.MapPath("~/Content/images/atalho")))
            {
                finfo = new System.IO.FileInfo(file);
                icones.Add(System.IO.Path.GetFileNameWithoutExtension(finfo.Name));
            }

            return View(new AcaoVM
            {
                listaIcones = icones
            });
        } 

        //
        // POST: /Acao/Create
        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Create(AcaoVM viewData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Acao acao = viewData.VM2E();
                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            acao.flMenu = acao.flMenu.ToLower() == "true" || acao.flMenu.ToLower() == "sim" ? "Sim" : "Não";

                            BLL.AcaoBLL acaoBLL = new BLL.AcaoBLL(db, _idUsuario);
                            acaoBLL.Insert(acao);
                            acaoBLL.SaveChanges();
                            trans.Complete();

                            this.AddFlashMessage(RP.Util.Resource.Message.INSERT_SUCCESS, FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                    return RedirectToAction("Index");
                }

            }

            return View(viewData);
        }
        
        //
        // GET: /Acao/Edit/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(int id)
        {
            List<string> icones = new List<string>();
            System.IO.FileInfo finfo;

            try
            {
                foreach (string file in System.IO.Directory.GetFiles(Server.MapPath("~/Content/images/atalho")))
                {
                    finfo = new System.IO.FileInfo(file);
                    icones.Add(System.IO.Path.GetFileNameWithoutExtension(finfo.Name));
                }

                using (Context db = new Context())
                {
                    BLL.AcaoBLL acaoBLL = new BLL.AcaoBLL(db, _idUsuario);
                    Acao acao = acaoBLL.FindSingle(e => e.idAcao == id, i => i.Controle, i => i.Menu);

                    var model = AcaoVM.E2VM(acao);
                    model.listaIcones = icones;
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Acao/Edit/5
        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Edit(AcaoVM viewData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Acao acao = viewData.VM2E();
                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            acao.flMenu = acao.flMenu.ToLower() == "true" || acao.flMenu.ToLower() == "sim" ? "Sim" : "Não";

                            BLL.AcaoBLL acaoBLL = new BLL.AcaoBLL(db, _idUsuario);
                            acaoBLL.Update(acao);
                            acaoBLL.SaveChanges();
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
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }        
		}

        //
        // GET: /Acao/Delete/5
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Delete(int id)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.AcaoBLL acaoBLL = new BLL.AcaoBLL(db, _idUsuario);
                    Acao acao = acaoBLL.FindSingle(e => e.idAcao == id, i => i.Controle, i => i.Menu);

                    return View(AcaoVM.E2VM(acao));
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Acao/Delete/5
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
                        BLL.AcaoBLL acaoBLL = new BLL.AcaoBLL(db, _idUsuario);
                        acaoBLL.Delete(e => e.idAcao == id);
                        acaoBLL.SaveChanges();
                        trans.Complete();

                        this.AddFlashMessage(RP.Util.Resource.Message.DELETE_SUCCESS, FlashMessage.SUCCESS);
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Delete", id);
            }
        }

		//
        // GET: /Acao/Report?filter=
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
            BLL.AcaoBLL acaoBLL;

            try
            {
                using (var db = new Context())
                {
                    acaoBLL = new BLL.AcaoBLL(db, idUsuario);
                    var acoes = acaoBLL.Search(filter);

                    table.Columns.Add(new System.Data.DataColumn("idacao", System.Type.GetType("System.Int32")));
                    table.Columns.Add(new System.Data.DataColumn("dsacao", System.Type.GetType("System.String")));
                    table.Columns.Add(new System.Data.DataColumn("nmmenu", System.Type.GetType("System.String")));
                    
                    foreach (Acao acao in acoes)
                    {
                        row = table.NewRow();
                        row["idacao"] = acao.idAcao;
                        row["dsacao"] = acao.dsAcao;
                        row["nmmenu"] = acao.nmMenu;
                        table.Rows.Add(row);
                    }

                    ds.Tables.Add(table);
                    listData.Add("subentidade.rpt", RP.Sistema.BLL.EntidadeBLL.getDtSetEntidade(db));
                    listData.Add("table", ds);
                }

                titulo = string.Format("<center>Relação de Ações{0}</center>", !string.IsNullOrEmpty(filter) ? ("</br>Nome contendo: " + filter) : "");
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
        // GET: /Acao/JsSearch?filter=
        [Auth.Class.Auth(true)]
        public JsonResult JsSearch(string filter, int? page, int? pagesize)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.AcaoBLL acaoBLL = new BLL.AcaoBLL(db, _idUsuario);
                    var result = acaoBLL.Search(filter, page, pagesize);
                    var list = result.Select(s => new 
                    {
                        s.idAcao,
                        s.nmAcao,
                        s.dsAcao
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
        // GET: /Acao/JsDetails/5
        [Auth.Class.Auth(true)]
        public JsonResult JsDetails(int idAcao)
        {
            try
            {
                using (Context db = new Context())
                {
                    BLL.AcaoBLL acaoBLL = new BLL.AcaoBLL(db, _idUsuario);
                    Acao acao = acaoBLL.FindSingle(e => e.idAcao == idAcao, i => i.Controle, i => i.Menu);

                    if (acao == null)
                    {
                        return Json(string.Empty, JsonRequestBehavior.AllowGet);
                    }

                    var result = new 
                    {
                        acao.idAcao,
                        acao.nmAcao,
                        acao.dsAcao
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

