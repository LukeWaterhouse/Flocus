using Flocus.Domain.Interfaces;
using Flocus.Domain.Interfacesl;
using BC = BCrypt.Net.BCrypt;

namespace Flocus.Domain.Services
{
    internal class UserService : IUserService
    {
        private readonly IRepositoryService _repositoryService;
        private readonly string adminKey = "123123";

        public UserService(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }   

        public async Task CreateUserAsync(string username, string password, bool isAdmin, string? key)
        {
            if(isAdmin && adminKey != key)
            {
                throw new Exception("admin key was not correct");
            }
            string passwordHash = BC.HashPassword(password);

            try
            {
                await _repositoryService.CreateDbUserAsync(username, passwordHash, isAdmin);
            }
            catch(Exception ex) 
            { 

            }
        }

        public async Task LoginUserAsync(string username, string password)
        {

                    
        }
    }
}
