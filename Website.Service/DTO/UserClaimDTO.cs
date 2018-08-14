using System;
using System.Collections.Generic;
using System.Text;

namespace Website.Service.DTO
{
    public class UserClaimDTO
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
