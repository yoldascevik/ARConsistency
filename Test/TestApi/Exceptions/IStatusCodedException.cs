namespace TestApi.Exceptions
{
    public interface IStatusCodedException
    {
        int StatusCode { get; }
    }
}