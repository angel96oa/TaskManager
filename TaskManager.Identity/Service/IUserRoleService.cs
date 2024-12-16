using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Identity
{
    public interface IUserRoleService
    {
        Task InitializeRoles();
        Task AssignRoleToUserAsync(string username, string roleName);
    }
}
