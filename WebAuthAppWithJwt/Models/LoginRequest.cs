using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAuthAppWithJwt.Models
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}