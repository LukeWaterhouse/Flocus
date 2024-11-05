using Flocus.Domain.Interfaces;
using Flocus.Identity.Interfaces;
using Flocus.Identity.Interfaces.AdminKeyInterfaces;
using Flocus.Identity.Interfaces.RegisterInterfaces;
using Flocus.Identity.Models;
using BC = BCrypt.Net.BCrypt;

namespace Flocus.Identity.Services.RegistrationServices;

public sealed class RegistrationService : IRegistrationService
{
    private readonly IRegistrationValidationService _registerValidationService;
    private readonly IUserRepositoryService _userRepositoryService;
    private readonly IAdminKeyService _checkAdminKeyService;

    public RegistrationService(
        IRegistrationValidationService registerValidationService,
        IUserRepositoryService userRepositoryService,
        IAdminKeyService checkAdminKeyService)
    {
        _registerValidationService = registerValidationService;
        _userRepositoryService = userRepositoryService;
        _checkAdminKeyService = checkAdminKeyService;
    }

    public async Task RegisterAsync(RegistrationModel registrationModel)
    {
        _registerValidationService.InputValidateRegistrationModel(registrationModel);

        if (registrationModel.IsAdmin)
        {
            _checkAdminKeyService.CheckAdminKeyCorrect(registrationModel.Key);
        }

        string passwordHash = BC.HashPassword(registrationModel.Password);
        await _userRepositoryService.CreateDbUserAsync(
            registrationModel.Username,
            passwordHash,
            registrationModel.EmailAddress,
            registrationModel.IsAdmin);
    }
}
