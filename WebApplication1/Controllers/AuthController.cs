 using Azure.Core;
using Gallery.Data;
using Gallery.Dto;
using Gallery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
 

namespace Gallery.Controllers
{
  
        public class AuthController : ControllerBase
        {

            private readonly IConfiguration _configuration;
            private readonly ApplicationDbContext _ctx;

            public AuthController(IConfiguration configuration, ApplicationDbContext dataContext)
            {
                _configuration = configuration;
                _ctx = dataContext;
            }


            [HttpPost("Auth/Register")]
            public async Task<ActionResult<string>> Register(UserDto request)
            {
                var user = new ApplicationUser();

                CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);


                if (_ctx.Users.Any(u => u.Username == request.Username))
                {
                    return BadRequest("User already exist.");
                }
                user.Username = request.Username;
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Role = "user";
                await _ctx.AddAsync(user);


                string token = CreateToken(user);
                var refreshToken = GenerateRefreshToken();
                SetRefreshToken(refreshToken, user);


                await _ctx.SaveChangesAsync();
                return Ok(token);
            }

            [HttpPost("Auth/Login")]
            public async Task<ActionResult<string>> Login(UserDto request)
            {

                var user = _ctx.Users.FirstOrDefaultAsync(u => u.Username == request.Username).Result;


                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return BadRequest("Wrong password.");
                }

                string token = CreateToken(user);

                //var refreshToken = GenerateRefreshToken();
                //SetRefreshToken(refreshToken, user);

                return Ok(token);
            }



            [HttpPost("refresh-token")]
            public async Task<ActionResult<string>> RefreshToken(int userid)
            {
                var user = _ctx.Users.FirstOrDefaultAsync(u => u.Id == userid).Result;


                var refreshToken = Request.Cookies["refreshToken"];

                if (!user.RefreshToken.Equals(refreshToken))
                {
                    return Unauthorized("Invalid Refresh Token.");
                }
                else if (user.TokenExpires < DateTime.Now)
                {
                    return Unauthorized("Token expired.");
                }

                string token = CreateToken(user);
                var newRefreshToken = GenerateRefreshToken();
                SetRefreshToken(newRefreshToken, user);

                return Ok(refreshToken);
            }

            private RefreshToken GenerateRefreshToken()
            {
                var refreshToken = new RefreshToken
                {
                    Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                    Expires = DateTime.Now.AddDays(7),
                    Created = DateTime.Now
                };

                return refreshToken;
            }

            private void SetRefreshToken(RefreshToken newRefreshToken, ApplicationUser user)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = newRefreshToken.Expires
                };
                Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

                user.RefreshToken = newRefreshToken.Token;
                user.TokenCreated = newRefreshToken.Created;
                user.TokenExpires = newRefreshToken.Expires;
            }
            private string CreateToken(ApplicationUser user)
            {

                List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
               new Claim("UserId", user.Id.ToString())
        };

                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                    _configuration.GetSection("AppSettings:Token").Value));
                Console.WriteLine(_configuration.GetSection("AppSettings:Token").Value);
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds);

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                return jwt;
            }
         

            private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
            {
                using (var hmac = new HMACSHA256())
                {
                    passwordSalt = hmac.Key;
                    passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                }
            }

            private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
            {
                using (var hmac = new HMACSHA256(passwordSalt))
                {

                    var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    return computedHash.SequenceEqual(passwordHash);
                }
            }
    
    }

    }
 
