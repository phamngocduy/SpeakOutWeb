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
using PagedList;

namespace SpeakOutWeb.Controllers
{
    public class VocabulariesController : Controller
    {
        private SpeakOutEntities db = new SpeakOutEntities();

        // GET: Vocabularies
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
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
            var vocab = db.Vocabularies.Where(s => s.UserId == userId);
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

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(vocab.ToPagedList(pageNumber, pageSize));
        }


        [HttpGet]
        public JsonResult getVocabularies()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var userName = User.Identity.GetUserName();
            if (userName == null || userName == "")
            {
                return Json("Không tìm thấy từ điển của bạn", JsonRequestBehavior.AllowGet);
            }
            var dictionaries = db.Vocabularies.Where(s => s.UserId == userName && s.Bookmark== false).OrderByDescending(x => x.Id).ToList();
            return Json(dictionaries, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult SaveEntity(Vocabulary vocabulary)
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

                if (vocabulary.Id == 0)
                {
                    if(vocabulary.Spelling==null || (vocabulary.Spelling == "" && vocabulary.VnWord==""))
                    {
                        return Json("Không tìm thấy tư vựng của bạn", JsonRequestBehavior.AllowGet);
                    }
                    var listAvailable = db.Vocabularies.Where(x=>x.UserId== currentUser).ToList();
                    foreach (var item in listAvailable)
                    {
                        if (item.EngWord == vocabulary.EngWord.ToLower())
                        {
                            return Json("Từ vựng của bạn đã được lưu", JsonRequestBehavior.AllowGet);
                        }
                    }
                    vocabulary.VnWord = vocabulary.VnWord.Trim();
                    vocabulary.EngWord = vocabulary.EngWord.ToLower();
                    vocabulary.CreatedDate = DateTime.Now;
                    vocabulary.Bookmark = false;
                    vocabulary.UserId = currentUser;
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
                return Json("Lưu thành công", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateBookmark(int Id)
        {
            Vocabulary vocabulary = db.Vocabularies.Single(x => x.Id == Id);
            vocabulary.Bookmark = true;
            db.Entry(vocabulary).State = EntityState.Modified;
            db.SaveChanges();
            return Json("Lưu thành công", JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult CheckAvailable(string checkWord)
        {
            var currentUser = User.Identity.GetUserName();
            var listVocab = db.Vocabularies.Where(x => x.UserId == currentUser).ToList();
            var countWord = 0;
            foreach (var item in listVocab)
            {
                if (item.EngWord == checkWord.ToLower())
                {
                    countWord++;
                }
            }
            return Json(countWord, JsonRequestBehavior.AllowGet);
        }
    }
}
