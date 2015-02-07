using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Util;
using System.Web;

namespace RP.Sistema.Web.Controllers
{
    public class EntidadeController : Controller
    {
        private int _idUsuario;
        public EntidadeController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

        #region ActionResult
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            try
            {
                using (Context db = new Context())
                {
                    var _bll = new BLL.EntidadeBLL(db, _idUsuario);
                    var entidade = _bll.FindSingle();

                    if (entidade == null)
                        return View(new RP.Sistema.Web.Models.Entidade.EntidadeVM());

                    var model = RP.Sistema.Web.Models.Entidade.EntidadeVM.E2VM(entidade);
                    if (entidade.imLogo != null)
                    {
                        var stream = new System.IO.MemoryStream(entidade.imLogo);

                        var image = new System.Drawing.Bitmap(stream);
                        var file = Guid.NewGuid() + ".jpg";
                        var fullPath = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["PathFile"], file);
                        image.Save(fullPath, System.Drawing.Imaging.ImageFormat.Jpeg);

                        var virtualPath = "/Files/" + file;
                        model.pathLogo = virtualPath;
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        [HttpPost]
        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index(RP.Sistema.Web.Models.Entidade.EntidadeVM viewData, HttpPostedFileBase fuFoto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entidade = viewData.VM2E();
                    using (Context db = new Context())
                    {
                        using (var trans = new RP.DataAccess.RPTransactionScope(db))
                        {
                            BLL.EntidadeBLL entidadeBLL = new BLL.EntidadeBLL(db, _idUsuario);

                            if (fuFoto != null)
                            {
                                var fileBytes = new byte[fuFoto.ContentLength];
                                fuFoto.InputStream.Read(fileBytes, 0, fileBytes.Length);
                                entidade.imLogo = fileBytes;
                            }
                            else if (this.HttpContext.Request.Params.AllKeys.Contains("fuFoto"))
                            {
                                if (string.IsNullOrEmpty(this.HttpContext.Request.Params["fuFoto"]))
                                {
                                    entidade.imLogo = null;
                                }
                            }
                            if (viewData.idEntidade != null)
                            {
                                entidadeBLL.Update(entidade);
                            }
                            else
                            {
                                entidadeBLL.Insert(entidade);
                            }
                            entidadeBLL.SaveChanges();

                            if (fuFoto != null)
                            {
                                entidadeBLL.UpdateLogo(entidade);
                            }
                            else if (this.HttpContext.Request.Params.AllKeys.Contains("fuFoto"))
                            {
                                if (string.IsNullOrEmpty(this.HttpContext.Request.Params["fuFoto"]))
                                {

                                    entidadeBLL.RemoveLogo(entidade);
                                }
                            }
                            entidadeBLL.SaveChanges();
                            trans.Complete();
                            viewData.idEntidade = entidade.idEntidade;
                            this.AddFlashMessage("Os dados da empresa foram salvos com sucesso!", FlashMessage.SUCCESS);
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    RP.Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                    return RedirectToAction("Index", "Erro");
                }
            }

            return View(viewData);
        }
        #endregion
    }
}

