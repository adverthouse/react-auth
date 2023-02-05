using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Adverthouse.Common.Data;
using Adverthouse.Common.Interfaces;
using Adverthouse.Core.Configuration;
using back_end.Domain.Entities;
using back_end.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace back_end.Services
{
    public class MemberService : IMemberService
    {
        private readonly IRepository<Member> _memberRepository;
        private readonly IRepository<RefreshToken> _refreshTokenRepository;

        private readonly AppSettings _appSettings;

        public MemberService(
            AppSettings appSettings, IRepository<Member> memberRepository,
            IRepository<RefreshToken> refreshTokenRepository)
        {
            _appSettings = appSettings;
            _memberRepository = memberRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var member = _memberRepository.FindBy(x => x.Username == model.Username && x.Password == model.Password);

            // return null if member not found
            if (member == null) return null;

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = generateJwtToken(member);
            var refreshToken = generateRefreshToken(ipAddress);

            refreshToken.MemberId = member.MemberId;
            // save refresh token
            _refreshTokenRepository.Add(refreshToken);
            _memberRepository.Update(member);
            

            return new AuthenticateResponse(member, jwtToken, refreshToken.Token);
        }

     public AuthenticateResponse RefreshJWTToken(string token, string ipAddress)
     {
            var refreshToken = _refreshTokenRepository.FindBy(x => x.Token == token);

            if (refreshToken == null) return null; 
            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            newRefreshToken.MemberId = refreshToken.MemberId;


            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
           

            _refreshTokenRepository.Update(refreshToken);
            _refreshTokenRepository.Add(newRefreshToken);

            var member = _memberRepository.FindBy(u => u.MemberId == refreshToken.MemberId);
         
            // generate new jwt
            var jwtToken = generateJwtToken(member);

            return new AuthenticateResponse(member, jwtToken, newRefreshToken.Token);
        }


        public bool RevokeToken(string token, string ipAddress)
        {
            var refreshToken = _refreshTokenRepository.FindBy(x => x.Token == token); // memberID kontrolü'de yapılacak

           if (!refreshToken.IsActive) return false;

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _refreshTokenRepository.Update(refreshToken);

            return true;
        }
 
        public IEnumerable<RefreshToken> GetAllTokens(){
            return _refreshTokenRepository.GetResult();
        }

        public IEnumerable<Member> GetAll()
        {
            return _memberRepository.GetResult();
        }

        public Member GetById(int id)
        {
            return _memberRepository.FindBy(a=>a.MemberId == id);
        }

        // helper methods

        private string generateJwtToken(Member member)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var key = Encoding.ASCII.GetBytes(_appSettings.AdditionalSettings["APISecurityKey"].Value<string>());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, member.MemberId.ToString())

                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var finalToken = tokenHandler.WriteToken(token);

      
            return finalToken;
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
    }
}