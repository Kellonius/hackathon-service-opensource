using Hackathon_Service.Models.Users.Requests;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;
using System.Web.Http.Cors;
using Hackathon_DataAccess;
using Hackathon_Service.Models.Users.Responses;

namespace Hackathon_Service.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("Users")]
    public class UserController : ApiController
    {
        [Route("CreateUser")]
        public void createUser(UserCreationRequest request)
        {
            var response = new HttpResponseMessage();
            try
            {
                using (var context = new HackathonEntities())
                {
                    var exists = context.users.Where(x => x.email.Equals(request.email) && x.delete_ts != null).FirstOrDefault();

                    if (exists != null && exists.email.Length > 0)
                    {
                        response = Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            "An account with this email already exists.");
                        throw new HttpResponseException(response);
                    }
                    var user = new user()
                    {
                        first_name = request.firstName,
                        last_name = request.lastName,
                        email = request.email,
                        password = hashPassword(request.password),
                        create_ts = DateTime.Now
                    };
                    context.users.Add(user);
                    context.SaveChanges();
                    context.Dispose();
                }          
            }
            catch (Exception e)
            {
                throw new HttpResponseException(response);
            }
        }

        [Route("LoginUser")]
        public UserResponse loginUser(UserLoginRequest userLoginRequest)
        {
            var response = new HttpResponseMessage();
            try
            {
                using (var context = new HackathonEntities())
                {
                    var user = context.users.FirstOrDefault(x => x.email.Equals(userLoginRequest.email) && x.delete_ts != null);
                    if (user == null)
                    {
                        response = Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        "Email supplied does not match account in our records.");
                        throw new HttpResponseException(response);
                    }
                    var success = unHashPassword(user.password, userLoginRequest.password);

                    if (!success)
                    {
                        response = Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            "User email and password combination does not match our records.");
                        throw new HttpResponseException(response);
                    }
                    
                    return new UserResponse()
                    {
                        id = user.id,
                        email = user.email,
                        firstName = user.first_name,
                        lastName = user.last_name
                    };
                }
            }
            catch (Exception e)
            {
                throw new HttpResponseException(response);
            }

        }

        //[HttpPost]
        //[Route("GetUserPreferences")]
        //public UserPreferencesResponse getUserPreferences(UserIdRequest request)
        //{
        //    try
        //    {
        //        using (var context = new RetroVizEntities())
        //        {
        //            var preferences = context.user_preferences.Where(x => x.user_id == request.userId).FirstOrDefault();
        //            return new UserPreferencesResponse()
        //            {
        //                userId = Convert.ToInt32(preferences.user_id),
        //                colorCode = preferences.color_code
        //            };
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}

        private string hashPassword(string word)
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

        private bool unHashPassword(string saved, string pass)
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

       // private string randomColor()
       // {
       //     var hex = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
       //     var color = "#";
       //     Random random = new Random();
       //
       //     for (int i = 0; i < 6; i++)
       //     {
       //         color += hex[random.Next(0, 15)];
       //     }
       //     return color;
       // }
    }
}