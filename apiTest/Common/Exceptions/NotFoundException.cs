using Microsoft.AspNetCore.Http;

namespace apiTest.Common.Exceptions;

public sealed class NotFoundException : AppException
{
    public NotFoundException(string resourceName, object resourceId)
        : base(
            "Resource not found",
            $"{resourceName} with id '{resourceId}' was not found.",
            StatusCodes.Status404NotFound,
            "resource_not_found")
    {
    }
}