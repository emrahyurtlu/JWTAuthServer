namespace Shared.Dtos;

public class Response<T> where T : class
{
    public T? Data { get; private set; }
    public int StatusCode { get; private set; } // It is useful in some projects such as mobile apps
    public ErrorDto? Error { get; private set; }    
    public bool IsSuccessful { get; private set; }

    public static Response<T> Success(T data, int statusCode)
    {
        return new Response<T>
        {
            Data = data,
            StatusCode = statusCode,
            IsSuccessful = true
        };
    }

    public static Response<T> Success(int statusCode)
    {
        return new Response<T>
        {
            StatusCode = statusCode,
            IsSuccessful = true
        };
    }

    public static Response<T> Fail(ErrorDto error, int statusCode)
    {
        return new Response<T>
        {
            StatusCode = statusCode,
            Error = error,
            IsSuccessful = false
        };
    }

    public static Response<T> Fail(string errorMessage, int statusCode, bool isShow = true)
    {
        return new Response<T>
        {
            StatusCode = statusCode,
            Error = new ErrorDto(errorMessage, isShow),
            IsSuccessful = false
        };
    }
}
