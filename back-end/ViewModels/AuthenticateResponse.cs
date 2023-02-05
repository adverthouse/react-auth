
using back_end.Domain.Entities;
using Newtonsoft.Json;

namespace back_end.ViewModels
{
    public class AuthenticateResponse
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; } 

        public AuthenticateResponse(Member member, string jwtToken, string refreshToken)
        {
            MemberId = member.MemberId;
            FirstName = member.FirstName;
            LastName = member.LastName;
            Username = member.Username;
            Avatar = member.Avatar;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}