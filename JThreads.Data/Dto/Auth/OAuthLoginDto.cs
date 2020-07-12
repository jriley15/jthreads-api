using System;
using System.Collections.Generic;
using System.Text;

namespace JThreads.Data.Dto.Auth
{
    public class OAuthLoginDto
    {
        public string Code { get; set; }

        public string AccessToken { get; set; }

        public string UserId { get; set; }

    }
}
