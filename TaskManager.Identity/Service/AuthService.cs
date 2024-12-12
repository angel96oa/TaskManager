using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
namespace TaskManager.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<string> AuthenticateAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var token = await _tokenService.GenerateJwtTokenAsync(user);

            return token;
        }

        public async Task<IdentityResult> RegisterUserAsync(string username, string password)
        {
            // Crea una instancia de un nuevo usuario
            var user = new ApplicationUser
            {
                UserName = username,
                PasswordHash = password
            };

            // Crea el usuario en la base de datos y aplica el hash a la contraseña
            var result = await _userManager.CreateAsync(user, password);

            // Devuelve el resultado de la creación del usuario
            return result;
        }
    }

}
