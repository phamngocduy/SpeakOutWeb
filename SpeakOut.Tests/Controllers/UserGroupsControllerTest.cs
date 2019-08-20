using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpeakOutWeb.Controllers;
using SpeakOutWeb.Models;
using System;
using System.Linq;
using System.Security.Principal;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SpeakOutWeb.Tests.Controllers
{
    [TestClass]
    public class UserGroupsControllerTest
    {
        [TestMethod]
        //Huynh Thi My Linh
        public void TestCreateGet()
        {
            var model = new UserGroup();
            var context = new Mock<HttpContextBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();

            context.Setup(x => x.User).Returns(user.Object);
            user.Setup(u => u.Identity).Returns(identity.Object);
            identity.Setup(i => i.Name).Returns("");
            var controller = new UserGroupsController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            var result0 = controller.Create(model) as RedirectToRouteResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Account", result0.RouteValues["controller"]);
            Assert.AreEqual("Login", result0.RouteValues["action"]);
            identity.Setup(i => i.Name).Returns("Tester");
            var result1 = controller.Create() as ActionResult;
            Assert.IsNotNull(result1);
        }
        //[TestMethod]
        //Trieu Duc Huy
        public void TestCreatePost()
        {
            var model = new UserGroup();
            var context = new Mock<HttpContextBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();

            context.Setup(x => x.User).Returns(user.Object);
            user.Setup(u => u.Identity).Returns(identity.Object);
            identity.Setup(i => i.Name).Returns("");
            var controller = new UserGroupsController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            var result0 = controller.Create(model) as RedirectToRouteResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Account", result0.RouteValues["controller"]);
            Assert.AreEqual("Login", result0.RouteValues["action"]);
            identity.Setup(i => i.Name).Returns("Tester");
            model.GroupName = "";
            model.Description = "dataaa";

            var result1 = controller.Create(model) as ViewResult;
            Assert.IsNotNull(result1);
            Assert.AreEqual("OK", result1.ViewBag.GetUserTrim);
            model.GroupName = "data1";
            model.Description = "";

            var result2= controller.Create(model) as ViewResult;
            Assert.IsNotNull(result2);
            Assert.AreEqual("OK", result2.ViewBag.GetUserTrim);
            model.GroupName = "data1";
            model.Description = "dataaaa";
            using (var scope = new TransactionScope())
                using(var db= new SpeakOutEntities())
            {
                var count = db.UserGroups.Count();
                var result3 = controller.Create(model) as RedirectToRouteResult;

                Assert.IsNotNull(result3);
                Assert.AreEqual("Index", result3.RouteValues["action"]);
                Assert.AreEqual(count + 1, db.UserGroups.Count());
                Assert.AreEqual("No", result2.ViewBag.GetUserTrim);
            }
            controller.ModelState.AddModelError("error", "error");
            var result4 = controller.Create(model) as ViewResult;
            Assert.IsNotNull(result4);
            Assert.AreEqual(model, result4.Model);
        }
    }

   
}