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
    public class player_statsController : Controller
    {
        private PokerDB db = new PokerDB();

        // GET: player_stats
        public ActionResult Index(int? itemsPerPage, int? page, string sortOrder, string searchString, string currentFilter)
        {

			ViewBag.CurrentSort = sortOrder;
			ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";  //String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
			ViewBag.HandsPlayedSortParm = sortOrder == "HandsPlayedSortParm" ? "HandsPlayedSortParm_desc" : "HandsPlayedSortParm";
			ViewBag.WinningsSortParm = sortOrder == "WinningsSortParm" ? "WinningsSortParm_desc" : "WinningsSortParm";
			ViewBag.VPIPSortParm = sortOrder == "VPIPSortParm" ? "VPIPSortParm_desc" : "VPIPSortParm";
			ViewBag.PFRSortParm = sortOrder == "PFRSortParm" ? "PFRSortParm_desc" : "PFRSortParm";
			ViewBag.ThreeBetSortParm = sortOrder == "ThreeBetSortParm" ? "ThreeBetSortParm_desc" : "ThreeBetSortParm";
			ViewBag.FourBetSortParm = sortOrder == "FourBet" ? "FourBet_desc" : "FourBet";
			ViewBag.PFAFSortParm = sortOrder == "PFAF" ? "PFAF_desc" : "PFAF";
		
			ViewBag.CurrentItemsPerPage = itemsPerPage;

			if (searchString != null)
			{
				page = 1;
			}
			else
			{
				searchString = currentFilter;
			}

			var player_stats = db.player_stats.Where(p => p.Name.Length>0);
			//parse search string
			if (!String.IsNullOrEmpty(searchString))
			{
				long searchID = 0;
				long.TryParse(searchString, out searchID);
				player_stats = player_stats.Where(s => s.Name.Equals(searchID));
			}
			ViewBag.CurrentFilter = searchString;
			//Handle toggling between asc and desc
			switch (sortOrder)
			{
				case "id":
					player_stats = player_stats.OrderBy(s => s.Name);
					break;

				case "id_desc":
					player_stats = player_stats.OrderByDescending(s => s.Name);
					break;

				case "HandsPlayedSortParm":
					player_stats = player_stats.OrderBy(s => s.HandsPlayed);
					break;
				case "HandsPlayedSortParm_desc":
					player_stats = player_stats.OrderByDescending(s => s.HandsPlayed);
					break;
				case "WinningsSortParm":
					player_stats = player_stats.OrderBy(s => s.Winnings);
					break;
				case "WinningsSortParm_desc":
					player_stats = player_stats.OrderByDescending(s => s.Winnings);
					break;
				case "VPIPSortParm":
					player_stats = player_stats.OrderBy(s => s.VPIP);
					break;
				case "VPIPSortParm_desc":
					player_stats = player_stats.OrderByDescending(s => s.VPIP);
					break;
				case "PFRSortParm":
					player_stats = player_stats.OrderBy(s => s.PFR);
					break;
				case "PFRSortParm_desc":
					player_stats = player_stats.OrderByDescending(s => s.PFR);
					break;
				case "ThreeBetSortParm_desc":
					player_stats = player_stats.OrderByDescending(s => s.ThreeBet);
					break;
				case "ThreeBetSortParm":
					player_stats = player_stats.OrderBy(s => s.ThreeBet);
					break;
				case "FourBet_desc":
					player_stats = player_stats.OrderByDescending(s => s.FourBet);
					break;
				case "FourBet":
					player_stats = player_stats.OrderBy(s => s.FourBet);
					break;
				case "PFAF_desc":
					player_stats = player_stats.OrderByDescending(s => s.PFAF);
					break;
				case "PFAF":
					player_stats = player_stats.OrderBy(s => s.PFAF);
					break;


				default:
					player_stats = player_stats.OrderBy(s => s.Name);
					break;
			}
            return View(player_stats.ToList().ToPagedList(pageNumber: page ?? 1, pageSize: itemsPerPage ?? 25));
        }

        // GET: player_stats/Details/5
        public ActionResult Details(long? id)
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

        // GET: players/Create
        public ActionResult Create()
        {
            return RedirectToAction("Create", "players");
        }

        // GET: players/Edit/5
        public ActionResult Edit(long? id)
        {
            return RedirectToAction("Edit", "players", new { id = id });
        }

        // GET: players/Delete/5
        public ActionResult Delete(long? id)
        {
            return RedirectToAction("Delete", "players", new { id = id });
        }
    }
}
