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
using System.Threading.Tasks;
using System.Diagnostics;

namespace WebChat.Services.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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
                    Avatar = @"https://dl.dropboxusercontent.com/s/3v8l4mflw50mtqz/default_avatar1234375017.jpg?token_hash=AAH4OQXSsLs4ZjTQYLzKkn_Wc8loL8aKnnX8xhi9t-z40A\u0026dl=1\"
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
        public HttpResponseMessage ChangeAvatar(string sessionKey)
        {
            //var responseMsg = this.PerformOperation(() =>
            //{
            //    var httpRequest = HttpContext.Current.Request;
            //    if (httpRequest.Files.Count > 0)
            //    {
            //        var files = new List<string>();

            //        // interate the files and save on the server
            //        foreach (string file in httpRequest.Files)
            //        {
            //            var postedFile = httpRequest.Files[file];
            //            var root = HttpContext.Current.Server.MapPath("~/App_Data/");
            //            var filePath = root + postedFile.FileName;
            //            postedFile.SaveAs(filePath);

            //            var dropBoxUrl = DropboxFileUploader.FileUpload(postedFile.FileName, true);

            //            System.IO.File.Delete(root + postedFile.FileName);

            //            files.Add(dropBoxUrl);
            //        }

            //        return files;
            //    }

            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //});
            //return responseMsg;

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
                    var root = HttpContext.Current.Server.MapPath("~/App_Data/");
                    var filePath = root + postedFile.FileName;
                    postedFile.SaveAs(filePath);

                    var dropBoxUrl = DropboxFileUploader.FileUpload(postedFile.FileName, true);

                    System.IO.File.Delete(root + postedFile.FileName);

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

        //[HttpPost]
        //[ActionName("changeavatar")]
        //public async Task<HttpResponseMessage> PostMultipartStream()
        //{
        //    // Check if the request contains multipart/form-data.
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }

        //    string root = HttpContext.Current.Server.MapPath("~/App_Data/");
        //    var provider = new MultipartFormDataStreamProvider(root);

        //    try
        //    {
        //        // Read the form data.
        //        var content = await Request.Content.ReadAsMultipartAsync(provider);
        //        var contents = content.Contents;
        //        // This illustrates how to get the file names.
        //        foreach (MultipartFileData file in provider.FileData)
        //        {
        //            Trace.WriteLine(file.Headers.ContentDisposition.FileName);
        //            Trace.WriteLine("Server file path: " + file.LocalFileName);
        //        }

        //        var fileName = content.FileData.First().LocalFileName;
        //        File postedFile = 
        //        postedFile.SaveAs(root + fileName);

        //        var dropBoxUrl = DropboxFileUploader.FileUpload(fileName, true);

        //        System.IO.File.Delete(HttpContext.Current.Server.MapPath(root + fileName));

        //        files.Add(dropBoxUrl);

        //        return Request.CreateResponse(HttpStatusCode.OK);
        //    }
        //    catch (System.Exception e)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
        //    }
        //}

        [HttpPut]
        [ActionName("updateAvatar")]
        public HttpResponseMessage UpdateAvatar(string sessionKey, [FromBody] AvatarModel avatar)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                var userid = UserDataPersister.LoginUser(sessionKey);

                UserDataPersister.UpdateUserAvatar(userid, avatar.Avatar);
            });
            return responseMsg;
        }
    }
}