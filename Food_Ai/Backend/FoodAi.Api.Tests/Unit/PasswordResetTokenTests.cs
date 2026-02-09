using FoodAi.Api.Services;
using FluentAssertions;

namespace FoodAi.Api.Tests.Unit;

public sealed class PasswordResetTokenTests
{
    [Fact]
    public void HashToken_is_deterministic_and_verifiable()
    {
        var token = "sample-reset-token";

        var hash = PasswordResetTokenHasher.HashToken(token);

        hash.Should().NotBeNullOrWhiteSpace();
        hash.Should().NotBe(token);
        PasswordResetTokenHasher.Verify(token, hash).Should().BeTrue();
        PasswordResetTokenHasher.Verify("wrong", hash).Should().BeFalse();
    }
}
