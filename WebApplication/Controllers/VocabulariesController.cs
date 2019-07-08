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
            ViewBag.CurrentSort = sortOrder;
            ViewBag.VocabSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var userId = HttpContext.User.Identity.GetUserId();
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
            var currentUser = User.Identity.GetUserId();
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
                    var listAvailable = db.Vocabularies.Where(x=>x.UserId== currentUser).ToList();
                    foreach (var item in listAvailable)
                    {
                        if (item.EngWord == vocabulary.EngWord.ToLower())
                        {
                            return Json("Từ điển của bạn đã được lưu", JsonRequestBehavior.AllowGet);
                        }
                    }
                    vocabulary.VnWord = vocabulary.VnWord.Trim();
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
