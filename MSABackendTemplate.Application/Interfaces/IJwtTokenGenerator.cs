using MSABackendTemplate.Domain.Entities;

namespace MSABackendTemplate.Application.Interfaces
{
    /// <summary>
    /// JWT Token üretimi için servis arayüzü.
    /// Bu servisi Persistence katmanında implement ediyoruz çünkü JWT configuration'a ihtiyaç var.
    /// </summary>
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Kullanıcı bilgilerine göre JWT token üretir.
        /// </summary>
        /// <param name="user">ApplicationUser entity</param>
        /// <param name="roles">Kullanıcının rolleri (opsiyonel)</param>
        /// <returns>JWT token string</returns>
        string GenerateToken(ApplicationUser user, IList<string>? roles = null);
    }
}
