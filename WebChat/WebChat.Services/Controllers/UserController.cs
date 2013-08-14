using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebChat.Services.Models;
using System.Text;
using System.Web.Http.Cors;
using WebChat.Services.Persisters;

namespace WebChat.Services.Controllers
{
    public class UserController : BaseApiController
    {
        /*
{
   "username": "Dodo",
    "nickname": "Doncho Minkov",
   "authCode": "96b828b4cc79f50bf8faef6e7b4a1dcfb356dea6"
}
       */
        [HttpPost]
        [ActionName("register")]
        public HttpResponseMessage RegisterUser(UserLoginModel user)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                UserDataPersister.CreateUser(user.Username, user.AuthCode);
                string username = string.Empty;
                var sessionKey = UserDataPersister.LoginUser(user.Username, user.AuthCode);
                return new UserLoggedModel()
                {
                    UserName = username,
                    SessionKey = sessionKey
                };
            });
            return responseMsg;
        }

        [HttpPost]
        [ActionName("login")]
        public HttpResponseMessage LoginUser(UserLoginModel user)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                string nickname = string.Empty;
                var sessionKey = UserDataPersister.LoginUser(user.Username, user.AuthCode);
                return new UserLoggedModel()
                {
                    SessionKey = sessionKey
                };
            });
            return responseMsg;
        }

        [HttpGet]
        [ActionName("logout")]
        public HttpResponseMessage LogoutUser(string sessionKey)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                UserDataPersister.LogoutUser(sessionKey);
            });
            return responseMsg;
        }

        [HttpGet]
        [ActionName("allusers")]
        public HttpResponseMessage GetAllUsers(string sessionKey)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                UserDataPersister.LoginUser(sessionKey);
                IEnumerable<UserModel> users = UserDataPersister.GetAllUsers();
                return users;
            });
            return responseMsg;
        }
    }
}