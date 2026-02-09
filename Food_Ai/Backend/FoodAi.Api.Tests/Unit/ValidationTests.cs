using FoodAi.Api.Validation;
using FluentAssertions;

namespace FoodAi.Api.Tests.Unit;

public sealed class ValidationTests
{
    [Fact]
    public void IsValidEmail_accepts_valid_email()
    {
        Validators.IsValidEmail("test@example.com").Should().BeTrue();
    }

    [Fact]
    public void IsValidEmail_rejects_invalid_email()
    {
        Validators.IsValidEmail("not-an-email").Should().BeFalse();
    }

    [Fact]
    public void IsValidPassword_requires_minimum_length()
    {
        Validators.IsValidPassword("12345").Should().BeFalse();
        Validators.IsValidPassword("123456").Should().BeTrue();
    }
}
