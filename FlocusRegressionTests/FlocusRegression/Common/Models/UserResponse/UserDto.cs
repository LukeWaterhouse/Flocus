namespace FlocusRegressionTests.Common.Models.UserResponse;

internal sealed record UserDto(
    string Username,
    string EmailAddress,
    DateTime CreatedAt);
