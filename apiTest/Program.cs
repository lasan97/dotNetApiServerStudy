using apiTest.Common.Data;
using apiTest.Extensions;
using apiTest.Services;

var builder = WebApplication.CreateBuilder(args);

// Controller 등록
builder.Services.AddControllers();

// Service 등록
builder.Services.AddApplicationServices();

// DB 등록
builder.Services.AddDatabase(builder.Configuration);

// Swagger 등록
builder.Services.AddSwaggerDocs();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocs();
}

app.MapControllers();

app.Run();