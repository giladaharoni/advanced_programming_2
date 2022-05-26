﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using advanced_programming_2.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Post(string username, string password)
        {
            ///need to validate the user name.
            if (true)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["JWTParams: Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("userId",username)
                };

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTParams:SecretKey"]));
                var mac = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["JWTParams:Issuer"],
                    _configuration["JWTParams:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(100),
                    signingCredentials: mac
                    );
                return Ok(new JwtSecurityTokenHandler().WriteToken(token));

            }
        }



    }
}