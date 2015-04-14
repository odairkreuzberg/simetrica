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
    public class LogController : Controller
    {
        private int _idUsuario;

        public LogController()
        {
            _idUsuario = RP.Sistema.Web.Helpers.Helper.UserId;
        }

        #region ActionResult

        [PersistDataSearch("Search")]
        public ActionResult Index()
        {
            return View();
        }

        [PersistDataSearch]
        public ActionResult Search(string filter, int? page, int? pagesize)
        {
            try
            {
                using (var db = new Context())
                {
                    var _bll = new BLL.LogDadoBLL(db, _idUsuario);

                    var result = _bll.Search(filter, page, pagesize);

                    return View("Index", result);
                }
            }
            catch (Exception ex)
            {
                Util.Entity.ErroLog.Add(ex, Session.SessionID, _idUsuario);
                return RedirectToAction("Index", "Erro", new { area = string.Empty });
            }
        }

        #endregion

        

        
    }
}

