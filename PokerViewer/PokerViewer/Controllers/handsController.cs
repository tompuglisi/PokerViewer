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
		public ActionResult Index(int? itemsPerPage, int? page, string sortOrder, string searchString, string currentFilter)
        {

			ViewBag.CurrentSort = sortOrder;
			ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";  //String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
			ViewBag.TableIdSortParm = sortOrder == "TableIdSortParm" ? "TableIdSortParm_desc" : "TableIdSortParm";
			ViewBag.NumPlayersSortParm = sortOrder == "NumPlayersSortParm" ? "NumPlayersSortParm_desc" : "NumPlayersSortParm";
			ViewBag.StartTimeSortParm = sortOrder == "StartTimeSortParm" ? "StartTimeSortParm_desc" : "StartTimeSortParm";
			ViewBag.ButtonPositionSortParm = sortOrder == "ButtonPositionSortParm" ? "ButtonPositionSortParm_desc" : "ButtonPositionSortParm";
			ViewBag.PotSizeSortParm = sortOrder == "PotSizeSortParm" ? "PotSizeSortParm_desc" : "PotSizeSortParm";
			ViewBag.FlopCard1 = sortOrder == "FlopCard1" ? "FlopCard1_desc" : "FlopCard1";
			ViewBag.FlopCard2 = sortOrder == "FlopCard2" ? "FlopCard2_desc" : "FlopCard2";
			ViewBag.FlopCard3 = sortOrder == "FlopCard3" ? "FlopCard3_desc" : "FlopCard3";
			ViewBag.TurnCard = sortOrder == "TurnCard" ? "TurnCard_desc" : "TurnCard";
			ViewBag.RiverCard = sortOrder == "RiverCard" ? "RiverCard_desc" : "RiverCard";
			ViewBag.tableStakes = sortOrder == "tableStakes" ? "tableStakes_desc" : "tableStakes";
			ViewBag.CurrentItemsPerPage = itemsPerPage;

			if (searchString != null)
			{
				page = 1;
			}
			else
			{
				searchString = currentFilter;
			}

			var hands = db.hands.Include(h => h.table);
			//parse search string
			if (!String.IsNullOrEmpty(searchString))
			{
				long searchID = 0;
				long.TryParse(searchString, out searchID);
				hands = hands.Where(s => s.HandID == searchID);
			}
			ViewBag.CurrentFilter = searchString;
			//Handle toggling between asc and desc
			switch (sortOrder)
			{
				case "id":
					hands = hands.OrderBy(s => s.HandID);
					break;

				case "id_desc":
					hands = hands.OrderByDescending(s => s.HandID);
					break;

				case "TableIdSortParm":
					hands = hands.OrderBy(s => s.TableID);
					break;
				case "TableIdSortParm_desc":
					hands = hands.OrderByDescending(s => s.TableID);
					break;
				case "NumPlayersSortParm":
					hands = hands.OrderBy(s => s.NumPlayers);
					break;
				case "NumPlayersSortParm_desc":
					hands = hands.OrderByDescending(s => s.NumPlayers);
					break;
				case "StartTimeSortParm":
					hands = hands.OrderBy(s => s.StartTime);
					break;
				case "StartTimeParm_desc":
					hands = hands.OrderByDescending(s => s.StartTime);
					break;
				case "ButtonPositionSortParm":
					hands = hands.OrderBy(s => s.ButtonPosition);
					break;
				case "ButtonPositionSortParm_desc":
					hands = hands.OrderByDescending(s => s.ButtonPosition);
					break;
				case "PotSizeParm_desc":
					hands = hands.OrderByDescending(s => s.PotSize);
					break;
				case "PotSizeSortParm":
					hands = hands.OrderBy(s => s.PotSize);
					break;
				case "FlopCard1_desc":
					hands = hands.OrderByDescending(s => s.FlopCard1);
					break;
				case "FlopCard1":
					hands = hands.OrderBy(s => s.FlopCard1);
					break;
				case "FlopCard2_desc":
					hands = hands.OrderByDescending(s => s.FlopCard2);
					break;
				case "FlopCard2":
					hands = hands.OrderBy(s => s.FlopCard2);
					break;
				case "FlopCard3_desc":
					hands = hands.OrderByDescending(s => s.FlopCard3);
					break;
				case "FlopCard3":
					hands = hands.OrderBy(s => s.FlopCard3);
					break;
				case "TurnCard_desc":
					hands = hands.OrderByDescending(s => s.TurnCard);
					break;
				case "TurnCard":
					hands = hands.OrderBy(s => s.TurnCard);
					break;
				case "RiverCard_desc":
					hands = hands.OrderByDescending(s => s.RiverCard);
					break;
				case "RiverCard":
					hands = hands.OrderBy(s => s.RiverCard);
					break;
				case "tableStakes_desc":
					hands = hands.OrderByDescending(s => s.table.Stakes);
					break;
				case "tableStakes":
					hands = hands.OrderBy(s => s.table.Stakes);
					break;

				default:

					break;
			}
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
