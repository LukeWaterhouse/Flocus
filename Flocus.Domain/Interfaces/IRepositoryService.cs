using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flocus.Domain.Interfacesl;

public interface IRepositoryService
{
    Task CreateDbUserAsync(string username, string passwordHash, bool AdminRights);
}
