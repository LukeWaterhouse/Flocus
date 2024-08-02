using Flocus.Domain.Common;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Flocus.Domain.Tests.Common;

public sealed class UtilitiesTests
{
    [Fact]
    public void UnixTimeStampStringToDateTime_ValidTimestamp_ReturnsDateTime()
    {
        // Arrange
        var timestamp = "1722425199";

        // Act
        var result = Utilities.UnixTimeStampStringToDateTime(timestamp);

        // Assert
        result.Should().Be(new DateTime(2024, 07, 31, 11, 26, 39));
    }

    [Fact]
    public void UnixTimeStampStringToDateTime_ValidTimestamp_ThrowsException()
    {
        // Arrange
        var timestamp = "invalid timestamp";

        // Act
        Exception exception = Record.Exception(() =>
        {
            var result = Utilities.UnixTimeStampStringToDateTime(timestamp);
        });

        // Assert
        using (new AssertionScope())
        {
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Invalid Unix timestamp string: 'invalid timestamp' (Parameter 'timestampString')");
        }
    }
}
