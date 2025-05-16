using Newtonsoft.Json;
using PrevisualizadorDocumentos.Models;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace PrevisualizadorDocumentos.Utils
{
    public static class UtilValidateStatusCode
    {
        /// <summary>
        /// Validar o código de status da resposta HTTP.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUri"></param>
        /// <param name="response"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static async Task<HttpResponse<T>> ValidateStatusCode<T>(string relativeUri, HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                //log.Info($"User: E002064R | API: {relativeUri} | StatusCode ({response.StatusCode})");
                var contentString = await response.Content.ReadAsStringAsync();
                T responseData = JsonConvert.DeserializeObject<T>(contentString);
                return new HttpResponse<T>(responseData, true, null, response.StatusCode);
            }

            else
            {
                ErrorResponse errorResponse = new ErrorResponse();
                if (response.Content.Headers.ContentType?.MediaType == "application/json")
                {
                    var contentString = await response.Content.ReadAsStringAsync();
                    errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(contentString);
                    errorResponse.StatusCode = response.StatusCode;
                }
                else
                {
                    errorResponse = new ErrorResponse
                    {
                        Message = "Formato de resposta inesperado",
                        Error = response.Content.ReadAsStringAsync().Result,
                        StatusCode = response.StatusCode
                    };
                }

                errorResponse.Message = GetStatusCodMessage(response);

                return new HttpResponse<T>(default, false, errorResponse, response.StatusCode);
            }
        }

        /// <summary>
        /// Returns a message based on the HTTP status code.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static string GetStatusCodMessage(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return "Recurso não encontrado. Verifique o URL e tente novamente.";
                case HttpStatusCode.InternalServerError:
                    return "Erro interno do servidor. Por favor, tente novamente mais tarde.";
                case HttpStatusCode.BadRequest:
                    return "O pedido era inválido ou não pode ser servido. Verifique os seus dados e tente novamente.";
                default:
                    return "Ocorreu um erro ao processar o seu pedido. Por favor, tente novamente.";
            }
        }

    }
}