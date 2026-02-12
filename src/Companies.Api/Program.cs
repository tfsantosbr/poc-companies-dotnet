using Companies.Api.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpoints();
builder.Services.AddCorrelation();
builder.Services.AddHealthChecks(configuration);
builder.Services.AddSerilog(configuration);
builder.Services.AddBearerAuthentication(configuration);
builder.Services.ConfigureApiVersioning();
builder.Services.ConfigureJsonOptions();
builder.Services.ConfigureExceptionHandlers(environment);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHealthChecks();
app.MapVersionedEndpoints();
app.MapEnvironmentConfigurations(environment);
app.UseCorrelationContext();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
