using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MSABackendTemplate.Application.Wrappers
{
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; }

        // Bu property API'ye dönerken JSON'da görünmesin, 
        // sadece Controller'da Status Code belirlemek için kullanalım.
        [JsonIgnore]
        public int StatusCode { get; set; }

        // Constructor'ı private yapıyoruz ki sadece Statik Metotlarla üretilsin (Factory Pattern)
        public ServiceResult()
        {
            Errors = new List<string>();
        }

        // --- BAŞARILI DURUMLAR ---

        public static ServiceResult Success(int statusCode = 200)
        {
            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = statusCode
            };
        }

        // --- HATALI DURUMLAR ---

        public static ServiceResult Failure(string error, int statusCode = 400)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Errors = new List<string> { error }
            };
        }

        public static ServiceResult Failure(List<string> errors, int statusCode = 400)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }
}