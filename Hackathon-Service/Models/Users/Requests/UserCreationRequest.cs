using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hackathon_Service.Models.Users.Requests
{
    public class UserCreationRequest
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}