using System.ComponentModel.DataAnnotations;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.Unit.Models.Requests;

public class PostPaymentRequestTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123412341234")]
    [InlineData("123412341234A")]
    [InlineData("12341234123412341234")]
    public void Validate_ReturnsValidationErrorsWhenCardNumberIsInvalid(string? cardNumber)
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = cardNumber,
            ExpiryMonth = 6,
            ExpiryYear = DateTime.UtcNow.Year + 1,
            Currency = Currency.GBP.ToString(),
            Amount = 1000,
            Cvv = "123"
        };

        var validationContext = new ValidationContext(request);

        // Act
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(request, validationContext, validationResults, true);

        // Assert
        Assert.Single(validationResults);
        Assert.Single(validationResults[0].MemberNames);
        Assert.Equal(nameof(PostPaymentRequest.CardNumber), validationResults[0].MemberNames.First());
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(13)]
    public void Validate_ReturnsValidationErrorsWhenExpiryMonthIsInvalid(int? expiryMonth)
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "12341234123412",
            ExpiryMonth = expiryMonth,
            ExpiryYear = DateTime.UtcNow.Year + 1,
            Currency = Currency.GBP.ToString(),
            Amount = 1000,
            Cvv = "123"
        };

        var validationContext = new ValidationContext(request);

        // Act
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(request, validationContext, validationResults, true);

        // Assert
        Assert.Single(validationResults);
        Assert.Single(validationResults[0].MemberNames);
        Assert.Equal(nameof(PostPaymentRequest.ExpiryMonth), validationResults[0].MemberNames.First());
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void Validate_ReturnsValidationErrorsWhenExpiryYearIsInvalid(int? expiryYear)
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "12341234123412",
            ExpiryMonth = 6,
            ExpiryYear = expiryYear,
            Currency = Currency.GBP.ToString(),
            Amount = 1000,
            Cvv = "123"
        };

        var validationContext = new ValidationContext(request);

        // Act
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(request, validationContext, validationResults, true);

        // Assert
        Assert.Single(validationResults);
        Assert.Single(validationResults[0].MemberNames);
        Assert.Equal(nameof(PostPaymentRequest.ExpiryYear), validationResults[0].MemberNames.First());
    }

    [Fact]
    public void Validate_ReturnsValidationErrorsWhenExpiryDateIsInThePast()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "12341234123412",
            ExpiryMonth = 1,
            ExpiryYear = 1970,
            Currency = Currency.GBP.ToString(),
            Amount = 1000,
            Cvv = "123"
        };

        var validationContext = new ValidationContext(request);

        // Act
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(request, validationContext, validationResults, true);

        // Assert
        Assert.Single(validationResults);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal("expiry_month", validationResults[0].MemberNames.ElementAt(0));
        Assert.Equal("expiry_year", validationResults[0].MemberNames.ElementAt(1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("US")]
    [InlineData("USd")]
    public void Validate_ReturnsValidationErrorsWhenCurrencyIsInvalid(string? currency)
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "12341234123412",
            ExpiryMonth = 6,
            ExpiryYear = DateTime.UtcNow.Year + 1,
            Currency = currency,
            Amount = 1000,
            Cvv = "123"
        };

        var validationContext = new ValidationContext(request);

        // Act
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(request, validationContext, validationResults, true);

        // Assert
        Assert.Single(validationResults);
        Assert.Single(validationResults[0].MemberNames);
        Assert.Equal("currency", validationResults[0].MemberNames.First().ToLower());
    }

    [Theory]
    [InlineData(null)]
    public void Validate_ReturnsValidationErrorsWhenAmountIsInvalid(int? amount)
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "12341234123412",
            ExpiryMonth = 6,
            ExpiryYear = DateTime.UtcNow.Year + 1,
            Currency = Currency.GBP.ToString(),
            Amount = amount,
            Cvv = "123"
        };

        var validationContext = new ValidationContext(request);

        // Act
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(request, validationContext, validationResults, true);

        // Assert
        Assert.Single(validationResults);
        Assert.Single(validationResults[0].MemberNames);
        Assert.Equal(nameof(PostPaymentRequest.Amount), validationResults[0].MemberNames.First());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12")]
    [InlineData("12345")]
    public void Validate_ReturnsValidationErrorsWhenCvvIsInvalid(string? cvv)
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "12341234123412",
            ExpiryMonth = 6,
            ExpiryYear = DateTime.UtcNow.Year + 1,
            Currency = Currency.GBP.ToString(),
            Amount = 1000,
            Cvv = cvv
        };

        var validationContext = new ValidationContext(request);

        // Act
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(request, validationContext, validationResults, true);

        // Assert
        Assert.Single(validationResults);
        Assert.Single(validationResults[0].MemberNames);
        Assert.Equal(nameof(PostPaymentRequest.Cvv), validationResults[0].MemberNames.First());
    }

    [Fact]
    public void Validate_ReturnsNoValidationErrorsWhenDataIsValid()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "12341234123412",
            ExpiryMonth = 6,
            ExpiryYear = DateTime.UtcNow.Year + 1,
            Currency = Currency.GBP.ToString(),
            Amount = 1000,
            Cvv = "123"
        };

        var validationContext = new ValidationContext(request);

        // Act
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(request, validationContext, validationResults, true);

        // Assert
        Assert.Empty(validationResults);
    }
}
