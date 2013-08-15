//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using WebChat.Services;
//using WebChat.Services.Controllers;
//using System.Transactions;
//using System.Net;

//namespace WebChat.Services.Tests.Controllers
//{
//    [TestClass]
//    public class UserControllerTest
//    {
//        //[TestMethod]
//        //public void Get()
//        //{
//        //    // Arrange
//        //    ValuesController controller = new ValuesController();

//        //    // Act
//        //    IEnumerable<string> result = controller.Get();

//        //    // Assert
//        //    Assert.IsNotNull(result);
//        //    Assert.AreEqual(2, result.Count());
//        //    Assert.AreEqual("value1", result.ElementAt(0));
//        //    Assert.AreEqual("value2", result.ElementAt(1));
//        //}

//        [TestMethod]
//        public void GetById()
//        {
//            // Arrange
//            UserController controller = new UserController();

//            // Act
//            var user = new UserLoginModel
//            {
//                Username = "pesho111",
//                AuthCode = "152c75d70810ffba3ad4bf931089925c727660f1" // talantliv
//            };

//            var result = controller.LoginUser(user);

//            // Assert
//            var expected = new HttpRequestMessage();
//            Assert.AreEqual(expected.CreateResponse(HttpStatusCode.OK), result);
//        }

//        //[TestMethod]
//        //public void Post()
//        //{
//        //    // Arrange
//        //    UserController controller = new UserController();

//        //    // Act
//        //    var user = new UserLoginModel
//        //    {
//        //        Username = "talantliv",
//        //        AuthCode = "d9fe89ffc19310e99f6cddef64bab7edd4ccfc50" // talantliv
//        //    };

//        //    using (var tran = new TransactionScope())
//        //    {
//        //        var response = controller.RegisterUser(user);
//        //        var expected = new HttpRequestMessage();
//        //        Assert.AreEqual(expected.CreateResponse(HttpStatusCode.OK), response.StatusCode);
//        //    }

//        //    // Assert
//        //}

//        //[TestMethod]
//        //public void Put()
//        //{
//        //    // Arrange
//        //    ValuesController controller = new ValuesController();

//        //    // Act
//        //    controller.Put(5, "value");

//        //    // Assert
//        //}
//    }
//}
