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
    public class player_matchupsController : Controller
    {
        private PokerDB db = new PokerDB();

        // GET: player_matchups
        public ActionResult Index()
        {
            return View(db.player_matchups.ToList());
        }

        public ActionResult Details(long? id)
        {
            if (id == null) return RedirectToAction("Index");
            return View(db.player_matchups.Where(s => s.Player1_ID == id).ToList());
        }
    }
}
