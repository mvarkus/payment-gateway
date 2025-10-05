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

## Key Design Considerations

### Project Structure (Multiple or Single Assembly)
I have considered implementing the project with multiple assemblies (Api, Application, Data) but decided to implement it with single Api assembly to keep the project simple and easier to maintain.

### N-Layer and Abstration
I have decided to use N-Layer architecture and depend on abstractions to improve project's testability and maintainability.

### Cancelation of Create Payment Request
I have decided to avoid cancelling POST `api/payments` requests because we depend on external service within the call. The external service might ignore interrupted HTTP connection and proceed completing the successful request, but on our side we would cancel the task without saving the successful payment to the storage.

## Assumptions

### HTTP server only
I assumed that the project will be used only as a HTTP server. If the requirements would specify that the project needs to implement a way of periodic execution (e.g. queue worker), I would implement the project in multiple assemblies using N-Layer or Clean Architecture.

### Out of Scope
I assumed that following is out of scope for this project:
* Health and readiness endpoints.
* Merchant authentication.
* Authorisation code processing.
* POST `api/payments` request idempotency.
* Circuit breaker and fallback options for bank acquirers.
* Internationalization.
* Docker support. 
