using System.IdentityModel.Tokens.Jwt;
using System.Text;
using JwtTokenServiceProvider;
using JwtTokenServiceProvider.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Tests;

public class JwtTokenService_Tests
{
    //Partly AI generated setup and AI gave me the idea how to create this test. These tests caused me a lot of headaches.
    private readonly JwtTokenService _jwtTokenService;
    
    public JwtTokenService_Tests()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            ["Jwt:Key"] = "Secret-Key-Much-Secret-WOW-amazing",
            ["Jwt:Issuer"] = "Issuer-Key-Much-Secret",
            ["Jwt:Audience"] = "Audience-Key-Much-Secret"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        
        var keyBytes = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
        var signingKey = new SymmetricSecurityKey(keyBytes);
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = signingKey,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
        };
        
        _jwtTokenService = new JwtTokenService(configuration, tokenHandler, validationParameters);
    }

    [Fact]
    public async Task GenerateToken_ReturnsSucceededAndValidToken_WithValidInput()
    {
        //Arrange 
        var request = new TokenRequest { Email = "felix@domain.com" };
        
        //Act
        var result = await _jwtTokenService.GenerateToken(request, context: null!);
        
        //Assert
        Assert.True(result.Succeeded);
        Assert.False(string.IsNullOrEmpty(result.TokenMessage));
    }

    [Fact]
    public async Task GenerateToken_ReturnsFailed_WithoutValidInput()
    {
        //Arrange
        var request = new TokenRequest{ Email = "" };
        
        //Act
        var result = await _jwtTokenService.GenerateToken(request, context: null!);
        
        //Assert
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task ValidateToken_ReturnsSucceededAndValidToken_WithValidInput()
    {
        //Arrange
        var request = new TokenRequest { Email = "felix@domain.com" };
        var arrangeResult = await _jwtTokenService.GenerateToken(request, context: null!);
        var tokenRequest = new ValidateRequest { Token = arrangeResult.TokenMessage };
        
        //Act
        var actResult = await _jwtTokenService.ValidateToken(tokenRequest, context: null!);
        
        //Assert
        Assert.NotNull(actResult);
        Assert.True(actResult.IsTokenOk);
    }

    [Theory]
    [InlineData("")]
    [InlineData("bogus-schmogus")]
    public async Task ValidateToken_ReturnsFailed_WithoutValidInput(string tokenMessage)
    {
        //Arrange
        var tokenRequest = new ValidateRequest { Token = tokenMessage };
        
        //Act
        var actResult = await _jwtTokenService.ValidateToken(tokenRequest, context: null!);
        
        //Assert
        Assert.NotNull(actResult);
        Assert.False(actResult.IsTokenOk);
    }
}