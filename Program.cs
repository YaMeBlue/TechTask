using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddRouting();
services.AddControllers();
services.AddMemoryCache();

//Swagger
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "HackerNews API",
        Description = "An ASP.NET Core Web API for managing HackerNews items",
    });
});

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.MapControllerRoute(name: "default", pattern: "api/{controller}/{action}");

app.Run();