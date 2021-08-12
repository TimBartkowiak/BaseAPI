using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BaseAPI.Database;
using BaseAPI.Entities;
using BaseAPI.Exceptions;
using BaseAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BaseAPI
{
    public class AuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly BaseDbContext _dbContext;
        private readonly UserService _userService;

        public AuthenticationService(IConfiguration configuration, BaseDbContext dbContext, UserService userService)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _userService = userService;
        }
        
        public string login(string encodedAuth)
        {
            string[] authParts = decodeAuth(encodedAuth);

            string username = authParts[0];
            string password = authParts[1];

            UserEntity userEntity = _dbContext.Users.FirstOrDefault(entity => entity.Username == username);
            if (userEntity == null)
            {
                throw new UnAuthorizedException();
            }

            if (hashPassword(password, userEntity.Salt) != userEntity.Password)
            {
                throw new UnAuthorizedException();
            }

            return generateToken(username);
        }

        public string register(string encodedAuth)
        {
            string[] authParts = decodeAuth(encodedAuth);
            
            string username = authParts[0];
            string password = authParts[1];

            string newSalt = generateSalt();
            string hashedPassword = hashPassword(password, newSalt);

            UserEntity userEntity = _dbContext.Users.FirstOrDefault(entity => entity.Username == username);
            if (userEntity != null)
            {
                throw new UnAuthorizedException("Username already taken");
            }
            
            _userService.add(new UserModel
            {
                Username = username,
                Password = hashedPassword,
                Salt = newSalt
            });

            return generateToken(username);
        }

        private string generateToken(string username)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] tokenKey = Encoding.ASCII.GetBytes(_configuration["JWT_SECRET"]);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, username)
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);
        }

        private string generateSalt()
        {
            byte[] bytes = new byte[128 / 8];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private string hashPassword(string password, string salt)
        {
            return Convert.ToBase64String(new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(salt), 10000).GetBytes(24));
        }
        
        private string[] decodeAuth(string encodedAuth)
        {
            // Basic user:pass
            string decodedAuth = Encoding.UTF8.GetString(Convert.FromBase64String(encodedAuth.Split(" ")[1]));
            string[] authParts = decodedAuth.Split(":");

            if (authParts.Length < 2)
            {
                throw new UnAuthorizedException();
            }

            foreach (string part in authParts)
            {
                if (string.IsNullOrEmpty(part))
                {
                    throw new UnAuthorizedException();
                }
            }

            return authParts;
        }
    }
}