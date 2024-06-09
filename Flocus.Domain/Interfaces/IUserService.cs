using Flocus.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flocus.Domain.Interfaces;

public interface IUserService
{
    Task<User> GetUserAsync(string username);
}
