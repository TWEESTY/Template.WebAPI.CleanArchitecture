namespace ProjectName.Domain.Common.Guards;

using ProjectName.Domain.Common.Exceptions;

/// <summary>
/// Guard clauses for domain validation. Throws DomainException when conditions fail.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Throws DomainException if value is null, empty, or whitespace.
    /// </summary>
    public static string ThrowIfEmptyOrNull(string? value, string propertyName, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message, propertyName);
        }

        return value;
    }

    /// <summary>
    /// Throws DomainException if value is null.
    /// </summary>
    public static T ThrowIfNull<T>(T? value, string propertyName, string message) where T : class
    {
        if (value is null)
        {
            throw new DomainException(message, propertyName);
        }

        return value;
    }

    /// <summary>
    /// Throws DomainException if condition is true.
    /// </summary>
    public static void ThrowIf(bool condition, string propertyName, string message)
    {
        if (condition)
        {
            throw new DomainException(message, propertyName);
        }
    }

    /// <summary>
    /// Throws DomainException if condition is false.
    /// </summary>
    public static void ThrowIfNot(bool condition, string propertyName, string message)
    {
        if (!condition)
        {
            throw new DomainException(message, propertyName);
        }
    }
}
