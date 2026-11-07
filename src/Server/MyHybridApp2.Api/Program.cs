using MyHybridApp2.Api.Endpoints;
using MyHybridApp2.Api.Infrastructure;
using MyHybridApp2.Application;
using MyHybridApp2.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");
app.MapPostEndpoints();

app.Run();

// Exposed for WebApplicationFactory in integration tests.
public partial class Program;
