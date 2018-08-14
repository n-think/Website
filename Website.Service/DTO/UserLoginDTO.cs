using System;
using System.Collections.Generic;
using System.Text;

namespace Website.Service.DTO
{
    public class UserLoginDTO
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string UserId { get; set; }
        public string ProviderDisplayName { get; set; }
    }
}
