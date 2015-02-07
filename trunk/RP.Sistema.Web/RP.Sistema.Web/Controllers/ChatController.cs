using System;
using System.Collections.Generic;
using System.Web.Mvc;
using RP.Sistema.BLL;

namespace RP.Sistema.Web.Controllers
{
    public class ChatController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Auth.Class.Auth(true)]
        public JsonResult JsListarHistoricoChat(int senderId, int recipientId)
        {
            try
            {
                List<BLL.ChatMessage> list = RPChatBLL.GetChatMessages(senderId, recipientId);                
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(RP.Util.Exception.Message.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
