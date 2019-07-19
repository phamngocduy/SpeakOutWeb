using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpeakOutWeb.Models
{
    public class RSSFeed
    {
        public string Title { set; get; }
        public string Link { set; get; }
        public string Description { set; get; }
        public string PubDate { set; get; }
    }
}