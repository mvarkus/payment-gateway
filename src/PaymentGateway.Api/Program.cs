using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Options;

using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Options;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options => options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider(JsonNamingPolicy.SnakeCaseLower)))
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower)
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var responseBody = new ProblemDetails
            {
               Title = "Invalid Payload",
               Status = StatusCodes.Status400BadRequest
            };

            responseBody.Extensions.TryAdd(
                "errors",
                context.ModelState.ToDictionary(x => x.Key, x =>  x.Value!.Errors.Select(e => e.ErrorMessage).ToList()));

            return new BadRequestObjectResult(responseBody);
        };
    }); ;

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPaymentsService, PaymentsService>();
builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddHttpClient<IAcquiringBankService, MountebankAcquiringBankService>((serviceProvider, client) =>
{
    var config = serviceProvider.GetRequiredService<IOptions<MountebankOption>>().Value;
    client.BaseAddress = new Uri(config.BaseAddress);
    client.Timeout = TimeSpan.FromSeconds(config.TimeoutInSeconds);
});

builder.Services.Configure<MountebankOption>(builder.Configuration.GetSection(MountebankOption.SectionName));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();
