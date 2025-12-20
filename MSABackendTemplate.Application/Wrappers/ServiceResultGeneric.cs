using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MSABackendTemplate.Application.Wrappers
{
    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; set; }

        // --- BAŞARILI DURUMLAR ---

        public static ServiceResult<T> Success(T data, int statusCode = 200)
        {
            return new ServiceResult<T>
            {
                Data = data,
                IsSuccess = true,
                StatusCode = statusCode,
                Errors = null // Başarılıysa hata listesi boşuna yer kaplamasın
            };
        }

        // --- HATALI DURUMLAR ---

        // Hata durumunda Data null döner, ama Errors dolu döner.
        // new keywordü ile base metodunu gizliyoruz (Method Hiding)
        public new static ServiceResult<T> Failure(string error, int statusCode = 400)
        {
            return new ServiceResult<T>
            {
                Data = default,
                IsSuccess = false,
                StatusCode = statusCode,
                Errors = new List<string> { error }
            };
        }

        public new static ServiceResult<T> Failure(List<string> errors, int statusCode = 400)
        {
            return new ServiceResult<T>
            {
                Data = default,
                IsSuccess = false,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }
}