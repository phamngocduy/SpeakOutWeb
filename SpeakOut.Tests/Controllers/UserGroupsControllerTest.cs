using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpeakOutWeb.Controllers;
using SpeakOutWeb.Models;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SpeakOutWeb.Tests.Controllers
{
    [TestClass]
    public class UserGroupsControllerTest
    {
        [TestMethod]
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
    }

    internal class ViewResult
    {
    }
}