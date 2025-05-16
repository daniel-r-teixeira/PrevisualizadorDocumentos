namespace VizualizadorDocumentos.Models
{
    public class DocumentViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public string Data { get; set; }
        public string Url { get; set; } // pode ser usado para abrir ou visualizar
    }
}
