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
    public class GamesControllerTest
    {
        [TestMethod]
        //Nguyen Manh Hung
        public void TestIndexGame()
        {
            var model = new Vocabulary();
            var context = new Mock<HttpContextBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();

            context.Setup(x => x.User).Returns(user.Object);
            user.Setup(u => u.Identity).Returns(identity.Object);
            identity.Setup(i => i.Name).Returns("");
            var controller = new GameController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            var result0 = controller.Index() as RedirectToRouteResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Account", result0.RouteValues["controller"]);
            Assert.AreEqual("Login", result0.RouteValues["action"]);
            identity.Setup(i => i.Name).Returns("Tester");
            var result1 = controller.Index() as ActionResult;
            Assert.IsNotNull(result1);
        }
    }
}
