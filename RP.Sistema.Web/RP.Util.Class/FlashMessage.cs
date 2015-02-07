using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace RP.Util
{
    public static class FlashMessage
    {
        public const string ERROR = "error";
        public const string ALERT = "alert";
        public const string INFO = "info";
        public const string SUCCESS = "success";

        [Serializable]
        public class Message
        {
            public string type { get; set; }
            public string textMessage { get; set; }
            public bool closeable { get; set; }

            public Message() {
                closeable = true;
                type = FlashMessage.ALERT;
            }
        }

        public static void AddFlashMessage(this HtmlHelper helper, Message message)
        {
            if (helper.ViewContext.Controller.TempData.ContainsKey(message.type))
            {
                List<Message> messages = null;
                if (helper.ViewContext.Controller.TempData[message.type].GetType() == typeof(List<Message>))
                {
                    messages = (List<Message>)helper.ViewContext.Controller.TempData[message.type];
                    messages.Add(message);
                }
                else
                {
                    messages = new List<Message> { message };
                }

                helper.ViewContext.Controller.TempData[message.type] = messages;
            }
            else
            {
                helper.ViewContext.Controller.TempData[message.type] = new List<Message> { message };
            }
        }

        public static void AddFlashMessage(this Controller controller, Message message)
        {
            if (controller.TempData.ContainsKey(message.type))
            {
                List<Message> messages = null;
                if (controller.TempData[message.type].GetType() == typeof(List<Message>))
                {
                    messages = (List<Message>)controller.TempData[message.type];
                    messages.Add(message);
                }
                else
                {
                    messages = new List<Message> { message };
                }

                controller.TempData[message.type] = messages;
            }
            else
            {
                controller.TempData[message.type] = new List<Message> { message };
            }
        }

        public static void AddFlashMessage(this HtmlHelper helper, string message, string type = FlashMessage.SUCCESS)
        {
            AddFlashMessage(helper, new Message
            {
                type = type,
                textMessage = message,
                closeable = true
            });
        }

        public static void AddFlashMessage(this Controller controller, string message, string type = FlashMessage.SUCCESS)
        {
            AddFlashMessage(controller, new Message
            {
                type = type,
                textMessage = message,
                closeable = true
            });
        }

        public static MvcHtmlString RenderFlashMessage(this HtmlHelper helper)
        {
            string cssClass, inner;
            string result = string.Empty;
            string format = @"<div class=""{0}"">{1}</div>";

            foreach (KeyValuePair<string, object> item in helper.ViewContext.Controller.TempData)
            {
                foreach (Message message in (List<Message>)item.Value)
                {
                    cssClass = string.Empty;
                    inner = string.Empty;

                    if (message.closeable)
                    {
                        inner += @"<button type=""button"" class=""close"" data-dismiss=""alert"">×</button>";
                        cssClass += "fade in ";
                    }

                    if ((item.Key == message.type) && item.Key == FlashMessage.INFO)
                    {
                        cssClass += "alert alert-info";
                    }
                    else if (item.Key == FlashMessage.ERROR)
                    {
                        cssClass += "alert alert-error";
                    }
                    else if (item.Key == FlashMessage.SUCCESS)
                    {
                        cssClass += "alert alert-success";
                    }
                    else
                    {
                        cssClass += "alert";
                    }

                    inner += message.textMessage;
                    result += string.Format(format, cssClass, inner);
                }
            }

            return MvcHtmlString.Create(result);
        }
    }
}
