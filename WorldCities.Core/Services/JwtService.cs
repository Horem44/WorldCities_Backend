﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorldCities.Core.Commands.Users.Models;
using WorldCities.Core.Interfaces.Services;
using WorldCities.Domain.Constants;
using WorldCities.Domain.Identity;

namespace WorldCities.Core.Services
{
    public class JwtService : IJwtService
    {
        private IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public UserDto CreateJwtToken(ApplicationUser user)
        {
            DateTime expiration = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration[ConfigurationConstants.JWT_EXPIRATION_MINUTES])
            );

            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Email!),
                new Claim(ClaimTypes.Name, user.PersonName)
            };

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration[ConfigurationConstants.JWT_KEY]!)
            );

            SigningCredentials signingCredentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                _configuration[ConfigurationConstants.JWT_ISSUER],
                _configuration[ConfigurationConstants.JWT_AUDIENCE],
                claims,
                expires: expiration,
                signingCredentials: signingCredentials
            );

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            string jwtToken = jwtSecurityTokenHandler.WriteToken(tokenGenerator);

            return new UserDto()
            {
                UserId = user.Id,
                Token = jwtToken,
                Email = user.Email,
                PersonName = user.PersonName,
                ExpirationTime = expiration,
            };
        }
    }
}
