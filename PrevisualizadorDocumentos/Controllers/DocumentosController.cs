using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using VizualizadorDocumentos.Models;

namespace VizualizadorDocumentos.Controllers
{

    public class DocumentosController : Controller
    {

        public DocumentosController()
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
            // Simula diferentes tipos de ficheiros com base no ID
            string tipo;
            string filename;
            byte[] bytes;

            switch (id)
            {
                case 1:
                    tipo = "application/pdf";
                    filename = "documento.pdf";
                    bytes = GerarSimulacaoPDF("Conteúdo PDF simulado");
                    break;
                case 2:
                    tipo = "image/jpeg";
                    filename = "imagem.jpg";
                    bytes = GerarImagemSimulada(); // função abaixo
                    break;
                case 3:
                    tipo = "image/tiff";
                    filename = "imagem.tif";
                    bytes = GerarImagemSimulada(); // mesmo conteúdo para exemplo
                    break;
                default:
                    tipo = "text/plain";
                    filename = "documento.txt";
                    bytes = Encoding.UTF8.GetBytes($"Documento simulado ID {id} gerado em {DateTime.Now}");
                    break;
            }

            var stream = new MemoryStream(bytes);
            return File(stream, tipo, filename);
        }
        private byte[] GerarSimulacaoPDF(string texto)
        {
            return Encoding.UTF8.GetBytes(texto); // Substituir por lib PDF real (ex: QuestPDF)
        }
        private byte[] GerarImagemSimulada()
        {
            return null;
        }



    }
}
