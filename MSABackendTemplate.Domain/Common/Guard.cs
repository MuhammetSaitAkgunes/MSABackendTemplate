namespace MSABackendTemplate.Domain.Common;

/// <summary>
/// Centralized guard clause helper for validation.
/// Reduces if statement duplication and follows DRY principle.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Guards against null values.
    /// </summary>
    public static void AgainstNull<T>(T? value, string paramName, string? message = null)
        where T : class
    {
        if (value == null)
            throw new ArgumentNullException(paramName, message ?? $"{paramName} cannot be null.");
    }

    /// <summary>
    /// Guards against null or empty strings.
    /// </summary>
    public static void AgainstNullOrEmpty(string? value, string paramName, string? message = null)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException(message ?? $"{paramName} cannot be null or empty.", paramName);
    }

    /// <summary>
    /// Guards against null or whitespace strings.
    /// </summary>
    public static void AgainstNullOrWhiteSpace(string? value, string paramName, string? message = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(message ?? $"{paramName} cannot be null or whitespace.", paramName);
    }

    /// <summary>
    /// Guards against negative or zero numeric values.
    /// </summary>
    public static void AgainstNegativeOrZero(decimal value, string paramName, string? message = null)
    {
        if (value <= 0)
            throw new ArgumentException(message ?? $"{paramName} must be greater than zero.", paramName);
    }

    /// <summary>
    /// Guards against negative or zero integer values.
    /// </summary>
    public static void AgainstNegativeOrZero(int value, string paramName, string? message = null)
    {
        if (value <= 0)
            throw new ArgumentException(message ?? $"{paramName} must be greater than zero.", paramName);
    }

    /// <summary>
    /// Guards against negative values.
    /// </summary>
    public static void AgainstNegative(int value, string paramName, string? message = null)
    {
        if (value < 0)
            throw new ArgumentException(message ?? $"{paramName} cannot be negative.", paramName);
    }

    /// <summary>
    /// Guards against invalid operations based on a condition.
    /// </summary>
    public static void AgainstInvalidOperation(bool invalidCondition, string message)
    {
        if (invalidCondition)
            throw new InvalidOperationException(message);
    }

    /// <summary>
    /// Guards against values outside of a specified range.
    /// </summary>
    public static void AgainstOutOfRange<T>(T value, T min, T max, string paramName, string? message = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(
                paramName,
                value,
                message ?? $"{paramName} must be between {min} and {max}.");
    }
}
