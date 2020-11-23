using API_Role_Based_Authorization.Entities;
using API_Role_Based_Authorization.Helpers;
using API_Role_Based_Authorization.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API_Role_Based_Authorization.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest request);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }

    public class UserService : IUserService
    {
        private List<User> _userRepository = new List<User>
        {
            new User { Id = 1, Username = "LeAdmin", Password = "admin1337", Role = Role.Admin},
            new User { Id = 2, Username = "pleb", Password = "plebPassword", Role = Role.User},
        };

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest request)
        {
            var user = _userRepository.SingleOrDefault(u => u.Username == request.Username && u.Password == request.Password);

            if (user == null) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return new AuthenticateResponse(user, user.Token);
        }

        public IEnumerable<User> GetAll()
        {
            return _userRepository.WithoutPassword();
        }

        public User GetById(int id)
        {
            var user = _userRepository.FirstOrDefault(u => u.Id == id);

            return user.WithoutPassword();
        }
    }
}
