using apiTest.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    "appsettings.Local.json",
    optional: true,
    reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Controller 등록
builder.Services.AddControllers();

// Service 등록
builder.Services.AddApplicationServices();

// DB 등록
builder.Services.AddDatabase(builder.Configuration);

// ErrorHandler 등록
builder.Services.AddErrorHandling();

// Swagger 등록
builder.Services.AddSwaggerDocs();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocs();
}

app.UseErrorHandling();
app.MapControllers();

app.Run();