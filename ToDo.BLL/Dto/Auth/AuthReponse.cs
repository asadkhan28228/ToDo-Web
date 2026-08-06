using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo.BLL.Dto.Auth
{
    public class AuthReponse
    {
        public string Token { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
