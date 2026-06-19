using apiTest.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace apiTest.Extensions;

// 에러 헨들링
// Spring의 @ControllerAdvice @ExceptionHandler 와 비슷한 역할
public static class ErrorHandlingExtensions
{
    public static IServiceCollection AddErrorHandling(this IServiceCollection services)
    {
        // HTTP API 에러 응답을 RFC 9457 표준 형식(ProblemDetails)으로 반환
        // 참고 - https://www.rfc-editor.org/info/rfc9457/#name-the-problem-details-json-ob
        services.AddProblemDetails();

        // Global Exception
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // API Validation Exception
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed",
                    Detail = "One or more validation errors occurred.",
                    Instance = context.HttpContext.Request.Path,
                };

                problemDetails.Extensions["code"] = "validation_failed";
                problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

                return new BadRequestObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json" }
                };
            };
        });

        return services;
    }

    public static WebApplication UseErrorHandling(this WebApplication app)
    {
        // HTTP 요청 파이프라인에 예외 처리 미들웨어 추가
        app.UseExceptionHandler();

        return app;
    }
}