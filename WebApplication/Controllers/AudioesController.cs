using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpeakOutWeb.Controllers
{
    public class AudioesController : Controller
    {
        // GET: Audioes
        public ActionResult Index()
        {
            return View();
        }
    }
}