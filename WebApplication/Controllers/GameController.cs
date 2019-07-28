using Microsoft.AspNet.Identity;
using SpeakOutWeb.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SpeakOutWeb.Controllers
{
    public class GameController : Controller
    {
        private SpeakOutEntities db = new SpeakOutEntities();

        // GET: Game
        public ActionResult Index()
        {
            if(HttpContext.User.Identity.GetUserName() == "" || HttpContext.User.Identity.GetUserName() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var userID = HttpContext.User.Identity.GetUserName();
            ViewBag.Count = db.Vocabularies.Where(x => x.UserId == userID && x.Bookmark==false).Count();
            return View();
        }
        public ActionResult MatchingGame()
        {
            if (HttpContext.User.Identity.GetUserName() == "" || HttpContext.User.Identity.GetUserName() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public ActionResult WritingGame()
        {
            if (HttpContext.User.Identity.GetUserName() == "" || HttpContext.User.Identity.GetUserName() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public static bool IsDup(int tmp, int[] arr)
        {
            foreach (var item in arr)
            {
                if (item == tmp)
                {
                    return true;
                }
            }
            return false;
        }

        [HttpGet]
        public ActionResult GetRandomCard()
        {
            var currentUser = User.Identity.GetUserName();
            var lstCard = db.Vocabularies.Where(x => x.UserId == currentUser && x.Bookmark==false).ToList();
            var newCard = lstCard.Take(8).ToList();
            var countCard = db.Vocabularies.Where(x => x.UserId == currentUser && x.Bookmark == false).Count();
            
            if (countCard >= 9)
            {
                Random rnd = new Random();
                int tmp;
                int[] arr = new int[8];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = -1;
                }
                for (int i = 0; i < 8; i++)
                {
                    tmp = rnd.Next(countCard);
                    while (IsDup(tmp, arr))
                    {
                        tmp = rnd.Next(countCard);
                    }
                    arr[i] = tmp;
                }
                for (int i = 0; i < arr.Length; i++)
                {
                    newCard[i] = lstCard[arr[i]];
                }
                return Json(newCard, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(lstCard, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetListCard(int number)
        {
            var currentUser = User.Identity.GetUserName();
            var lstCard = db.Vocabularies.Where(x => x.UserId == currentUser && x.Bookmark == false).ToList();
            var countCard = db.Vocabularies.Where(x => x.UserId == currentUser && x.Bookmark == false).Count();
            ViewBag.CountWord = countCard;
            var newCard = lstCard.Take(number).ToList();
            if (countCard < number)
            {
                return Json("Bạn nhập quá số lượng từ", JsonRequestBehavior.AllowGet);
            }
            Random rnd = new Random();
            int tmp;
            int[] arr = new int[number];
            //Set -1 for all element in array
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = -1;
            }
            for (int i = 0; i < number; i++)
            {
                tmp = rnd.Next(countCard);
                while (IsDup(tmp, arr))
                {
                    tmp = rnd.Next(countCard);
                }
                arr[i] = tmp;
            }
            for (int i = 0; i < arr.Length; i++)
            {
                newCard[i] = lstCard[arr[i]];
            }
            return Json(newCard, JsonRequestBehavior.AllowGet);
        }
    }
}