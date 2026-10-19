using dotenv.net;
using Portfolio.Api.Extensions;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
builder.AddSerilogLogging();

builder.Services
    .AddDatabase(builder.Configuration)
    .AddRepositories()
    .AddApplicationServices()
    .AddVertexAi(builder.Configuration)
    .AddApiControllers()
    .AddSwaggerDocumentation()
    .AddFrontendCors(builder.Configuration)
    .AddReverseProxySupport()
    .AddAuth0Authentication(builder.Configuration)
    .AddRateLimitPolicies()
    .AddGlobalExceptionHandling();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseSecurityHeaders();
app.UseSwaggerDocumentation();
app.MapHealthEndpoint();
app.UseHttpsRedirection();
app.UseFrontendCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.Run();

public partial class Program;
