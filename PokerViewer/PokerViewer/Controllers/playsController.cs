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
    public class playsController : Controller
    {
        private PokerDB db = new PokerDB();

        // GET: plays
        public ActionResult Index()
        {
            var plays = db.plays.Include(p => p.hand).Include(p => p.player);
            return View(plays.ToList());
        }

        // GET: plays/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            play play = db.plays.Find(id);
            if (play == null)
            {
                return HttpNotFound();
            }
            return View(play);
        }

        // GET: plays/Create
        public ActionResult Create()
        {
            ViewBag.HandID = new SelectList(db.hands, "HandID", "TableID");
            ViewBag.PlayerName = new SelectList(db.players, "Name", "Name");
            return View();
        }

        // POST: plays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PlayerName,HandID,StartingStack,EndingStack,SeatPosition,HoleCard1,HoleCard2")] play play)
        {
            if (ModelState.IsValid)
            {
                db.plays.Add(play);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HandID = new SelectList(db.hands, "HandID", "TableID", play.HandID);
            ViewBag.PlayerID = new SelectList(db.players, "Name", "Name", play.PlayerID);
            return View(play);
        }

        // GET: plays/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            play play = db.plays.Find(id);
            if (play == null)
            {
                return HttpNotFound();
            }
            ViewBag.HandID = new SelectList(db.hands, "HandID", "TableID", play.HandID);
            ViewBag.PlayerID = new SelectList(db.players, "Name", "Name", play.PlayerID);
            return View(play);
        }

        // POST: plays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PlayerName,HandID,StartingStack,EndingStack,SeatPosition,HoleCard1,HoleCard2")] play play)
        {
            if (ModelState.IsValid)
            {
                db.Entry(play).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HandID = new SelectList(db.hands, "HandID", "TableID", play.HandID);
            ViewBag.PlayerID = new SelectList(db.players, "Name", "Name", play.PlayerID);
            return View(play);
        }

        // GET: plays/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            play play = db.plays.Find(id);
            if (play == null)
            {
                return HttpNotFound();
            }
            return View(play);
        }

        // POST: plays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            play play = db.plays.Find(id);
            db.plays.Remove(play);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
