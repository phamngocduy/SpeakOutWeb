using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SpeakOutWeb.Models;

namespace SpeakOutWeb.Controllers
{
    public class UserGroupDetailsController : Controller
    {
        private SpeakOutEntities db = new SpeakOutEntities();

        // GET: UserGroupDetails
        public ActionResult Index()
        {
            var userGroupDetails = db.UserGroupDetails.Include(u => u.UserGroup);
            return View(userGroupDetails.ToList());
        }
        public JsonResult getCurrentUser(string email, int groupId)
        {
            var currentUser = HttpContext.User.Identity.GetUserName();
            var checkAttend = db.UserGroupDetails.Where(x => x.GroupId == groupId && x.StudentEmail == email).Count();
            if (checkAttend == 0)
            {
                return Json("Join", JsonRequestBehavior.AllowGet);
            }
            return Json("Detail", JsonRequestBehavior.AllowGet);
        }
        public JsonResult getAllUser(string email, int groupId)
        {
            var currentUser = HttpContext.User.Identity.GetUserName();
            var checkAttend = db.UserGroupDetails.Where(x => x.GroupId == groupId && x.StudentEmail == email).Count();
            if (checkAttend == 0)
            {
                return Json("Join", JsonRequestBehavior.AllowGet);
            }
            return Json("Detail", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AcceptRequest(string name, int idClass, int id, string email)
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
                UserGroupDetail userGroupDetail = new UserGroupDetail();
                userGroupDetail.Id = 0;
                userGroupDetail.StudentEmail = email;
                userGroupDetail.StudentName = name;
                userGroupDetail.GroupId = idClass;
                if (userGroupDetail.Id == 0)
                {
                    userGroupDetail.CreatedDate = DateTime.Now;
                    db.UserGroupDetails.Add(userGroupDetail);
                    db.SaveChanges();
                }
                var userAcceptance = db.UserAcceptances.Find(id);
                db.UserAcceptances.Remove(userAcceptance);
                db.SaveChanges();
            }
            return Json("Gửi thành công", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //Xử lý xóa request sau khi chấp nhận hoặc sau khi lưu
        public ActionResult DeleteRequest(int idClass)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var userAcceptance = db.UserAcceptances.Find(idClass);
            db.UserAcceptances.Remove(userAcceptance);
            db.SaveChanges();
            return Json("Gửi thành công", JsonRequestBehavior.AllowGet);
        }
       
    }
}
