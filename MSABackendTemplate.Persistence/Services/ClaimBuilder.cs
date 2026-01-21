using System.Security.Claims;

namespace MSABackendTemplate.Persistence.Services;

/// <summary>
/// Fluent API builder for creating JWT claims.
/// Eliminates conditional logic and improves readability.
/// Follows Builder Pattern and Fluent Interface design patterns.
/// </summary>
public class ClaimBuilder
{
    private readonly List<Claim> _claims = new();

    /// <summary>
    /// Adds a mandatory claim (value must not be null).
    /// </summary>
    public ClaimBuilder AddMandatory(string type, string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException($"Mandatory claim '{type}' cannot be null or empty.", nameof(value));

        _claims.Add(new Claim(type, value));
        return this;
    }

    /// <summary>
    /// Adds an optional claim (only if value is not null or empty).
    /// Automatically handles null/empty checks - no if statements needed in caller.
    /// </summary>
    public ClaimBuilder AddOptional(string type, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            _claims.Add(new Claim(type, value));

        return this;
    }

    /// <summary>
    /// Adds multiple claims from a collection (only if collection is not null or empty).
    /// Automatically handles null/empty checks - no if statements needed in caller.
    /// </summary>
    public ClaimBuilder AddRange(string type, IEnumerable<string>? values)
    {
        if (values?.Any() == true)
            _claims.AddRange(values.Select(v => new Claim(type, v)));

        return this;
    }

    /// <summary>
    /// Adds a claim with a generated unique identifier.
    /// </summary>
    public ClaimBuilder AddUniqueId(string type)
    {
        _claims.Add(new Claim(type, Guid.NewGuid().ToString()));
        return this;
    }

    /// <summary>
    /// Builds and returns the final list of claims.
    /// </summary>
    public List<Claim> Build() => _claims;

    /// <summary>
    /// Returns the number of claims currently in the builder.
    /// </summary>
    public int Count => _claims.Count;
}
