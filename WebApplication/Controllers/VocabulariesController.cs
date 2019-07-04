using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpeakOutWeb.Models;
using Microsoft.AspNet.Identity;

namespace SpeakOutWeb.Controllers
{
    public class VocabulariesController : Controller
    {
        private SpeakOutEntities db = new SpeakOutEntities();

        // GET: Vocabularies
        public ActionResult Index()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var vocab = db.Vocabularies.Where(s => s.UserId == userId).ToList();
            return View(vocab);
        }


        [HttpGet]
        public JsonResult getVocabularies()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var userName = User.Identity.GetUserId();
            var dictionaries = db.Vocabularies.Select(p => new
            {
                p.Id,
                p.UserId,
                p.VnWord,
                p.EngWord,
                p.Spelling,
                p.CreatedDate,
                p.Bookmark
            }).Where(s => s.UserId == userName || s.Bookmark== false).OrderByDescending(x => x.Id).ToList();
            if (userName == null || userName == "")
            {
                return Json("Không tìm thấy từ điển", JsonRequestBehavior.AllowGet);
            }

            return Json(dictionaries, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult SaveEntity(Vocabulary vocabulary)
        {
            db.Configuration.ProxyCreationEnabled = false;
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {

                if (vocabulary.Id == 0)
                {
                    if(vocabulary.Spelling==null || (vocabulary.Spelling == "" && vocabulary.VnWord==""))
                    {
                        return Json("Không tìm thấy tư điển của bạn", JsonRequestBehavior.AllowGet);
                    }
                    var listAvailable = db.Vocabularies.ToList();
                    foreach (var item in listAvailable)
                    {
                        if (item.EngWord == vocabulary.EngWord)
                        {
                            return Json("Từ điển của bạn đã được lưu", JsonRequestBehavior.AllowGet);
                        }
                    }
                    vocabulary.CreatedDate = DateTime.Now;
                    vocabulary.Bookmark = false;
                    vocabulary.UserId = HttpContext.User.Identity.GetUserId();
                    if (vocabulary.UserId == null)
                    {
                        return Json("Check your login!", JsonRequestBehavior.AllowGet);
                    }
                    db.Vocabularies.Add(vocabulary);
                }
                else
                {
                    vocabulary.Bookmark = true;
                    db.Entry(vocabulary).State = EntityState.Modified;
                }
                db.SaveChanges();
                return Json("Save successfully", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateBookmark(int Id)
        {
            Vocabulary vocabulary = db.Vocabularies.Single(x => x.Id == Id);
            vocabulary.Bookmark = true;
            db.Entry(vocabulary).State = EntityState.Modified;
            db.SaveChanges();
            return Json("Save successfully", JsonRequestBehavior.AllowGet);

        }




    }
}
