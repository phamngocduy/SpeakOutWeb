using Microsoft.AspNet.Identity;
using SpeakOutWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SpeakOutWeb.Controllers
{
    public class UserGroupsAcceptanceController : Controller
    {
        private SpeakOutEntities db = new SpeakOutEntities();
        // GET: UserGroupsAcceptance
        public ActionResult Index()
        {
            return View();
        }
        //Tạo yêu cầu join vào lớp từ Interface danh sách lớp học
        [HttpPost]
        public ActionResult CreateRequest(string name, int idClass)
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
                var getUser = db.UserAcceptances.Where(x => x.UserEmail == currentUser && x.GroupId == idClass).Count();
                if (getUser > 0)
                {
                    return Json("Bạn đã gửi lời mời tham gia nhóm", JsonRequestBehavior.AllowGet);
                }
                var getUserinClass = db.UserGroupDetails.Where(x => x.StudentEmail == currentUser && x.GroupId == idClass).Count();
                if (getUserinClass > 0)
                {
                    return Json("Bạn đã tham gia nhóm", JsonRequestBehavior.AllowGet);
                }
                var getUserOwner = db.UserGroups.Where(x => x.Email == currentUser && x.Id == idClass).Count();
                if (getUserOwner > 0)
                {
                    return Json("Bạn đã là người tạo nhóm", JsonRequestBehavior.AllowGet);
                }
                UserAcceptance userAcceptance = new UserAcceptance();
                userAcceptance.Id = 0;
                if (userAcceptance.Id == 0)
                {
                    userAcceptance.UserName = name;
                    userAcceptance.GroupId = idClass;
                    userAcceptance.CreatedDate = DateTime.Now;
                    userAcceptance.UserEmail = currentUser;
                    db.UserAcceptances.Add(userAcceptance);
                    db.SaveChanges();
                }
            }
            return Json("Gửi thành công", JsonRequestBehavior.AllowGet);
        }
    }
}