using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace BrassLoon.JwtUtility
{
    public static class JwtSecurityTokenUtility
    {
        public static string CreateJwtId() => Guid.NewGuid().ToString("N");

        public static JwtSecurityToken Create(
            string tknCsp,
            string issuer,
            string audience,
            IEnumerable<Claim> claims,
            Func<DateTime> expiration)
        => Create(tknCsp, issuer, audience, claims, expiration, CreateJwtId);

        public static JwtSecurityToken Create(
            string tknCsp,
            string issuer,
            string audience,
            IEnumerable<Claim> claims,
            Func<DateTime> expiration,
            Func<string> jwtId)
        {
            RsaSecurityKey securityKey = RsaSecurityKeySerializer.GetSecurityKey(tknCsp, true);
            return Create(securityKey, issuer, audience, claims, expiration, jwtId);
        }

        public static JwtSecurityToken Create(
            RsaSecurityKey securityKey,
            string issuer,
            string audience,
            IEnumerable<Claim> claims,
            Func<DateTime> expiration) => Create(securityKey, issuer, audience, claims, expiration, CreateJwtId);

        public static JwtSecurityToken Create(
            RsaSecurityKey securityKey,
            string issuer,
            string audience,
            IEnumerable<Claim> claims,
            Func<DateTime> expiration,
            Func<string> jwtId)
        {
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha512);
            if (claims == null)
                claims = new List<Claim>();
            claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, jwtId())
            }
            .Concat(claims);
            return new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expiration(),
                signingCredentials: credentials);
        }

        public static string Write(JwtSecurityToken jwtSecurityToken) => new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}
