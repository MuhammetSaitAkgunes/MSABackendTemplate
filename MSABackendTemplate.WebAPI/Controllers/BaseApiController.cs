using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSABackendTemplate.Application.Wrappers;

namespace MSABackendTemplate.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        // ServiceResult (Verisiz) dönüşümü
        // NonAction: Bu metodun bir endpoint olmadığını belirtir.
        [NonAction]
        public IActionResult CreateActionResult(ServiceResult result)
        {
            if (result.StatusCode == 204)
            {
                return NoContent();
            }

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return ProblemDetails(result);
        }

        // ServiceResult<T> (Verili) dönüşümü
        [NonAction]
        public IActionResult CreateActionResult<T>(ServiceResult<T> result)
        {
            if (result.IsSuccess)
            {
                // 201 Created durumunda Resource URL dönülebilir ama şimdilik data dönüyoruz.
                if (result.StatusCode == 201)
                {
                    return Created("", result);
                }
                return Ok(result);
            }

            return ProblemDetails(result);
        }

        // Hata detaylarını standartlaştıran yardımcı metot
        private IActionResult ProblemDetails(ServiceResult result)
        {
            if (result.Errors != null && result.Errors.Count > 0)
            {
                // Hataları ProblemDetails formatında dönüyoruz (RFC 7807 standardı)
                return base.Problem(
                    detail: string.Join(", ", result.Errors),
                    statusCode: result.StatusCode,
                    title: "One or more errors occurred."
                );
            }

            return StatusCode(result.StatusCode, result);
        }
    }
}
