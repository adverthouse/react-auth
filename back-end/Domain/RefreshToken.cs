using System.Text.Json.Serialization;
using System.Collections.Generic;
using System;
using Adverthouse.Common.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace back_end.Domain.Entities
{ 
    public class RefreshToken : IEntity
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; } 
        public int MemberId {get;set;}
        
        public string Token { get; set; } = "";
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; } = "";
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; } = "";
        public string ReplacedByToken { get; set; } = "";
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
