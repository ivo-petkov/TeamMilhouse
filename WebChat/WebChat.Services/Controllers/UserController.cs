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
using System.Web;
using WebChat.DropboxUploader;

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
                var sessionKey = UserDataPersister.LoginUser(user.Username, user.AuthCode);
                return new UserLoggedModel()
                {
                    UserName = user.Username,
                    SessionKey = sessionKey,
                    Avatar = "https://dl-web.dropbox.com/get/Apps/TeamMilhouse/Avatars/default_avatar1234375017.jpg?w=AAChV5kjQusx9YZOewlGvifCNfzmIfmeYOF_18UR8BvxHg"
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
                var sessionKey = UserDataPersister.LoginUser(user.Username, user.AuthCode);
                var userId = UserDataPersister.LoginUser(sessionKey);
                var avatar = UserDataPersister.GetAvatar(userId);
                return new UserLoggedModel()
                {
                    SessionKey = sessionKey,
                    UserName = user.Username,
                    Avatar = avatar
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


        [HttpPost]
        [ActionName("send")]
        public HttpResponseMessage SendMessage(string sessionKey, SendMessageModel message)
        {
            var responseMsg = this.PerformOperation(() =>
                {
                    var sender = UserDataPersister.LoginUser(sessionKey);

                    UserDataPersister.SendMessage(sender, message.Receiver, message.Content);

                });
            return responseMsg;
        }

        [HttpPost]
        [ActionName("getmessages")]
        public HttpResponseMessage GetAllMessages(string sessionKey, GetMessagesModel message)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                var sender = UserDataPersister.LoginUser(sessionKey);

                IEnumerable<CollectedMessages> messages = MessageDataPersister.GetAllMessagesWithUser(sender, message.Receiver);
                return messages;
            });
            return responseMsg;
        }

        [HttpPost]
        [ActionName("clear")]
        public HttpResponseMessage ClearNotifications(string sessionKey, ClearWithUser otherUser)
        {
            var responseMsg = this.PerformOperation(() =>
                {
                    var thisAcc = UserDataPersister.LoginUser(sessionKey);
                    MessageDataPersister.ClearNotificationsWithUser(thisAcc, otherUser.User);
                });
            return responseMsg;
        }

        [HttpPost]
        [ActionName("changeavatar")]
        public HttpResponseMessage ChangeAvatar()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;

            // Check if files are available
            if (httpRequest.Files.Count > 0)
            {
                var files = new List<string>();

                // interate the files and save on the server
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];


                    var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    var dropBoxUrl = DropboxFileUploader.FileUpload(postedFile.FileName, true);

                    System.IO.File.Delete(HttpContext.Current.Server.MapPath("~/" + postedFile.FileName));

                    files.Add(dropBoxUrl);
                }

                // return result
                result = Request.CreateResponse(HttpStatusCode.Created, files);
            }
            else
            {
                // return BadRequest (no file(s) available)
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }

        [HttpPut]
        [ActionName("updateAvatar")]
        public HttpResponseMessage UpdateAvatar(string sessionKey, [FromBody] string avatar)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                var userid = UserDataPersister.LoginUser(sessionKey);

                UserDataPersister.UpdateUserAvatar(userid, avatar);
            });
            return responseMsg;
        }
    }
}