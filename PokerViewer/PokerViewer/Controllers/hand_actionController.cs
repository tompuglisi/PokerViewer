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
    public class hand_actionController : Controller
    {
        private PokerDB db = new PokerDB();

        // GET: hand_action
        public ActionResult Index()
        {
            var hand_action = db.hand_action.Include(h => h.hand).Include(h => h.player);
            return View(hand_action.ToList());
        }

        // GET: hand_action/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hand_action hand_action = db.hand_action.Find(id);
            if (hand_action == null)
            {
                return HttpNotFound();
            }
            return View(hand_action);
        }

        // GET: hand_action/Create
        public ActionResult Create()
        {
            ViewBag.HandID = new SelectList(db.hands, "HandID", "TableID");
            ViewBag.PlayerName = new SelectList(db.players, "Name", "Name");
            return View();
        }

        // POST: hand_action/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HandID,ActionID,PlayerName,ActionName,Street,Amount,IsPFR,IsVPIP,Is3Bet,Is4Bet")] hand_action hand_action)
        {
            if (ModelState.IsValid)
            {
                db.hand_action.Add(hand_action);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HandID = new SelectList(db.hands, "HandID", "TableID", hand_action.HandID);
            ViewBag.PlayerID = new SelectList(db.players, "Name", "Name", hand_action.PlayerID);
            return View(hand_action);
        }

        // GET: hand_action/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hand_action hand_action = db.hand_action.Find(id);
            if (hand_action == null)
            {
                return HttpNotFound();
            }
            ViewBag.HandID = new SelectList(db.hands, "HandID", "TableID", hand_action.HandID);
            ViewBag.PlayerID = new SelectList(db.players, "Name", "Name", hand_action.PlayerID);
            return View(hand_action);
        }

        // POST: hand_action/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HandID,ActionID,PlayerName,ActionName,Street,Amount,IsPFR,IsVPIP,Is3Bet,Is4Bet")] hand_action hand_action)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hand_action).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HandID = new SelectList(db.hands, "HandID", "TableID", hand_action.HandID);
            ViewBag.PlayerID = new SelectList(db.players, "Name", "Name", hand_action.PlayerID);
            return View(hand_action);
        }

        // GET: hand_action/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hand_action hand_action = db.hand_action.Find(id);
            if (hand_action == null)
            {
                return HttpNotFound();
            }
            return View(hand_action);
        }

        // POST: hand_action/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            hand_action hand_action = db.hand_action.Find(id);
            db.hand_action.Remove(hand_action);
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
