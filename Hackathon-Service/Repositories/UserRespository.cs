using Hackathon_DataAccess;
using Hackathon_Service.Models.Users.Requests;
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hackathon_Service.Repositories
{
    public class UserRespository
    {
        public user checkIfUserExists(string email)
        {
            using (var context = new HackathonEntities())
            {
                return context.users.FirstOrDefault(x => x.email.Equals(email) && x.delete_ts != null);
            }
        }

        public void createNewUser(UserCreationRequest request)
        {
            using (var context = new HackathonEntities())
            {
                var user = new user()
                {
                    first_name = request.firstName,
                    last_name = request.lastName,
                    email = request.email,
                    password = HashPassword(request.password),
                    create_ts = DateTime.Now
                };
                context.users.Add(user);
                context.SaveChanges();
                context.Dispose();
            }
        }

        private string HashPassword(string word)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdsf2 = new Rfc2898DeriveBytes(word, salt, 10000);

            byte[] hash = pbkdsf2.GetBytes(20);

            byte[] hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string savedPasswordHash = Convert.ToBase64String(hashBytes);

            return savedPasswordHash;
        }

        public bool UnHashPassword(string saved, string pass)
        {
            byte[] hashBytes = Convert.FromBase64String(saved);

            byte[] salt = new byte[16];

            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pbkdsf2 = new Rfc2898DeriveBytes(pass, salt, 10000);

            byte[] hash = pbkdsf2.GetBytes(20);

            int ok = 1;

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    ok = 0;
                }
            }

            if (ok == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}