# Payment Gateway API
Payment Gateway API is responsible for validating requests, storing card information and forwarding payment requests and accepting payment responses to and from the acquiring bank.

## How to run

### API

1. Navigate to the project folder.
2. Start bank simulator.
```console
docker compose up -d
```
3. Run PaymentGateway.Api project.
```console
dotnet run --launch-profile PaymentGateway.Api --project .\src\PaymentGateway.Api\PaymentGateway.Api.csproj
```
The project should run on https://localhost:7092 and http://localhost:5067

### Tests
1. Navigate to the project folder.
2. Start bank simulator if not started already.
```console
docker compose up -d
```
3. Test PaymentGateway.Api.Tests project.
```console
dotnet test .\test\PaymentGateway.Api.Tests\PaymentGateway.Api.Tests.csproj
```