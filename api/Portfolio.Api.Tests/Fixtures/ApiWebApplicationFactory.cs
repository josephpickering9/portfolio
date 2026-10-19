using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Portfolio.Api.Services;
using Xunit;

namespace Portfolio.Api.Tests.Fixtures;

public abstract class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _webRoot = Directory.CreateTempSubdirectory("portfolio-tests-").FullName;
    private string? _connectionString;

    public async Task InitializeAsync() => _connectionString = await TestDatabase.CreateAsync();

    async Task IAsyncLifetime.DisposeAsync()
    {
        await DisposeAsync();
        if (_connectionString is not null) await TestDatabase.DropAsync(_connectionString);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = _connectionString
            ?? throw new InvalidOperationException("The test database is created in InitializeAsync, before the host is built.");

        builder.UseWebRoot(_webRoot);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<Database>>();
            services.RemoveAll<IDbContextOptionsConfiguration<Database>>();
            services.AddDbContext<Database>(options => options.UseNpgsql(connectionString));

            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = null;
                options.Configuration = new OpenIdConnectConfiguration { Issuer = TestJwt.Issuer };
                options.TokenValidationParameters.ValidIssuer = TestJwt.Issuer;
                options.TokenValidationParameters.ValidAudience = TestJwt.Audience;
                options.TokenValidationParameters.IssuerSigningKey = TestJwt.SigningKey;
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && Directory.Exists(_webRoot)) Directory.Delete(_webRoot, true);
    }
}
