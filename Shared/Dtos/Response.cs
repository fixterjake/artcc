using System.Net;

namespace ZDC.Shared.Dtos;

public class Response<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}

public class ResponsePaging<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public int TotalCount { get; set; }
    public int ResultCount { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}