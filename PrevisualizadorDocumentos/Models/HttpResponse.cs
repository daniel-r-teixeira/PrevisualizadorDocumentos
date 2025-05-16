using System.Net;

namespace PrevisualizadorDocumentos.Models
{
    public class HttpResponse<T>
    {
        /// <summary>
        /// Gets the data contained in the HTTP response.
        /// </summary>
        public T Data { get; }

        /// <summary>
        /// Gets a value indicating whether the HTTP request was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the error message in case the HTTP request failed.
        /// </summary>
        public ErrorResponse ErrorResponse { get; }

        /// <summary>
        /// Gets the HTTP status code of the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponse{T}"/> class.
        /// </summary>
        /// <param name="data">The data contained in the HTTP response.</param>
        /// <param name="isSuccess">A value indicating whether the HTTP request was successful.</param>
        /// <param name="errorResponse">The error message in case the HTTP request failed.</param>
        /// <param name="statusCode">The HTTP status code of the response.</param>
        public HttpResponse(T data, bool isSuccess, ErrorResponse errorResponse, HttpStatusCode statusCode)
        {
            Data = data;
            IsSuccess = isSuccess;
            ErrorResponse = errorResponse;
            StatusCode = statusCode;
        }
    }
}