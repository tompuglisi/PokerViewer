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
    public class playsController : Controller
    {
        private PokerDB db = new PokerDB();

        // GET: plays
		public ActionResult Index(int? itemsPerPage, int? page, string sortOrder, string searchString, string currentFilter)
		{

			ViewBag.CurrentSort = sortOrder;
			ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";  //String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
			ViewBag.StartingStack = sortOrder == "StartingStack" ? "StartingStack_desc" : "StartingStack";
			ViewBag.EndingStackSortParm = sortOrder == "EndingStackSortParm" ? "EndingStackSortParm_desc" : "EndingStackSortParm";
			ViewBag.SeatPositionSortParm = sortOrder == "SeatPositionSortParm" ? "SeatPositionSortParm_desc" : "SeatPositionSortParm";
			ViewBag.HoleCard1SortParm = sortOrder == "HoleCard1SortParm" ? "HoleCard1SortParm_desc" : "HoleCard1SortParm";
			ViewBag.HoleCard2SortParm = sortOrder == "HoleCard2SortParm" ? "HoleCard2SortParm_desc" : "HoleCard2SortParm";
			ViewBag.handTableID = sortOrder == "hand.TableID" ? "hand.TableID_desc" : "hand.TableID";
			ViewBag.playerName = sortOrder == "player.Name" ? "player.Name_desc" : "player.Name";
			ViewBag.CurrentItemsPerPage = itemsPerPage;

			if (searchString != null)
			{
				page = 1;
			}
			else
			{
				searchString = currentFilter;
			}

			var plays = db.plays.Include(p => p.hand).Include(p => p.player);
			//parse search string
			if (!String.IsNullOrEmpty(searchString))
			{
				plays = plays.Where(s => s.player.Name.Contains(searchString));
			}
			ViewBag.CurrentFilter = searchString;
			//Handle toggling between asc and desc
			switch (sortOrder)
			{
				case "id":
					plays = plays.OrderBy(s => s.HandID);
					break;

				case "id_desc":
					plays = plays.OrderByDescending(s => s.HandID);
					break;

				case "StartingStack":
					plays = plays.OrderBy(s => s.StartingStack);
					break;
				case "StartingStack_desc":
					plays = plays.OrderByDescending(s => s.StartingStack);
					break;
				case "EndingStackSortParm":
					plays = plays.OrderBy(s => s.EndingStack);
					break;
				case "EndingStackSortParm_desc":
					plays = plays.OrderByDescending(s => s.EndingStack);
					break;
				case "SeatPositionSortParm":
					plays = plays.OrderBy(s => s.SeatPosition);
					break;
				case "SeatPositionSortParm_desc":
					plays = plays.OrderByDescending(s => s.SeatPosition);
					break;
				case "HoleCard1SortParm":
					plays = plays.OrderBy(s => s.HoleCard1);
					break;
				case "HoleCard1SortParm_desc":
					plays = plays.OrderByDescending(s => s.HoleCard1);
					break;
				case "HoleCard2SortParm_desc":
					plays = plays.OrderByDescending(s => s.HoleCard2);
					break;
				case "HoleCard2SortParm":
					plays = plays.OrderBy(s => s.HoleCard2);
					break;
				case "hand.TableID_desc":
					plays = plays.OrderByDescending(s => s.hand.TableID);
					break;
				case "hand.TableID":
					plays = plays.OrderBy(s => s.hand.TableID);
					break;
				case "player.Name_desc":
					plays = plays.OrderByDescending(s => s.player.Name);
					break;
				case "player.Name":
					plays = plays.OrderBy(s => s.player.Name);
					break;
			

				default:
					plays = plays.OrderBy(s => s.HandID);
					break;
			}
            
			return View(plays.ToList().ToList().ToPagedList(pageNumber: page ?? 1, pageSize: itemsPerPage ?? 25));
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
            ViewBag.HandID = new SelectList(db.plays, "HandID", "StartingStack");
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

            ViewBag.HandID = new SelectList(db.plays, "HandID", "StartingStack", play.HandID);
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
            ViewBag.HandID = new SelectList(db.plays, "HandID", "StartingStack", play.HandID);
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
            ViewBag.HandID = new SelectList(db.plays, "HandID", "StartingStack", play.HandID);
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