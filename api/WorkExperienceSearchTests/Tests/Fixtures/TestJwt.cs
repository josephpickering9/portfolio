using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace WorkExperienceSearchTests.Tests.Fixtures;

public static class TestJwt
{
    public const string Issuer = "https://work-experience-tests.local/";
    public const string Audience = "work-experience-tests";
    private const string Subject = "test-admin";

    public static readonly SymmetricSecurityKey SigningKey = CreateKey("work-experience-test-signing-key-0123456789");

    public static string Create(DateTime? expires = null, SecurityKey? signingKey = null, string audience = Audience)
    {
        var expiresAt = expires ?? DateTime.UtcNow.AddMinutes(10);
        return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = audience,
            Subject = new ClaimsIdentity([new Claim(JwtRegisteredClaimNames.Sub, Subject)]),
            NotBefore = expiresAt.AddHours(-1),
            IssuedAt = expiresAt.AddHours(-1),
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(signingKey ?? SigningKey, SecurityAlgorithms.HmacSha256),
        });
    }

    public static SymmetricSecurityKey CreateKey(string secret) => new(Encoding.UTF8.GetBytes(secret));
}
