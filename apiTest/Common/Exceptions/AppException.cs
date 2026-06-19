namespace apiTest.Common.Exceptions;

public abstract class AppException : Exception
{
    protected AppException(
        string title,
        string detail,
        int statusCode,
        string code)
        : base(detail)
    {
        Title = title;
        Detail = detail;
        StatusCode = statusCode;
        Code = code;
    }

    public string Title { get; }
    public string Detail { get; }
    public int StatusCode { get; }
    public string Code { get; }
}