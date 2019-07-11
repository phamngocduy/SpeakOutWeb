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
            var userID = HttpContext.User.Identity.GetUserId();
            ViewBag.Count = db.Vocabularies.Where(x => x.UserId == userID).Count();
            return View();
        }
        public ActionResult MatchingGame()
        {
            return View();
        }
        public ActionResult WritingGame()
        {
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
            var currentUser = User.Identity.GetUserId();
            var lstCard = db.Vocabularies.Where(x => x.UserId == currentUser).ToList();
            var newCard = lstCard.Take(8).ToList();
            var countCard = db.Vocabularies.Where(x => x.UserId == currentUser).Count();
            if (countCard >= 9)
            {
                Random rnd = new Random();
                int tmp;
                int[] arr = new int[countCard];
                for (int i = 0; i < countCard; i++)
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
            var currentUser = User.Identity.GetUserId();
            var lstCard = db.Vocabularies.Where(x => x.UserId == currentUser)
                .Select(s => new Vocabulary
                {
                    EngWord = s.EngWord,
                    VnWord = s.VnWord,
                    Spelling = s.Spelling
                }).ToList();
            var countCard = db.Vocabularies.Where(x => x.UserId == currentUser).Count();
            var newCard = lstCard.Take(number).ToList();
            if (countCard < number)
            {
                return Json("Bạn nhập quá số lượng từ", JsonRequestBehavior.AllowGet);
            }
            Random rnd = new Random();
            int tmp;
            int[] arr = new int[number];
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