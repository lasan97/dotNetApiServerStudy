using apiTest.Services;

var builder = WebApplication.CreateBuilder(args);

// Controller 등록
builder.Services.AddControllers();

// Service 등록
builder.Services.AddScoped<FirstService>();

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

app.Run();