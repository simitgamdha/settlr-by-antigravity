namespace Settlr.Common.Response;

public class Response<T>
{
    public T? Data { get; set; }
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public string[]? Errors { get; set; }
    public int StatusCode { get; set; }

    public static Response<T> Success(T data, string message = "")
    {
        return new Response<T>
        {
            Data = data,
            Succeeded = true,
            Message = message,
            StatusCode = 200
        };
    }

    public static Response<T> Fail(string message, int statusCode = 400, string[]? errors = null)
    {
        return new Response<T>
        {
            Succeeded = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors
        };
    }
}
