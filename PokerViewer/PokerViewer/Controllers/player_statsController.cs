using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PokerViewer.Models;

namespace PokerViewer.Controllers
{
    public class player_statsController : Controller
    {
        private PokerDB db = new PokerDB();

        // GET: player_stats
        public ActionResult Index()
        {
            return View(db.player_stats.ToList());
        }

        // GET: player_stats/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            player_stats player_stats = db.player_stats.Find(id);
            if (player_stats == null)
            {
                return HttpNotFound();
            }
            return View(player_stats);
        }
    }
}
