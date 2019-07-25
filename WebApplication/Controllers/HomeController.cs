using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.AspNet.Identity;
using SpeakOutWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SpeakOutWeb.Controllers
{
    public class HomeController : Controller
    {
        private SpeakOutEntities db = new SpeakOutEntities();
        public ActionResult Index()
        {
            ViewBag.Account = HttpContext.User.Identity.GetUserId();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost, ActionName("Index")]
        public ActionResult Process(string text = "", string type = null)
        {
            if (text.Trim() == "")
            {
                return View();
            }
            ViewBag.Type = type;
            ViewBag.Text = text = text.Replace('“', '"').Replace('”', '"')
                                      .Replace('‘', '\'').Replace('’', '\'');
            return View("Process", split(text).Split('\n'));
        }

        readonly float MAX = 500;
        public String split(string text = "")
        {
            var texts = new List<String>();
            var paragraphs = Regex.Split(text, @"(?<=\n+)")
                .Where(s => !String.IsNullOrWhiteSpace(s)).Select(s => s.Trim());
            foreach (var paragraph in paragraphs)
            {
                var sentences = Regex.Split(paragraph, @"(?<=[.;:?!]\p{P}*)").ToList();
                int i = 0;
                while (++i < sentences.Count)
                    if (sentences[i].Length == 1 && Char.IsPunctuation(sentences[i][0]))
                    {
                        sentences[i - 1] += sentences[i];
                        sentences.RemoveAt(i);
                    }
                    else if (String.IsNullOrWhiteSpace(sentences[i]))
                        sentences.RemoveAt(i);
                foreach (var sentence in sentences)
                    if (sentence.Length <= 2)
                    {
                        if (texts.Count > 0)
                        {
                            if (texts.Last().Length + sentence.Length <= MAX)
                            {
                                texts.Add(texts.Last() + sentence);
                                texts.RemoveAt(texts.Count - 2);
                            }
                        }
                    }
                    else if (sentence.Length <= MAX)
                        texts.Add(sentence);
                    else
                    {
                        var phrases = Regex.Split(sentence, @"(?<=\p{P}+)").ToList();
                        i = 0;
                        text = "";
                        while (i < phrases.Count)
                        {
                            if (text.Length + phrases[i].Length <= MAX)
                            {
                                text = text + phrases[i++];
                                continue;
                            }
                            else if (text.Length > 50)
                                goto output;
                            if (sentence[i] > 50 && text.Length > 0)
                                goto output;
                            var words = Regex.Split(phrases[i], @"(?<=\s+)");
                            int j = 0;
                            int max = text.Length + phrases[i].Length;
                            max = max / (int)Math.Ceiling(max / MAX);
                            while (text.Length + words[j].Length < max)
                                text = text + words[j++];
                            phrases[i] = String.Join("", words, j, words.Length - j);
                        output:
                            texts.Add(text);
                            text = "";
                        }
                        if (!String.IsNullOrEmpty(text))
                            texts.Add(text);
                    }
                texts.Add(String.Empty);
            }
            return String.Join("\n", texts);
        }

        [HttpPost]
        public string Lookup(string findWord)
        {
            var foundWord = "";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HtmlWeb htmlWeb = new HtmlWeb();
            {
                int count = 0;
                //Load trang web, n?p html vào document
                HtmlDocument document = htmlWeb.Load("https://dictionary.cambridge.org/vi/dictionary/english-vietnamese/" + findWord);
                var threadItems = document.DocumentNode.QuerySelectorAll(".di-body.normal-entry-body .pos-body .def-block .def-body .trans").ToList();
                var threadItems1 = document.DocumentNode.QuerySelectorAll("div.di-head.normal-entry .di-info .pron").ToList();
                var threadItems2 = document.DocumentNode.QuerySelectorAll("div.di-head.normal-entry .di-info .posgram .pos").ToList();
                foreach (var item in threadItems1)
                {
                    foundWord += item.InnerText + " ";
                }
                foundWord += "|";
                foreach (var item in threadItems)
                {
                    if (count <= 10)
                    {
                        foundWord += item.InnerText + "<br/>";
                        count++;
                    }
                }
                foundWord += "|";
                foreach (var item in threadItems2)
                {
                    foundWord += item.InnerText + " ";
                }
                return foundWord;
            }
        }
        [HttpPost]
        public ActionResult SaveLogRecord(string textRead)
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
                    userLog.ContentReading = textRead;
                    userLog.UserId = currentUser;
                    userLog.Type = "Bài nói";
                    db.UserLogs.Add(userLog);
                    db.SaveChanges();
                }

                return Json("Lưu thành công", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult getClasses()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var current = User.Identity.GetUserName();
            var results = from t1 in db.UserGroups
                          join t2 in db.UserGroupDetails
                          on t1.Id equals t2.GroupId
                          where t2.StudentEmail== current
                          select new { GroupName = t1.GroupName };
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult getClassesCreated()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var current = User.Identity.GetUserName();
            var results = db.UserGroups.Where(x => x.Email == current).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }

    }
}