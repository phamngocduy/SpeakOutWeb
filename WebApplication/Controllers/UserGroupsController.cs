using Microsoft.AspNet.Identity;
using PagedList;
using SpeakOutWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SpeakOutWeb.Controllers
{
    public class UserGroupsController : Controller
    {
        private SpeakOutEntities db = new SpeakOutEntities();

        // GET: UserGroups
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (HttpContext.User.Identity.GetUserName() == "" || HttpContext.User.Identity.GetUserName() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.CurrentSort2 = sortOrder;
            ViewBag.VocabSortParm2 = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm2 = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter2 = searchString;
            var userId = HttpContext.User.Identity.GetUserName();
            var classRooms = db.UserGroups.Where(x => x.GroupName != null);
            if (!String.IsNullOrEmpty(searchString))
            {
                var search = searchString;
                int numberParse;
                bool isNumber= Int32.TryParse(search, out numberParse);
                if (isNumber)
                {
                    classRooms = classRooms.Where(x => x.Id == numberParse);
                }
                else
                {
                    classRooms = classRooms.Where(s => s.GroupName.Contains(searchString) || s.Email.Contains(searchString));
                }
                
            }
            switch (sortOrder)
            {
                case "name_desc":
                    classRooms = classRooms.OrderByDescending(s => s.GroupName);
                    break;

                case "Date":
                    classRooms = classRooms.OrderBy(s => s.CreatedDate);
                    break;

                case "date_desc":
                    classRooms = classRooms.OrderByDescending(s => s.CreatedDate);
                    break;

                default:  // Name ascending
                    classRooms = classRooms.OrderBy(s => s.GroupName);
                    break;
            }

            int pageSize = 10000;
            int pageNumber = (page ?? 1);
            return View(classRooms.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult MemberGroups(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (HttpContext.User.Identity.GetUserName() == "" || HttpContext.User.Identity.GetUserName() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.CurrentSort1 = sortOrder;
            ViewBag.VocabSortParm1 = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm1 = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter1 = searchString;
            var userId = HttpContext.User.Identity.GetUserName();
            var classRooms = db.UserGroups.Where(x => x.GroupName != null);
            if (!String.IsNullOrEmpty(searchString))
            {
                var search = searchString;
                int numberParse;
                bool isNumber = Int32.TryParse(search, out numberParse);
                if (isNumber)
                {
                    classRooms = classRooms.Where(x => x.Id == numberParse);
                }
                else
                {
                    classRooms = classRooms.Where(s => s.GroupName.Contains(searchString) || s.Email.Contains(searchString));
                }

            }
            switch (sortOrder)
            {
                case "name_desc":
                    classRooms = classRooms.OrderByDescending(s => s.GroupName);
                    break;

                case "Date":
                    classRooms = classRooms.OrderBy(s => s.CreatedDate);
                    break;

                case "date_desc":
                    classRooms = classRooms.OrderByDescending(s => s.CreatedDate);
                    break;

                default:  // Name ascending
                    classRooms = classRooms.OrderBy(s => s.GroupName);
                    break;
            }

            int pageSize = 10000;
            int pageNumber = (page ?? 1);
            return View(classRooms.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult GetLinkClasses(int? classId)
        {
            return View();
        }

        // GET: UserGroups/Details/5
        public ActionResult Details(int? id)
        {
            var currentUser = HttpContext.User.Identity.GetUserName();
            if (HttpContext.User.Identity.GetUserName() == "" || HttpContext.User.Identity.GetUserName() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var checkAttend = db.UserGroupDetails.Where(x => x.GroupId == id && x.StudentEmail == currentUser).Count();
            ViewBag.CountRequest = db.UserAcceptances.Where(x => x.GroupId == id).Count();
            UserGroup userGroup = db.UserGroups.Find(id);
            ViewBag.IdRequestClass=id;
            if (userGroup.Email == currentUser)
            {
                return View(userGroup);
            }
            if (checkAttend == 0)
            {
                return RedirectToAction("Index", "UserGroups");
            }

            if (userGroup == null)
            {
                return HttpNotFound();
            }

            return View(userGroup);
        }

        // GET: UserGroups/Create
        public ActionResult Create()
        {
            if (HttpContext.User.Identity.GetUserName() == "" || HttpContext.User.Identity.GetUserName() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult RequestList(int? idClass)
        {
            if (HttpContext.User.Identity.GetUserName() == "" || HttpContext.User.Identity.GetUserName() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var currentUser = HttpContext.User.Identity.GetUserName();
            var getCurrentClass = db.UserGroups.Where(x => x.Id == idClass && x.Email == currentUser).Count();
            if (getCurrentClass > 0)
            {
                IEnumerable<UserAcceptance> getRequestList = db.UserAcceptances.Where(x => x.GroupId == idClass).ToList();
                return View(getRequestList);
            }
            return RedirectToAction("Index", "UserGroups");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,GroupName,Description,Email,CreatedDate")] UserGroup userGroup)
        {
            if (ModelState.IsValid)
            {
                var currentUser = User.Identity.GetUserName();
                userGroup.CreatedDate = DateTime.Now;
                userGroup.Email = currentUser;
                db.UserGroups.Add(userGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userGroup);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Xem từ điển hiện tại của học sinh trong lớp do người dùng quản lý
        [HttpGet]
        public ActionResult StudentDetails(string studentId, int? classId, string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (HttpContext.User.Identity.GetUserName() == "" || HttpContext.User.Identity.GetUserName() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var currentUser = HttpContext.User.Identity.GetUserName();
            var getCurrentClass = db.UserGroups.Where(x => x.Id == classId && x.Email == currentUser).SingleOrDefault();
            if (getCurrentClass != null)
            {
                ViewBag.classId = classId;
                ViewBag.studentId = studentId;
                if (HttpContext.User.Identity.GetUserName() == "" || HttpContext.User.Identity.GetUserName() == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                ViewBag.CurrentSort = sortOrder;
                ViewBag.VocabSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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
                var vocab = db.Vocabularies.Where(s => s.UserId == studentId);
                if (!String.IsNullOrEmpty(searchString))
                {
                    vocab = vocab.Where(s => s.EngWord.Contains(searchString)
                                           || s.VnWord.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        vocab = vocab.OrderByDescending(s => s.EngWord);
                        break;

                    case "Date":
                        vocab = vocab.OrderBy(s => s.CreatedDate);
                        break;

                    case "date_desc":
                        vocab = vocab.OrderByDescending(s => s.CreatedDate);
                        break;

                    default:  // Name ascending
                        vocab = vocab.OrderBy(s => s.EngWord);
                        break;
                }

                int pageSize = 5;
                int pageNumber = (page ?? 1);
                return View(vocab.ToPagedList(pageNumber, pageSize));
            }
            return RedirectToAction("Index", "UserGroups");
        }

        //Hiển thị danh sách các Request
        [HttpGet]
        public ActionResult getListRequest(int? classId)
        {
            if (HttpContext.User.Identity.GetUserName() == "" || HttpContext.User.Identity.GetUserName() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var currentUser = HttpContext.User.Identity.GetUserName();
            var getCurrentClass = db.UserGroups.Where(x => x.Id == classId && x.Email == currentUser).Single();
            if (getCurrentClass != null)
            {
                var getRequestList = db.UserAcceptances.Where(x => x.GroupId == classId).ToList();
                return Json(getRequestList, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Index", "UserGroups");
        }

        //Người chủ lớp khi tạo request sẽ lấy trả về giao diện hiện tại và để gọi hàm acceptRequest hoặc denyRequest
        [HttpGet]
        public ActionResult currentRequest(int? classId, int idRequest)
        {
            var currentRequest = db.UserAcceptances.Where(x => x.Id == idRequest).ToList();
            return Json(currentRequest, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult getListRequestUser()
        {
            var currentUser = User.Identity.GetUserName();
            var lstRequest = db.UserAcceptances.Where(x => x.UserEmail == currentUser).ToList();
            return Json(lstRequest, JsonRequestBehavior.AllowGet);
        }
    }
}