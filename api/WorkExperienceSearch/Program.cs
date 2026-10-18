using dotenv.net;
using Work_Experience_Search.Extensions;

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
    .AddAllowAllCors()
    .AddAuth0Authentication(builder.Configuration)
    .AddRateLimitPolicies()
    .AddGlobalExceptionHandling();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.MapHealthEndpoint();
app.UseHttpsRedirection();
app.UseAllowAllCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.Run();

public partial class Program;
