using Newtonsoft.Json;
using PrevisualizadorDocumentos.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text;
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

        //public async Task<ActionResult> Previsualizar(string file)
        //{
        //    var httpService = new HttpService();

        //    var payload = new
        //    {
        //        NomeFicheiroOriginal = "c544677a-b2c2-4338-9985-ef44df24a2b5_08052025_17235710.pdf",
        //        NomeUtilizador = "E002064R",
        //        IsDocumentoTratado = true,
        //    };

        //    // Chamar API
        //    var response = await httpService.SendRawRequestAsync(HttpMethod.Post, "api/Visualizador/Previsualizar", payload);

        //    if (!response.IsSuccessStatusCode)
        //        return new HttpStatusCodeResult((int)response.StatusCode, "Erro ao obter ficheiro da API");

        //    var contentStream = await response.Content.ReadAsStreamAsync();

        //    // Validar se há conteúdo
        //    if (contentStream == null || contentStream.Length == 0)
        //        return new HttpStatusCodeResult(HttpStatusCode.NoContent, "Ficheiro vazio devolvido pela API");

        //    // Copiar para memória para garantir reusabilidade do stream
        //    var memoryStream = new MemoryStream();
        //    await contentStream.CopyToAsync(memoryStream);
        //    memoryStream.Position = 0;

        //    // Obter MIME e nome do ficheiro
        //    var mimeType = response.Content.Headers.ContentType?.MediaType ?? "application/pdf";
        //    var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "ficheiro.pdf";

        //    Response.AppendHeader("Content-Disposition", $"inline; filename={fileName}");
        //    return File(memoryStream, mimeType);
        //}

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

        public async Task<ActionResult> Download(string fileName)
        {
            var apiBaseUrl = "https://localhost:44354";
            using (HttpClient httpClient = new HttpClient())
            {
                var apiUrl = $"{apiBaseUrl}/api/Visualizador/Download";

                var response = await httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpException(404, $"Ficheiro '{fileName}' não encontrado.");
                }

                var contentStream = await response.Content.ReadAsStreamAsync();
                var contentType = response.Content.Headers.ContentType.MediaType;

                return File(contentStream, contentType, fileName);
            }
        }

        public async Task<ActionResult> DownloadWithDataPayload(string fileName)
        {
            // file = 93cd3584-9f6d-4852-bdf7-395f9d285fa8_14052025_10534328.pdf

            var payload = new
            {
                NomeFicheiroOriginal = "93cd3584-9f6d-4852-bdf7-395f9d285fa8_14052025_10534328.pdf",
                NomeUtilizador = "E002064R",
                IsDocumentoTratado = true,
            };

            var apiBaseUrl = "https://localhost:44354";
            using (HttpClient httpClient = new HttpClient())
            {
                var apiUrl = $"{apiBaseUrl}/api/Visualizador/Download";

                // Serializar o payload para JSON
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpException(404, $"Ficheiro '{fileName}' não encontrado.");
                }

                var contentStream = await response.Content.ReadAsStreamAsync();
                var contentType = response.Content.Headers.ContentType.MediaType;

                return File(contentStream, contentType, fileName);
            }
        }


    }
}