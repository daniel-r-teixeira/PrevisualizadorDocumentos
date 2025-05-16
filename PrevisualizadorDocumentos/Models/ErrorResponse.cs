using System;
using System.Net;

namespace PrevisualizadorDocumentos.Models
{
    public class ErrorResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
        public string StackTrace { get; set; }
        public Guid ErrorId { get; set; }
    }
}