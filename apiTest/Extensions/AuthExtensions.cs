using System.Security.Cryptography;
using System.Text;
using apiTest.Common.Data;
using apiTest.Services.Auth;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace apiTest.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddAuth(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        services.AddAuthorization();

        var signingKey = CreateSigningKey(configuration);
        var encryptionKey = CreateEncryptionKey(configuration);

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
                // options.AddEphemeralEncryptionKey()
                //     .AddEphemeralSigningKey();
                
                options.AddSigningCredentials(new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256))
                    .AddEncryptionCredentials(new EncryptingCredentials(
                    encryptionKey,
                    SecurityAlgorithms.Aes256KW,
                    SecurityAlgorithms.Aes256CbcHmacSha512));

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

    private static RsaSecurityKey CreateSigningKey(IConfiguration configuration)
    {
        const string keyName = "TokenSigningPrivateKey";
        var privateKey = configuration[$"Auth:{keyName}"];

        if (string.IsNullOrWhiteSpace(privateKey))
        {
            throw new InvalidOperationException($"Auth:{keyName} must be configured.");
        }

        var rsa = RSA.Create();

        try
        {
            rsa.ImportFromPem(privateKey.Replace("\\n", "\n"));
        }
        catch (CryptographicException exception)
        {
            rsa.Dispose();
            throw new InvalidOperationException($"Auth:{keyName} must be a valid RSA private key in PEM format.", exception);
        }

        var key = new RsaSecurityKey(rsa);
        key.KeyId = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(privateKey)));

        return key;
    }

    private static SymmetricSecurityKey CreateEncryptionKey(IConfiguration configuration)
    {
        const string keyName = "TokenEncryptionKey";
        var secret = configuration[$"Auth:{keyName}"];

        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException($"Auth:{keyName} must be configured.");
        }

        var secretBytes = Encoding.UTF8.GetBytes(secret);

        if (secretBytes.Length < 32)
        {
            throw new InvalidOperationException($"Auth:{keyName} must be at least 32 bytes.");
        }

        return new SymmetricSecurityKey(SHA256.HashData(secretBytes));
    }
}