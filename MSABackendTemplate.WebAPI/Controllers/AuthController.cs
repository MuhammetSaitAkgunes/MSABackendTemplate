using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MSABackendTemplate.Application.DTOs;
using MSABackendTemplate.Application.Interfaces;

namespace MSABackendTemplate.WebAPI.Controllers
{
    /// <summary>
    /// Authentication Controller: Register ve Login işlemleri için.
    /// BaseApiController'dan miras alarak standart response yapısını kullanıyoruz.
    /// </summary>
    [EnableRateLimiting("auth")] // Token bucket: 10 tokens, 5/10sec refill
    public class AuthController : BaseApiController
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Yeni kullanıcı kaydı oluşturur.
        /// </summary>
        /// <param name="request">Kullanıcı kayıt bilgileri</param>
        /// <returns>Oluşturulan kullanıcının ID'si</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            var result = await _accountService.RegisterAsync(request);
            return CreateActionResult(result);
        }

        /// <summary>
        /// Kullanıcı girişi yapar ve JWT token döner.
        /// </summary>
        /// <param name="request">Email ve şifre bilgileri</param>
        /// <returns>JWT token ve kullanıcı bilgileri</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var result = await _accountService.LoginAsync(request);
            return CreateActionResult(result);
        }
    }
}
