using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using VizualizadorDocumentos.Models;

namespace VizualizadorDocumentos.Controllers
{

    public class HomeController : Controller
    {

        public HomeController()
        {
        }

        public ActionResult Index()
        {
            var documentos = new List<DocumentViewModel>
        {
            new DocumentViewModel { Id = 1, Titulo = "Documento Simulado 1", Tipo = "PDF", Data = "2025-05-16" },
            new DocumentViewModel { Id = 2, Titulo = "Documento Simulado 2", Tipo = "PDF", Data = "2025-05-10" }
        };

            return View(documentos);
        }
        // Gera um PDF diretamente da memória
        public ActionResult Visualizar(int id)
        {
            var titulo = $"Documento Simulado #{id}";
            var content = $"Este é um conteúdo simulado para o documento com ID {id}. Gerado em {DateTime.Now:yyyy-MM-dd HH:mm:ss}.";

            // Simula um PDF como texto plano (podes integrar biblioteca de PDF real ex: iTextSharp, QuestPDF)
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);

            // Content-Type genérico, substitui por application/pdf se usares PDF real
            return File(stream, "application/octet-stream", $"{titulo}.txt");
        }

        public ActionResult Privacy()
        {
            return View();
        }

    }
}
