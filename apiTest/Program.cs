var builder = WebApplication.CreateBuilder(args);

// Controller 등록
builder.Services.AddControllers();

// Swagger 등록
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// 이것은 Minimal API
app.MapGet("/", () =>
{
    return Results.Ok("Hello World!");
});

app.Run();