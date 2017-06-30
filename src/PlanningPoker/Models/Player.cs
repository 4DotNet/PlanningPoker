namespace ScrumPlanningPoker.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Round { get; set; }
        public int Score { get; set; }

        public string ConnectionId { get; set; }
        public long LastPingResponse { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}