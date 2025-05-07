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
    ServiceName = "MailService"
});

// Validate Token
var valReply = await client.ValidateTokenAsync(new ValidateRequest {
    Token = genReply.TokenMessage
});
```
