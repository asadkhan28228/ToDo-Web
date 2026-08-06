using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo.BLL.Dto.Auth
{
    public class RegisteredDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
