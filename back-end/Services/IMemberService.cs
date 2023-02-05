using System.Collections.Generic;
using Adverthouse.Common.Data;
using back_end.Domain.Entities;
using back_end.ViewModels;

namespace back_end.Services
{
    public interface IMemberService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshJWTToken(string token, string ipAddress);
        bool RevokeToken(string token, string ipAddress);
        Member GetById(int id);
        IEnumerable<RefreshToken> GetAllTokens();
        IEnumerable<Member> GetAll();
    }
}