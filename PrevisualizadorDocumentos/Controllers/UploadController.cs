using PrevisualizadorDocumentos.Services;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;

namespace PrevisualizadorDocumentos.Controllers
{
    public class UploadController : Controller
    {
        private readonly string _uploadPath;

        public UploadController()
        {
            _uploadPath = HostingEnvironment.MapPath("~/uploads");
            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);
        }

        public ActionResult Index()
        {
            var files = Directory.GetFiles(_uploadPath)
                                 .Select(Path.GetFileName)
                                 .ToList();
            return View(files);
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var filename = Path.GetFileName(file.FileName);
                var path = Path.Combine(_uploadPath, filename);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.InputStream.CopyTo(stream);
                }
            }

            return RedirectToAction("Index");
        }

        // ----

        //public IActionResult Preview(string file)
        //{
        //    var filePath = Path.Combine(_uploadPath, file);
        //    if (!System.IO.File.Exists(filePath))
        //        return NotFound();

        //    var mime = GetMimeType(file);
        //    var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        //    return File(stream, mime);
        //}

        public async Task<ActionResult> Preview(string file)
        {
            HttpService httpService = new HttpService();

            var payload = new
            {
                NomeFicheiroOriginal = "c544677a-b2c2-4338-9985-ef44df24a2b5_08052025_17235710.pdf",
                NomeUtilizador = "E002064R",
                IsDocumentoTratado = true,
            };

            var filePath = await httpService.SendRequestAsync<string>(
                HttpMethod.Post,
                $"api/Visualizador/PrepararFicheiroParaVisualizar",
                payload
            );

            var mime = GetMimeType(file);
            var stream = new FileStream(filePath.Data, FileMode.Open, FileAccess.Read);
            var ficheiro = File(stream, mime);
            return ficheiro;
        }

        public async Task<ActionResult> Previsualizar(string file)
        {
            var httpService = new HttpService();

            var _payload = new
            {
                NomeFicheiroOriginal = "c544677a-b2c2-4338-9985-ef44df24a2b5_08052025_17235710.pdf",
                NomeUtilizador = "E002064R",
                IsDocumentoTratado = true,
            };

            var response = await httpService.SendRawRequestAsync(HttpMethod.Post, "api/Visualizador/Previsualizar", _payload);

            if (!response.IsSuccessStatusCode)
                return new HttpStatusCodeResult((int)response.StatusCode, "Erro ao obter ficheiro da API");

            var contentStream = await response.Content.ReadAsStreamAsync();

            var mimeType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "ficheiro";

            // Devolve diretamente o conteúdo binário
            var ficheiro = File(contentStream, mimeType, fileName);
            return ficheiro;
        }

        private string GetMimeType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLower();
            return GetMimeTypeByExtension(ext);
        }

        private string GetMimeTypeByExtension(string ext)
        {
            switch (ext)
            {
                case ".pdf":
                    return "application/pdf";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".tif":
                case ".tiff":
                    return "image/tiff";
                default:
                    return "application/octet-stream";
            }
        }


    }
}