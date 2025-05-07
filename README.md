# JwtTokenServiceProvider

## How to use it

Make sure you have an exact copy of the proto-file in the client application. Don't forget to register the proto in the project.

### Install following packages in the client:
- Google.Protobuf
- Grpc.Net.Client
- Grpc.Tools

### Example use

```
var channel = new Grpc.Core.Channel("localhost:5000");
var client = new JwtTokenServiceContract.JwtTokenServiceContractClient(channel);

// Generate Token
var genReply = await client.GenerateTokenAsync(new TokenRequest {
<<<<<<< HEAD
    UserId = "123",
    Email = "user@example.com"
=======
    ServiceName = "MailService"
>>>>>>> 03ea5a4a6b50d1e2ea18d29e34cc00f00d0e115d
});

// Validate Token
var valReply = await client.ValidateTokenAsync(new ValidateRequest {
    Token = genReply.TokenMessage
});
```
