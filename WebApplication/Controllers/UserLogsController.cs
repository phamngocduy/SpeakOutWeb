using Microsoft.AspNet.Identity;
using PagedList;
using SpeakOutWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SpeakOutWeb.Controllers
{
    public class UserLogsController : Controller
    {
        private SpeakOutEntities db = new SpeakOutEntities();

        // GET: UserLogs
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (HttpContext.User.Identity.GetUserName() == "" || HttpContext.User.Identity.GetUserName() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.LogSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var userId = HttpContext.User.Identity.GetUserName();
            var log = db.UserLogs.Where(s => s.UserId == userId);
            if (!String.IsNullOrEmpty(searchString))
            {
                log = log.Where(s => s.ContentReading.Contains(searchString)
                                       || s.Type.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    log = log.OrderByDescending(x => x.UserId);
                    break;
                case "Date":
                    log = log.OrderBy(s => s.CreateDate);
                    break;
                case "date_desc":
                    log = log.OrderByDescending(s => s.CreateDate);
                    break;
                case "name_asc":
                    log = log.OrderByDescending(s => s.Type).Where(x => x.Type == "Bài nói");
                    break;
                default:  // Name ascending 
                    log = log.OrderByDescending(x=>x.CreateDate);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(log.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult SaveLogs(string text)
        {
            var currentUser = HttpContext.User.Identity.GetUserName();
            db.Configuration.ProxyCreationEnabled = false;
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                if (currentUser != "" || currentUser != null)
                {
                    UserLog userLog = new UserLog();
                    userLog.CreateDate = DateTime.Now;
                    userLog.ContentReading = text;
                    userLog.UserId = currentUser;
                    userLog.Type = "Đoạn văn";
                    db.UserLogs.Add(userLog);
                    db.SaveChanges();
                }
               
                return Json("Lưu thành công", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult SaveLogDetails(string text, DateTime createDate, DateTime endDate)
        {
            var currentUser = HttpContext.User.Identity.GetUserName();
            db.Configuration.ProxyCreationEnabled = false;
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                if (currentUser != "" || currentUser != null)
                {
                    UserLog userLog = new UserLog();
                    userLog.CreateDate = createDate;
                    userLog.EndDate = endDate;
                    userLog.ContentReading = text;
                    userLog.UserId = currentUser;
                    userLog.Type = "Đoạn văn";
                    db.UserLogs.Add(userLog);
                    db.SaveChanges();
                }

                return Json("Lưu thành công", JsonRequestBehavior.AllowGet);
            }
        }



    }
}