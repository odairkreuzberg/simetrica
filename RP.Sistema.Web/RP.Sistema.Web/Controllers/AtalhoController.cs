using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using RP.Util;
using Newtonsoft.Json;

namespace RP.Sistema.Web.Controllers
{
    public class AtalhoController : Controller
    {
        #region ActionResult

        [Auth.Class.Auth("sistema", "padrao")]
        public ActionResult Index()
        {
            return View();
        }

        [Auth.Class.Auth("sistema", "padrao", "index")]
        public ActionResult GerarSprite()
        {
            try
            {
                var sprite = new RP.Sprite.Shell.SCEngine();

                var option = new RP.Sprite.Shell.SCEngineOptions();

                string storageRoot = ConfigurationManager.AppSettings["PathAtalho"];

                string fileName = @"atalho-*.png";
                foreach (var file in Directory.GetFiles(Path.Combine(storageRoot, "images"), fileName))
                {
                    System.IO.File.Delete(file);
                }
                
                option.BinPackingLevel = 3;
                option.CssClassPrefix = "atalho-";
                option.DestinationDirectoryImage = Path.Combine(storageRoot, "images");
                option.DestinationVirtualPathImage = @"images/";
                option.DestinationDirectoryCSS = storageRoot;

                option.FileNameImage = string.Format("atalho-{0}", Util.Class.Util.randomString(4, false, false));
                option.FileNameCSS = "sprites-atalho";
                option.Override = true;
                option.Padding = 50;
                option.SourceDirectory = Path.Combine(storageRoot, "images", "atalho");

                sprite.GenerateSprite(option);

                this.AddFlashMessage("Sprite gerado com sucesso!");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                this.AddFlashMessage(RP.Util.Exception.Message.Get(ex), FlashMessage.ERROR);
                return RedirectToAction("Index");
            }
        }

        [Auth.Class.Auth("sistema", "padrao", "index")]
        public void Upload()
        {
            string StorageRoot = ConfigurationManager.AppSettings["PathAtalho"];
            StorageRoot = Path.Combine(StorageRoot, "images", "atalho");
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Cache-Control", "private, no-cache");

            switch (Request.HttpMethod)
            {
                case "HEAD":
                case "GET":
                    if (GivenFilename())
                    {
                        DeliverFile(StorageRoot);
                    }
                    else
                    {
                        ListarImagens(StorageRoot);
                    }
                    break;

                case "POST":
                case "PUT":
                    UploadFile(StorageRoot);
                    break;

                case "DELETE":
                    DeleteFile(StorageRoot);
                    break;

                case "OPTIONS":
                    ReturnOptions();
                    break;

                default:
                    Response.ClearHeaders();
                    Response.StatusCode = 405;
                    break;
            }
        }

        #endregion

        #region  Uploads Methods

        private bool GivenFilename()
        {
            return !string.IsNullOrEmpty(Request["f"]);
        }

        private void DeleteFile(string StorageRoot)
        {
            string fileName = HttpContext.Request["f"];
            string filePath = Path.Combine(StorageRoot, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

        }

        private void UploadFile(string StorageRoot)
        {

            var statuses = new List<FilesStatus>();

            if (!Directory.Exists(StorageRoot))
            {
                Directory.CreateDirectory(StorageRoot);
            }

            for (int i = 0; i < HttpContext.Request.Files.Count; i++)
            {
                var file = HttpContext.Request.Files[i];

                if (file != null)
                {
                    file.SaveAs(Path.Combine(StorageRoot, file.FileName));

                    statuses.Add(new FilesStatus(Path.GetFileName(file.FileName), file.ContentLength));
                }

                WriteJsonIframeSafe(statuses);
            }
        }

        private void DeliverFile(string StorageRoot)
        {
            var filename = Request["f"];
            var filePath = StorageRoot + filename;
            Response.ClearHeaders();
            if (System.IO.File.Exists(filePath))
            {
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + HttpUtility.UrlEncode(filename) + "\"");
                Response.ContentType = "application/octet-stream";
                Response.ClearContent();
                Response.WriteFile(filePath);
            }
            else
            {
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + HttpUtility.UrlEncode(filename) + "\"");

                Response.ClearContent();

            }
            Response.End();
        }

        private void ListarImagens(string StorageRoot)
        {
            var files = new List<FileInfo>();

            if (Directory.Exists(StorageRoot))
            {
                files = new DirectoryInfo(StorageRoot)
                    .GetFiles("*", SearchOption.TopDirectoryOnly)
                    .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
                    .Select(f => new FileInfo(f.FullName))
                    .ToList();
            }

            var result = files.Select(item => new FilesStatus(item.Name, item.Length)).ToList();

            string jsonObj = JsonConvert.SerializeObject(result);
            HttpContext.Response.AddHeader("Content-Disposition", "inline; filename=\"files.json\"");
            HttpContext.Response.Write(jsonObj);
            HttpContext.Response.ContentType = "application/json";


        }

        public class FilesStatus
        {
            public string name { get; set; }
            public string type { get; set; }
            public long size { get; set; }
            public string url { get; set; }
            public string thumbnail_url { get; set; }
            public string delete_url { get; set; }
            public string delete_type { get; set; }
            public string error { get; set; }

            public FilesStatus(string fileName, long fileLength)
            {
                SetValues(fileName, fileLength);
            }

            private void SetValues(string fileName, long fileLength)
            {
                var urlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);

                name = fileName;
                type = "image/png";
                size = fileLength;
                url = urlHelper.Content(string.Format("~/content/Images/Atalho/{0}", fileName));
                thumbnail_url = urlHelper.Content(string.Format("~/content/Images/Atalho/{0}", fileName));
                delete_url = urlHelper.Action("Upload", "Atalho", new { f = fileName });
                delete_type = "DELETE";
                error = fileName == "103.jpg" ? "erro customizado" : "";
            }
        }

        private void ReturnOptions()
        {
            Response.AddHeader("Allow", "DELETE,GET,HEAD,POST,PUT,OPTIONS");
            Response.StatusCode = 200;
        }

        private void WriteJsonIframeSafe(List<FilesStatus> statuses)
        {
            HttpContext.Response.AddHeader("Vary", "Accept");
            try
            {
                HttpContext.Response.ContentType = HttpContext.Request["HTTP_ACCEPT"].Contains("application/json") ? "application/json" : "text/plain";
            }
            catch
            {
                HttpContext.Response.ContentType = "text/plain";
            }

            var jsonObj = JsonConvert.SerializeObject(statuses.ToArray());
            HttpContext.Response.Write(jsonObj);
        }

        #endregion
    }
}

