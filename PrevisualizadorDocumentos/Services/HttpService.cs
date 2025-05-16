using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PrevisualizadorDocumentos.Models;
using PrevisualizadorDocumentos.Utils;
using System.Net.Http.Headers;

namespace PrevisualizadorDocumentos.Services
{
    public class HttpService
    {
        private readonly string _baseUri = "https://localhost:44354/";

        public async Task<HttpResponse<T>> SendRequestAsync<T>(HttpMethod method, string relativeUri, object content = null, string accessToken = null)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUri);

                if (!string.IsNullOrEmpty(accessToken))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                HttpResponseMessage response;

                try
                {
                    switch (method.Method)
                    {
                        case "GET":
                            response = await client.GetAsync(relativeUri);
                            break;

                        case "POST":
                            var json = JsonConvert.SerializeObject(content);
                            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                            response = await client.PostAsync(relativeUri, httpContent);
                            break;

                        case "PUT":
                            var jsonPut = JsonConvert.SerializeObject(content);
                            var httpContentPut = new StringContent(jsonPut, Encoding.UTF8, "application/json");
                            response = await client.PutAsync(relativeUri, httpContentPut);
                            break;

                        case "DELETE":
                            response = await client.DeleteAsync(relativeUri);
                            break;

                        default:
                            throw new ArgumentException("Método HTTP inválido");
                    }

                    stopwatch.Stop();

                    Debug.WriteLine($"{relativeUri} | {stopwatch.Elapsed.TotalSeconds:F2}s | StatusCode ({response.StatusCode})");
                    Debug.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");

                    return await UtilValidateStatusCode.ValidateStatusCode<T>(relativeUri, response);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();

                    Debug.WriteLine($"{relativeUri} | {stopwatch.Elapsed.TotalSeconds:F2}s | Error: {ex.Message}");

                    var errorResponse = new ErrorResponse
                    {
                        Message = "Ocorreu uma exceção ao processar o seu pedido.",
                        Error = ex.InnerException?.Message ?? ex.Message,
                        StackTrace = ex.StackTrace,
                        ErrorId = Guid.NewGuid()
                    };

                    return new HttpResponse<T>(default, false, errorResponse, HttpStatusCode.InternalServerError);
                }
            }
        }

        public async Task<HttpResponseMessage> SendRawRequestAsync(HttpMethod method, string relativeUri, object content = null)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUri);

                HttpResponseMessage response;
                try
                {
                    switch (method.Method)
                    {
                        case "POST":
                            var json = JsonConvert.SerializeObject(content);
                            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                            response = await client.PostAsync(relativeUri, httpContent);
                            break;

                        case "GET":
                            response = await client.GetAsync(relativeUri);
                            break;

                        default:
                            throw new ArgumentException("Método HTTP inválido para envio raw.");
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erro em SendRawRequestAsync: {ex.Message}");
                    throw; // Lança para ser tratado externamente
                }
            }
        }
    }
}
