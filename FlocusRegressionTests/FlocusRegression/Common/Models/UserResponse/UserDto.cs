namespace FlocusRegressionTests.Common.Models.UserResponse;

public sealed record UserDto(
    string Username,
    string EmailAddress,
    DateTime CreatedAt);
