namespace NBlog.Api.Exceptions;

[Serializable]
public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException() {}

    public ResourceNotFoundException(string message) : base(message) {}

    public ResourceNotFoundException(string message, Exception inner) : base(message, inner) {}
}