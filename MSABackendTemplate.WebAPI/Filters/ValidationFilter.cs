using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MSABackendTemplate.Application.Wrappers;

namespace MSABackendTemplate.WebAPI.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // ModelState valid değilse (FluentValidation hataları buraya dolar)
            if (!context.ModelState.IsValid)
            {
                // Hataları topla
                var errors = context.ModelState.Values
                    .Where(v => v.Errors.Count > 0)
                    .SelectMany(v => v.Errors)
                    .Select(v => v.ErrorMessage)
                    .ToList();

                // Bizim standart formatımızda cevap oluştur
                var responseModel = ServiceResult.Failure(errors);

                // İsteği kes ve cevabı dön (Controller'a hiç girmez)
                context.Result = new BadRequestObjectResult(responseModel);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // İşlem bittikten sonra bir şey yapmamıza gerek yok.
        }
    }
}
