using Microsoft.AspNetCore.Mvc;
using MSABackendTemplate.Application.Wrappers;

namespace MSABackendTemplate.WebAPI.Factories;

/// <summary>
/// Factory for creating IActionResult responses based on ServiceResult.
/// Uses Dictionary-based strategy to eliminate if statements.
/// Follows Strategy Pattern and Open/Closed Principle.
/// </summary>
public static class ActionResultFactory
{
    // Dictionary mapping status codes to action result creators
    private static readonly Dictionary<int, Func<object, ControllerBase, IActionResult>> _successHandlers = new()
    {
        { 200, (data, controller) => controller.Ok(data) },
        { 201, (data, controller) => controller.Created("", data) },
        { 204, (_, controller) => controller.NoContent() }
    };

    /// <summary>
    /// Creates an IActionResult from a ServiceResult (non-generic).
    /// </summary>
    public static IActionResult CreateResult(ServiceResult result, ControllerBase controller)
    {
        if (!result.IsSuccess)
            return CreateProblemDetails(result, controller);

        return CreateSuccessResult(result.StatusCode, result, controller);
    }

    /// <summary>
    /// Creates an IActionResult from a ServiceResult<T> (generic).
    /// </summary>
    public static IActionResult CreateResult<T>(ServiceResult<T> result, ControllerBase controller)
    {
        if (!result.IsSuccess)
            return CreateProblemDetails(result, controller);

        return CreateSuccessResult(result.StatusCode, result, controller);
    }

    /// <summary>
    /// Creates success result using dictionary lookup (eliminates if/switch statements).
    /// </summary>
    private static IActionResult CreateSuccessResult(int statusCode, object data, ControllerBase controller)
    {
        // Dictionary lookup instead of if/switch - O(1) complexity
        return _successHandlers.TryGetValue(statusCode, out var handler)
            ? handler(data, controller)
            : controller.StatusCode(statusCode, data); // Default fallback
    }

    /// <summary>
    /// Creates ProblemDetails result for errors.
    /// </summary>
    private static IActionResult CreateProblemDetails(ServiceResult result, ControllerBase controller)
    {
        var hasErrors = result.Errors?.Count > 0;

        return hasErrors
            ? controller.Problem(
                detail: string.Join(", ", result.Errors!),
                statusCode: result.StatusCode,
                title: "One or more errors occurred.")
            : controller.StatusCode(result.StatusCode, result);
    }
}
