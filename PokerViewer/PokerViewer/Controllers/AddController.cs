using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PokerViewer.Models;
using HandHistories.Objects.Hand;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.Factory;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Objects.Players;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Actions;

namespace PokerViewer.Controllers
{
    public class AddController : Controller
    {
        private PokerDB db = new PokerDB();

        // GET: Add
        public ActionResult Index()
        {
            return RedirectToAction("Index","Hands");
        }

        public ActionResult Result()
        {
            return View();
        }

        // GET: Add/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Add/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FilePath")] Add add)
        {
            List<string> messages = new List<string>();
            if (ModelState.IsValid)
            {
                messages.AddRange(Parse(add.FilePath));
            }
            else messages.Add("Invalid model state. Input was not parsed.");
            ViewData["messages"] = messages as IList<string>;
            return View("Result");
        }

        private List<string> Parse(string filePath)
        {
            List<string> messages = new List<string>();
            try
            {
                int parsedHands = 0;
                int thrownOutHands = 0;
                // Open the text file using a stream reader.
                {
                    SiteName site = SiteName.PokerStars;
                    string contents = System.IO.File.ReadAllText(filePath);
                    messages.AddRange(ParseHands(site, contents, ref parsedHands, ref thrownOutHands));
                    messages.Add("Number of parsed hands:" + parsedHands);
                    messages.Add("Number of thrown hands:" + thrownOutHands);
                }
            }
            catch (Exception e)
            {
                messages.Add("The file could not be read: " + e.Message);
            }
            return messages;
        }
        private List<string> ParseHands(SiteName site, string handText, ref int parsedHands, ref int thrownOutHands)
        {
            List<string> messages = new List<string>();
            // Each poker site has its own parser so we use a factory to get the right parser.
            IHandHistoryParserFactory handHistoryParserFactory = new HandHistoryParserFactoryImpl();

            // Get the correct parser from the factory.
            IHandHistoryParser handHistoryParser = handHistoryParserFactory.GetFullHandHistoryParser(site);
            try
            {
                List<string> hands = new List<string>();
                hands = handHistoryParser.SplitUpMultipleHands(handText).ToList();
                foreach (string hand in hands)
                {
                    try
                    {
                        // The true causes hand-parse errors to get thrown. If this is false, hand-errors will
                        // be silent and null will be returned.
                        HandHistory handHistory = handHistoryParser.ParseFullHandHistory(hand, true);

                        //handhistory can now be broken down to be put into the database

                        // Add to player and play table
                        addPlayersToDB(handHistory);

                        //Add to hand table
                        addHandToDB(handHistory);

                        // Add to table table
                        addTableToDB(handHistory);

                        // Add to hand_action
                        addHandActionToDB(handHistory);

                        db.SaveChanges();

                        parsedHands++;
                    }
                    catch (Exception ex)
                    {
                        messages.Add("Parsing Error: " + ex.Message);
                        thrownOutHands++;
                    }
                }
            }
            catch (Exception ex) // Catch hand-parsing exceptions
            {
                messages.Add("Parsing Error: " + ex.Message);
            }
            return messages;
        }

        private void addPlayersToDB(HandHistory handHistory)
        {
            foreach (Player item in handHistory.Players)
            {
                player newPlayer = new Models.player { Name = item.PlayerName };
                if (newPlayer.Name == null) continue;
                if (db.players.Find(item.PlayerName) == null)
                {
                    db.players.Add(newPlayer);
                }
                addPlayToDB(handHistory, item);
            }
        }

        private void addHandToDB(HandHistory handHistory)
        {
            if (db.hands.Find(handHistory.HandId) != null) return;
            IEnumerator<Card> cardList = handHistory.CommunityCards.GetEnumerator();
            hand newHand = new Models.hand
            {
                HandID = handHistory.HandId,
                TableID = handHistory.TableName,
                NumPlayers = handHistory.NumPlayersActive,
                StartTime = handHistory.DateOfHandUtc,
                ButtonPosition = handHistory.DealerButtonPosition,
                PotSize = handHistory.TotalPot,
                FlopCard1 = (cardList.MoveNext()) ? cardList.Current.ToString() : null,
                FlopCard2 = (cardList.MoveNext()) ? cardList.Current.ToString() : null,
                FlopCard3 = (cardList.MoveNext()) ? cardList.Current.ToString() : null,
                TurnCard = (cardList.MoveNext()) ? cardList.Current.ToString() : null,
                RiverCard = (cardList.MoveNext()) ? cardList.Current.ToString() : null,
            };
            db.hands.Add(newHand);
        }

        private void addPlayToDB(HandHistory handHistory, Player player)
        {
            if (player.IsSittingOut || player.PlayerName == null) return;
            if (db.plays.Find(handHistory.HandId, player.PlayerName) != null) return;
            IEnumerator<Card> cardList = (player.hasHoleCards) ? player.HoleCards.GetEnumerator() : null;
            play newPlay = new Models.play
            {
                PlayerName = player.PlayerName,
                HandID = handHistory.HandId,
                StartingStack = player.StartingStack,
                SeatPosition = player.SeatNumber,
                HoleCard1 = (cardList.MoveNext()) ? cardList.Current.ToString() : null,
                HoleCard2 = (cardList.MoveNext()) ? cardList.Current.ToString() : null,
            };
            db.plays.Add(newPlay);
        }

        private void addTableToDB(HandHistory handHistory)
        {
            if (db.tables.Find(handHistory.TableName) != null) return;
            table newTable = new Models.table
            {
                TableID = handHistory.TableName,
                MaxPlayers = handHistory.GameDescription.SeatType.MaxPlayers,
                Stakes = handHistory.GameDescription.Limit.ToDbSafeString(),
                Site = handHistory.GameDescription.Site.ToString(),
            };
            db.tables.Add(newTable);
        }

        private void addHandActionToDB(HandHistory handHistory)
        {
            int aggro=0;
            Street curStreet = Street.Preflop;
            foreach (HandAction item in handHistory.HandActions)
            {
                if (db.hand_action.Find(handHistory.HandId, item.ActionNumber) != null) continue;
                if (curStreet != item.Street)
                {
                    curStreet = item.Street;
                    aggro = 0;
                }
                if (item.IsAggressiveAction) aggro++;

                hand_action newHandAction = new Models.hand_action
                {
                    HandID = handHistory.HandId,
                    ActionID = item.ActionNumber,
                    PlayerName = item.PlayerName,
                    ActionName = item.HandActionType.ToString(),
                    Street = item.Street.ToString(),
                    Amount = item.Amount,
                    IsPFR = item.IsPreFlopRaise,
                    IsVPIP = (item.Street==Street.Preflop) ? (item.IsAggressiveAction || item.HandActionType==HandActionType.CALL) : false,
                    Is3Bet = (item.IsAggressiveAction && aggro==3),
                    Is4Bet = (item.IsAggressiveAction && aggro==4),
                };
                db.hand_action.Add(newHandAction);
            }
        }
    }
}