using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Core.Entities;
using ToDoApp.Core.Interfaces;
using ToDoApp.Infrastructure.Data;

namespace ToDoApp.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ToDoDbContext _db;

        private readonly IJwtService _jwtService;

        private readonly ILogger<AuthService> _logger;

        public AuthService(ToDoDbContext db, IJwtService jwtService, ILogger<AuthService> logger) 
        { 
            _db = db;
            _jwtService = jwtService;
            _logger = logger;
        
        }

        public async Task<bool> RegisterAsync(string userName, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) 
                || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            

            bool userExist = await _db.Users.AnyAsync(u => u.UserName == userName);

            bool emailExist = await _db.Users.AnyAsync(e => e.Email == email);

            if (userExist || emailExist)
            {
                return false;
            } else
            {
                User newUser = new User
                {
                    UserName = userName,
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    RegisteredAt = DateTime.UtcNow

                };

                _db.Users.Add(newUser);
                await _db.SaveChangesAsync();

                _logger.LogInformation("New user registered: {UserName} ({Email})", userName, email);

                return true;
            }
           
        }

        public async Task<string> LoginAsync(string userName, string password) 
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return null;
            }

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!isPasswordCorrect)
            {
                return null;
            }


                
            var token = _jwtService.GenerateToken(user.Id, user.UserName);

            _logger.LogInformation("User {UserName} logged in", userName);

            return token;
           
        }
    }
}
