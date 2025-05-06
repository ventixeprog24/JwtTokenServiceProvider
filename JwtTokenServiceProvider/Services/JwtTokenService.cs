using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using System.Text;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;

namespace JwtTokenServiceProvider.Services;

public class JwtTokenService(IConfiguration configuration, JwtSecurityTokenHandler tokenHandler, TokenValidationParameters validationParameters) : JwtTokenServiceContract.JwtTokenServiceContractBase
{
    private readonly IConfiguration _configuration = configuration;
    private readonly JwtSecurityTokenHandler _tokenHandler = tokenHandler;
    private readonly TokenValidationParameters _validationParameters = validationParameters;
    
    public override Task<TokenReply> GenerateToken(TokenRequest request, ServerCallContext context)
    {
        try
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            var issuer = _configuration["Jwt:Issuer"]!;

            List<Claim> claims = new()
            {
                new(ClaimTypes.NameIdentifier, request.ServiceName),
            };

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Issuer = issuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
        
            JwtSecurityTokenHandler tokenHandler = new();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Task.FromResult(new TokenReply
            {
                Succeeded = true,
                TokenMessage = tokenHandler.WriteToken(token)
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new TokenReply
            {
                Succeeded = false,
                TokenMessage = ex.Message
            });
        }
    }

    public override Task<ValidateReply> ValidateToken(ValidateRequest request, ServerCallContext context)
    {
        bool ok;

        try
        {
            //ChatGPT told me how to use this method.
            _tokenHandler.ValidateToken(request.Token, _validationParameters, out _);
            ok = true;
        }
        catch (Exception ex)
        {
            ok = false;
        }

        return Task.FromResult(new ValidateReply
        {
            IsTokenOk = ok
        });
    }
}