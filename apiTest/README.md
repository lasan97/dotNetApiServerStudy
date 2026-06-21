# ASP.NET Core API 서버

ASP.NET Core, EF Core, PostgreSQL 기반 API 서버다.

# 설치 의존성
```bash
# Postgres
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
# EF Core
dotnet add package Microsoft.EntityFrameworkCore.Design
# OAuth/OIDC 서버 및 토큰 검증
dotnet add package OpenIddict.EntityFrameworkCore
dotnet add package OpenIddict.Server.AspNetCore
dotnet add package OpenIddict.Validation.AspNetCore
dotnet add package OpenIddict.Validation.ServerIntegration
# https 프로필 테스트시 필요한 의존성
#dotnet dev-certs https --trust
```

# 로컬 실행 설정

DB 커넥션 정보는 각 환경에서 관리 (repo에 포함시키지 않음)
로컬 개발환경의 경우 `appsettings.Local.json`에 두고 사용

```bash
cp appsettings.Local.example.json appsettings.Local.json

dotnet tool restore
dotnet ef database update

dotnet run
```

Rider로 실행해도 `appsettings.Local.json`은 자동으로 읽음 
운영 환경에서는 `appsettings.Local.json` 대신 배포 환경의 Secret 또는 환경 변수를 사용

# 내부 인증 토큰 발급

초기 인증은 OpenIddict 기반의 내부 first-party client만 사용한다.

```bash
curl -X POST http://localhost:5188/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=internal-first-party" \
  -d "username=user@example.com" \
  -d "password=pass" \
  -d "scope=api offline_access"
```

`password` grant는 내부 client 전용이다. 외부 개발자 client를 열 때는 Authorization Code + PKCE 흐름으로 확장한다.
