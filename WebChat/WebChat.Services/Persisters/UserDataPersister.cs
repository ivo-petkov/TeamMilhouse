using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WebChat.Data;
using WebChat.Model;
using WebChat.Services.Controllers;
using WebChat.Services.Models;

namespace WebChat.Services.Persisters
{

    public class UserDataPersister
    {
        private const string SessionKeyChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const int SessionKeyLen = 50;

        private const int Sha1CodeLength = 40;
        private const string ValidUsernameChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_1234567890";
        private const string ValidNicknameChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_1234567890 -";
        private const int MinUsernameNicknameChars = 6;
        private const int MaxUsernameNicknameChars = 30;

        private static Random rand = new Random();

        /* private methods */

        private static void ValidateSessionKey(string sessionKey)
        {
            if (sessionKey.Length != SessionKeyLen || sessionKey.Any(ch => !SessionKeyChars.Contains(ch)))
            {
                throw new ServerErrorException("Invalid Password", "ERR_INV_AUTH");
            }
        }

        private static string GenerateSessionKey(int userId)
        {
            StringBuilder keyChars = new StringBuilder(50);
            keyChars.Append(userId.ToString());
            while (keyChars.Length < SessionKeyLen)
            {
                int randomCharNum;
                lock (rand)
                {
                    randomCharNum = rand.Next(SessionKeyChars.Length);
                }
                char randomKeyChar = SessionKeyChars[randomCharNum];
                keyChars.Append(randomKeyChar);
            }
            string sessionKey = keyChars.ToString();
            return sessionKey;
        }

        private static void ValidateUsername(string username)
        {
            if (username == null || username.Length < MinUsernameNicknameChars || username.Length > MaxUsernameNicknameChars)
            {
                throw new ServerErrorException(string.Format("Username should be between {0} and {1} symbols long", MinUsernameNicknameChars, MaxUsernameNicknameChars), "INV_USR_LEN");
            }
            else if (username.Any(ch => !ValidUsernameChars.Contains(ch)))
            {
                throw new ServerErrorException("Username contains invalid characters", "INV_USR_CHARS");
            }
        }

        private static void ValidateNickname(string nickname)
        {
            if (nickname == null || nickname.Length < MinUsernameNicknameChars || nickname.Length > MaxUsernameNicknameChars)
            {
                throw new ServerErrorException(string.Format("Nickname should be between {0} and {1} symbols long", MinUsernameNicknameChars, MaxUsernameNicknameChars), "INV_NICK_LEN");
            }
            else if (nickname.Any(ch => !ValidNicknameChars.Contains(ch)))
            {
                throw new ServerErrorException("Nickname contains invalid characters", "INV_NICK_CHARS");
            }
        }

        private static void ValidateAuthCode(string authCode)
        {
            if (authCode.Length != Sha1CodeLength || authCode == "da39a3ee5e6b4b0d3255bfef95601890afd80709")
            {
                throw new ServerErrorException("Invalid user authentication", "INV_USR_AUTH");
            }
        }

        /* public methods */

        public static void CreateUser(string username, string authCode)
        {
            ValidateUsername(username);
            ValidateAuthCode(authCode);
            using (WebChatContext context = new WebChatContext())
            {
                var usernameToLower = username.ToLower();

                var dbUser = context.Users.FirstOrDefault(u => u.Username == usernameToLower);

                if (dbUser != null)
                {
                    if (dbUser.Username.ToLower() == usernameToLower)
                    {
                        throw new ServerErrorException("Username already exists", "ERR_DUP_USR");
                    }
                    else
                    {
                        throw new ServerErrorException("Nickname already exists", "ERR_DUP_NICK");
                    }
                }

                dbUser = new User()
                {
                    Username = usernameToLower,
                    AuthCode = authCode,
                    Avatar = @"https://dl.dropboxusercontent.com/s/3v8l4mflw50mtqz/default_avatar1234375017.jpg?token_hash=AAH4OQXSsLs4ZjTQYLzKkn_Wc8loL8aKnnX8xhi9t-z40A\u0026dl=1\"
                };
                context.Users.Add(dbUser);
                context.SaveChanges();
            }
        }

        public static string LoginUser(string username, string authCode)
        {
            ValidateUsername(username);
            ValidateAuthCode(authCode);
            var context = new WebChatContext();
            using (context)
            {
                var usernameToLower = username.ToLower();
                var user = context.Users.FirstOrDefault(u => u.Username == usernameToLower && u.AuthCode == authCode);
                if (user == null)
                {
                    throw new ServerErrorException("Invalid username or password", "ERR_INV_USR");
                }

                var sessionKey = GenerateSessionKey((int)user.UserId);
                user.SessionKey = sessionKey;
                context.SaveChanges();
                return sessionKey;
            }
        }

        public static int LoginUser(string sessionKey)
        {
            ValidateSessionKey(sessionKey);
            var context = new WebChatContext();
            using (context)
            {
                var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new ServerErrorException("Invalid user authentication", "INV_USR_AUTH");
                }
                return (int)user.UserId;
            }
        }

        public static void LogoutUser(string sessionKey)
        {
            ValidateSessionKey(sessionKey);
            var context = new WebChatContext();
            using (context)
            {
                var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new ServerErrorException("Invalid user authentication", "INV_USR_AUTH");
                }
                user.SessionKey = null;
                context.SaveChanges();
            }
        }

        public static IEnumerable<UserModel> GetAllUsers()
        {
            var context = new WebChatContext();
            using (context)
            {
                var users =
                    (from user in context.Users
                     select new UserModel()
                     {
                         Id = (int)user.UserId,
                         UserName = user.Username,
                         Avatar = user.Avatar
                     });
                return users.ToList();
            }
        }

        public static void SendMessage(int sender, string username, string content)
        {
            using (var context = new WebChatContext())
            {
                var receiver = context.Users.Where(x => x.Username == username).FirstOrDefault();
                var senderUser = context.Users.Find(sender);
                if (receiver != null)
                {

                    var message = new Message()
                    {
                        Content = content,
                        Sender = senderUser,
                        Receiver = receiver,
                        SentTime = DateTime.Now,
                        Status = false
                    };
                    context.Messages.Add(message);
                    context.SaveChanges();
                }
                else
                {
                    throw new ServerErrorException("Invalid username", "ERR_INV_USR");
                }
            }
        }

        public static string GetAvatar(int userId)
        {
            var context = new WebChatContext();
            using (context)
            {
                var user = context.Users.Find(userId);
                return user.Avatar;
            }
        }

        public static void UpdateUserAvatar(int userId, string avatar)
        {
            var context = new WebChatContext();
            using (context)
            {
                var user = context.Users.Find(userId);
                user.Avatar = avatar;
                context.SaveChanges();
            }
        }
    }
}