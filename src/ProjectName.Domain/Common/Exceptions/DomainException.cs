namespace ProjectName.Domain.Common.Exceptions;

/// <summary>
/// Represents a domain exception that can be thrown when a business rule is violated in the domain layer.
/// </summary>
public class DomainException : Exception
{
    public string? PropertyName { get; set; }

    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, string? propertyName) : base(message)
    {
        PropertyName = propertyName;
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public DomainException(string message, string? propertyName, Exception innerException) : base(message, innerException)
    {
        PropertyName = propertyName;
    }
}
