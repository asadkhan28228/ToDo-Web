using System;
using System.Collections.Generic;
using System.Text;
using ToDo.DAL.Entities;

namespace ToDo.BLL.Interface
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
