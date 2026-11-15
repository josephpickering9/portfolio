using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api.Extensions;

namespace Portfolio.Api.Tests.Fixtures;

public static class TestJwt
{
    public const string Issuer = "https://portfolio-tests.local/";
    public const string Audience = "portfolio-tests";
    private const string Subject = "test-admin";

    public static readonly SymmetricSecurityKey SigningKey = CreateKey("portfolio-test-signing-key-0123456789");
    public static readonly string[] AdminPermissions = [AuthExtensions.ManageContentPermission];

    public static string Create(
        DateTime? expires = null,
        SecurityKey? signingKey = null,
        string audience = Audience,
        string[]? permissions = null)
    {
        var expiresAt = expires ?? DateTime.UtcNow.AddMinutes(10);
        var claims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, Subject) };
        claims.AddRange((permissions ?? AdminPermissions).Select(permission => new Claim(AuthExtensions.PermissionsClaim, permission)));

        return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            NotBefore = expiresAt.AddHours(-1),
            IssuedAt = expiresAt.AddHours(-1),
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(signingKey ?? SigningKey, SecurityAlgorithms.HmacSha256),
        });
    }

    public static SymmetricSecurityKey CreateKey(string secret) => new(Encoding.UTF8.GetBytes(secret));
}
