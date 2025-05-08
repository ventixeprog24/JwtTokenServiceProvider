# JwtTokenServiceProvider

## How to use it

Make sure you have an exact copy of the proto-file in the client application. Don't forget to register the proto in the project.

### Install following packages in the client:
- Google.Protobuf
- Grpc.Net.Client
- Grpc.Net.ClientFactory
- Grpc.Tools

### Example use

```
var channel = new Grpc.Core.Channel("localhost:5000");
var client = new JwtTokenServiceContract.JwtTokenServiceContractClient(channel);

// Generate Token
var genReply = await client.GenerateTokenAsync(new TokenRequest {
    Email = "user@example.com"
});

// Validate Token
var valReply = await client.ValidateTokenAsync(new ValidateRequest {
    Token = genReply.TokenMessage
});
```

### Possible Return statuses
| Method            |   ReturnStatus   | Message           | Meaning                                                                       |
| ----------------- | :--------------: | ----------------- | ----------------------------------------------------------------------------- |
| **GenerateToken** |  succeeded: true | JWT string        | Token generated successfully and signed with HMAC-SHA256                      |
|                   | succeeded: false | Exception message | Error during token creation (e.g., missing config, encoding issue)            |
| **ValidateToken** |  isTokenOk: true | –                 | Token is well-formed, signature valid, issuer matches, and not expired        |
|                   | isTokenOk: false | –                 | Token invalid due to signature failure, expiry, issuer mismatch, or malformed |
