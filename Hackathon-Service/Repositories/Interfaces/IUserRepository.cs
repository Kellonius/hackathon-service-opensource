using Hackathon_DataAccess;
using Hackathon_Service.Models.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hackathon_Service.Repositories.Interfaces
{
    public interface IUserRepository
    {
        user checkIfUserExists(string email);
        void createNewUser(UserCreationRequest request);
        bool UnHashPassword(string saved, string pass);
    }
}