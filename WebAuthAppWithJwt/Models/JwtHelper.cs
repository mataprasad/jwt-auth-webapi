using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Security;
using System.Security.Principal;
using System.Threading;

namespace WebAuthAppWithJwt.Models
{
    public static class JwtHelper
    {
        public static readonly byte[] _signInKey = Encoding.UTF8.GetBytes("MbQeThWmZq4t7w!z");
        public static string CreateJwtToken(LoginRequest request)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, request.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, request.UserName));
            claims.Add(new Claim(ClaimTypes.Role, request.Role));
            var id = new ClaimsIdentity(claims);
            var h = new JwtSecurityTokenHandler();
            var token = h.CreateToken(new SecurityTokenDescriptor()
            {
                Expires = DateTime.UtcNow.AddDays(1),
                Subject = id,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_signInKey), SecurityAlgorithms.HmacSha256)
            }); 
            return h.WriteToken(token);
        }

        public static IPrincipal ValidateJwtToken(string token)
        {
            var h = new JwtSecurityTokenHandler();
            h.ValidateToken(token, new TokenValidationParameters()
            {
                ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(_signInKey),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true
            }, out var securityToken);
            var jwt = securityToken as JwtSecurityToken;
            var id = new ClaimsIdentity(jwt.Claims, "jwt", "name", "role");
            return new ClaimsPrincipal(id);
        }

        public static void AuthenticateRequest()
        {
            try
            {
                var token = HttpContext.Current.Request.Headers.Get("Authorization");
                var principal = ValidateJwtToken(token);
                HttpContext.Current.User = principal;
                Thread.CurrentPrincipal = principal;
            }
            catch { }
        }
    }
}