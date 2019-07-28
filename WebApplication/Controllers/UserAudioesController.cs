using Microsoft.AspNet.Identity;
using PagedList;
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
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (HttpContext.User.Identity.GetUserName() == "" || HttpContext.User.Identity.GetUserName() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.CurrentSort1 = sortOrder;
            ViewBag.DateSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var currentUser = HttpContext.User.Identity.GetUserName();
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter1 = searchString;
            var userAudios = db.UserAudioes.Where(x => x.UserId == currentUser);
            if (!String.IsNullOrEmpty(searchString))
            {
                userAudios = userAudios.Where(s => s.TextAudio.Contains(searchString));
            }
            switch(sortOrder)
            {
                case "name_desc":
                    userAudios = userAudios.OrderBy(s => s.CreateDate);
                break;
                default:  // Name ascending 
                    userAudios = userAudios.OrderByDescending(s => s.CreateDate);
                break;
            }
            int pageSize = 1;
            int pageNumber = (page ?? 1);
            return View(userAudios.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult LoadAudio(int id)
        {
            var audioBytes = db.UserAudioes.Where(w => w.Id == id).Single();
            return base.File(audioBytes.LinkAudio, "audio/wav");
        }
    }
}