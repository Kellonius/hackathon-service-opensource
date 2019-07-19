using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hackathon_Service.Models.Users.Requests
{
    public class UserLoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}