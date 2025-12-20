using MSABackendTemplate.Application.Wrappers;
using Serilog;
using System.Net;
using System.Text.Json;

namespace MSABackendTemplate.WebAPI.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // İsteği bir sonraki adıma (Controller'a) ilet
                await _next(context);
            }
            catch (Exception error)
            {
                // Hata olursa yakala!
                var response = context.Response;
                response.ContentType = "application/json";

                // Loglama yap (Serilog devreye giriyor)
                Log.Error(error, "Sistemde beklenmedik bir hata oluştu!");

                var responseModel = ServiceResult.Failure("Internal Server Error. Please try again later.", 500);

                // Hata tipine göre özel durumlar (Opsiyonel)
                switch (error)
                {
                    case KeyNotFoundException e:
                        // Veri bulunamadı hatası
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        responseModel = ServiceResult.Failure(e.Message, 404);
                        break;

                    default:
                        // Genel hata (500)
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}
