using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpeakOutWeb.Controllers;
using SpeakOutWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Transactions;

using System.Web.Mvc;

namespace SpeakOutWeb.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        //[TestMethod]
        //public void Index()
        //{
        //    // Arrange
        //    HomeController controller = new HomeController();

        //    // Act
        //    ViewResultBase result = controller.Index() as ViewResultBase;

        //    // Assert
        //    Assert.IsNotNull(result);
        //}

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResultBase result = controller.About() as ViewResultBase;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ActionResult result = controller.Contact() as ActionResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
