using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PokerViewer.Models;
using PagedList;

namespace PokerViewer.Controllers
{
    public class handsController : Controller
    {
        private PokerDB db = new PokerDB();

        // GET: hands
		public ActionResult Index(int? itemsPerPage, int? page)
        {
            var hands = db.hands.Include(h => h.table);
			return View(hands.ToList().ToPagedList(pageNumber: page ?? 1, pageSize: itemsPerPage ?? 25));
            
        }

        // GET: hands/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hand hand = db.hands.Find(id);
            if (hand == null)
            {
                return HttpNotFound();
            }
            return View(hand);
        }

        // GET: hands/Create
        public ActionResult Create()
        {
            ViewBag.TableID = new SelectList(db.tables, "TableID", "Stakes");
            return View();
        }

        // POST: hands/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HandID,TableID,NumPlayers,StartTime,ButtonPosition,PotSize,FlopCard1,FlopCard2,FlopCard3,TurnCard,RiverCard")] hand hand)
        {
            if (ModelState.IsValid)
            {
                db.hands.Add(hand);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TableID = new SelectList(db.tables, "TableID", "Stakes", hand.TableID);
            return View(hand);
        }

        // GET: hands/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hand hand = db.hands.Find(id);
            if (hand == null)
            {
                return HttpNotFound();
            }
            ViewBag.TableID = new SelectList(db.tables, "TableID", "Stakes", hand.TableID);
            return View(hand);
        }

        // POST: hands/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HandID,TableID,NumPlayers,StartTime,ButtonPosition,PotSize,FlopCard1,FlopCard2,FlopCard3,TurnCard,RiverCard")] hand hand)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hand).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TableID = new SelectList(db.tables, "TableID", "Stakes", hand.TableID);
            return View(hand);
        }

        // GET: hands/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hand hand = db.hands.Find(id);
            if (hand == null)
            {
                return HttpNotFound();
            }
            return View(hand);
        }

        // POST: hands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            hand hand = db.hands.Find(id);
            db.hands.Remove(hand);
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
