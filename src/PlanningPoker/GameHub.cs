using Microsoft.AspNet.SignalR;
using ScrumPlanningPoker.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ScrumPlanningPoker.Models;

namespace ScrumPlanningPoker
{
    // Note: most methods only used from client calls, that is why there are no apparent references
    public class GameHub : Hub
    {
        public void SignIn(string playerName, int gameId, bool isScrumMaster)
        {
            if (isScrumMaster && !GameManager.Instance.GameExists(gameId))
                GameManager.Instance.CreateGame(gameId, playerName);

            if (!isScrumMaster)
            {
                if (!GameManager.Instance.GameExists(gameId))
                {
                    Clients.Caller.receiveError("Game not found", true);
                    return;
                }

                var player = GameManager.Instance.GetPlayerByName(gameId, playerName);

                if (player == null)
                {
                    GameManager.Instance.AddPlayerToGame(playerName, gameId, Context.ConnectionId);

                    Clients.Group(gameId.ToString(CultureInfo.InvariantCulture)).playerSignedIn(playerName);
                }
                else
                {
                    if (DateTime.Now.Ticks - player.LastPingResponse >= 80000000)
                        GameManager.Instance.ReconnectPlayerToGame(playerName, gameId, Context.ConnectionId);
                }
            }
        }

        public void SendScore(string playerName, int gameId, int score)
        {
            if (!GameManager.Instance.GameExists(gameId))
                throw new Exception("Game not found");

            var currentGame = GameManager.Instance.GetGameById(gameId);

            currentGame.LastActivity = DateTime.Now;

            var currentPlayer = GameManager.Instance.GetPlayerByName(gameId, playerName);

            if (currentGame.Scores.ContainsKey(currentPlayer))
                currentGame.Scores[currentPlayer] = score;
            else
                currentGame.Scores.Add(currentPlayer, score);

            if (!currentGame.Rounds.ContainsKey(currentPlayer))
                currentGame.Rounds.Add(currentPlayer, new List<Score> { new Score { Rating = score, Round = currentGame.CurrentRound } });
            else
            {
                var playerScore = currentGame.Rounds[currentPlayer].Find(s => s.Round == currentGame.CurrentRound);

                if (playerScore == null)
                    currentGame.Rounds[currentPlayer].Add(new Score { Rating = score, Round = currentGame.CurrentRound });
                else
                    playerScore.Rating = score;
            }

            if (currentGame.Scores.Count == currentGame.Players.Count(p => (DateTime.Now.Ticks - p.LastPingResponse) < 80000000))
            {
                Clients.Group(gameId.ToString(CultureInfo.InvariantCulture)).allScoresReceived(currentGame);

                currentGame.Scores.Clear();

                currentGame.CurrentRound++;
            }
            else
            {
                Clients.Group(gameId.ToString(CultureInfo.InvariantCulture)).scoreReceived(currentPlayer);
            }
        }

        public void Join(int gameId)
        {
            Groups.Add(Context.ConnectionId, gameId.ToString(CultureInfo.InvariantCulture));
        }

        public void Ping(int gameId)
        {
            GameManager.Instance.GetPlayerByConnectionId(gameId, Context.ConnectionId).LastPingResponse =
                DateTime.Now.Ticks;
        }

        public Dictionary<string, int> GetDisconnectedClients(int gameId)
        {
            var resultClients = new Dictionary<string, int>();

            if (!GameManager.Instance.GameExists(gameId))
                throw new Exception("Game not found");

            var currentGame = GameManager.Instance.GetGameById(gameId);

            foreach (var player in currentGame.Players)
            {
                var pingDiff = (DateTime.Now.Ticks - player.LastPingResponse);

                if (pingDiff >= 40000000 && pingDiff < 80000000)
                    resultClients.Add(player.Name, 1);
                else if (pingDiff >= 80000000)
                    resultClients.Add(player.Name, 2);
            }

            return resultClients;
        }

        // TODO do something when the user leaves?
        public void Leave()
        {
            throw new NotImplementedException();
        }
    }
}