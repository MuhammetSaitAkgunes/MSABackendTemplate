using FluentAssertions;
using MSABackendTemplate.Domain.Common;
using Xunit;

namespace MSABackendTemplate.Tests.Domain.Common;

/// <summary>
/// Unit tests for Guard helper class.
/// Tests validation methods for input preconditions.
/// </summary>
public class GuardTests
{
    [Fact]
    public void AgainstNull_ShouldThrowArgumentNullException_WhenValueIsNull()
    {
        // Arrange
        object? nullValue = null;

        // Act
        Action act = () => Guard.AgainstNull(nullValue, nameof(nullValue));

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*nullValue*");
    }

    [Fact]
    public void AgainstNull_ShouldNotThrow_WhenValueIsNotNull()
    {
        // Arrange
        var validValue = "test";

        // Act
        Action act = () => Guard.AgainstNull(validValue, nameof(validValue));

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AgainstNullOrWhiteSpace_ShouldThrowArgumentException_WhenStringIsNullOrWhiteSpace(string? value)
    {
        // Act
        Action act = () => Guard.AgainstNullOrWhiteSpace(value, nameof(value));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be null or whitespace*");
    }

    [Fact]
    public void AgainstNullOrWhiteSpace_ShouldNotThrow_WhenStringIsValid()
    {
        // Arrange
        var validString = "test";

        // Act
        Action act = () => Guard.AgainstNullOrWhiteSpace(validString, nameof(validString));

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void AgainstNegativeOrZero_ShouldThrowArgumentException_WhenValueIsNotPositive(int value)
    {
        // Act
        Action act = () => Guard.AgainstNegativeOrZero(value, nameof(value));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*must be greater than zero*");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public void AgainstNegativeOrZero_ShouldNotThrow_WhenValueIsPositive(int value)
    {
        // Act
        Action act = () => Guard.AgainstNegativeOrZero(value, nameof(value));

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(0.0)]
    public void AgainstNegativeOrZeroDecimal_ShouldThrowArgumentException_WhenValueIsNotPositive(decimal value)
    {
        // Act
        Action act = () => Guard.AgainstNegativeOrZero(value, nameof(value));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*must be greater than zero*");
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(99.99)]
    public void AgainstNegativeOrZeroDecimal_ShouldNotThrow_WhenValueIsPositive(decimal value)
    {
        // Act
        Action act = () => Guard.AgainstNegativeOrZero(value, nameof(value));

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void AgainstOutOfRange_ShouldThrowArgumentOutOfRangeException_WhenValueBelowMin()
    {
        // Arrange
        int value = 5;
        int min = 10;
        int max = 20;

        // Act
        Action act = () => Guard.AgainstOutOfRange(value, min, max, nameof(value));

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*must be between 10 and 20*");
    }

    [Fact]
    public void AgainstOutOfRange_ShouldThrowArgumentOutOfRangeException_WhenValueAboveMax()
    {
        // Arrange
        int value = 25;
        int min = 10;
        int max = 20;

        // Act
        Action act = () => Guard.AgainstOutOfRange(value, min, max, nameof(value));

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public void AgainstOutOfRange_ShouldNotThrow_WhenValueWithinRange(int value)
    {
        // Arrange
        int min = 10;
        int max = 20;

        // Act
        Action act = () => Guard.AgainstOutOfRange(value, min, max, nameof(value));

        // Assert
        act.Should().NotThrow();
    }
}
