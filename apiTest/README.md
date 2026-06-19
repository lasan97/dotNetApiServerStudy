# ASP.NET Core API 서버

ASP.NET Core, EF Core, PostgreSQL 기반 API 서버다.

# 설치 의존성
```bash
# Postgres
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
# EF Core
dotnet add package Microsoft.EntityFrameworkCore.Design
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
