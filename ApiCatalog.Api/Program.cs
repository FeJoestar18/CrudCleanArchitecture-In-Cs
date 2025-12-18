using ApiCatalog.Infra;
using ApiCatalog.Application;
using ApiCatalog.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfraServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCorsConfiguration(builder.Configuration);

builder.Services.AddSmartAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddCustomAuthorization();
builder.Services.AddRateLimiterPolicies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCorsConfiguration();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();