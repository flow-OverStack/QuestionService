using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using QuestionService.Domain.Dtos.ExternalEntity;

namespace QuestionService.Tests.FunctionalTests.Helper;

internal static class TokenHelper
{
    private const string Audience = "TestAudience";
    private const string Issuer = "TestIssuer";
    private const string Kid = "test-key-id";

    private static readonly RsaSecurityKey PrivateKey;

    private static readonly string PublicJwk;

    static TokenHelper()
    {
        var rsa = RSA.Create();
        PrivateKey = new RsaSecurityKey(rsa);
        var publicKey = new RsaSecurityKey(rsa.ExportParameters(false));

        PrivateKey.KeyId = Kid;

        var rsaParams = publicKey.Parameters;

        var modulus = Base64UrlEncode(rsaParams.Modulus!);
        var exponent = Base64UrlEncode(rsaParams.Exponent!);

        var jwks = new
        {
            keys = new[]
            {
                new
                {
                    kty = "RSA",
                    use = "sig",
                    alg = "RS256",
                    kid = Kid,
                    n = modulus,
                    e = exponent
                }
            }
        };

        PublicJwk = JsonConvert.SerializeObject(jwks);
    }

    public static string GetJwk()
    {
        return PublicJwk;
    }

    public static string GetAudience()
    {
        return Audience;
    }

    public static string GetIssuer()
    {
        return Issuer;
    }

    public static string GetRsaTokenWithRoleClaims(string username, long userId, IEnumerable<RoleDto> roles)
    {
        var claims = new Dictionary<string, object>
        {
            { ClaimTypes.Role, roles.Select(x => x.Name).ToList() },
            { ClaimTypes.Name, username },
            { ClaimTypes.NameIdentifier, userId },
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(PrivateKey, SecurityAlgorithms.RsaSha256),
            Audience = Audience,
            Issuer = Issuer
        };

        var jwt = new JwtSecurityToken(
            issuer: tokenDescriptor.Issuer,
            audience: tokenDescriptor.Audience,
            claims: null,
            expires: tokenDescriptor.Expires,
            signingCredentials: tokenDescriptor.SigningCredentials
        );

        foreach (var claim in claims)
        {
            jwt.Payload[claim.Key] = claim.Value;
        }

        var tokenString = tokenHandler.WriteToken(jwt);

        return tokenString;
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}