﻿namespace Flocus.Identity.Interfaces;

public interface IIdentityService
{
    Task RegisterAsync(string username, string password, string emailAddress, bool isAdmin, string? key);
    Task<string> GetAuthTokenAsync(string username, string password);
    Task DeleteUser(string username, string password);
}