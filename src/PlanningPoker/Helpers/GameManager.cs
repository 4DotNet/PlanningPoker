using System.Globalization;
using System.IO;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ScrumPlanningPoker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

namespace ScrumPlanningPoker.Helpers
{
    public class GameManager
    {
        private readonly Dictionary<int, Game> _games = new Dictionary<int, Game>();

        private static readonly Lazy<GameManager> _instance =
            new Lazy<GameManager>(() => new GameManager(GlobalHost.ConnectionManager.GetHubContext<GameHub>().Clients));

        public bool GameExists(int gameId)
        {
            return _games.ContainsKey(gameId);
        }

        public Game GetGameById(int gameId)
        {
            return _games[gameId];
        }

        public void CreateGame(int gameId, string scrumMasterName)//, ICollection<int> cards)
        {
            foreach (var game in _games)
            {
                if (DateTime.Now.Subtract(game.Value.LastActivity).Hours >= 1)
                    _games.Remove(game.Key);
            }

            var speler = new Player { Name = scrumMasterName, Round = 1, Score = 0 };

            var newGame = new Game { Master = speler.Id, UniqueKey = gameId };

            // TODO betere foutafhandeling?
            //foreach (var card in cards)
            //    newGame.AvailableCards.Add((Card)Enum.Parse(typeof(Card), card.ToString(CultureInfo.InvariantCulture)));

            _games.Add(gameId, newGame);
        }

        //public void GetHistoryInPdf(int gameId)
        //{
        //    //Set page size as A4
        //    var pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);

        //    try
        //    {
        //        PdfWriter.GetInstance(pdfDoc, HttpContext.Current.Response.OutputStream);

        //        //Open PDF Document to write data
        //        pdfDoc.Open();

        //        // Html content in a string to write in PDF
        //        var contents = "<table id=\"historyRecords\" class=\"table table-striped\"><th>Naam</th><th>Ronde</th><th>Score</th>";

        //        foreach (var round in GetGameById(gameId).Rounds)
        //        {
        //            contents += "<tr id=\"row" + round.Key + "\"><td id=\"row" + round.Key + "Name\" rowspan=\"" +
        //                        round.Value.Count + "\">" + round.Key + "</td></tr>";

        //            foreach (var score in round.Value)
        //            {
        //                contents += "<tr><td>" + score.Round + "</td><td>" + score.Rating + "</td></tr>";
        //            }
        //        }

        //        contents += "</table>";

        //        //Read string contents using stream reader and convert html to parsed conent
        //        var parsedHtmlElements = HTMLWorker.ParseToList(new StringReader(contents), null);

        //        //Get each array values from parsed elements and add to the PDF document
        //        foreach (var htmlElement in parsedHtmlElements)
        //            pdfDoc.Add(htmlElement);

        //        //Close your PDF
        //        pdfDoc.Close();
        //        Response.ContentType = "application/pdf";

        //        //Set default file Name as current datetime
        //        Response.AddHeader("content-disposition",
        //                           "attachment; filename=" + DateTime.Now.ToString("yyyyMMdd") + ".pdf");
        //        HttpContext.Current.Response.Write(pdfDoc);

        //        Response.Flush();
        //        Response.End();

        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write(ex.ToString());
        //    }
        //}

        public void AddPlayerToGame(string playerName, int gameId, string connectionId)
        {
            var player = new Player
                {
                    Name = playerName,
                    ConnectionId = connectionId,
                    LastPingResponse = DateTime.Now.Ticks
                };

            _games[gameId].Players.Add(player);
        }

        public Player GetPlayerByConnectionId(int gameId, string connectionId)
        {
            if (!GameExists(gameId))
                throw new Exception("Game not found");

            // TODO Checken of ie bestaat? of deze exception afvangen
            return GetGameById(gameId).Players.First(player => player.ConnectionId == connectionId);
        }

        public Player GetPlayerByName(int gameId, string name)
        {
            if (!GameExists(gameId))
                throw new Exception("Game not found");

            // TODO Checken of ie bestaat? of deze exception afvangen
            return GetGameById(gameId).Players.FirstOrDefault(player => string.Equals(player.Name, name));
        }

        public void CloseGame(int gameId)
        {
            _games.Remove(gameId);
        }

        private GameManager(IHubConnectionContext clients)
        {
        }

        public static GameManager Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public void ReconnectPlayerToGame(string playerName, int gameId, string connectionId)
        {
            if (!GameExists(gameId))
                throw new Exception("Game not found");

            var player = GetGameById(gameId).Players.First(p => string.Equals(p.Name, playerName));
            player.ConnectionId = connectionId;
            player.LastPingResponse = DateTime.Now.Ticks;
        }
    }
}