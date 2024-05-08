using Flocus.Domain.Interfacesl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flocus.Repository.Services
{
    internal class RepositoryService : IRepositoryService
    {
        public async Task CreateDbUserAsync(string username, string passwordHash, bool AdminRights)
        {
            
        }
    }
}
