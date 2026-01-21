using MSABackendTemplate.Application.DTOs;
using MSABackendTemplate.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSABackendTemplate.Application.Interfaces
{
    public interface IAccountService
    {
        // Login işlemi: Geriye Token döner
        Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto request);

        // Register işlemi: Geriye oluşan User Id döner
        Task<ServiceResult<string>> RegisterAsync(RegisterRequestDto request);
    }
}
