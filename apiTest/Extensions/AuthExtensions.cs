using apiTest.Common.Data;
using apiTest.Services.Auth;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace apiTest.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddAuth(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        services.AddAuthorization();

        services.AddOpenIddict()
            .AddCore(options =>
            {
                // OpenIddict의 client/token 도 기존 AppDbContext 사용
                options.UseEntityFrameworkCore()
                    .UseDbContext<AppDbContext>();
            })
            .AddServer(options =>
            {
                options.SetTokenEndpointUris("/connect/token");

                // password, refresh grant만 허용
                options.AllowPasswordFlow()
                    .AllowRefreshTokenFlow();

                options.RegisterScopes(AuthConstants.ApiScope, Scopes.Email, Scopes.Profile);

                // 개발용 임시 키
                options.AddEphemeralEncryptionKey()
                    .AddEphemeralSigningKey();

                var aspNetCoreBuilder = options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough();

                if (environment.IsDevelopment())
                {
                    // 로컬 http 프로필 테스트용 설정
                    aspNetCoreBuilder.DisableTransportSecurityRequirement();
                }
            })
            .AddValidation(options =>
            {
                // OpenIddict 서버 설정과 키를 재사용해 access token을 검증
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        services.AddHostedService<OpenIddictApplicationSeeder>();

        return services;
    }
}