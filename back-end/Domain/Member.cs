using System.Text.Json.Serialization;
using System.Collections.Generic;
using System;
using Adverthouse.Common.Interfaces;

namespace back_end.Domain.Entities
{
    public class Member : IEntity
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Username { get; set; } = "";
        public string Avatar { get; set; } = "";
        public string Gender {get;set;} = "";
        public DateTime BirthDay {get;set;}

        [JsonIgnore]
        public string Password { get; set; } = "";

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
