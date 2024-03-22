using System.Text.Json.Serialization;

namespace Shared.Dtos;

public class Response<T> where T : class
{
    public T? Data { get; private set; }
    public int Status { get; private set; } // It is useful in some projects such as mobile apps
    public ErrorDto? Error { get; private set; }    
    public bool IsSuccessful { get; private set; }

    public static Response<T> Success(T data, int statusCode)
    {
        return new Response<T>
        {
            Data = data,
            Status = statusCode,
            IsSuccessful = true
        };
    }

    public static Response<T> Success(int statusCode)
    {
        return new Response<T>
        {
            Status = statusCode,
            IsSuccessful = true
        };
    }

    public static Response<T> Fail(ErrorDto error, int statusCode)
    {
        return new Response<T>
        {
            Status = statusCode,
            Error = error,
            IsSuccessful = false
        };
    }

    public static Response<T> Fail(string errorMessage, int statusCode, bool isShow = true)
    {
        return new Response<T>
        {
            Status = statusCode,
            Error = new ErrorDto(errorMessage, isShow),
            IsSuccessful = false
        };
    }
}
