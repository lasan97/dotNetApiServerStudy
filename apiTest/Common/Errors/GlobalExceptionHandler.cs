using apiTest.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace apiTest.Common.Errors;

// IExceptionHandler의 구현체
public sealed class GlobalExceptionHandler(
    // .NET 표준 로거
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    // 요청 파이프라인에서 처리되지 않은 예외가 올라오면
    // UseExceptionHandler 미들웨어가 이 메서드를 호출
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = CreateProblemDetails(httpContext, exception);

        LogException(exception, problemDetails.Status);

        // 처리한 모든 예외 응답은 ProblemDetails 으로 통일
        httpContext.Response.StatusCode = problemDetails.Status
                                          ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        Exception exception)
    {
        var traceId = httpContext.TraceIdentifier;

        return exception switch
        {
            // AppException은 애플리케이션에서 예상 가능한 에러
            // 서비스 계층은 404, 409처럼 클라이언트에 알려도 되는 실패를 표현
            AppException appException => new ProblemDetails
            {
                Status = appException.StatusCode,
                Title = appException.Title,
                Detail = appException.Detail,
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["code"] = appException.Code,
                    ["traceId"] = traceId
                }
            },
            // 예상하지 못한 예외의 상세 내용은 클라이언트에 노출하지 않음
            // 상세 원인은 로그에 남기고, 응답에는 안정적인 에러 코드와 traceId만 내려줌
            // default 대신 _ 사용
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal server error",
                Detail = "An unexpected error occurred.",
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["code"] = "internal_server_error",
                    ["traceId"] = traceId
                }
            }
        };
    }

    private void LogException(Exception exception, int? statusCode)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception occurred.");
            return;
        }

        if (exception is ConflictException)
        {
            logger.LogWarning(exception, "Handled conflict exception occurred.");
        }
    }
}