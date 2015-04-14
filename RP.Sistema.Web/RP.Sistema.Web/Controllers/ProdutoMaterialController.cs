using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Transactions;
using RP.Sistema.Model.Entities;
using RP.Sistema.Model;
using RP.Util;
using System.Data;
using System.Runtime.Serialization;
using RP.Sistema.BLL;

namespace RP.Sistema.Web.Controllers
{ 
    public class ProdutoMaterialController : Controller
    {
        private int _idUsuario;

        public ProdutoMaterialController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

        #region JsonResult

        [Auth.Class.Auth(true)]
        public JsonResult JsGetItens(int idProduto)
        {
            try
            {
                LogBLL.Insert(new LogDado("JsGetItens", "ProdutoMaterial", _idUsuario));
                using (var db = new Context())
                {
                    var _bll = new BLL.ProdutoMaterialBLL(db, _idUsuario);
                    var list = _bll.Find(u => u.idProduto == idProduto, u => u.Material).ToList();
                    
                    var result = list.Select(s => new
                    {
                        s.idMaterial,
                        s.idProduto,
                        s.quantidade,
                        s.margemGanho,
                        s.valor,
                        s.Material.nome,
                        s.idProdutoMaterial,
                        totalCusto = s.valor * s.quantidade,
                        totalGanho = ((s.margemGanho / 100) * (s.valor * s.quantidade)) + (s.valor * s.quantidade),
                        totalLiquido = (((s.margemGanho / 100) * (s.valor * s.quantidade)) + (s.valor * s.quantidade)) - (s.valor * s.quantidade)
                    });

                    return Json(new RP.Util.Class.JsonCollection { result = result, count = result.Count() }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(RP.Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }

        [Auth.Class.Auth(true)]
        public JsonResult JsCreate(ProdutoMaterial model)
        {
            try
            {
                LogBLL.Insert(new LogDado("JsCreate", "ProdutoMaterial", _idUsuario));
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.ProdutoMaterialBLL(db, _idUsuario);
                        var _produtoBLL = new BLL.ProdutoBLL(db, _idUsuario);

                        _bll.Insert(model);
                        _bll.SaveChanges();
                        trans.Complete();

                        decimal totalCusto = model.quantidade * model.valor;
                        decimal totalGanho = ((model.margemGanho / 100) * totalCusto) + totalCusto;
                        decimal totalLiquido = totalGanho - totalCusto;
                        var result = new
                        {
                            idMaterial = model.idMaterial,
                            idProduto = model.idProduto,
                            model.quantidade,
                            model.margemGanho,
                            model.valor,
                            model.Material.nome,
                            model.idProdutoMaterial,
                            totalCusto = totalCusto,
                            totalGanho = totalGanho,
                            totalLiquido = totalLiquido
                        };

                        return Json(new { model = result }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(RP.Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }

        [Auth.Class.Auth(true)]
        public JsonResult JsUpdate(ProdutoMaterial model)
        {
            try
            {
                LogBLL.Insert(new LogDado("JsUpdate", "ProdutoMaterial", _idUsuario));
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.ProdutoMaterialBLL(db, _idUsuario);

                        _bll.Update(model);
                        _bll.SaveChanges();

                        trans.Complete();

                        decimal totalCusto = model.quantidade * model.valor;
                        decimal totalGanho = ((model.margemGanho / 100) * totalCusto) + totalCusto;
                        decimal totalLiquido = totalGanho - totalCusto;
                        var result = new
                        {
                            idMaterial = model.idMaterial,
                            idProduto = model.idProduto,
                            model.quantidade,
                            model.margemGanho,
                            model.valor,
                            model.Material.nome,
                            model.idProdutoMaterial,
                            totalCusto = totalCusto,
                            totalGanho = totalGanho,
                            totalLiquido = totalLiquido
                        };

                        return Json(new { model = result }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(RP.Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }

        [Auth.Class.Auth(true)]
        public JsonResult JsDelete(int idProdutoMaterial)
        {
            try
            {
                LogBLL.Insert(new LogDado("JsDelete", "ProdutoMaterial", _idUsuario));
                using (var db = new Context())
                {
                    using (var trans = new RP.DataAccess.RPTransactionScope(db))
                    {
                        var _bll = new BLL.ProdutoMaterialBLL(db, _idUsuario);

                        _bll.Delete(u => u.idProdutoMaterial == idProdutoMaterial);
                        _bll.SaveChanges();

                        trans.Complete();

                        return Json(new { msg = "Item removido com sucesso!" }, JsonRequestBehavior.AllowGet);
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

