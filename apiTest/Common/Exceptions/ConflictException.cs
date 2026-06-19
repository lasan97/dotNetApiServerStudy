using Microsoft.AspNetCore.Http;

namespace apiTest.Common.Exceptions;

public sealed class ConflictException : AppException
{
    public ConflictException(string detail, string code = "conflict")
        : base("Conflict", detail, StatusCodes.Status409Conflict, code)
    {
    }
}