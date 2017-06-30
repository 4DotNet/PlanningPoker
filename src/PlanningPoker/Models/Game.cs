using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScrumPlanningPoker.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int UniqueKey { get; set; }
        public int Master { get; set; }
        public int CurrentRound { get; set; }
        public DateTime GameStarted { get; private set; }
        public DateTime LastActivity { get; set; }
        public Dictionary<Player, int> Scores { get; set; }
        public Collection<Player> Players { get; set; }
        public Dictionary<Player, List<Score>> Rounds { get; set; }
        
        private readonly ICollection<Card> _availableCards = new Collection<Card>();
        public ICollection<Card> AvailableCards { get { return _availableCards; } }

        public Game()
        {
            Scores = new Dictionary<Player, int>();
            Players = new Collection<Player>();
            Rounds = new Dictionary<Player, List<Score>>();

            CurrentRound = 1;
            GameStarted = DateTime.Now;
            LastActivity = DateTime.Now;
        }
    }
}