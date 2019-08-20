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
    public class UserLogsControllerTest
    {
        [TestMethod]
        //Trieu Duc Huy
        public void TestIndexUserLogs()
        {
            var model = new UserLog();
            var context = new Mock<HttpContextBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();

            context.Setup(x => x.User).Returns(user.Object);
            user.Setup(u => u.Identity).Returns(identity.Object);
            identity.Setup(i => i.Name).Returns("");
            var controller = new UserLogsController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            var result0 = controller.Index("", "", "", 1) as RedirectToRouteResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Account", result0.RouteValues["controller"]);
            Assert.AreEqual("Login", result0.RouteValues["action"]);
            identity.Setup(i => i.Name).Returns("Tester");
            var result1 = controller.Index("", "", "", 1) as ActionResult;
            Assert.IsNotNull(result1);
        }

    }
}
