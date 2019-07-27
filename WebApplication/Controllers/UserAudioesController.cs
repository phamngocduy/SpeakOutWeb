using Microsoft.AspNet.Identity;
using SpeakOutWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpeakOutWeb.Controllers
{
    public class UserAudioesController : Controller
    {
        private SpeakOutEntities db = new SpeakOutEntities();
        // GET: Audioes
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.GetUserName() == "" || HttpContext.User.Identity.GetUserName() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var currentUser = HttpContext.User.Identity.GetUserName();
            IEnumerable<UserAudio> userAudios = db.UserAudioes.Where(x => x.UserId == currentUser).ToList();
            return View(userAudios);
        }
        public ActionResult LoadAudio(int id)
        {
            var audioBytes = db.UserAudioes.Where(w => w.Id == id).Single();
            return base.File(audioBytes.LinkAudio, "audio/wav");
        }
    }
}