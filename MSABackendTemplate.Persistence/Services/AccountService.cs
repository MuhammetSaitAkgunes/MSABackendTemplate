using Microsoft.AspNetCore.Identity;
using MSABackendTemplate.Application.DTOs;
using MSABackendTemplate.Application.Interfaces;
using MSABackendTemplate.Application.Wrappers;
using MSABackendTemplate.Domain.Entities;

namespace MSABackendTemplate.Persistence.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            // 1. Kullanıcıyı email ile bul
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return ServiceResult<LoginResponseDto>.Failure("Invalid credentials.", 401);
            }

            // 2. Şifre kontrolü (SignInManager kullanarak)
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return ServiceResult<LoginResponseDto>.Failure("Invalid credentials.", 401);
            }

            // 3. Kullanıcının rollerini al
            var roles = await _userManager.GetRolesAsync(user);

            // 4. JWT Token üret
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            // 5. Response DTO oluştur
            var response = new LoginResponseDto
            {
                Token = token,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return ServiceResult<LoginResponseDto>.Success(response);
        }

        public async Task<ServiceResult<string>> RegisterAsync(RegisterRequestDto request)
        {
            // 1. Email ile kullanıcı var mı kontrol et
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return ServiceResult<string>.Failure("User with this email already exists.", 400);
            }

            // 2. Yeni ApplicationUser oluştur
            var newUser = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email, // Email'i username olarak kullanıyoruz
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            // 3. UserManager ile kullanıcıyı oluştur (şifre hash'leme otomatik)
            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                // Identity'den gelen hataları liste halinde dön
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<string>.Failure(errors, 400);
            }

            // 4. Başarılı: User ID'yi dön
            return ServiceResult<string>.Success(newUser.Id, 201);
        }
    }
}
