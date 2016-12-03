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
                db.Configuration.AutoDetectChangesEnabled = false;
                foreach (string hand in hands)
                {
                    try
                    {
                        // The true causes hand-parse errors to get thrown. If this is false, hand-errors will
                        // be silent and null will be returned.
                        HandHistory handHistory = handHistoryParser.ParseFullHandHistory(hand, true);

                        //handhistory can now be broken down to be put into the database

                        // Add to player table
                        Dictionary<string, player> playerDict = addPlayersToDB(handHistory);

                        // Add to table table
                        table dbTable = addTableToDB(handHistory);

                        db.SaveChanges();

                        //Add to hand table
                        hand dbHand = addHandToDB(handHistory, dbTable);

                        // Add to plays table
                        addPlaysToDB(handHistory, playerDict);

                        // Add to hand_action table
                        addHandActionToDB(handHistory, dbHand, playerDict);

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
            db.Configuration.AutoDetectChangesEnabled = true;
            return messages;
        }

        private Dictionary<string,player> addPlayersToDB(HandHistory handHistory)
        {
            Dictionary<string,player> playerDict = new Dictionary<string, player>();
            foreach (Player item in handHistory.Players)
            {

                if (item.PlayerName == null) continue;
                player dbPlayer = db.players
                    .Where(s => s.Name == item.PlayerName)
                    .SingleOrDefault();
                if (dbPlayer == null)
                {
                    dbPlayer = new Models.player { Name = item.PlayerName };
                    db.players.Add(dbPlayer);
                }
                playerDict.Add(dbPlayer.Name, dbPlayer);
            }
            return playerDict;
        }

        private void addPlaysToDB(HandHistory handHistory, Dictionary<string,player> playerDict)
        {
            foreach (Player parserPlayer in handHistory.Players)
            {
                player dbPlayer = playerDict[parserPlayer.PlayerName];
                IEnumerator<Card> cardList = null;
                string card1 = null;
                string card2 = null;
                if (parserPlayer.IsSittingOut) return;
                if (db.plays.Find(dbPlayer.PlayerID, handHistory.HandId) != null) return; //this is failing
                if (parserPlayer.hasHoleCards)
                {
                    cardList = parserPlayer.HoleCards.GetEnumerator();
                    card1 = (cardList.MoveNext()) ? cardList.Current.ToString() : null;
                    card2 = (cardList.MoveNext()) ? cardList.Current.ToString() : null;
                }
                play newPlay = new Models.play
                {
                    PlayerID = dbPlayer.PlayerID,
                    HandID = handHistory.HandId,
                    StartingStack = parserPlayer.StartingStack,
                    SeatPosition = parserPlayer.SeatNumber,
                    HoleCard1 = card1,
                    HoleCard2 = card2,
                    hand = db.hands.Find(handHistory.HandId),
                    player = dbPlayer,
                };
                db.plays.Add(newPlay);
            }
        }

        private hand addHandToDB(HandHistory handHistory, table handTable)
        {
            hand dbHand = db.hands.Find(handHistory.HandId);
            if (dbHand != null) return dbHand;
            IEnumerator<Card> cardList = handHistory.CommunityCards.GetEnumerator();
            dbHand = new Models.hand
            {
                HandID = handHistory.HandId,
                TableID = handTable.TableID,
                NumPlayers = handHistory.NumPlayersActive,
                StartTime = handHistory.DateOfHandUtc,
                ButtonPosition = handHistory.DealerButtonPosition,
                PotSize = handHistory.TotalPot,
                FlopCard1 = (cardList.MoveNext()) ? cardList.Current.ToString() : null,
                FlopCard2 = (cardList.MoveNext()) ? cardList.Current.ToString() : null,
                FlopCard3 = (cardList.MoveNext()) ? cardList.Current.ToString() : null,
                TurnCard = (cardList.MoveNext()) ? cardList.Current.ToString() : null,
                RiverCard = (cardList.MoveNext()) ? cardList.Current.ToString() : null,
				table = handTable,
            };
            db.hands.Add(dbHand);
            return dbHand;
        }

        private table addTableToDB(HandHistory handHistory)
        {
            table dbTable = db.tables
                .Where(s => s.TableName == handHistory.TableName)
                .SingleOrDefault();
            if (dbTable != null) return dbTable;
            dbTable = new Models.table
            {
                TableName = handHistory.TableName,
                MaxPlayers = handHistory.GameDescription.SeatType.MaxPlayers,
                Stakes = handHistory.GameDescription.Limit.ToDbSafeString(),
                Site = handHistory.GameDescription.Site.ToString(),
            };
            db.tables.Add(dbTable);
            db.SaveChanges();
            return dbTable;
        }

        private void addHandActionToDB(HandHistory handHistory, hand dbHand, Dictionary<string,player> playerDict)
        {
            int aggro = 0;
            int actionNumber = 0;
            Street curStreet = Street.Preflop;
            foreach (HandAction item in handHistory.HandActions)
            {
                if (db.hand_action.Find(handHistory.HandId, actionNumber) != null) continue;
                if (curStreet != item.Street)
                {
                    curStreet = item.Street;
                    aggro = 0;
                }
                if (item.IsAggressiveAction) aggro++;

                hand_action newHandAction = new Models.hand_action
                {
                    HandID = handHistory.HandId,
                    ActionID = actionNumber,
                    PlayerID = playerDict[item.PlayerName].PlayerID,
                    ActionName = item.HandActionType.ToString(),
                    Street = item.Street.ToString(),
                    Amount = item.Amount,
                    IsPFR = item.IsPreFlopRaise,
                    IsVPIP = (item.Street==Street.Preflop) ? (item.IsAggressiveAction || item.HandActionType==HandActionType.CALL) : false,
                    Is3Bet = (item.IsAggressiveAction && aggro==3),
                    Is4Bet = (item.IsAggressiveAction && aggro==4),
					hand = dbHand,
					player = playerDict[item.PlayerName],
                };
                db.hand_action.Add(newHandAction);
                actionNumber++;
            }
        }
    }
}